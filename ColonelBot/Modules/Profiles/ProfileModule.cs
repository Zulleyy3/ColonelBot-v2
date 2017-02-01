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
using System.Xml;
using System.Xml.Linq;
using ColonelBot;
using ColonelBot.Services;

namespace ColonelBot.Modules.Profiles
{
    class ProfileModule : IModule
    {

        private ModuleManager _manager;
        private DiscordClient _client;
        private bool _isRunning;
        private SettingsManager<Settings> _settings;

        void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _client = manager.Client;
            _settings = _client.GetService<SettingsService>().AddModule<ProfileModule, Settings>(manager);

           

            manager.CreateCommands("profile", group =>
            {
                group.CreateCommand()
                    .Parameter("Mentioned User", ParameterType.Optional)
                    .Description("Views the profile of the Mentioned User. Views your own if nobody is mentioned.")
                    .Do(async e =>
                    {
                        if (e.Args[0] != "")
                        {
                            foreach (User ment in e.Message.MentionedUsers)
                            {
                                await e.Channel.SendMessage(GetProfile(ment.Id.ToString()));
                            }
                        }
                        else
                        {//Viewing the profile(s) of the caller.
                            await e.Channel.SendMessage(GetProfile(e.User.Id.ToString()));   
                        }
                        await e.Channel.SendMessage("Beepboop");
                    });
                group.CreateCommand("3ds")
                    .Parameter("3DS Friend Code", ParameterType.Required)
                    .Description("Updates your profiles' 3DS Friend Code")
                    .Do(async e =>
                    {
                        UpdateField(e.User.Id.ToString(), Fields.N3DS, e.Args[2]);
                        await e.Channel.SendMessage("Updated 3DS Friend Code.");
                    });
                group.CreateCommand("psn")
                    .Parameter("PSN ID", ParameterType.Required)
                    .Description("Updates your profiles' PSN ID.")
                    .Do(async e =>
                    {
                        UpdateField(e.User.Id.ToString(), Fields.PSN, e.Args[2]);
                        await e.Channel.SendMessage("Updated PSN ID.");
                    });
                group.CreateCommand("xbl")
                    .Parameter("Xbox Live Gamertag", ParameterType.Required)
                    .Description("Updates your profiles' Xbox Live Gamertag.")
                    .Do(async e =>
                    {
                        UpdateField(e.User.Id.ToString(), Fields.XBL, e.Args[2]);
                        await e.Channel.SendMessage("Updated Xbox Live Gamertag.");
                    });
                group.CreateCommand("nnid")
                    .Parameter("Nintendo Network ID", ParameterType.Required)
                    .Description("Updates your profiles' Nintendo Network ID.")
                    .Do(async e =>
                    {
                        UpdateField(e.User.Id.ToString(), Fields.NNID, e.Args[2]);
                        await e.Channel.SendMessage("Updated Nintendo Network ID.");
                    });
                group.CreateCommand("cx")
                    .Parameter("Chrono X Mate Code", ParameterType.Required)
                    .Description("Updates your profiles' Chrono X Mate Code")
                    .Do(async e =>
                    {
                        UpdateField(e.User.Id.ToString(), Fields.CX, e.Args[2]);
                        await e.Channel.SendMessage("Updated Chrono X Mate Code.");
                    });
                group.CreateCommand("bnct")
                    .Parameter("Battle Network Cyber Tournament Code", ParameterType.Required)
                    .Description("Updates your profiles' BNCT Friend Code. http://bnct.us")
                    .Do(async e =>
                    {
                        UpdateField(e.User.Id.ToString(), Fields.BNCT, e.Args[2]);
                        await e.Channel.SendMessage("Updated BNCT Friend Code.");
                    });
                group.CreateCommand("lock")
                    .Description("Locks your profile so it's not publicly viewable.")
                    .Do(async e =>
                    {
                        UpdateField(e.User.Id.ToString(), Fields.Locked, "True");
                        await e.Channel.SendMessage("Your profile is now locked.");
                    });
                group.CreateCommand("unlock")
                    .Description("Unlocks your profile so it can be publicly viewed.")
                    .Do(async e =>
                    {
                        UpdateField(e.User.Id.ToString(), Fields.Locked, "False");
                        await e.Channel.SendMessage("Your profile is now unlocked.");
                    });
            });
        }


        //XML Code

        public static string MainDir = FileTools.BotDirectory + "\\Profiles\\";

        public void UpdateField(string userid, Fields target, string value)
        {
            //Phase 1: Check to see if the profile exists.
            if (System.IO.File.Exists(MainDir + userid + ".xml") == false)
            { //The profile does not exist. Generate a new one.
                CreateProfile(userid);
            }

            //Phase 2: Update the node contingent upon the target Field. 

            if (value.Contains("http") == false)
            {//Anti Zulley Code. This disallows URLs being placed in profiles and showing images in Discord.
                switch (target)
                {
                    case Fields.N3DS:
                        ChangeField(value, userid, 2);
                        break;
                    case Fields.XBL:
                        ChangeField(value, userid, 3);
                        break;
                    case Fields.PSN:
                        ChangeField(value, userid, 4);
                        break;
                    case Fields.NNID:
                        ChangeField(value, userid, 5);
                        break;
                    case Fields.CX:
                        ChangeField(value, userid, 6);
                        break;
                    case Fields.BNCT:
                        ChangeField(value, userid, 7);
                        break;
                    case Fields.Locked:
                        if (value == "True")
                            ChangeField("True", userid, 8);
                        else
                            ChangeField("False", userid, 8);
                        break;
                    default:
                        break;
                }
            }
            
            
        }

        private void ChangeField(string CodeToUpdate, string UserID, int IndexToUpdate)
        {
            XmlDocument m_xmld = new XmlDocument();
            m_xmld.Load(MainDir + UserID + ".xml");
            XmlNode XmlMasterNode = m_xmld.SelectSingleNode("/Profile");
            XmlMasterNode.ChildNodes.Item(IndexToUpdate).InnerText = CodeToUpdate;
            m_xmld.Save(MainDir + UserID + ".xml");
        }

        public static string GetProfile(string UserID)
        {//This should only be called after the file is verified to exist.
            string result = "";
            XmlDocument m_xmld = new XmlDocument();
            m_xmld.Load(MainDir + UserID + ".xml");
            XmlNode masterNode = m_xmld.SelectSingleNode("/Profile");

            bool Locked = Convert.ToBoolean(masterNode.ChildNodes.Item(8).InnerText);
            string N3DS = masterNode.ChildNodes.Item(2).InnerText;
            string XBL = masterNode.ChildNodes.Item(3).InnerText;
            string PSN = masterNode.ChildNodes.Item(4).InnerText;
            string NNID = masterNode.ChildNodes.Item(5).InnerText;
            string CX = masterNode.ChildNodes.Item(6).InnerText;
            string BNCT = masterNode.ChildNodes.Item(7).InnerText;

            if (Locked)
                result = "Profile Locked.";
            else
            {
                result = "User Profile for <@" + UserID + ">" +
                    "\n==================================\n";
                if (N3DS != "")
                    result += "**Nintendo 3DS Friend Code:** " + N3DS + "\n";
                if (XBL != "")
                    result += "**Xbox Live Gamertag:** " + XBL + "\n";
                if (PSN != "")
                    result += "**PSN ID:** " + PSN + "\n";
                if (NNID != "")
                    result += "**Nintendo Network ID:** " + NNID + "\n";
                if (CX != "")
                    result += "**Chrono X Mate Code:** " + CX + "\n";
                if (BNCT != "")
                    result += "**BN Cyber Tournament Friend Code:** " + BNCT;
            }
            return result;
        }

        private void CreateProfile(string UserID)
        {
            XmlTextWriter writer = new XmlTextWriter(MainDir + UserID + ".xml", System.Text.Encoding.UTF8);
            writer.WriteStartDocument(true);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;
            writer.WriteStartElement("Profile");
            CreateNode("UserID", UserID, writer);
            CreateNode("Nickname", "", writer);
            CreateNode("N3DS", "", writer);
            CreateNode("XBL", "", writer);
            CreateNode("PSN", "", writer);
            CreateNode("NNID", "", writer);
            CreateNode("CX", "", writer);
            CreateNode("BNCT", "", writer);
            CreateNode("Locked", "False", writer);
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
        }

        private void CreateNode(string NodeName, string NodeValue, XmlTextWriter tWriter)
        {
            tWriter.WriteStartElement(NodeName);
            tWriter.WriteString(NodeValue);
            tWriter.WriteEndElement();
        }

        //TODO: Implement all XML shenanagins, including profile checks, updates, and creation in centralized methods
        public  enum Fields
        {
            N3DS,
            XBL,
            PSN,
            NNID,
            CX,
            BNCT,
            Locked
        }
    }
}
