using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Captain_Alex.Services.Handlers;
using Captain_Alex.Services.Registry;

namespace Captain_Alex.Modules
{
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        [Summary("Lists all commands")]
        public async Task CommandList()
        {
            List<CommandInfo> commands = BotRegistry.CommandService.Commands.ToList();
            var embed = EmbedHandler.getBaseEmbed(Context.User);
            var page = 1;
            embed.WithTitle($"Command List Page #{page}");
            var lastCommand = "";
            
            foreach (CommandInfo command in commands)
            {
                if (lastCommand == command.Name)
                {
                    continue;
                }

                lastCommand = command.Name;
                // Get the command Summary attribute information
                string embedFieldText = command.Summary ?? "No description available\n";

                embed.AddField("$"+command.Name, embedFieldText);
                if (embed.Fields.Count == 5 || command == commands.Last())
                {
                    await Context.User.SendMessageAsync("", false, embed.Build());
                    page++;
                    embed = EmbedHandler.getBaseEmbed(Context.User);
                    embed.WithTitle($"Command List Page #{page}");
                }
            }

            embed = EmbedHandler.getBaseEmbed(Context.User);
            embed.WithDescription($"<@{Context.User.Id}>, check your dms!");
            await ReplyAsync("", false, embed.Build());
        }
    }
}