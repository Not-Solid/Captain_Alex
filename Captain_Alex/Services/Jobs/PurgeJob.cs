using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Quartz;
using Captain_Alex.Modules;
using Captain_Alex.Services.Registry;

namespace Captain_Alex.Services.Jobs
{
    public class PurgeJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var guild = BotRegistry.Client.Guilds.First();
            var textChannels = guild.TextChannels;
            var purger = new PurgeModule();
            
            foreach (ITextChannel channel in textChannels)
            {
                try
                {
                    await purger.DeleteMessages(channel);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}