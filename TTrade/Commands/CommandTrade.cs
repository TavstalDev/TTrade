using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tavstal.TLibrary.Models.Commands;
using Tavstal.TLibrary.Models.Plugin;
using Tavstal.TLibrary.Helpers.Unturned;
using Tavstal.Trade.Managers;

namespace Tavstal.Trade.Commands
{
    public class CommandTrade : CommandBase
    {
        protected override IPlugin Plugin => TTrade.Instance; 
        public override AllowedCaller AllowedCaller => AllowedCaller.Player;
        public override string Name => "trade";
        public override string Help => "Securely exchange items with other players through a simple request and confirmation process.";
        public override string Syntax => "[player] | accept | deny | done | open | view | cancel";
        public override List<string> Aliases => new List<string>() { "deal", "swap", "barter" };
        public override List<string> Permissions => new List<string> { "ttrade.command.trade" };

        // 'help' subcommand is built-in, you don't need to add it
        protected override List<SubCommand> SubCommands => new List<SubCommand>()
        {
            new SubCommand("accept", "", "accept [player]", new List<string>() { "allow" }, new List<string>() { "ttrade.command.trade.accept" }, 
                (caller,  args) =>
                {
                    if (args.Length != 1)
                    {
                        ExecuteHelp(caller, true, "accept", args);
                        return Task.CompletedTask;
                    }

                    UnturnedPlayer callerPlayer = (UnturnedPlayer)caller;
                    UnturnedPlayer targetPlayer = UnturnedPlayer.FromName(args[0]);
                    if (targetPlayer == null)
                    {
                        TTrade.Instance.SendCommandReply(caller, "error_player_not_found", args[0]);
                        return Task.CompletedTask;
                    }

                    //VaultManager.SendTradeRequest(callerPlayer, targetPlayer);
                    return Task.CompletedTask;
                }),
            new SubCommand("deny", "", "deny [player]", new List<string>() { "disallow" }, new List<string>() { "ttrade.command.trade.deny" }, 
                (caller,  args) =>
                {
                    if (args.Length != 1)
                    {
                        ExecuteHelp(caller, true, "deny", args);
                        return Task.CompletedTask;
                    }

                    UnturnedPlayer callerPlayer = (UnturnedPlayer)caller;
                    UnturnedPlayer targetPlayer = UnturnedPlayer.FromName(args[0]);
                    if (targetPlayer == null)
                    {
                        TTrade.Instance.SendCommandReply(caller, "error_player_not_found", args[0]);
                        return Task.CompletedTask;
                    }

                    //VaultManager.SendTradeRequest(callerPlayer, targetPlayer);
                    return Task.CompletedTask;
                }),
            new SubCommand("done", "", "done", new List<string>() { "finish", "complete" }, new List<string>() { "ttrade.command.trade.done" }, 
                (caller,  args) =>
                {
                    if (args.Length != 0)
                    {
                        ExecuteHelp(caller, true, "done", args);
                        return Task.CompletedTask;
                    }

                    UnturnedPlayer callerPlayer = (UnturnedPlayer)caller;
                    //VaultManager.SendTradeRequest(callerPlayer, targetPlayer);
                    return Task.CompletedTask;
                }),
            new SubCommand("open", "", "open", new List<string>(), new List<string>() { "ttrade.command.trade.open" }, 
                (caller,  args) =>
                {
                    if (args.Length != 0)
                    {
                        ExecuteHelp(caller, true, "open", args);
                        return Task.CompletedTask;
                    }

                    UnturnedPlayer callerPlayer = (UnturnedPlayer)caller;
                    //VaultManager.SendTradeRequest(callerPlayer, targetPlayer);
                    return Task.CompletedTask;
                }),
            new SubCommand("view", "", "view", new List<string>() { "check" }, new List<string>() { "ttrade.command.trade.view" }, 
                (caller,  args) =>
                {
                    if (args.Length != 0)
                    {
                        ExecuteHelp(caller, true, "view", args);
                        return Task.CompletedTask;
                    }

                    UnturnedPlayer callerPlayer = (UnturnedPlayer)caller;
                    //VaultManager.SendTradeRequest(callerPlayer, targetPlayer);
                    return Task.CompletedTask;
                }),
            new SubCommand("cancel", "", "cancel", new List<string>() { "quit" }, new List<string>() { "ttrade.command.trade.cancel" }, 
                (caller,  args) =>
                {
                    if (args.Length != 0)
                    {
                        ExecuteHelp(caller, true, "cancel", args);
                        return Task.CompletedTask;
                    }

                    UnturnedPlayer callerPlayer = (UnturnedPlayer)caller;
                    //VaultManager.SendTradeRequest(callerPlayer, targetPlayer);
                    return Task.CompletedTask;
                })
        };

        protected override Task<bool> ExecutionRequested(IRocketPlayer caller, string[] args)
        {
            if (args.Length != 1)
            {
                ExecuteHelp(caller, true, null, args);
                return Task.FromResult(true);
            }

            UnturnedPlayer callerPlayer = (UnturnedPlayer)caller;
            UnturnedPlayer targetPlayer = UnturnedPlayer.FromName(args[0]);
            if (targetPlayer == null)
            {
                TTrade.Instance.SendCommandReply(caller, "error_player_not_found", args[0]);
                return Task.FromResult(true);
            }

            VaultManager.SendTradeRequest(callerPlayer, targetPlayer);
            return Task.FromResult(true);
        }
    }
}
