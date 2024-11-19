using Newtonsoft.Json;
using SDG.Unturned;

namespace Tavstal.Trade.Models
{
    public class VaultStorage
    {
        [JsonProperty("storageDrop")] 
        public BarricadeDrop StorageDrop;
        [JsonProperty("sizeX")]
        public int SizeX;
        [JsonProperty("sizeY")]
        public int SizeY;

        public VaultStorage()
        {
        }

        public VaultStorage(BarricadeDrop drop, int sizeX, int sizeY)
        {
            StorageDrop = drop;
            SizeX = sizeX;
            SizeY = sizeY;
        }
    }
}