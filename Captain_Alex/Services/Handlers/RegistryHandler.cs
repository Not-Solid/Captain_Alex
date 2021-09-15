using Discord.Commands;
using Discord.WebSocket;
using Captain_Alex.Services.Registry;

namespace Captain_Alex.Services.Handlers
{
    public class RegistryHandler
    {
        public static void FillRegistries(DiscordSocketClient client, CommandService commandService)
        {
            BotRegistry.FillRegistry(client, commandService);
            GuildRegistry.FillRegistry();
        }
    }
}