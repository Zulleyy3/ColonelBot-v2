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
            _manager = manager;
            _client = manager.Client;
            _settings = _client.GetService<SettingsService>().AddModule<EventModule, Settings>(manager);

            

            manager.CreateCommands("event", group =>
            {
                
                group.CreateCommand("title")
                    .Parameter("Event Title", ParameterType.Required)
                    .Do(async e =>
                    {
                        if (e.User.HasRole(e.Server.GetRole(276401950185095168))) //This is the Event Organizer's ULONG ID.
                        {
                            var settings = _settings.Load(e.Server);
                            settings.CurrentEventTitle = e.Args[0];
                            await e.Channel.SendMessage("The event title has been updated.");
                        }else
                        {
                            await e.Channel.SendMessage("You do not have permission to update the event title.");
                        }
                        
                    });

                group.CreateCommand("url")
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

        }           
    }
}
