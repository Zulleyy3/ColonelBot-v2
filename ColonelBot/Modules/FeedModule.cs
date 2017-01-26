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

namespace ColonelBot.Modules
{
    class FeedModule : IModule
    {
        public class Artucle
        {
            public string Title;
            public string Link;
            public DateTimeOffset PublishedAt;
        }

        private ModuleManager _manager;
        private DiscordClient _client;
        private bool _isRunning;
        //private HttpService _http;
        //private SettingsManager<Settings> _settings;


        void IModule.Install(ModuleManager manager)
        {

        }
    }
}
