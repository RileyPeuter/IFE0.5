using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace IFE_0._3
{
    internal class GameController
    {
        public bool acceptingCoords = false;
        public Dictionary<int, int> captVoteData = new Dictionary<int,int>();
        public Dictionary<int, int> jumpVoteData = new Dictionary<int, int>();
        List<Player> players = new List<Player>();
        public Universe currentTarget = new Universe(0);
        Universe currentUniverse;
        int targetCount = 0;
        UniverseMap map;
        public DiscordChannel gameChannel;
        public bool evasiveManuvers = false;

        List<ulong?> serverCleanupList = new List<ulong?>();

        public async void cleanupChannel()
        {
            List <DiscordMessage> messages = new List<DiscordMessage>();
            foreach (ulong msgID in serverCleanupList)
            {
                //Fix this
                var y = await gameChannel.GetMessageAsync(msgID);
                gameChannel.DeleteMessageAsync(y);
            }
        }

        public List<Player> getPlayerList()
        {
            return players;
        }

        public int[] getTargets()
        {
            return map.targetU;
        }

        public int[] getVoid()
        {
            return map.voidU;
        }

        public GameController()
        {
            map = new UniverseMap();
            //Bro, this line is wack, we gotta fix this
            currentTarget = map.map[map.targetU[0]];
            currentUniverse = map.map[0];

        }

        public void intro()
        {
            foreach(Player p in players)
            {
                if (p.abyss)
                {
                    var msg = new DiscordMessageBuilder().WithContent("" +
                        "These filth do not know what you do. They have not experienced the utter black as you have" +
                        "These fools press their ears up against their windows and mistake the last whimper of the past as a guiding light" +
                        "You will let this existence end. Help me stuff it out so we can begin this cursed existance anew" +
                        "").SendAsync(p.getChannel());
                }
                else
                {
                    var msg = new DiscordMessageBuilder().WithContent("" +
                        " Everything must end. No matter for far our technology and progress goes" +
                        "" +
                        "" +
                        "").SendAsync(p.getChannel());
                }
            }
        }

        public void captVote()
        {

            var msg = new DiscordMessageBuilder().WithContent("VoteStuff").SendAsync(gameChannel);
            List<DiscordComponent> y = new List<DiscordComponent>();
            foreach (Player p in players)
            {
                y.Add(new DiscordButtonComponent(DSharpPlus.ButtonStyle.Primary, "captVote*" + p.userPlayer.Username, p.userPlayer.Username, false));
            }

            foreach(Player p in players)
            {
                new DiscordMessageBuilder().WithContent("Vote for your captain").AddComponents(y).SendAsync(p.getChannel());
            }
        }
        
        //Vote Handling
        public void castCaptainVote(int  voteeID)
        {
            int voteCount;
            if (captVoteData.TryGetValue(voteeID, out voteCount))
            {
                captVoteData.Add(voteeID, voteCount + 1);
                return;
            }
            captVoteData.Add(voteeID, 1);
        }

        public void castCoCount(int coVal)
        {
            int voteCount;
            if (captVoteData.TryGetValue(coVal, out voteCount))
            {
                captVoteData.Add(coVal, voteCount + 1);
                return;
            }
            captVoteData.Add(coVal, 1);
        }


        public void assignRoles()
        {
            var msg = new DiscordMessageBuilder().WithContent("With the captain being democratically elected, they will now assign people to their stations").SendAsync(gameChannel);
        }

        public async Task sendClues()
        {
            foreach(Player p in players)
            {
                await p.sendClue(generateClue(p.abyss));
            }

        }

        public async void abilities()
        {
            foreach(Player p in players)
            {
                await p.sendAbilityAsk();
            }
        }

        public void nominate()
        {
            acceptingCoords = true;
        }

        public void numberVote()
        {
            //This could probably be done better. 
            List<DiscordComponent> y = new List<DiscordComponent>();
            int z = 0;
            foreach(KeyValuePair<int ,int> x in  jumpVoteData)
            {
                y.Add(new DiscordButtonComponent(DSharpPlus.ButtonStyle.Primary, "jumpCoord*" + x.Key.ToString(), x.Key.ToString(), false));
             }

            foreach (Player p in players)
            {
                new DiscordMessageBuilder().WithContent("Vote for your next jump").AddComponents(y).SendAsync(p.getChannel());
            }
        }

        public void jump()
        {
            if(currentUniverse.state == UniverseState.abyss)
            {
                new DiscordMessageBuilder().WithContent("As feel the rotational inertia piter out, it's not met with the same feeling of existential inertia that you normally feel");
                gameLose();
                return;
            }
            
        }

        public void gameLose()
        {

        }

        

        public string generateClue(bool aby)
        {
            string output = "";
            Random random = new Random();
            int clueType = random.Next(6);
            switch (clueType)
            {
                //UpperBounds
                case 1:
                    output = "The IFE coordinates are lower than: " + (currentTarget.coordinate + random.Next(100));

                    break;
                //LowerBounds
                case 2: 
                    output = "The IFE coordinates are higher than: " + (currentTarget.coordinate - random.Next(100));
                    break;

                //Random Digit
                    //Broooooo, this is some wacky coding. What i'm doing here is converting the coordinate to a string, then streating it as an arry
                    // of chars, using rng to get an element, then returning that. 
                case 3:
                    output = "The IFE coordinate contains a: " + currentTarget.coordinate.ToString().ElementAt<char>(random.Next(4));
                    break;
                //Modulo
                case 4:
                    int modu = random.Next(5);
                    output = "When divided by " + modu + " there is a remainder of: " + currentTarget.coordinate % modu;
                    break;

                 //Error Catch
                default:
                    output = "Hey, so ya boi, M Riley made a mild fuckywucky and didn't program this correctly. " +
                        "I mean, ahhhhhh, due to the universal continuum central shift the telepathic clue was lost";
                    break;
            }       

            return output;
        }
        public void addPlayer(Player newPlayer)
        {
            players.Add(newPlayer);
        }

        //Cleanup
        //Many of these may be empty
         public void cleanupIntro()
        {
            
        }
        public void cleanupCaptVote()
        {
            Player.cleanUpAll(players);
        }

        public void cleanupAssign()
        {

        }
        public void cleanupClue()
        {

        }
        public void cleanupAbilities()
        {

        }
        public void cleanupNominate()
        {

        }
        public void cleanupVote()
        {

        }
        public void cleanupEnd()
        {

        }


        //Utility for mass cleaning. 
        public void cleanupAll()
        {
            cleanupAbilities();
            cleanupAssign();
            cleanupCaptVote();
            cleanupIntro();
            cleanupNominate(); 
            cleanupVote();
            cleanupEnd();
            cleanupClue();
            
        }
    }
}
