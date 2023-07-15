using System;
using System.Reflection;
using System.Threading.Tasks;
using IFE_0._3;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using IFE_0._3.Commands;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using System.Collections.Generic;
using System.IO;

enum GameStates
{
    intro,
    captVote,
    assign,
    clue,
    abilities,
    nominate,
    vote,
    jump,
    end
}

namespace IFE_0._3
{
    
    class Program
    {

        public static DiscordClient discord;
        public static GameController ggc;


        static GameStates gameState = GameStates.intro;

        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {

            ggc = new GameController();

            CommandsNextConfiguration commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { "/" },
                EnableDms = true
            };



            discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = File.ReadAllText("token.txt"),
                TokenType = TokenType.Bot,
                MinimumLogLevel = LogLevel.Debug,
                Intents = DiscordIntents.All
            });;


            discord.ComponentInteractionCreated += async (s, e) =>
            {
                Console.Write("kappa");
                
                await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent("That is bullshit BLAZING, STILL MY HEART IS BLAZING"));

                if (e.Id.Contains("btnVoteFor"))
                {
                    ggc.castCaptainVote(Int32.Parse(e.Id.Split('*')[1]));
                    return;
                }

                if (e.Id.Contains("jumpCoord*"))
                {
                    ggc.castCoCount(Int32.Parse(e.Id.Split('*')[1]));
                }

                if (e.Id.Contains("abilitySecurity*"))
                {
                    //Ok, this might look wacky, but what it does is get the number at the end of the ID string, finds the player with that ID, then sends its last vote. 
                    //All that is just string processing.
                    int lv = ggc.getPlayerList()[Int32.Parse(e.Id.Split("*")[1])].lastVote;
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent("Thier last vote was "+ lv));
                    return;
                }

                if (e.Id.Contains("abilitySecurity*"))
                {
                    //Ok, this might look wacky, but what it does is get the number at the end of the ID string, finds the player with that ID, then sends its last vote. 
                    //All that is just string processing.
                    ggc.getPlayerList()[Int32.Parse(e.Id.Split("*")[1])].dead();
                    return;
                }

                if (e.Id.Contains("abilityScience*"))
                {
                    //Ok, this might look wacky, but what it does is get the number at the end of the ID string, finds the player with that ID, then sends its last vote. 
                    //All that is just string processing.
                    string lc = ggc.getPlayerList()[Int32.Parse(e.Id.Split("*")[1])].lastClue;
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent("Thier last clue was " + lc));
                    return;
                }


                if (e.Id.Contains("ability"))
                {
                    switch (e.Id)
                    {
                        case "abilityPilot":
                            ggc.evasiveManuvers = true;
                            break;

                        case "abilitySecurity":
                            //This just creates a button for every player in the game. 
                            //Note, it also does this for the player that activates it
                            List<DiscordButtonComponent> abSecButtonList = new List<DiscordButtonComponent>(); ;
                            var abSecButtonEmoji = DiscordEmoji.FromName(Program.discord, ":bucket:");
                            foreach (Player x in ggc.getPlayerList())
                            {
                                abSecButtonList.Add(new DiscordButtonComponent(DSharpPlus.ButtonStyle.Primary, "abilitySecurity*" + x.id, x.userPlayer.Username, false, new DiscordComponentEmoji(abSecButtonEmoji)));
                            }
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddComponents(abSecButtonList));
                            break;

                        case "abilityCaptain":
                            //This just creates a button for every player in the game. 
                            //Note, it also does this for the player that activates it
                            List<DiscordButtonComponent> abCaptButtonList = new List<DiscordButtonComponent>(); ;
                            var abCaptButtonEmoji = DiscordEmoji.FromName(Program.discord, ":gun:");
                            foreach (Player x in ggc.getPlayerList())
                            {
                                abCaptButtonList.Add(new DiscordButtonComponent(DSharpPlus.ButtonStyle.Primary, "abilityCaptain*" + x.id, x.userPlayer.Username, false, new DiscordComponentEmoji(abCaptButtonEmoji)));
                            }
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddComponents(abCaptButtonList));

                            break;

                        case "abilityEngineering":
                            Random rng = new Random(0);
                            int pos = rng.Next(4);
                            char l = ggc.currentTarget.coordinate.ToString()[pos];
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent("The digit in position " + pos + " is " + l));
                            break;

                        case "abilityScience":
                            //This just creates a button for every player in the game. 
                            //Note, it also does this for the player that activates it
                            List<DiscordButtonComponent> abSciButtonList = new List<DiscordButtonComponent>(); ;
                            var abSciButtonEmoji = DiscordEmoji.FromName(Program.discord, ":gun:");
                            foreach (Player x in ggc.getPlayerList())
                            {
                                abSciButtonList.Add(new DiscordButtonComponent(DSharpPlus.ButtonStyle.Primary, "abilityScience*" + x.id, x.userPlayer.Username, false, new DiscordComponentEmoji(abSciButtonEmoji)));
                            }
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddComponents(abSciButtonList));
                            break;

                    }
                }



                if (e.Id.Contains("btnVote"))
                {

                }


                switch (e.Id)
                {
                    case "btnReady":
                        break;


                }

            };

            //Make shit here, motherfucker
            discord.MessageCreated += async (s, e) =>
            {
                //This long arse shit is just to chekc if it's a valid input

                if (ggc.acceptingCoords && e.Channel.Id == ggc.gameChannel.Id)
                {
                    Player x = Player.getPlayerByDiscordID(ggc.getPlayerList(), e.Author);
                    if (!x.voted)
                    {
                        if (Int32.TryParse(e.Message.Content, out int y))
                        {
                            if (y > 0 && y < 10000)
                            {
                                if (!ggc.jumpVoteData.ContainsKey(y))
                                {
                                    ggc.jumpVoteData.Add(y, 1);
                                    return;
                                }
                            }
                        }
                    }
                }

                if (e.Message.Content.ToLower().StartsWith("o"))
                    await e.Message.RespondAsync("k!");
                
            };


            discord.UseInteractivity(new InteractivityConfiguration()
            {
                PollBehaviour = PollBehaviour.KeepEmojis,
                Timeout = TimeSpan.FromSeconds(30)
            });

            var coms = discord.UseCommandsNext(commandsConfig);

            coms.RegisterCommands<CommandModule>();


            await discord.ConnectAsync();
            await Task.Delay(-1);

        }
    }

}