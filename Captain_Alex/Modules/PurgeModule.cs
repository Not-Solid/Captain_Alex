using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using Captain_Alex.Services.Handlers;
using Captain_Alex.Services.Registry;

namespace Captain_Alex.Modules
{
    public class PurgeModule : ModuleBase<SocketCommandContext>
    {
        
        [Command("purge")]
        [Summary("Deletes a specific amount of messages using a bad word filter")]
        public async Task PurgeChannel(int amount = -1)
        {
            if (!CommandHandler.IsPostedInCorrectChannel(Context.Message, GetType().Name))
                return;
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
            
            await DeleteMessages((ITextChannel)Context.Channel, amount);
        }
        
        [Command("purgeall")]
        [Summary("Deletes a specific amount of messages using a bad word filter from all channels")]
        public async Task PurgeAllChannels(int amount = -1)
        {
            if (!CommandHandler.IsPostedInCorrectChannel(Context.Message, GetType().Name))
                return;
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
            
            var guild = BotRegistry.Client.Guilds.First();
            var textChannels = guild.TextChannels;

            foreach (ITextChannel channel in textChannels)
            {
                try
                {
                    await DeleteMessages(channel, amount);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public async Task DeleteMessages(ITextChannel channel, int amount = -1)
        {
            Console.WriteLine(channel.Name);
            //var purgeMessages = new List<IMessage>();
            
            //purgeMessages.Add(await channel.SendMessageAsync("Purging bad words..."));
            
            IMessage latestMessage = null;
            var reachedLastMessage = false;
            var badWords = GetBadWords();
            var badMessageIds = new List<ulong>();
            
            do
            {
                IEnumerable<IMessage> messages;

                if (amount != -1)
                {
                    var messageBulkLimit = (amount <= 100) ? amount : 100;
                    amount -= messageBulkLimit;
                    reachedLastMessage = amount == 0;
                } else
                {

                }
                
                if (latestMessage == null)
                {
                    messages = await channel.GetMessagesAsync().FlattenAsync();
                }
                else
                {
                    messages = await channel.GetMessagesAsync(latestMessage, Direction.Before).FlattenAsync();
                }
                
                if (messages.Any())
                {
                    messages = messages.Where(x => (DateTimeOffset.UtcNow - x.Timestamp).TotalDays <= 14);
                    if (!messages.Any())
                    {
                        break;
                    }
                    
                    foreach (IMessage message in messages)
                    {
                        if (GuildRegistry.UnpurgableChannelIdList.Contains(message.Channel.Id))
                        {
                            if (messages.Last() == message)
                            {
                                reachedLastMessage = true;
                                break;
                            } 
                            else 
                            { 
                                continue; 
                            }
                        }

                        if (message == messages.Last())
                        {
                            latestMessage = message;
                        }

                        foreach (var badWord in badWords)
                        {
                            if (badWord.Value.ToString() == "contains")
                            {
                                if (message.Content.ToLower().Contains(badWord.Key.ToLower()))
                                {
                                    if (!badMessageIds.Contains(message.Id))
                                    {
                                        badMessageIds.Add(message.Id);
                                        break;
                                    }
                                }
                            } 
                            else if (badWord.Value.ToString() == "equals")
                            {
                                if (message.Content.ToLower().Equals(badWord.Key.ToLower()))
                                {
                                    if (!badMessageIds.Contains(message.Id))
                                    {
                                        badMessageIds.Add(message.Id);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    reachedLastMessage = true;
                }
                
            } while (!reachedLastMessage);

            if (badMessageIds.Count <= 0) return;
           
            IList<ulong> tempBadMessageIds = new List<ulong>();
            var counter = 0;
            var deletedMessages = badMessageIds.Count;
            
            foreach (var badMessageId in badMessageIds)
            {
                counter++;
                tempBadMessageIds.Add(badMessageId);
                if (tempBadMessageIds.Count == 100 || counter == deletedMessages)
                {
                    try
                    {
                        await ((ITextChannel) channel).DeleteMessagesAsync(tempBadMessageIds);
                        tempBadMessageIds = new List<ulong>();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }

            if (deletedMessages > 0)
            {
                var purger = (Context != null) ? Context.User.Id : BotRegistry.Client.CurrentUser.Id;
            
                var embed = EmbedHandler.getBaseEmbed(BotRegistry.Client.GetUser(purger));
            
                embed.AddField("Purged channel",
                        $"<#{channel.Id}>")
                    .AddField("Deleted messages",
                        $"{deletedMessages}")
                    .WithTitle("Message Purge");

                ITextChannel logChannel = BotRegistry.Client.Guilds.First().GetTextChannel(GuildRegistry.TC_Logs);
                await logChannel.SendMessageAsync("", false, embed.Build());
                await Task.Delay(10000); //rate limit
            }
        }
        
        public static JObject GetBadWords()
        {
            string badWordsString = File.ReadAllText(@"Config\wordfilter.json");
            if (badWordsString == "")
            {
                return null;
            }
            
            var badWordsJObject = JObject.Parse(badWordsString);
            
            return badWordsJObject;
        }
    }
}