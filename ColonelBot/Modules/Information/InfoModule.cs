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


namespace ColonelBot.Modules.Information
{
    class InfoModule : IModule
    {
        private ModuleManager _manager;
        private DiscordClient _client;
        private SettingsManager<Settings> _settings;

        void IModule.Install(ModuleManager manager)
        {
            Console.WriteLine("Installed Information Module.");
            _manager = manager;
            _client = manager.Client;
            _settings = _client.GetService<SettingsService>().AddModule<InfoModule, Settings>(manager);

            manager.CreateCommands("welcome", group =>
            {
                group.CreateCommand()
                    .Description("Gives you all the information you will need to Netbattle online.")
                    .Do(async e =>
                    {
                        var settings = _settings.Load(e.Server);
                        await e.User.SendMessage("ColonelBot v2 Welcome Kit \n=========================\n " + settings.Welcome);
                        await e.Channel.SendMessage("You have e-mail, " + e.User.Name);
                    });
            });

        }
    }
}
