using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Captain_Alex.Services.Registry
{
    static class GuildRegistry
    {
        public static string GuildName;
        public static ulong GuildId;
        public static ulong TC_Main;
        public static ulong TC_Bot;
        public static ulong TC_Reserved;
        public static ulong TC_Logs;
        public static ulong TC_Announcements;
        public static ulong TC_Ranks;
        public static ulong TC_Rules;
        public static ulong TC_ArtContest;
        public static ulong R_BotAdmin;
        public static ulong R_CommunityManager;
        public static ulong R_CommunityAdmin;
        public static ulong R_Mods;

        public static List<ulong> AdminRoleIdList = new List<ulong>();
        public static List<ulong> UnpurgableChannelIdList = new List<ulong>();

        public static bool FillRegistry()
        {
            var guildString = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"Config\guild.json");
            if (guildString == "")
            {
                return false;
            }
            var guild = JObject.Parse(guildString);
            
            GuildName = (string)guild.GetValue("GuildName");
            GuildId = (ulong)guild.GetValue("GuildId");
            TC_Main = (ulong)guild.GetValue("TC_Main");
            TC_Bot = (ulong)guild.GetValue("TC_Bot");
            TC_Reserved = (ulong)guild.GetValue("TC_Reserved");
            TC_Logs = (ulong)guild.GetValue("TC_Logs");
            UnpurgableChannelIdList.Add(TC_Announcements = (ulong)guild.GetValue("TC_Announcements"));
            UnpurgableChannelIdList.Add(TC_Ranks = (ulong)guild.GetValue("TC_Ranks"));
            UnpurgableChannelIdList.Add(TC_Rules = (ulong)guild.GetValue("TC_Rules"));
            UnpurgableChannelIdList.Add(TC_ArtContest = (ulong)guild.GetValue("TC_ArtContest"));
            AdminRoleIdList.Add(R_BotAdmin = (ulong)guild.GetValue("R_BotAdmin"));
            AdminRoleIdList.Add(R_BotAdmin = (ulong)guild.GetValue("R_BotAdmin"));
            AdminRoleIdList.Add(R_CommunityManager = (ulong)guild.GetValue("R_CommunityManager"));
            AdminRoleIdList.Add(R_CommunityAdmin = (ulong)guild.GetValue("R_CommunityAdmin"));
            AdminRoleIdList.Add(R_Mods = (ulong)guild.GetValue("R_Mods"));
            
            return true;
        }
    }
}