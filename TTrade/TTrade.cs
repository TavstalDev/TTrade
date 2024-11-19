using System.Collections.Generic;
using Tavstal.Trade.Handlers;
using Tavstal.Trade.Managers;
using Tavstal.TLibrary.Models.Plugin;
using Tavstal.Trade.Components;
using Tavstal.Trade.Models;

namespace Tavstal.Trade
{
    /// <summary>
    /// The main plugin class.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class TTrade : PluginBase<TradeConfig>
    {
        public static TTrade Instance { get; private set; }
        public new static readonly TLogger Logger = new TLogger("TTrade", false);

        /// <summary>
        /// Fired when the plugin is loaded.
        /// </summary>
        public override void OnLoad()
        {
            Instance = this;
            // Attach player related events
            PlayerEventHandler.AttachEvents();

            Logger.LogWarning("████████╗████████╗██████╗  █████╗ ██████╗ ███████╗");
            Logger.LogWarning("╚══██╔══╝╚══██╔══╝██╔══██╗██╔══██╗██╔══██╗██╔════╝");
            Logger.LogWarning("   ██║      ██║   ██████╔╝███████║██║  ██║█████╗  ");
            Logger.LogWarning("   ██║      ██║   ██╔══██╗██╔══██║██║  ██║██╔══╝  ");
            Logger.LogWarning("   ██║      ██║   ██║  ██║██║  ██║██████╔╝███████╗");
            Logger.LogWarning("   ╚═╝      ╚═╝   ╚═╝  ╚═╝╚═╝  ╚═╝╚═════╝ ╚══════╝");
            Logger.Log("#########################################");
            Logger.Log("# Thanks for using my plugin");
            Logger.Log("# Plugin Created By Tavstal");
            Logger.Log("# Discord: Tavstal");
            Logger.Log("# Website: https://redstoneplugins.com");
            // Please do not remove this region and its code, because the license require credits to the author.
            #region Credits to Tavstal
            Logger.Log("#########################################");
            Logger.Log($"# This plugin uses TLibrary.");
            Logger.Log($"# TLibrary Created By: Tavstal"); 
            Logger.Log($"# Github: https://github.com/TavstalDev/TLibrary/tree/master");
            #endregion
            Logger.Log("#########################################");
            Logger.Log($"# Build Version: {Version}");
            Logger.Log($"# Build Date: {BuildDate}");
            Logger.Log("#########################################");

            Logger.Log($"# {GetPluginName()} has been loaded.");
        }

        /// <summary>
        /// Fired when the plugin is unloaded.
        /// </summary>
        public override void OnUnLoad()
        {
            PlayerEventHandler.DetachEvents();
            Logger.Log($"# {GetPluginName()} has been successfully unloaded.");

            foreach (var player in PlayerEventHandler.Players)
            {
                TradeComponent comp = player.GetComponent<TradeComponent>();
                if (comp.State == ETradeState.None)
                    continue;

                TradeManager.CancelTrade(player);
            }
        }
        
        public override Dictionary<string, string> DefaultLocalization =>
           new Dictionary<string, string>
           {
               { "prefix", $"&e[{GetPluginName()}]" },
               { "error_player_not_found", "&cPlayer was not found." },
               { "error_not_in_trade", "&cYou are not in a trade." },
               { "error_trade_partner_not_found", "&cFailed to get your trade partner." },
               { "error_trade_distance", "&cYou are too far from your trade partner. ({0} > {1})" },
               { "error_wait_partner_finish", "&cYou have already finished your part. Please wait for your partner to finish." },
               { "error_finished_trade_part", "&cYou have finished your part of the trading. You can not edit the items, but you can cancel the trade." },
               { "error_trade_not_approved", "&cYou have to approve the trade before finishing it." },
               { "error_self_trade", "&cYou can not trade with yourself." },
               { "error_already_trading", "&cYou are already trading." },
               { "error_trade_target_busy", "&cThe provided player is already trading." },
               { "error_trade_request_already_sent", "&cYou have already sent trade request to the same player." },
               { "success_trade_request_sent", "&aYou have successfully sent the trade request to &e{0}&a." },
               { "success_trade_request_received", "&e{0}&a has sent a trade request to you. Use &e/trade accept {0}&a or &e/trade deny {0}&a." },
               { "success_trade_accept", "&aYou have successfully accepted &e{0}'s&a trade request. Use &e/trade cancel&a to cancel." },
               { "success_trade_accept_send", "&e{0}&a has accepted your trade request. Use &e/trade cancel&a to cancel." },
               { "success_trade_deny", "&aYou have successfully denied &e{0}'s&a trade request." },
               { "success_trade_deny_send", "&e{0}&a has denied your trade request." },
               { "success_trade_cancel", "&aYou have successfully canceled the ongoing trade." },
               { "success_trade_cancel_send", "&e{0}&a has cancelled the trade." },
               { "success_trade_approve", "&aYou have successfully approved your trade. Use the command again to finish." },
               { "success_trade_approve_send", "&e{0}&a has approved his trade." },
               { "success_trade_finish", "&aYou have successfully finished your part of trade." },
               { "success_trade_finish_send", "&e{0}&a has finished their part of the trade." },
               { "success_trade_complete", "&aThe trade has been completed." },
           };
    }
}