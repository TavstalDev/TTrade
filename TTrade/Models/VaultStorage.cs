using SDG.Unturned;
using YamlDotNet.Serialization;

namespace Tavstal.TTrade.Models
{
    public class VaultStorage
    {
        [YamlMember(Order = 0)] 
        public BarricadeDrop StorageDrop;
        [YamlMember(Order = 1)]
        public int SizeX;
        [YamlMember(Order = 2)]
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