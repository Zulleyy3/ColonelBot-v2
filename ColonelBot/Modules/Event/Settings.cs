using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace ColonelBot.Modules.Event
{
    class Settings
    {
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
