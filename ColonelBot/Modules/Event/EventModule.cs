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
                        if (settings.AddRegistration(e.Args[0], e.User.Id.ToString()))
                        {
                            Console.WriteLine("Added registration to event.");
                            await _settings.Save(e.Server, settings);
                            await e.User.AddRoles(e.Server.GetRole(203848231803813889));

                            await e.Channel.SendMessage("You are registered for " + settings.CurrentEventTitle + " successfully.");
                        }
                        else
                        {
                            await e.Channel.SendMessage("You are already registered for the event.");
                            Console.WriteLine("User tried to register for the event but was already registered.");
                        }

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

                group.CreateGroup("admin", grp =>
                {

                    grp.CreateCommand("end")
                        .Description("Ends the event, dropping all participants.")
                        .Do(async e =>
                        {
                            var settings = _settings.Load(e.Server);
                            foreach (KeyValuePair<string, Settings.Registration> reg in settings.Registrations)
                            {
                                Console.WriteLine("Removing " + reg.Key.ToString() + " battler name " + reg.Value.NetbattlerName);

                                settings.RemoveRegistration(reg.Key.ToString());
                                
                            }
                            await e.Channel.SendMessage("The event has been closed.");
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
                        }
                        else
                        {
                            await e.Channel.SendMessage("You do not have permission to update the event title.");
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
