using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Captain_Alex.Modules;
using Captain_Alex.Services.Registry;

namespace Captain_Alex.Services.Handlers
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        // Retrieve client and CommandService instance via ctor
        public CommandHandler(DiscordSocketClient client, CommandService commands)
        {
            _commands = commands;
            _client = client;
        }

        public CommandService getCommandService()
        {
            return _commands;
        }
        
        public async Task InstallCommandsAsync()
        {
            // Hook the MessageReceived event into our command handler
            _client.MessageReceived += HandleCommandAsync;

            // Here we discover all of the command modules in the entry 
            // assembly and load them. Starting from Discord.NET 2.0, a
            // service provider is required to be passed into the
            // module registration method to inject the 
            // required dependencies.
            //
            // If you do not use Dependency Injection, pass null.
            // See Dependency Injection guide for more information.
            await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), 
                                            services: null);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasCharPrefix('€', ref argPos) || 
                message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;
            
            // Do not accept dms
            if (message.Channel is Discord.IDMChannel || message.Channel is IPrivateChannel) return;

            // Bot should not react to messages that are sent on reserved text channels
            // These channels typically have a bot that has the same prefix and would thus collide with one another
            if (message.Channel.Id == GuildRegistry.TC_Reserved) return;

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(_client, message);

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            await _commands.ExecuteAsync(
                context: context, 
                argPos: argPos,
                services: null);
        }

        public static bool IsPostedInCorrectChannel(SocketUserMessage message, string classname)
        {
            // If the bot has been turned off via the shutdown command, it must not react to any commands
            if (ShutdownModule.TurnedOn == false && classname.ToLower() != "shutdownmodule")
            {
                return false;
            }
            
            return true;
        }
    }
}