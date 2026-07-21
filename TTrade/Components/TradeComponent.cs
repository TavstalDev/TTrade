using System;
using System.Collections.Generic;
using Tavstal.TTrade.Models;
// ReSharper disable ClassNeverInstantiated.Global

namespace Tavstal.TTrade.Components
{
    public class TradeComponent
    {
        public ETradeState State = ETradeState.NONE;
        public ulong TradePartner;
        public Guid VaultId;
        public readonly List<ulong> TradeRequests = new List<ulong>();
    }
}