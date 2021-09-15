using System.IO;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;

namespace Captain_Alex.Services.Registry
{
    static class BotRegistry
    {
        public static string Token;
        public static DiscordSocketClient Client;
        public static CommandService CommandService;

        public static bool FillRegistry(DiscordSocketClient client, CommandService commandService)
        {
            Client = client;
            CommandService = commandService;
            var botString = File.ReadAllText(@"Config\bot.json");
            if (botString == "")
            {
                return false;
            }
            var bot = JObject.Parse(botString);
            
            Token = (string)bot.GetValue("Token");
            
            return true;
        }
    }
}