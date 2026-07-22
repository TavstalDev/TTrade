using System;
using System.Collections.Generic;
using System.Text;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Models.Logging;
using Tavstal.TLibrary.Models.Plugin;
using Tavstal.TTrade.Components;
using Tavstal.TTrade.Models;
using Tavstal.TTrade.Utils.Handlers;
using Tavstal.TTrade.Utils.Managers;

namespace Tavstal.TTrade
{
    /// <summary>
    /// The main plugin class.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class TTrade : PluginBase<TradeConfig>
    {
        public static TTrade Instance { get; private set; } = null!;

        public override void OnPreLoad()
        {
            Instance = this;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("────────────────────────────────────────────────────────");
            sb.AppendLine();
            sb.AppendLine("████████╗████████╗██████╗  █████╗ ██████╗ ███████╗");
            sb.AppendLine("╚══██╔══╝╚══██╔══╝██╔══██╗██╔══██╗██╔══██╗██╔════╝");
            sb.AppendLine("   ██║      ██║   ██████╔╝███████║██║  ██║█████╗  ");
            sb.AppendLine("   ██║      ██║   ██╔══██╗██╔══██║██║  ██║██╔══╝  ");
            sb.AppendLine("   ██║      ██║   ██║  ██║██║  ██║██████╔╝███████╗");
            sb.AppendLine("   ╚═╝      ╚═╝   ╚═╝  ╚═╝╚═╝  ╚═╝╚═════╝ ╚══════╝");
            sb.AppendLine();
            sb.AppendLine("[ About ]");
            sb.AppendLine(" ▸ Developer : Tavstal");
            sb.AppendLine(" ▸ Discord   : @Tavstal");
            sb.AppendLine(" ▸ Website   : https://redstoneplugins.com");
            sb.AppendLine(" ▸ GitHub    : https://github.com/TavstalDev");
            sb.AppendLine();
            sb.AppendLine("[ Build ]");
            sb.AppendLine($" ▸ Version   : {Version}");
            sb.AppendLine($" ▸ Build Date: {BuildDate} UTC");
            sb.AppendLine($" ▸ TLibrary  : {LibraryVersion}");
            sb.AppendLine();
            sb.AppendLine("[ Support ]");
            sb.AppendLine(" ▸ Report issues or request features:");
            sb.AppendLine(" ▸ https://github.com/TavstalDev/TTrade/issues");
            sb.AppendLine();
            sb.AppendLine("────────────────────────────────────────────────────────");
            Logger.Log(ELogLevel.COMMAND, sb.ToString(), includePrefixes: false, color:  ConsoleColor.Cyan);
        }
        
        /// <summary>
        /// Fired when the plugin is loaded.
        /// </summary>
        public override void OnLoad()
        {
            PlayerEventHandler.AttachEvents();
            Logger.Info($"# {GetPluginName()} has been loaded.");
        }

        /// <summary>
        /// Fired when the plugin is unloaded.
        /// </summary>
        public override void OnUnLoad()
        {
            PlayerEventHandler.DetachEvents();
            Logger.Info($"# {GetPluginName()} has been successfully unloaded.");

            foreach (var player in PlayerEventHandler.Players)
            {
                TradeComponent comp = player.GetComponent<TradeComponent>();
                if (comp.State == ETradeState.NONE)
                    continue;

                TradeManager.CancelTrade(player);
            }
        }
        
        public override Dictionary<string, string> DefaultLocalization =>
           new Dictionary<string, string>
           {
               { "prefix", $"&e[{GetPluginName()}] " },
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