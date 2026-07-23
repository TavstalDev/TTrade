using System.Collections.Generic;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using Tavstal.TTrade.Components;
using Tavstal.TTrade.Models;
using Tavstal.TTrade.Utils.Managers;

namespace Tavstal.TTrade.Utils.Handlers
{
    public static class PlayerEventHandler
    {
        private static bool _isAttached;
        // ReSharper disable once InconsistentNaming
        private static readonly List<UnturnedPlayer> _players = new List<UnturnedPlayer>();
        public static List<UnturnedPlayer> Players => _players;

        public static void AttachEvents()
        {
            if (_isAttached)
                return;

            _isAttached = true;

            U.Events.OnPlayerConnected += OnPlayerConnected;
            U.Events.OnPlayerDisconnected += OnPlayerDisconnected;
        }

        public static void DetachEvents()
        {
            if (!_isAttached)
                return;

            _isAttached = false;

            U.Events.OnPlayerConnected -= OnPlayerConnected;
            U.Events.OnPlayerDisconnected -= OnPlayerDisconnected;
        }
        
        private static void OnPlayerConnected(UnturnedPlayer player)
        {
            _players.Add(player);
        }

        private static void OnPlayerDisconnected(UnturnedPlayer player)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (player == null)
                return;
            
            var localPlayer = player;
            if (_players.Contains(localPlayer))
                _players.Remove(localPlayer);
            TradeComponent component = localPlayer.GetComponent<TradeComponent>();
            if (component == null || component.State == ETradeState.NONE)
                return;

            TradeManager.CancelTrade(localPlayer);
        }
    }
}
