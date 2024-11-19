using Newtonsoft.Json;
using Tavstal.TLibrary.Models.Plugin;

namespace Tavstal.Trade
{
    public class TradeConfig : ConfigurationBase
    {
        [JsonProperty(Order = 3)]
        public int Distance{ get; set; }
        [JsonProperty(Order = 4)]
        public int TradeRowX{ get; set; }
        [JsonProperty(Order = 5)]
        public int TradeRowY{ get; set; }
        [JsonProperty(Order = 6)]
        public double Cooldown{ get; set; }

        public override void LoadDefaults()
        {
            DebugMode = false;
            Locale = "en";
            DownloadLocalePacks = true;
            Distance = 3;
            TradeRowX = 6;
            TradeRowY = 6;
            Cooldown = 30;
        }

        // Required because of the library
        public TradeConfig() { }
        public TradeConfig(string fileName, string path) : base(fileName, path) { }
    }
}
