﻿using System;
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
using System.Xml;
using ColonelBot;
using ColonelBot.Services;
using System.ServiceModel.Syndication;

namespace ColonelBot.Modules.Feeds
{
    class FeedModule : IModule
    {
        public class Article
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
            Console.WriteLine("Installed Feeds Module.");
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
                        {
                            channel = _client.GetChannel(210751707431305217);
                            Console.WriteLine("connected to 210751707431305217");
                        }
                        else
                            channel = e.Channel;
                        if (channel == null)
                        {
                            Console.WriteLine("invalid");
                            return;
                        }
                        if (settings.AddFeed(e.Args[0], channel.Id))
                        {
                            Console.WriteLine("you actually go here?");
                            await _settings.Save(e.Server, settings);
                            await e.Channel.SendMessage($"Linked feed {e.Args[0]} to {channel.Name}");
                        }
                        else
                            await e.Channel.SendMessage($"Feed {e.Args[0]} is already linked to a channel.");
                    });
                group.CreateCommand("remove")
                   .Parameter("url")
                   .Do(async e =>
                   {
                       var settings = _settings.Load(e.Server);
                       if (settings.RemoveFeed(e.Args[0]))
                       {
                           await _settings.Save(e.Server, settings);
                           await e.Channel.SendMessage($"Unlinked feed {e.Args[0]}.");
                       }
                       else
                           await e.Channel.SendMessage($"Feed {e.Args[0]} is not currently linked to a channel.");
                   });

                _client.Ready += (s, e) =>
                {
                    if (!_isRunning)
                    {
                        Task.Run(Run);
                        Console.WriteLine("Feeds Task Running. ColonelBot v2 is ready for operation.");
                        _isRunning = true;
                    }
                };
            });
        }
        
            public async Task Run()
        {
            var cancelToken = _client.CancelToken;
            try
            {
                while (!_client.CancelToken.IsCancellationRequested)
                {
                    foreach (var settings in _settings.AllServers)
                    {
                        foreach (var feed in settings.Value.Feeds)
                        {
                            try {
                                
                                Discord.Channel channel = _client.GetChannel(feed.Value.ChannelId);
                                DateTimeOffset newestArticleTime = feed.Value.LastUpdate;
                                XmlReader r = XmlNodeReader.Create(feed.Key);
                                SyndicationFeed posts= SyndicationFeed.Load(r);
                                r.Close();
                                foreach(SyndicationItem item in posts.Items)
                                {
                                    if (item.LastUpdatedTime.CompareTo(feed.Value.LastUpdate) > 0)
                                    {
                                        foreach(SyndicationLink link in item.Links) //reddit only has one which links to the comments of the post
                                        {
                                            _client.Log.Info("Feed", $"New article: {item.Title}");
                                            Console.WriteLine(item.Title);
                                            Console.WriteLine("Article written at " + feed.Value.LastUpdate);
                                            Console.WriteLine(link.Uri.OriginalString);
                                            await channel.SendMessage(link.Uri.OriginalString);
                                        }
                                        if (item.LastUpdatedTime.CompareTo(newestArticleTime) > 0)
                                        {
                                            newestArticleTime = item.LastUpdatedTime;
                                        }
                                    }
                                    else
                                    {
                                        // break; //Assuming feed is sorted. Like reddits
                                        // might as well do nothing and be safe.
                                    }

                                }
                                feed.Value.LastUpdate = newestArticleTime;
                                Console.WriteLine("Setting Updatetime to newest Article " + feed.Value.LastUpdate);
                                await _settings.Save(settings.Key, settings.Value);
                            }
                            catch(Exception ex) when (!(ex is TaskCanceledException))
                            {
                                _client.Log.Error("Feed", ex);
                            }



                    }
                        await Task.Delay(2000, cancelToken);
                        //await Task.Delay(1000 * 300, cancelToken); //Wait 5 minutes between updates
                    }
                }
            }
            catch (TaskCanceledException) { }
        }
    }
    }
