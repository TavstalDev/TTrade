using System;
using System.Collections.Generic;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Tavstal.Trade.Components;
using Tavstal.Trade.Models;
using UnityEngine;

namespace Tavstal.Trade.Managers
{
    public static class VaultManager
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Dictionary<Guid, VaultStorage> _vaultList = new Dictionary<Guid, VaultStorage>();
        public static Dictionary<Guid, VaultStorage> VaultList => _vaultList;

        private static VaultStorage AddVault(UnturnedPlayer player)
        {
            ItemBarricadeAsset barricadeAsset = Assets.find(EAssetType.ITEM, 328) as ItemBarricadeAsset;
            Transform transform = BarricadeManager.dropBarricade(new Barricade(barricadeAsset), null, new Vector3(player.Position.x, -100, player.Position.z), 0, 0, 0, ulong.Parse(player.Id), 29832);
            BarricadeDrop drop = BarricadeManager.FindBarricadeByRootTransform(transform);
            if (drop == null)
                return null;
            
            TradeComponent comp = player.GetComponent<TradeComponent>();
            comp.VaultId = Guid.NewGuid();
            VaultStorage storage = new VaultStorage
            {
                SizeX = TTrade.Instance.Config.TradeRowX,
                SizeY = TTrade.Instance.Config.TradeRowY,
                StorageDrop = drop
            };
            _vaultList.Add(comp.VaultId, storage);
            return storage;
        }
        
        public static void OpenVault(UnturnedPlayer player, Guid guid, bool edit)
        {
            if (!_vaultList.TryGetValue(guid, out VaultStorage vaultStorage))
                vaultStorage = AddVault(player);
            
            InteractableStorage interactableStorage = (InteractableStorage)vaultStorage.StorageDrop.interactable;
            interactableStorage.items.resize((byte)vaultStorage.SizeX, (byte)vaultStorage.SizeY);
            interactableStorage.isOpen = true;
            interactableStorage.opener = player.Player;
            player.Inventory.isStoring = !edit;
            player.Inventory.storage = interactableStorage;
            player.Inventory.updateItems(PlayerInventory.STORAGE, interactableStorage.items);
            player.Inventory.sendStorage();
        }
        
        public static void RemoveVault(UnturnedPlayer player, Guid id, bool keepItems)
        {
            if (_vaultList.TryGetValue(id, out VaultStorage vault))
            {
                BarricadeData barricadeData = vault.StorageDrop.GetServersideData();
                InteractableStorage interactableStorage = (InteractableStorage)vault.StorageDrop.interactable;

                if (keepItems)
                {
                    foreach (var itemJar in interactableStorage.items.items)
                        player.Inventory.forceAddItem(itemJar.item, false);
                }
                
                interactableStorage.items.clear();
                barricadeData.barricade.askDamage(ushort.MaxValue);
                _vaultList.Remove(id);
            }
        }
    }
}