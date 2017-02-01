using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Commands.Permissions.Levels;
using Discord.Modules;
using Discord.Net;
using System.Net;
using System.Net.Http;
using System.Xml.Linq;
using ColonelBot;
using ColonelBot.Services;

namespace ColonelBot.Modules.Profiles
{
    class ProfileModule : IModule
    {

        private ModuleManager _manager;
        private DiscordClient _client;
        private bool _isRunning;
        private SettingsManager<Settings> _settings;

        void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _client = manager.Client;
            _settings = _client.GetService<SettingsService>().AddModule<ProfileModule, Settings>(manager);

           

            manager.CreateCommands("profile", group =>
            {
                group.CreateCommand()
                    .Parameter("Mentioned User", ParameterType.Optional)
                    .Description("Views the profile of the Mentioned User. Views your own if nobody is mentioned.")
                    .Do(async e =>
                    {
                        //PSUDEO: Check to see if there is a parameter in e.Args[1]. if not, 
                        //PSUEDO:       Check to see if the users profile exists. If not, create it and display the result
                        //PSUEDO: If there is something in e.args[1], check to see if that profile exists. If not and the user id of the mentioned user doesn't match the one calling
                        //PSUEDO:       await e.channel.sendmessage("Profile doesn't exist");
                        //PSUEDO        if the mention user ID matches the callers user ID
                        //PSUEDO:           Create the users profile and display it
                        await e.Channel.SendMessage("Beepboop");
                    });
                group.CreateCommand("3ds")
                    .Parameter("3DS Friend Code", ParameterType.Required)
                    .Description("Updates your profiles' 3DS Friend Code")
                    .Do(async e =>
                    {
                        //PSUEDO: Check for an existing profile
                        //PSUEDO: If it exists, update the 3DS Friend Code
                        //PSUEDO: Else, create a profile and update the 3ds friend code.
                        await e.Channel.SendMessage("Updated 3DS Friend Code.");
                    });
                group.CreateCommand("psn")
                    .Parameter("PSN ID", ParameterType.Required)
                    .Description("Updates your profiles' PSN ID.")
                    .Do(async e =>
                    {
                        //PSUEDO: Check for an existing Profile
                        //PSUEDO: If it exists, update the PSN ID
                        //PSUEDO: Else, create a profile/ and update the 3ds friend code.
                        await e.Channel.SendMessage("Updated PSN ID.");
                    });
                group.CreateCommand("xbl")
                    .Parameter("Xbox Live Gamertag", ParameterType.Required)
                    .Description("Updates your profiles' Xbox Live Gamertag.")
                    .Do(async e =>
                    {
                        //PSUEDO: Check for an existing profile
                        //PSUEDO: If it exists, update the XBL GT
                        //PSUEDO: Else, create profile & Update XBL GT
                        await e.Channel.SendMessage("Updated Xbox Live Gamertag.");
                    });
                group.CreateCommand("nnid")
                    .Parameter("Nintendo Network ID", ParameterType.Required)
                    .Description("Updates your profiles' Nintendo Network ID.")
                    .Do(async e =>
                    {
                        //PSUEDO: Check for an existing Profile
                        //PSUEDO: If it exists, update the NNID
                        //PSUEDO: Else, create profile and update NNID 
                        await e.Channel.SendMessage("Updated Nintendo Network ID.");
                    });
                group.CreateCommand("cx")
                    .Parameter("Chrono X Mate Code", ParameterType.Required)
                    .Description("Updates your profiles' Chrono X Mate Code")
                    .Do(async e =>
                    {
                        //PSUEDO: Check for an existing profile
                        //PSUEDO: If it exists, update the CX M8 Code
                        //PSUEDO: Else, create the profile and update the CX M8 Code
                        await e.Channel.SendMessage("Updated Chrono X Mate Code.");
                    });
                group.CreateCommand("bnct")
                    .Parameter("Battle Network Cyber Tournament Code", ParameterType.Required)
                    .Description("Updates your profiles' BNCT Friend Code. http://bnct.us")
                    .Do(async e =>
                    {
                        //PSUEDO: Check for an existing profile.
                        //PSUEDO: If it exists, update the BNCT Code
                        //PSUEDO: Else, create the profile/ and update the BNCT Friend Code.
                        await e.Channel.SendMessage("Updated BNCT Friend Code.");
                    });
            });
        }

        //TODO: Implement all XML shenanagins, including profile checks, updates, and creation in centralized methods

    }
}
