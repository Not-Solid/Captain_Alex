using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Captain_Alex.Services;
using Captain_Alex.Services.Handlers;

namespace Captain_Alex
{
    class Program
    {
        private static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();
        
        private DiscordSocketClient _client;

        private async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            
            var commandConfig = new CommandServiceConfig{ DefaultRunMode = RunMode.Async };
            var commandHandler = new CommandHandler(_client, new CommandService(commandConfig));
            await commandHandler.InstallCommandsAsync();
            
            RegistryHandler.FillRegistries(_client, commandHandler.getCommandService());
            
            await InitHandler.HandleInit(_client);

            var scheduleHandler = new ScheduleHandler();
            await scheduleHandler.HandleSchedules();

            var reactionHandler = new ReactionHandler();
            reactionHandler.HandleReactions(_client);
            
            await Task.Delay(-1);
        }
    }
}