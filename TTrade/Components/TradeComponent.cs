using System.Collections.Generic;
using Tavstal.Trade.Models;

namespace Tavstal.Trade.Components
{
    public class TradeComponent
    {
        public ETradeState State;
        public ulong TradePartner;
        public List<ulong> TradeRequests;
    }
}