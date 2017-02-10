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
        
        private static string[] BlacklistedPhrases;
        private static string[] WhitelistedPhrases;
        public bool ModInitialized = false;

        private static bool CheckMessage(string MessageToCheck)
        {//Checks the message against the blacklist. Returns true if it should be logged/blocked, false if not.
            bool result = false;
            foreach (string entry in BlacklistedPhrases)
            {
                if (MessageToCheck.ToUpper().Contains(entry.ToUpper()) == true)
                {
                    //Check to see if it's on the whitelist.
                    foreach (string phrase in WhitelistedPhrases)
                    {
                        if (MessageToCheck.ToUpper().Contains(phrase.ToUpper()) == true)
                        {
                            result = false;
                        }else
                            result = true;
                    }
                    
                }
            }

            return result;

        }

        void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _client = manager.Client;
            _settings = _client.GetService<SettingsService>().AddModule<AutoModModule, Settings>(manager);

            Console.WriteLine("Installed AutoModeration");

            BlacklistedPhrases = System.IO.File.ReadAllLines(FileTools.BotDirectory + "\\Config\\BlacklistLocation.txt");
            WhitelistedPhrases = System.IO.File.ReadAllLines(FileTools.BotDirectory + "\\Config\\WhitelistLocation.txt");

            manager.CreateCommands("updatelists", group =>
            {
                group.CreateCommand()
                    .Do(async e =>
                    {
                        if (e.User.HasRole(e.Server.GetRole(132109612118704128)))
                        {
                            BlacklistedPhrases = System.IO.File.ReadAllLines("E:\\OneDrive\\N1 GP Admin\\Phrase Blacklist.txt");
                            WhitelistedPhrases = System.IO.File.ReadAllLines("E:\\OneDrive\\N1 GP Admin\\Phrase Whitelist.txt");
                            await e.Channel.SendMessage("Updated the whitelist and blacklist.");
                        }
                    });  
            });

            manager.CreateCommands("autodelete", group =>
            {

                group.CreateCommand()
                    .Description("Toggles the bot's Auto-Delete function.")
                    .Do(async e =>
                    {
                        if (e.User.HasRole(e.Server.GetRole(132109612118704128)))
                        {
                            var settings = _settings.Load(e.Server);
                            settings.AutoDelete = !settings.AutoDelete;
                            await _settings.Save(e.Server, settings);
                            Console.WriteLine("Toggled autodelete");
                            await e.Channel.SendMessage("Auto-Delete: " + settings.AutoDelete.ToString());
                        }
                    });
            });

            manager.CreateCommands("autoreport", group =>
            {
                group.CreateCommand()
                    .Description("Toggles the bot's auto-reporting capabilities.")
                    .Do(async e =>
                    {
                        if (e.User.HasRole(e.Server.GetRole(132109612118704128)))
                        {
                            var settings = _settings.Load(e.Server);
                            settings.AutoReport = !settings.AutoReport;
                            await _settings.Save(e.Server, settings);
                            await e.Channel.SendMessage("Auto-report: " + settings.AutoReport.ToString());
                            if (settings.AutoReport == true)
                            {
                                _client.MessageReceived += async (s, x) =>
                                {
                                    if (CheckMessage(x.Message.Text) == true)
                                    {

                                        if (x.User.Id != 249632069238390784)
                                        {
                                            await e.Server.GetChannel(settings.ReportChannelID).SendMessage(x.User.Name + " said ''" + x.Message.Text + "'' in " + x.Channel.Name + " on " + DateTime.Now.Month + " " + DateTime.Now.Day + " " + DateTime.Now.TimeOfDay);
                                            if (settings.AutoDelete == true)
                                            {
                                                await x.Message.Delete();
                                            }
                                        }

                                    }
                                };
                            }
                        }
                       
                    });
            });


            manager.CreateCommands("setchannel", group =>
            {

                group.CreateCommand()
                    .Description("Sets the reporting channel.")
                    .Do(async e =>
                    {

                         
                        if (e.User.HasRole(e.Server.GetRole(132109612118704128)))
                        {
                            var settings = _settings.Load(e.Server);
                            
                            settings.ReportChannelID = e.Channel.Id;
                            
                            
                            await _settings.Save(e.Server, settings);
                            await e.Channel.SendMessage("Set this channel to be the default reporting channel.");
                            Console.WriteLine("AutoMod: Reporting Channel set to " + e.Server.GetChannel(settings.ReportChannelID).Name);
                    }
                            
                        
                    });
            });


           

          
    }

        private void _client_MessageReceived(object sender, MessageEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}