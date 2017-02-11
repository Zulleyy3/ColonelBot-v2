using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace ColonelBot
{
    class LegacyTools
    {
        public static int CleanMessages(Channel chan, int toDelete, Message fromMsg)
        {
            // Keep track of the messages to delete.
            List<Message> deletedMsgs = new List<Message>();

            // Download the last 100 messages (100 is the max for the Discord API).
            Task<Message[]> downloadTask = chan.DownloadMessages(100, fromMsg.Id, Relative.Before);
            downloadTask.Wait();
            List<Message> msgs = downloadTask.Result.ToList();
            // Add the fromMsg.
            msgs.Insert(0, fromMsg);

            // Cycle through the received messages.
            foreach (Message msg in msgs)
            {
                // Stop when number of requested messages to delete is reached.
                if (deletedMsgs.Count >= toDelete)
                {
                    break;
                }

                bool isMatch = false;

                // Check for command calls that should be cleaned.
                

                // Check for own messages.
                if (msg.IsAuthor)
                {
                    isMatch = true;
                }

                // Delete the message if it matched.
                if (isMatch)
                {
                    deletedMsgs.Add(msg);
                }
            }

            // Delete all the messages that were marked for deletion in one go.
            chan.DeleteMessages(deletedMsgs.ToArray());

            // Return number of messages that were actually deleted.
            return deletedMsgs.Count;
        }
    }
}
