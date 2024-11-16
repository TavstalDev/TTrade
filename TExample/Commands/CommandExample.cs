using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tavstal.TLibrary.Models.Commands;
using Tavstal.TLibrary.Models.Plugin;
using Tavstal.TLibrary.Helpers.Unturned;

namespace Tavstal.TExample.Commands
{
    public class CommandExample : CommandBase
    {
        protected override IPlugin Plugin => ExampleMain.Instance; 
        public override AllowedCaller AllowedCaller => AllowedCaller.Both;
        public override string Name => "example";
        public override string Help => "This is an example description what the command does.";
        public override string Syntax => "";
        public override List<string> Aliases => new List<string>() { "ex" };
        public override List<string> Permissions => new List<string> { "texample.command.example" };

        // 'help' subcommand is built-in, you don't need to add it
        protected override List<SubCommand> SubCommands => new List<SubCommand>()
        {
            new SubCommand("hi", "Example subcommand for the command", "hi <player>", new List<string>() { "hello" }, new List<string>() { "texample.command.example.hi" }, 
                (caller,  args) =>
                {
                    if (args.Length == 1)
                    {
                        UnturnedPlayer player = UnturnedPlayer.FromName(args[0]);
                        if (player == null)
                        {
                            Plugin.SendCommandReply(caller, "error_player_not_found");
                            return Task.CompletedTask;
                        }

                        Plugin.SendPlainCommandReply(player, "&aHello world!");
                        Plugin.SendCommandReply(caller, "success_command_example_hi_sent");
                    }
                    else
                    {
                        Plugin.SendPlainCommandReply(caller, "Hello world!");
                    }
                    return Task.CompletedTask;
                })
        };

        protected override Task<bool> ExecutionRequested(IRocketPlayer caller, string[] args)
        {
            // Called if there was no subcommand to execute
            // Add stuff what the command does he
            // return true if you don't want to send the command usage to the caller, otherwise return false
            return Task.FromResult(true);
        }
    }
}
