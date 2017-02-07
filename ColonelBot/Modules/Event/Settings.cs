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
            public string UserID { get; set; }
            public string NetbattlerName { get; set; }
        }

        public ConcurrentDictionary<string, Registration> Registrations = new ConcurrentDictionary<string, Registration>();

        public bool AddRegistration(string BattlerName, string UID)
            => Registrations.TryAdd(UID, new Registration { NetbattlerName = BattlerName }); //This may need to be flipped.
        //Rewrite this all.
        public bool RemoveRegistration(string UID)
        {
            Registration ignored;
            return Registrations.TryRemove(UID, out ignored);
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
        public bool IsEventActive = true; //Set to true for testing/dev.
    }
}
