using System;
using System.Collections.Generic;
using Rocket.Unturned.Player;
using Tavstal.TTrade.Models;
using Tavstal.TTrade.Utils.Managers;

// ReSharper disable ClassNeverInstantiated.Global

namespace Tavstal.TTrade.Components
{
    public class TradeComponent : UnturnedPlayerComponent
    {
        public ETradeState State = ETradeState.NONE;
        public ulong TradePartner;
        public Guid VaultId;
        public readonly List<ulong> TradeRequests = new List<ulong>();
        
        private void OnDestroy()
        {
            try
            {
                if (State != ETradeState.NONE)
                    TradeManager.CancelTrade(Player);
            }
            catch { /* ignored */ }
        }
    }
}