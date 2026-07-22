using Tavstal.TLibrary.Models.Config;
using YamlDotNet.Serialization;
// ReSharper disable ClassNeverInstantiated.Global

namespace Tavstal.TTrade
{
    public class TradeConfig : YamlConfiguration
    {
        [YamlMember(Order = 3)]
        public int Distance{ get; set; }
        [YamlMember(Order = 4)]
        public int TradeRowX{ get; set; }
        [YamlMember(Order = 5)]
        public int TradeRowY{ get; set; }
        [YamlMember(Order = 6)]
        public double Cooldown{ get; set; }

        public override void LoadDefaults()
        {
            General = new GeneralConfig
            {
                MessageIcon = "https://raw.githubusercontent.com/TavstalDev/TTrade/refs/heads/master/assets/icon.png"
            };
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
