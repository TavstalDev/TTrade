using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using System.Threading.Tasks;
using Steamworks;
using Tavstal.TLibrary.Models.Commands;
using Tavstal.TLibrary.Models.Plugin;
using Tavstal.TLibrary.Helpers.Unturned;
using Tavstal.Trade.Components;
using Tavstal.Trade.Managers;
using Tavstal.Trade.Models;
using UnityEngine;

namespace Tavstal.Trade.Commands
{
    public class CommandTrade : CommandBase
    {
        protected override IPlugin Plugin => TTrade.Instance; 
        public override AllowedCaller AllowedCaller => AllowedCaller.Player;
        public override string Name => "trade";
        public override string Help => "Securely exchange items with other players through a simple request and confirmation process.";
        public override string Syntax => "[player] | accept | deny | open | view | finish | cancel";
        public override List<string> Aliases => new List<string>() { "deal", "swap", "barter" };
        public override List<string> Permissions => new List<string> { "ttrade.command.trade" };

        // 'help' subcommand is built-in, you don't need to add it
        protected override List<SubCommand> SubCommands => new List<SubCommand>()
        {
            new SubCommand("accept", "Accept a pending trade request.", "accept [player]", new List<string>() { "allow", "approve" }, new List<string>() { "ttrade.command.trade.accept" }, 
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

                    TradeManager.AcceptTradeRequest(callerPlayer, targetPlayer);
                    return Task.CompletedTask;
                }),
            new SubCommand("deny", "Decline a trade request.", "deny [player]", new List<string>() { "disallow", "disapprove" }, new List<string>() { "ttrade.command.trade.deny" }, 
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

                    TradeManager.DenyTradeRequest(callerPlayer, targetPlayer);
                    return Task.CompletedTask;
                }),
            new SubCommand("finish", "Complete, then finalize the trade.", "finish", new List<string>() { "done", "complete" }, new List<string>() { "ttrade.command.trade.finish" }, 
                (caller,  args) =>
                {
                    if (args.Length != 0)
                    {
                        ExecuteHelp(caller, true, "done", args);
                        return Task.CompletedTask;
                    }

                    UnturnedPlayer callerPlayer = (UnturnedPlayer)caller;
                    TradeComponent comp = callerPlayer.GetComponent<TradeComponent>();
                    if (comp.State == ETradeState.None)
                    {
                        TTrade.Instance.SendCommandReply(callerPlayer, "error_not_in_trade");
                        return Task.CompletedTask;
                    }

                    if (comp.State == ETradeState.Finished)
                    {
                        TTrade.Instance.SendCommandReply(callerPlayer, "error_wait_partner_finish");
                        return Task.CompletedTask;
                    }

                    if (comp.State == ETradeState.Approved)
                        TradeManager.FinishTrade(callerPlayer);
                    else
                        TradeManager.ApproveTrade(callerPlayer);
                    return Task.CompletedTask;
                }),
            new SubCommand("open", "Open the trade inventory.", "open", new List<string>(), new List<string>() { "ttrade.command.trade.open" }, 
                (caller,  args) =>
                {
                    if (args.Length != 0)
                    {
                        ExecuteHelp(caller, true, "open", args);
                        return Task.CompletedTask;
                    }

                    UnturnedPlayer callerPlayer = (UnturnedPlayer)caller;
                    TradeComponent comp = callerPlayer.GetComponent<TradeComponent>();
                    
                    if (comp.State == ETradeState.None)
                    {
                        TTrade.Instance.SendCommandReply(callerPlayer, "error_not_in_trade");
                        return Task.CompletedTask;
                    }

                    UnturnedPlayer tradePartner = UnturnedPlayer.FromCSteamID((CSteamID)comp.TradePartner);
                    if (tradePartner == null)
                    {
                        TTrade.Instance.SendCommandReply(callerPlayer, "error_trade_partner_not_found");
                        return Task.CompletedTask;
                    }
                    
                    float distance = Vector3.Distance(callerPlayer.Position, tradePartner.Position);
                    if (distance > TTrade.Instance.Config.Distance)
                    {
                        TTrade.Instance.SendCommandReply(callerPlayer, "error_trade_distance", distance, TTrade.Instance.Config.Distance);
                        return Task.CompletedTask;
                    }

                    if (comp.State == ETradeState.Approved || comp.State == ETradeState.Finished)
                    {
                        TTrade.Instance.SendCommandReply(callerPlayer, "error_finished_trade_part");
                        return Task.CompletedTask;
                    }
                    
                    VaultManager.OpenVault(callerPlayer, comp.VaultId, true);
                    return Task.CompletedTask;
                }),
            new SubCommand("view", "View the other player's trade inventory.", "view", new List<string>() { "check" }, new List<string>() { "ttrade.command.trade.view" }, 
                (caller,  args) =>
                {
                    if (args.Length != 0)
                    {
                        ExecuteHelp(caller, true, "view", args);
                        return Task.CompletedTask;
                    }
                    

                    UnturnedPlayer callerPlayer = (UnturnedPlayer)caller;
                    TradeComponent comp = callerPlayer.GetComponent<TradeComponent>();

                    if (comp.State == ETradeState.None)
                    {
                        TTrade.Instance.SendCommandReply(callerPlayer, "error_not_in_trade");
                        return Task.CompletedTask;
                    }

                    UnturnedPlayer tradePartner = UnturnedPlayer.FromCSteamID((CSteamID)comp.TradePartner);
                    if (tradePartner == null)
                    {
                        TTrade.Instance.SendCommandReply(callerPlayer, "error_trade_partner_not_found");
                        return Task.CompletedTask;
                    }

                    TradeComponent partnerComp = tradePartner.GetComponent<TradeComponent>();
                    float distance = Vector3.Distance(callerPlayer.Position, tradePartner.Position);
                    if (distance > TTrade.Instance.Config.Distance)
                    {
                        TTrade.Instance.SendCommandReply(callerPlayer, "error_trade_distance", distance, TTrade.Instance.Config.Distance);
                        return Task.CompletedTask;
                    }
                    VaultManager.OpenVault(callerPlayer, partnerComp.VaultId, false);
                    return Task.CompletedTask;
                }),
            new SubCommand("cancel", "Cancel an ongoing trade.", "cancel", new List<string>() { "quit" }, new List<string>() { "ttrade.command.trade.cancel" }, 
                (caller,  args) =>
                {
                    if (args.Length != 0)
                    {
                        ExecuteHelp(caller, true, "cancel", args);
                        return Task.CompletedTask;
                    }

                    UnturnedPlayer callerPlayer = (UnturnedPlayer)caller;
                    TradeComponent comp = callerPlayer.GetComponent<TradeComponent>();
                    if (comp.State == ETradeState.None)
                    {
                        TTrade.Instance.SendCommandReply(callerPlayer, "error_not_in_trade");
                        return Task.CompletedTask;
                    }
                    
                    TradeManager.CancelTrade(callerPlayer);
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

            TradeManager.SendTradeRequest(callerPlayer, targetPlayer);
            return Task.FromResult(true);
        }
    }
}
