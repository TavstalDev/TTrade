using System;
using System.Collections.Concurrent;
using Rocket.Unturned.Player;
using Steamworks;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Models.Logging;
using Tavstal.TTrade.Components;

namespace Tavstal.TTrade.Utils.Managers
{
    public static class ComponentManager
    {
        private static readonly ConcurrentDictionary<string, TradeComponent> _components = new ConcurrentDictionary<string, TradeComponent>();
        private static TLogger Logger => TTrade.Logger;

        public static TradeComponent? Get(UnturnedPlayer? player)
        {
            if (player == null || player.CSteamID == CSteamID.Nil || player.Player == null)
                return null;
            return  _components.GetOrAdd(player.Id, player.GetComponent<TradeComponent>());
        }

        public static void Invalidate(string id)
        {
            try
            {
                _components.TryRemove(id, out _);
            }
            catch (Exception ex)
            {
                Logger.Error($"Unexpected occured while invalidating {id}'s component.", ex);
            }
        }
    }
}