using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;

namespace IFE_0._3.Commands
{
    public class CommandModule : BaseCommandModule
    {
        [Command("enrol")]
        public async Task GreetCommand(CommandContext ctx)
        {

            await ctx.RespondAsync("Greetings, command");

            var chan = await ctx.Member.CreateDmChannelAsync();
            Program.ggc.addPlayer(new Player(ctx.Member, chan));
            var msb = new DiscordMessageBuilder().WithContent("Congratulations. You made it to the ship").SendAsync(chan);
            
        }

        [Command("sendClue")]
        public async Task SendClueTest(CommandContext ctx)
        {
            Console.Write("kappa");
            await Program.ggc.sendClues();
        }
         

        [Command("ShowPlayers")]
        public async Task ShowPlayers(CommandContext ctx)
        {
            List<Player> players = Program.ggc.getPlayerList();
            foreach (Player player in players)
            {
                Console.WriteLine(player.toString());
            }
        }

        [Command("showUniverses")]
        public async Task ShowUniverse(CommandContext ctx)
        {
            Console.WriteLine("Target");
            int[] tagets = Program.ggc.getTargets();
            foreach (int ta in tagets)
            {
                Console.WriteLine(ta);
            }
            Console.WriteLine("/n");

            Console.WriteLine("Voids");

            int[] voidUs = Program.ggc.getVoid();
            foreach (int vo in voidUs)
            {
                Console.WriteLine(vo);
            }

        }

        [Command("start")]
        public async Task startGame(CommandContext ctx)
        {

            Program.ggc.intro();
        }

        [Command("testReact")]
        public async Task reactTest(CommandContext ctx)
        {
            var emoji = DiscordEmoji.FromName(Program.discord, ":coffee:");
            var button = new DiscordButtonComponent(
                DSharpPlus.ButtonStyle.Primary,
                "my_button",
                "Heh. Women",
                false,
                new DiscordComponentEmoji(emoji));
            
            var message = await new DiscordMessageBuilder().WithContent("Hehe").AddComponents(button).SendAsync(ctx.Channel);
            //var res = await message.waitFor

            var result = await message.WaitForButtonAsync("Heh. Women");

           if (!result.TimedOut) await ctx.RespondAsync("Thank you!");
        }

    }
}
