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

//This module automatically reports any phrases on the Blacklist to a moderation chat channel.

//N1GP Mod Role ID: 132109612118704128	
namespace ColonelBot.Modules.AutoModeration
{
    class AutoModModule : IModule
    {
        private ModuleManager _manager;
        private DiscordClient _client;
        private SettingsManager<Settings> _settings;
        private static Settings mySettings;
        private string[] BlacklistedPhrases;
        public bool ModInitialized = false;

        private static bool CheckMessage(string MessageToCheck)
        {//Checks the message against the blacklist. Returns true if it should be logged/blocked, false if not.
            bool result = false;
            if (mySettings.AutoReport == true)
            { //If the AutoReporter is enabled, do the check and report accordingly.
                string test = "test";

            }

            return result;

        }

        void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _client = manager.Client;
            _settings = _client.GetService<SettingsService>().AddModule<AutoModModule, Settings>(manager);

            Console.WriteLine("Installed AutoModeration");



            manager.CreateCommands("autodelete", group =>
            {

                group.CreateCommand("toggle")
                    .Description("Toggles the bot's Auto-Delete function.")
                    .Do(async e =>
                    {
                        var settings = _settings.Load(e.Server);
                        settings.AutoDelete = !settings.AutoDelete;
                        await _settings.Save(e.Server, settings);
                        Console.WriteLine("Toggled autodelete");
                        await e.Channel.SendMessage("Auto-Delete: " + settings.AutoDelete.ToString());
                    });
            });

            manager.CreateCommands("autoreport", group =>
            {
                group.CreateCommand("toggle")
                    .Description("Toggles the bot's auto-reporting capabilities.")
                    .Do(async e =>
                    {
                        var settings = _settings.Load(e.Server);
                        settings.AutoReport = !settings.AutoReport;
                        await _settings.Save(e.Server, settings);
                        await e.Channel.SendMessage("Auto-Delete: " + settings.AutoReport.ToString());
                    });
            });


            manager.CreateCommands("setchannel", group =>
            {

                group.CreateCommand("reporting")
                    .Description("Sets the reporting channel.")
                    .Do(async e =>
                    {

                        if (ModInitialized == true)
                        {
                            //if (e.User.HasRole(e.Server.GetRole(132109612118704128)))
                            //{
                                mySettings.ReportChannel = e.Channel;
                                await _settings.Save(e.Server, mySettings);
                                await e.Channel.SendMessage("Set this channel to be the default reporting channel.");
                            //}
                            
                        }
                    });
            });


           

          
    }
        
    }
}