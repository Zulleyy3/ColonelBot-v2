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

namespace ColonelBot.Modules.AutoModeration
{
    class Settings
    {
        public string OneDriveBlacklistLocation;
        public string OneDriveLogFile;
        public Channel ReportChannel;
        public bool AutoDelete; //Switch to enable/disable automatic deletion
        public bool AutoReport; //Switch to enable/disable reporting to the Chat Channel

    }
}
