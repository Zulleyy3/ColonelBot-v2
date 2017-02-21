using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Modules;
using Discord.Net;
using Discord.Commands.Permissions.Levels;
using Discord.Commands.Permissions.Userlist;
using ColonelBot;
using ColonelBot.Services;
using System.Net;


namespace ColonelBot.Modules.Event
{
    class EventModule : IModule
    {
        private ModuleManager _manager;
        private DiscordClient _client;
        private bool _isRunning;
        private SettingsManager<Settings> _settings;
        
        void IModule.Install(ModuleManager manager)
        {
            Console.WriteLine("Installed Event Tools Module.");
            _manager = manager;
            _client = manager.Client;
            _settings = _client.GetService<SettingsService>().AddModule<EventModule, Settings>(manager);

            manager.CreateCommands("round", group =>
            {
                group.CreateCommand("report")
                    .Parameter("wins", ParameterType.Required)
                    .Description("Report your current rounds' wins to ColonelBot.");
                    //.Do(async e =>
                
            });


            manager.CreateCommands("event", group =>
            {
                group.CreateCommand("register")
                    .Parameter("Netbattler Name", ParameterType.Required)
                    .Description("Register for the currently active event, if any. Requires Netbattler Name")
                    .Do(async e =>
                    {
                        var settings = _settings.Load(e.Server);
                        if (settings.AcceptingRegistrations == true)
                        {
                            if (settings.AddRegistration(e.Args[0], e.User.Id.ToString()))
                            {
                                Console.WriteLine("Added registration to event.");
                                await _settings.Save(e.Server, settings);
                                await e.Channel.SendMessage("You are registered for " + settings.CurrentEventTitle + " successfully.");
                                await e.User.AddRoles(e.Server.GetRole(203848231803813889));

                                
                            }
                            else
                            {
                                await e.Channel.SendMessage("You are already registered for the event.");
                                Console.WriteLine("User tried to register for the event but was already registered.");
                            }
                        }
                        else
                            await e.Channel.SendMessage("The event is not accepting registrations at this time.");
                        

                    });
                group.CreateCommand("drop")
                    .Description("Drops you from the active event, if any.")
                    .Do(async e =>
                    {
                        var settings = _settings.Load(e.Server);
                        if (settings.RemoveRegistration(e.User.Id.ToString()))
                        {
                            await e.Channel.SendMessage("You have been removed from " + settings.CurrentEventTitle);
                            await e.User.RemoveRoles(e.Server.GetRole(203848231803813889));
                            await _settings.Save(e.Server, settings);
                        }
                        else
                        {
                            await e.Channel.SendMessage("You're not enrolled in the current event.");
                        }
                        
                    });
                group.CreateCommand("info")
                    .Description("Provides information about the currently active event.")
                    .Do(async e =>
                    {
                        var settings = _settings.Load(e.Server);
                        string RegStatus = string.Empty;
                        if (settings.AcceptingRegistrations == true)
                            RegStatus = "Open";
                        else
                            RegStatus = "Closed";

                        if (settings.BracketHidden == true)
                        {//Is the bracket hidden?
                            if (settings.BracketLocked == true)
                            {//Is the caller a participant in the event?
                                bool IsUserAParticipant = false;
                                foreach (KeyValuePair<string, Settings.Registration> reg in settings.Registrations)
                                { //reg.Key.ToString() = User ID
                                    if (e.User.Id.ToString() == reg.Key.ToString())
                                    {
                                        IsUserAParticipant = true;
                                    }
                                    
                                }
                                if (IsUserAParticipant == true) //Self-explanatory
                                    await e.Channel.SendMessage("Current Active Event: **" + settings.CurrentEventTitle + "**\nRegistration: **" + RegStatus + "**\nEvent Information: <" + settings.CurrentEventURL + ">\nEvent Bracket: <" + settings.BracketURL + ">");
                                else //They're not.
                                    await e.Channel.SendMessage("Current Active Event: **" + settings.CurrentEventTitle + "**\nRegistration: **" + RegStatus + "**\nEvent Information: <" + settings.CurrentEventURL + ">");
                            }
                            else // The caller is not a participant, give them the output with the bracket hidden.
                                await e.Channel.SendMessage("Current Active Event: **" + settings.CurrentEventTitle + "**\nRegistration: **" + RegStatus + "**\nEvent Information: <" + settings.CurrentEventURL + ">");
                        }else //The bracket is not hidden
                            await e.Channel.SendMessage("Current Active Event: **" + settings.CurrentEventTitle + "**\nRegistration: **" + RegStatus + "**\nEvent Information: <" + settings.CurrentEventURL + ">\nEvent Bracket: <" + settings.BracketURL + ">");



                    });

                group.CreateGroup("admin", grp =>
                {

                    grp.CreateCommand("end")
                        .Description("Ends the event, dropping all participants.")
                        .Do(async e =>
                        {
                            if (e.User.HasRole(e.Server.GetRole(276401950185095168))) //This is the Event Organizer's ULONG ID.
                            {
                                var settings = _settings.Load(e.Server);
                                foreach (KeyValuePair<string, Settings.Registration> reg in settings.Registrations)
                                {
                                    Console.WriteLine("Removing " + reg.Key.ToString() + " battler name " + reg.Value.NetbattlerName);

                                    settings.RemoveRegistration(reg.Key.ToString());

                                }
                                settings.CurrentEventTitle = "None";
                                settings.AcceptingRegistrations = false;
                                settings.CurrentEventURL = "None";
                                settings.BracketURL = "None";
                                settings.BracketHidden = true;
                                settings.StreamChannelURL = "None";
                                settings.BracketLocked = false;
                                await e.Channel.SendMessage("The event has been closed.");
                                await _settings.Save(e.Server, settings);
                            }
                        });

                    grp.CreateCommand("registration")
                        .Parameter("Open/Close", ParameterType.Required)
                        .Do(async e =>
                        {
                            if (e.User.HasRole(e.Server.GetRole(276401950185095168))) //This is the Event Organizer's ULONG ID.
                            {
                                var settings = _settings.Load(e.Server);
                                if (e.Args[0] == "open")
                                    settings.AcceptingRegistrations = true;
                                else if (e.Args[0] == "close")
                                    settings.AcceptingRegistrations = false;
                                await _settings.Save(e.Server, settings);

                                string RegStatus = string.Empty;
                                if (settings.AcceptingRegistrations == true)
                                    RegStatus = "Open";
                                else
                                    RegStatus = "Closed";

                                await e.Channel.SendMessage("Registrations are now **" + RegStatus + "**.");
                            }
                        });

                    grp.CreateCommand("report")
                        .Description("Provides the organizer a list of currently enrolled battlers.")
                        .Do(async e =>
                        {
                            if (e.User.HasRole(e.Server.GetRole(276401950185095168))) //This is the Event Organizer's ULONG ID.
                            {
                                var settings = _settings.Load(e.Server);
                                string result = "Event Participant List\n\n";
                                foreach (KeyValuePair<string, Settings.Registration> reg in settings.Registrations)
                                {
                                    result += e.Server.GetUser(Convert.ToUInt64(reg.Key)).Name + " (ID: " + reg.Key.ToString() + " ) - Battler Name: " + reg.Value.NetbattlerName + "\n";
                                }

                                result += "\n\n====Challonge Export====\n\n";

                                foreach (KeyValuePair<string, Settings.Registration> reg  in settings.Registrations)
                                {
                                    result += reg.Value.NetbattlerName + "\n";
                                }
                                await e.User.SendMessage(result);
                                await e.Channel.SendMessage("You have e-mail.");
                                
                            }
                           
                        });

                    grp.CreateCommand("title")
                    .Parameter("Event Title", ParameterType.Required)
                    .Do(async e =>
                    {
                        if (e.User.HasRole(e.Server.GetRole(276401950185095168))) //This is the Event Organizer's ULONG ID.
                        {
                            var settings = _settings.Load(e.Server);
                            settings.CurrentEventTitle = e.Args[0];
                            await e.Channel.SendMessage("The event title has been updated.");
                            await _settings.Save(e.Server, settings);
                        }
                        else
                        {
                            await e.Channel.SendMessage("You do not have permission to update the event title.");
                        }
                        

                    });

                    grp.CreateCommand("bracket")
                        .Parameter("Bracket URL", ParameterType.Required)
                        .Description("Updates the Bracket URL.")
                        .Do(async e =>
                        {
                            if (e.User.HasRole(e.Server.GetRole(276401950185095168))) //This is the Event Organizer's ULONG ID.
                            {
                                var settings = _settings.Load(e.Server);
                                settings.BracketURL = e.Args[0];
                                await _settings.Save(e.Server, settings);
                                await e.Channel.SendMessage("The Bracket URL has been updated.");
                            }
                        });

                    grp.CreateCommand("hidebracket")
                        .Description("Hides the bracket from !event info")
                        .Do(async e =>
                        {
                            if (e.User.HasRole(e.Server.GetRole(276401950185095168))) //This is the Event Organizer's ULONG ID.
                            {
                                var settings = _settings.Load(e.Server);
                                settings.BracketHidden = true;
                                await _settings.Save(e.Server, settings);
                                await e.Channel.SendMessage("The bracket has been hidden from !event info");
                            }
                        });

                    grp.CreateCommand("showbracket")
                        .Description("Hides the bracket from !event info")
                        .Do(async e =>
                        {
                            if (e.User.HasRole(e.Server.GetRole(276401950185095168))) //This is the Event Organizer's ULONG ID.
                            {
                                var settings = _settings.Load(e.Server);
                                settings.BracketHidden = false;
                                await _settings.Save(e.Server, settings);
                                await e.Channel.SendMessage("The bracket has been hidden from !event info");
                            }
                        });

                    grp.CreateCommand("lockbracket")
                        .Description("Sets the bracket to be PMed to the caller if the brackets are hidden but they are an event participant.")
                        .Do(async e =>
                        {
                            if (e.User.HasRole(e.Server.GetRole(276401950185095168))) //This is the Event Organizer's ULONG ID.
                            {
                                var settings = _settings.Load(e.Server);
                                settings.BracketLocked = !settings.BracketLocked;
                                await _settings.Save(e.Server, settings);
                                await e.Channel.SendMessage("Brackets will be provided to participants only if they are participating in the event? " + settings.BracketLocked.ToString() + ".");
                            }
                        });


                    grp.CreateCommand("url")
                        .Parameter("Event URL", ParameterType.Required)
                        .Do(async e =>
                        {
                            if (e.User.HasRole(e.Server.GetRole(276401950185095168))) //This is the Event Organizer's ULONG ID.
                        {
                                var settings = _settings.Load(e.Server);
                                settings.CurrentEventURL = e.Args[0];
                                await e.Channel.SendMessage("The event URL has been updated.");
                                await _settings.Save(e.Server, settings);
                            }
                            else
                            {
                                await e.Channel.SendMessage("You do not have permission to update the event URL.");
                            }
                        });
                });
                //EVENT ORGANIZER COMMANDS
                
            });

        }           
    }
}
