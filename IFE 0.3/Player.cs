using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;


enum position
{
    crewMemeber,
    captain,
    pilot,
    security,
    science,
    engineering
}

namespace IFE_0._3
{
    internal class Player
    {
        public bool voted = false;
        static int susCounter = 1;
        public static int idCoutner = 1;
        public int id;
        public string lastClue = "";
        public List<ulong?> cleanupList = new List<ulong?>();

        public static void cleanUpAll(List<Player> x)
        {
            foreach (Player p in x)
            {
                p.cleanUpMessages();
            }
        }

        public static Player getPlayerByDiscordID(List<Player> x, DiscordUser y)
        {
            foreach (Player player in x)
            {
                if (player.userPlayer.Id == y.Id)
                {
                    return player;
                }
            }
            Console.WriteLine("ERROR: Null player search");
            return null;
        }

        public Player(DiscordUser user, DiscordChannel dm)
        {
            userPlayer = user;
            dmChannel = dm;
            id = idCoutner;
            idCoutner = idCoutner + 1;
        }

        public DiscordUser userPlayer;
        position playerPosition = position.crewMemeber;
        string clue = "";
        public bool abyss = false;
        DiscordChannel dmChannel;
        public int lastVote = 0000;

        public DiscordChannel getChannel()
        {
            return dmChannel;
        }

        public string toString()
        {
            string output = "";
            output = output + userPlayer.Username;
            output = output + playerPosition;
            output = output + abyss;
            return output;
        }

        public async Task sendClue(String clue)
        {
            lastClue = clue;
            var msg = new DiscordMessageBuilder().WithContent(clue).SendAsync(dmChannel);
        }

        public async Task sendAbilityAsk()
        {
            var msg = abilityPrompt().SendAsync(dmChannel);
        }

        public static void initializePlayers(List<Player> ps)
        {
            Random rng = new Random();
            if (ps.Count > 5)
            {
                susCounter = 2;
            }

            if (ps.Count > 8)
            {
                susCounter = 3;
            }

            for(int x = 0; x < susCounter; x++)
            {
                bool nFlag = true;
                while (nFlag) {
                    int abyssed = rng.Next(ps.Count);
                    if (!ps[abyssed].abyss)
                    {
                        ps[abyssed].abyss = true;
                        nFlag = false;
                    }

                }
            }
            

        }


        //The below two functions are wacky, we're gonna have to fuck with aysnc stuff. 
        public void sendMessage(DiscordMessageBuilder messageData, bool transient)
        {
            
            messageData.SendAsync(dmChannel);
            if (transient)
            {//This might not correctly add things because the Async Send might have been completed.
                cleanupList.Add(dmChannel.LastMessageId);
            }
        }

        public async void cleanUpMessages()
        {
            foreach(ulong x in cleanupList)
            {
                //Gotta Fix This
                var y = await dmChannel.GetMessageAsync(x);
                dmChannel.DeleteMessageAsync(y);
            } 
        }

        public void dead()
        {

        }

        public DiscordMessageBuilder abilityPrompt()
        {
            string abText = "";
            string buttonText = "";
            string buttonEmojiName = ":coffee:";
            string buttonId = "";

            switch (playerPosition)
            {
                case position.pilot:
                    abText = "Perform Evasive Manuvers?";
                    buttonText = "skrrrrrrt";
                    buttonEmojiName = ":wheel:";
                    buttonId = "abilityPilot";
                    break;

                case position.security:
                    abText = "Interrogate Voter?";
                    buttonText = "Make them sing";
                    buttonEmojiName = ":police_officer:";
                    buttonId = "abilitySecurity";
                    break;

                case position.captain:
                    abText = "Quel Defiance(eliminate a crew member)";
                    buttonText = "Pop tha' glizzy";
                    buttonEmojiName = ":gun:";
                    buttonId = "abilityCaptain";
                    break;

                case position.engineering:
                    abText = "Fine tune equiptment to show correct digits";
                    buttonText = "Dial it in";
                    buttonEmojiName = ":triangular_ruler:";
                    buttonId = "abilityEngineering";
                    break;

                case position.science:
                    abText = "Reveal telepathic communications?";
                    buttonText = "leak them logs";
                    buttonEmojiName = ":1234:";
                    buttonId = "abilityScience";
                    break;  
            }
            var buttonEmoji = DiscordEmoji.FromName(Program.discord, buttonEmojiName);

            return new DiscordMessageBuilder().WithContent(abText).AddComponents(new DiscordButtonComponent(DSharpPlus.ButtonStyle.Primary, buttonId, buttonText, false, new DiscordComponentEmoji(buttonEmoji)));
        }


    }

    
}
