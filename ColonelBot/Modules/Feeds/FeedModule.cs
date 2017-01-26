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

namespace ColonelBot.Modules.Feeds
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
        private HttpService _http;
        private SettingsManager<Settings> _settings;


        void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _client = manager.Client;
            _http = _client.GetService<HttpService>();
            _settings = _client.GetService<SettingsService>().AddModule<FeedModule, Settings>(manager);

            manager.CreateCommands("feeds", group =>
            {
                group.CreateCommand("add")
                    .Parameter("url")
                    .Parameter("channel", ParameterType.Optional)
                    .Do(async e =>
                    {
                        var settings = _settings.Load(e.Server);
                        Channel channel;
                        if (e.Args[1] != "")
                            channel = await e.Server.FindChannels("reddit-feed", ChannelType.Text, true);
                        //TODO: Finish this crap
                        else
                            channel = e.Channel;
                        if (channel == null) return;

                        
                    });
            });
        }
    }
}
