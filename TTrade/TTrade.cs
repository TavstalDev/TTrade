﻿using System;
using System.Collections.Generic;
using Tavstal.TLibrary.Models.Plugin;
using Tavstal.Trade.Components;
using Tavstal.Trade.Models;
using Tavstal.Trade.Utils.Handlers;
using Tavstal.Trade.Utils.Managers;

namespace Tavstal.Trade
{
    /// <summary>
    /// The main plugin class.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class TTrade : PluginBase<TradeConfig>
    {
        public static TTrade Instance { get; private set; }

        /// <summary>
        /// Fired when the plugin is loaded.
        /// </summary>
        public override void OnLoad()
        {
            Instance = this;
            // Attach player related events
            PlayerEventHandler.AttachEvents();

            Logger.Log("████████╗████████╗██████╗  █████╗ ██████╗ ███████╗", ConsoleColor.Cyan, prefix: null);
            Logger.Log("╚══██╔══╝╚══██╔══╝██╔══██╗██╔══██╗██╔══██╗██╔════╝", ConsoleColor.Cyan, prefix: null);
            Logger.Log("   ██║      ██║   ██████╔╝███████║██║  ██║█████╗  ", ConsoleColor.Cyan, prefix: null);
            Logger.Log("   ██║      ██║   ██╔══██╗██╔══██║██║  ██║██╔══╝  ", ConsoleColor.Cyan, prefix: null);
            Logger.Log("   ██║      ██║   ██║  ██║██║  ██║██████╔╝███████╗", ConsoleColor.Cyan, prefix: null);
            Logger.Log("   ╚═╝      ╚═╝   ╚═╝  ╚═╝╚═╝  ╚═╝╚═════╝ ╚══════╝", ConsoleColor.Cyan, prefix: null);
            Logger.Log("#########################################", prefix: null);
            Logger.Log("#       Thanks for using this plugin!   #", prefix: null);
            Logger.Log("#########################################", prefix: null);
            Logger.Log("# Developed By: Tavstal", prefix: null);
            Logger.Log("# Discord:      @Tavstal", prefix: null);
            Logger.Log("# Website:      https://redstoneplugins.com", prefix: null);
            Logger.Log("# My GitHub:    https://tavstaldev.github.io", prefix: null);
            Logger.Log("#########################################", prefix: null);
            Logger.Log($"# Plugin Version:    {Version}", prefix: null);
            Logger.Log($"# Build Date:        {BuildDate}", prefix: null);
            Logger.Log($"# TLibrary Version:  {LibraryVersion}", prefix: null);
            Logger.Log("#########################################", prefix: null);
            Logger.Log("# Found an issue or have a suggestion?", prefix: null);
            Logger.Log("# Report it here: https://github.com/TavstalDev/TTrade/issues", prefix: null); 
            Logger.Log("#########################################", prefix: null);

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