using Newtonsoft.Json;
using SDG.Unturned;
using UnityEngine;

namespace Tavstal.Trade.Models
{
    public class VaultStorage
    {
        [JsonProperty("lockerTransform")]
        public Transform LockerTransform;
        [JsonProperty("isOpened")]
        public bool IsOpened;
        [JsonProperty("items")]
        public Items ItemList;
        [JsonProperty("sizeX")]
        public int SizeX;
        [JsonProperty("sizeY")]
        public int SizeY;
        
        public VaultStorage() { }

        public VaultStorage(Transform lockerTransform, bool isOpened, Items itemList, int sizeX, int sizeY)
        {
            LockerTransform = lockerTransform;
            IsOpened = isOpened;
            ItemList = itemList;
            SizeX = sizeX;
            SizeY = sizeY;
        }
    }
}