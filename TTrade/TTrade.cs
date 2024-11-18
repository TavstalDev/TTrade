using SDG.Unturned;
using System.Collections.Generic;
using Tavstal.Trade.Handlers;
using Tavstal.Trade.Hooks;
using Tavstal.Trade.Managers;
using Tavstal.TLibrary.Models.Plugin;
using Tavstal.TLibrary.Models.Hooks;
using Tavstal.TLibrary.Managers;
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
            // Attach event, which will be fired when all plugins are loaded.
            Level.onPostLevelLoaded += Event_OnPluginsLoaded;
            // Attach player related events
            PlayerEventHandler.AttachEvents();

            Logger.Log("#########################################");
            Logger.Log("# Thanks for using my plugin");
            Logger.Log($"# Plugin Created By Tavstal");
            Logger.Log("# Discord: @YourDiscordName");
            Logger.Log("# Website: https://your.website.example");
            Logger.Log("# Discord Guild: https://discord.gg/your_invite");
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
            Level.onPostLevelLoaded -= Event_OnPluginsLoaded;
            PlayerEventHandler.DetachEvents();
            Logger.Log($"# {GetPluginName()} has been successfully unloaded.");
        }

        private void Event_OnPluginsLoaded(int i)
        {
            Logger.LogLateInit();
            
        }


        public override Dictionary<string, string> DefaultLocalization =>
           new Dictionary<string, string>
           {
               { "prefix", $"&e[{GetPluginName()}]" },
               { "error_player_not_found", "&cPlayer was not found." },
               { "success_command_example_hi_sent", "&aThe message has been successfully sent to the player." }
           };
    }
}