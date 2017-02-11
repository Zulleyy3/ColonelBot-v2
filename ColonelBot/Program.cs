using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using Discord;
using Discord.Audio;
using Discord.Modules;
using Discord.Commands;
//using ColonelBot.Modules.Twitch;
using NAudio;
using NAudio.Wave;
using NAudio.CoreAudioApi;
using ColonelBot.Modules.Profiles;
using ColonelBot.Modules.Feeds;
using ColonelBot.Modules.Event;
using ColonelBot.Modules.AutoModeration;
using ColonelBot.Modules.Information;
namespace ColonelBot
{
    class Program
    {
        static void Main(string[] args) => new Program().Start();

        private DiscordClient _client;

        public void Start()
        {
            _client = new DiscordClient();

            //ChipLookup.InitializeLibrary();

            //Phase 1: Initial Configuration
            _client.UsingCommands(x =>
            {
                //This lambda installs the Command Service on the bot.
                x.PrefixChar = '!';
                x.HelpMode = HelpMode.Public;
            });

            _client.UsingAudio(x =>
            {
                x.Mode = AudioMode.Outgoing;
                x.EnableEncryption = true;
                x.Bitrate = AudioServiceConfig.MaxBitrate;
                x.BufferLength = 10000;
            });

            

            _client.AddService<Services.SettingsService>(); //Install the settings service to the bot.
            _client.AddService<Services.HttpService>(); //Installs the Http service to the bot.
            _client.UsingModules();
            
            _client.AddModule<ProfileModule>("Profiles", ModuleFilter.None);
            _client.AddModule<FeedModule>("Feeds", ModuleFilter.None);
            _client.AddModule<EventModule>("Event", ModuleFilter.None);
            _client.AddModule<AutoModModule>("AutoModeration", ModuleFilter.None);
            //_client.AddModule<InfoModule>("Information", ModuleFilter.None);

            //Phase 2: Command Implementation

            _client.GetService<CommandService>().CreateGroup("lookup", cgb =>
            {//TODO: Offload this to the Lookup Module
                cgb.CreateCommand("chip")
                    .Description("Looks up a chip by name.")
                    .Parameter("ChipName", ParameterType.Required)
                    .Do(async e =>
                    {
                        await e.Channel.SendMessage("Looking up chip."); //replace later.
                        //Lookup Code Here. Use e.GetArg("ChipName") to obtain the parameter.
                    });
                cgb.CreateCommand("pa")
                    .Description("Looks up a Program Advance by name.")
                    .Parameter("PAName", ParameterType.Required)
                    .Do(async e =>
                    {
                        await e.Channel.SendMessage("Looking up Program Advance");
                    });
                cgb.CreateCommand("code")
                    .Description("Looks up all chips for the specified code.")
                    .Parameter("LookupCode", ParameterType.Required)
                    .Do(async e =>
                    {
                        await e.Channel.SendMessage("Looking up chip by code.");
                    });
            });

            _client.GetService<CommandService>().CreateCommand("sad")
                .Description("Displays a sad meme.")
                .Do(async e =>
                {
                    await e.Channel.SendFile(GetRandomFile("Images/sad"));
                });

            _client.GetService<CommandService>().CreateCommand("smug")
                .Description("Displays a smug meme.")
                .Do(async e =>
                {
                    await e.Channel.SendFile(GetRandomFile("Images/smug"));
                });

            _client.GetService<CommandService>().CreateCommand("savenettokun")
                .Description("SAVE HIM.")
                .Do(async e =>
                {
                    await e.Channel.SendFile(GetRandomFile("Images/savenettokun"));
                });

            _client.GetService<CommandService>().CreateCommand("barrelsan")
                .Description("Honor Barryl the Immortal.")
                .Do(async e =>
                {
                    await e.Channel.SendFile(GetRandomFile("Images/barrelsan"));
                });

            _client.GetService<CommandService>().CreateCommand("hamachi")
                .Description("Asks ColonelBot to Privately Message you the credentials to access the N1GP Hamachi Server.")
                .Do(async e =>
                {
                    await e.User.SendMessage(FileTools.BuildHamachiMessage());
                    await e.Channel.SendMessage("You have e-mail, " + e.User.Name + ".");
                    Console.WriteLine(e.User.Name + " requested Hamachi inforamtion.");
                });

            _client.GetService<CommandService>().CreateCommand("welcome")
                .Description("Provides you everything you need to begin Netbattling with the N1 Grand Prix!")
                .Do(async e =>
                {
                    await e.User.SendMessage(FileTools.BuildWelcomeMessage());
                    await e.Channel.SendMessage("You have e-mail, " + e.User.Name + ".");
                    Console.WriteLine(e.User.Name + " has requested a welcome kit.");
                });
            _client.GetService<CommandService>().CreateCommand("onedrive")
                .Description("Provides the Participant OneDrive link, containing all saves, patches and tournament format information.")
                .Do(async e =>
                {
                    await e.Channel.SendMessage(FileTools.BuildOneDriveMessage());
                });

            _client.GetService<CommandService>().CreateCommand("delete")
                .Parameter("Messages to Delet", ParameterType.Required)
                .Description("Deletes the commands and responses up to the amount of messages specified.")
                .Do(async e =>
                {
                    int toDelete = Convert.ToInt16(e.Args[0] + 1);
                    int deleted;
                    //await deleted = LegacyTools.CleanMessages(e.Channel, toDelete, e.Message);
                    await e.Channel.SendMessage("Deleted.");
                    
                
                });

            //Phase 3: Connect
            _client.ExecuteAndWait(async () => { await _client.Connect(System.IO.File.ReadAllText(FileTools.BotDirectory + "/Config/botKey.txt"), TokenType.Bot); });
            
            _client.SetGame("in your PET.");
        }





            private static string GetRandomFile(string folder)
        {
            //Ported from v1.4. Credit to Prof. 9.
            // TODO: Do this only once.
            string baseDir = new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location).DirectoryName;
            if (baseDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                baseDir = baseDir.Substring(0, baseDir.Length - 1);
            }

            // Remove directory separator chars at the start and end.
            if (folder.StartsWith(Path.DirectorySeparatorChar.ToString()))
            {
                folder = folder.Substring(1);
            }
            if (folder.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                folder = folder.Substring(0, folder.Length - 1);
            }

            // Get the files in the requested folder.
            string dir = baseDir + Path.DirectorySeparatorChar + folder;
            string[] files = Directory.GetFiles(dir);

            // TODO: Use a single RNG.
            int r = new Random().Next(files.Length);

            return files[r];
        }
    }
}

        
    
     

       
   