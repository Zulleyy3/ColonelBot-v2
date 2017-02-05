using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.Collections.Concurrent;

namespace ColonelBot.Modules.Event
{
    class Settings
    {
        public class Registration
        {
            public ulong ChannelId { get; set; }
            public string NetbattlerName { get; set; }
        }

        public ConcurrentDictionary<string, Registration> Registrations = new ConcurrentDictionary<string, Registration>();

        public bool AddRegistration(string url, ulong channelId)
            => Registrations.TryAdd(url, new Registration { ChannelId = channelId });

        public bool RemoveRegistration(string url)
        {
            Registration ignored;
            return Registrations.TryRemove(url, out ignored);
        }

        public string CurrentEventTitle = "None";
        public string CurrentEventURL = "None";
        public string EventDescription = "None";
        public string RegistrationURL = "None";
        public string RegistrantXMLFile = "None";
        public DateTime EventStartDateTime;
        public string BracketURL = "None";
        public bool BracketHidden = false;
        public string StreamChannelURL = "None";
        public Channel EventChatChannel;
        public Role EventParticipantRole;
    }
}
