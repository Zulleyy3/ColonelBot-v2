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

            _client.UsingCommands(x =>
            {
                //This lambda installs the Command Service on the bot.
                x.PrefixChar = '!';
                x.HelpMode = HelpMode.Public;
            });

            _client.GetService<CommandService>().CreateGroup("lookup", cgb =>
            {
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



            _client.ExecuteAndWait(async () => { await _client.Connect("TOKEN HERE!", TokenType.Bot); });
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

        
    
     

       
   