using System;
using System.Collections.Generic;
using Tavstal.Trade.Models;

namespace Tavstal.Trade.Components
{
    public class TradeComponent
    {
        public ETradeState State = ETradeState.None;
        public ulong TradePartner;
        public Guid VaultId;
        public readonly List<ulong> TradeRequests = new List<ulong>();
    }
}