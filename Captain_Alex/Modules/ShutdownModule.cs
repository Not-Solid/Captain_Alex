using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Captain_Alex.Services.Handlers;
using Captain_Alex.Services.Registry;

namespace Captain_Alex.Modules
{
    public class ShutdownModule : ModuleBase<SocketCommandContext>
    {
        public static bool TurnedOn = true;
        
        [Command("off")]
        [Summary("Disables the bot so it wont listen to any commands")]
        public async Task TurnOff()
        {
            var embed = EmbedHandler.getBaseEmbed(Context.User);
            
            if (Context.User is SocketGuildUser user)
            {
                if(!user.Roles.Any(r => GuildRegistry.AdminRoleIdList.Contains(r.Id)))
                {
                    embed.WithDescription("Only Admins are allowed to use that command!")
                         .WithColor(Color.Red);
                    await ReplyAsync("", false, embed.Build());
                    
                    return;
                }
            }
            
            if (TurnedOn == true)
            {
                TurnedOn = false;
            }
            else
            {
                embed.WithTitle("Shutdown")
                     .AddField("Bot status",
                        "Already turned OFF!")
                     .WithColor(Color.Red);
                await ReplyAsync("", false, embed.Build());
                
                return;
            }
            
            embed.WithTitle("Shutdown")
                 .AddField("Bot status",
                    "TURNED OFF")
                 .WithColor(Color.Green);
            await ReplyAsync("", false, embed.Build());

            embed.AddField("Called by",
                $"<@{Context.User.Id}>")
                 .WithColor(Color.Orange);
            var logChannel = BotRegistry.Client.Guilds.First().GetTextChannel(GuildRegistry.TC_Logs);
            await logChannel.SendMessageAsync("", false, embed.Build());
        }
        
        [Command("on")]
        [Summary("Re-enables the bot so it will continue to listen to commands")]
        public async Task TurnOn()
        {
            var embed = EmbedHandler.getBaseEmbed(Context.User);
            
            if (Context.User is SocketGuildUser user)
            {
                if(!user.Roles.Any(r => GuildRegistry.AdminRoleIdList.Contains(r.Id)))
                {
                    embed.WithDescription("Only Admins are allowed to use that command!")
                        .WithColor(Color.Red);
                    await ReplyAsync("", false, embed.Build());
                    
                    return;
                }
            }
            
            if (TurnedOn == false)
            {
                TurnedOn = true;
            }
            else
            {
                embed.WithTitle("Reboot")
                     .AddField("Bot status",
                        "Already turned ON!")
                     .WithColor(Color.Red);
                await ReplyAsync("", false, embed.Build());
                
                return;
            }
            
            embed.WithTitle("Reboot")
                 .AddField("Bot status",
                    "TURNED ON")
                 .WithColor(Color.Green);
            await ReplyAsync("", false, embed.Build());

            embed.AddField("Called by",
                $"<@{Context.User.Id}>")
                 .WithColor(Color.Orange);
            var logChannel = BotRegistry.Client.Guilds.First().GetTextChannel(GuildRegistry.TC_Logs);
            await logChannel.SendMessageAsync("", false, embed.Build());
        }
    }
}