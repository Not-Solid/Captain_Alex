using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Captain_Alex.Services.Registry;

namespace Captain_Alex.Services.Handlers
{
    public class InitHandler
    {
        public static async Task HandleInit(DiscordSocketClient client)
        {
            client.Log += Log;

            var token = BotRegistry.Token;
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
            
            client.Ready += async () =>
            {
                var botChannel = client.GetChannel(GuildRegistry.TC_Bot) as ITextChannel;
                var logChannel = client.GetChannel(GuildRegistry.TC_Logs) as ITextChannel;
                
                var embed = EmbedHandler.getBaseEmbed(BotRegistry.Client.CurrentUser);
                embed.WithTitle("Boot");
                
                if (botChannel != null)
                {
                    embed.WithDescription("Captain_Alex, ready for action!");
                    await botChannel.SendMessageAsync("", false, embed.Build());
                }
                if (logChannel != null)
                {
                    embed.WithDescription("Captain_Alex, ready!");
                    await logChannel.SendMessageAsync("", false, embed.Build());
                }
            };
        }
        
        private static Task Log(LogMessage message)
        {
            Console.WriteLine(message);
            return Task.CompletedTask;
        }
        
    }
}