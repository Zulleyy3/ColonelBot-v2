using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ColonelBot
{
    class FileTools
    {
        public static string BotDirectory = new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location).Directory.ToString();
 
        public static string BuildWelcomeMessage()
        {
            string result = "ColonelBot v2 N1 Grand Prix Welcome Kit\n=====================";
            result +=
@"
- Netbattle 101 [Guide for getting started with Netbattling]:   <http://bit.ly/1Rr14oN>
- Netbattle 101 EX [Advanced guide containing deep detail]:     <http://bit.ly/1RszYzG>
- Netbattle 101 Video Guides:                                   <https://www.youtube.com/playlist?list=PLZURQZLRE85FfM3W0dlsCtqn2clgGG-vm>
- ModCard Guides:                                               <http://bombch.us/CHCd>
- Participant OneDrive [Saves, Patches]:                        <https://1drv.ms/f/s!AlnkPup_z_U0tZUD-gZeNJ6BsSnkuA>
- Hamachi Download:                                             <http://vpn.net/>
";
            return result;

        }

        public static string BuildHamachiMessage()
        {
            string result = "**N1GP Hamachi Information**\n\n";
            result += "Server Name: " + File.ReadAllText(BotDirectory + "//Config//HamachiServerName.txt") + " \n";
            result += "Server Password: " + File.ReadAllText(BotDirectory + "//Config//HamachiServerPassword.txt");
            return result;
        }

        public static string BuildOneDriveMessage()
        {
            return "Participant OneDrive (Containing patches, saves, guides, and more: " + File.ReadAllText(BotDirectory + "//Config//ParticipantOneDrive.txt");
        }
    }
}
