using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;
using Steamworks;
using Tavstal.TLibrary.Extensions.Unturned;
using Tavstal.TLibrary.Helpers.Unturned;
using Tavstal.TLibrary.Models.Commands;
using Tavstal.TLibrary.Models.Plugin;
using Tavstal.TTrade.Components;
using Tavstal.TTrade.Models;
using Tavstal.TTrade.Utils.Managers;
using UnityEngine;

namespace Tavstal.TTrade.Commands
{
    public class CommandTrade : CustomCommandBase
    {
        public override IPlugin Plugin => TTrade.Instance;
        public override bool UseBackgroundThread => false;

        public override AllowedCaller AllowedCaller => AllowedCaller.Player;
        public override string Name => "trade";
        public override string Help => "Securely exchange items with other players through a simple request and confirmation process.";
        public override string Syntax => "[player] | accept | deny | open | view | finish | cancel";
        public override List<string> Aliases => new List<string>() { "deal", "swap", "barter" };
        public override List<string> Permissions => new List<string> { "ttrade.command.trade" };

        // 'help' subcommand is built-in, you don't need to add it
        public override List<ISubcommand> SubCommands => new List<ISubcommand>()
        {
            new SubCommand("accept", "Accept a pending trade request.", "accept [player]", new List<string>() { "allow", "approve" }, new List<string>() { "ttrade.command.trade.accept" }, 
                Plugin, AllowedCaller,
                (caller,  args) => () =>
                {
                    if (args.Length != 1)
                    {
                        this.ExecuteHelp(caller, true, "accept");
                        return;
                    }

                    UnturnedPlayer callerPlayer = (UnturnedPlayer)caller;
                    UnturnedPlayer targetPlayer = UnturnedPlayer.FromName(args[0]);
                    if (targetPlayer == null)
                    {
                        TTrade.Instance.SendCommandReply(caller, "error_player_not_found", TTrade.Instance.Config.General.MessageIcon, args[0]);
                        return;
                    }

                    TradeManager.AcceptTradeRequest(callerPlayer, targetPlayer);
                }),
            new SubCommand("deny", "Decline a trade request.", "deny [player]", new List<string>() { "disallow", "disapprove" }, new List<string>() { "ttrade.command.trade.deny" }, 
                Plugin, AllowedCaller,
                (caller,  args) => () =>
                {
                    if (args.Length != 1)
                    {
                        this.ExecuteHelp(caller, true, "deny");
                        return;
                    }

                    UnturnedPlayer callerPlayer = (UnturnedPlayer)caller;
                    UnturnedPlayer targetPlayer = UnturnedPlayer.FromName(args[0]);
                    if (targetPlayer == null)
                    {
                        TTrade.Instance.SendCommandReply(caller, "error_player_not_found", TTrade.Instance.Config.General.MessageIcon, args[0]);
                        return;
                    }

                    TradeManager.DenyTradeRequest(callerPlayer, targetPlayer);
                }),
            new SubCommand("finish", "Complete, then finalize the trade.", "finish", new List<string>() { "done", "complete" }, new List<string>() { "ttrade.command.trade.finish" }, 
                Plugin, AllowedCaller,
                (caller,  args) => () =>
                {
                    if (args.Length != 0)
                    {
                        this.ExecuteHelp(caller, true, "done");
                        return;
                    }

                    UnturnedPlayer callerPlayer = (UnturnedPlayer)caller;
                    TradeComponent comp = callerPlayer.GetComponent<TradeComponent>();
                    if (comp.State == ETradeState.NONE)
                    {
                        TTrade.Instance.SendCommandReply(callerPlayer, "error_not_in_trade", TTrade.Instance.Config.General.MessageIcon);
                        return;
                    }

                    if (comp.State == ETradeState.FINISHED)
                    {
                        TTrade.Instance.SendCommandReply(callerPlayer, "error_wait_partner_finish", TTrade.Instance.Config.General.MessageIcon);
                        return;
                    }

                    if (comp.State == ETradeState.APPROVED)
                        TradeManager.FinishTrade(callerPlayer);
                    else
                        TradeManager.ApproveTrade(callerPlayer);
                }),
            new SubCommand("open", "Open the trade inventory.", "open", new List<string>(), new List<string>() { "ttrade.command.trade.open" }, 
                Plugin, AllowedCaller,
                (caller,  args) => () =>
                {
                    if (args.Length != 0)
                    {
                        this.ExecuteHelp(caller, true, "open");
                        return;
                    }

                    UnturnedPlayer callerPlayer = (UnturnedPlayer)caller;
                    TradeComponent comp = callerPlayer.GetComponent<TradeComponent>();
                    
                    if (comp.State == ETradeState.NONE)
                    {
                        TTrade.Instance.SendCommandReply(callerPlayer, "error_not_in_trade", TTrade.Instance.Config.General.MessageIcon);
                        return;
                    }

                    UnturnedPlayer tradePartner = UnturnedPlayer.FromCSteamID((CSteamID)comp.TradePartner);
                    if (tradePartner == null)
                    {
                        TTrade.Instance.SendCommandReply(callerPlayer, "error_trade_partner_not_found", TTrade.Instance.Config.General.MessageIcon);
                        return;
                    }
                    
                    float distance = Vector3.Distance(callerPlayer.Position, tradePartner.Position);
                    if (distance > TTrade.Instance.Config.Distance)
                    {
                        TTrade.Instance.SendCommandReply(callerPlayer, "error_trade_distance", TTrade.Instance.Config.General.MessageIcon, distance, TTrade.Instance.Config.Distance);
                        return;
                    }

                    if (comp.State == ETradeState.APPROVED || comp.State == ETradeState.FINISHED)
                    {
                        TTrade.Instance.SendCommandReply(callerPlayer, "error_finished_trade_part", TTrade.Instance.Config.General.MessageIcon);
                        return;
                    }
                    
                    VaultManager.OpenVault(callerPlayer, comp.VaultId, true);
                }),
            new SubCommand("view", "View the other player's trade inventory.", "view", new List<string>() { "check" }, new List<string>() { "ttrade.command.trade.view" }, 
                Plugin, AllowedCaller,
                (caller,  args) => () =>
                {
                    if (args.Length != 0)
                    {
                        this.ExecuteHelp(caller, true, "view");
                        return;
                    }
                    

                    UnturnedPlayer callerPlayer = (UnturnedPlayer)caller;
                    TradeComponent comp = callerPlayer.GetComponent<TradeComponent>();

                    if (comp.State == ETradeState.NONE)
                    {
                        TTrade.Instance.SendCommandReply(callerPlayer, "error_not_in_trade", TTrade.Instance.Config.General.MessageIcon);
                        return;
                    }

                    UnturnedPlayer tradePartner = UnturnedPlayer.FromCSteamID((CSteamID)comp.TradePartner);
                    if (tradePartner == null)
                    {
                        TTrade.Instance.SendCommandReply(callerPlayer, "error_trade_partner_not_found", TTrade.Instance.Config.General.MessageIcon);
                        return;
                    }

                    TradeComponent partnerComp = tradePartner.GetComponent<TradeComponent>();
                    float distance = Vector3.Distance(callerPlayer.Position, tradePartner.Position);
                    if (distance > TTrade.Instance.Config.Distance)
                    {
                        TTrade.Instance.SendCommandReply(callerPlayer, "error_trade_distance", TTrade.Instance.Config.General.MessageIcon, distance, TTrade.Instance.Config.Distance);
                        return;
                    }
                    VaultManager.OpenVault(callerPlayer, partnerComp.VaultId, false);
                }),
            new SubCommand("cancel", "Cancel an ongoing trade.", "cancel", new List<string>() { "quit" }, new List<string>() { "ttrade.command.trade.cancel" }, 
                Plugin, AllowedCaller,
                (caller,  args) => () =>
                {
                    if (args.Length != 0)
                    {
                        this.ExecuteHelp(caller, true, "cancel");
                        return;
                    }

                    UnturnedPlayer callerPlayer = (UnturnedPlayer)caller;
                    TradeComponent comp = callerPlayer.GetComponent<TradeComponent>();
                    if (comp.State == ETradeState.NONE)
                    {
                        TTrade.Instance.SendCommandReply(callerPlayer, "error_not_in_trade", TTrade.Instance.Config.General.MessageIcon);
                        return;
                    }
                    
                    TradeManager.CancelTrade(callerPlayer);
                })
        };

        protected override bool HandleExecute(IRocketPlayer caller, string[] args)
        {
            if (args.Length != 1)
            {
                this.ExecuteHelp(caller, true);
                return true;
            }

            UnturnedPlayer callerPlayer = (UnturnedPlayer)caller;
            UnturnedPlayer targetPlayer = UnturnedPlayer.FromName(args[0]);
            if (targetPlayer == null)
            {
                TTrade.Instance.SendCommandReply(caller, "error_player_not_found", TTrade.Instance.Config.General.MessageIcon, args[0]);
                return true;
            }

            TradeManager.SendTradeRequest(callerPlayer, targetPlayer);
            return true;
        }
    }
}
