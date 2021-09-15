using Discord;
using Captain_Alex.Services.Registry;

namespace Captain_Alex.Services.Handlers
{
    public static class EmbedHandler
    {
        public static EmbedBuilder getBaseEmbed(IUser author = null)
        {
            author = (author == null) ? BotRegistry.Client.CurrentUser : author;
            
            var embed = new EmbedBuilder();
            embed.WithAuthor(author)
                .WithFooter(footer => footer.Text = "Bot made by Not Solid")
                .WithColor(Color.Orange)
                .WithCurrentTimestamp();

            return embed;
        }
    }
}