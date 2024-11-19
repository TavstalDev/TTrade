using System;
using System.Collections.Generic;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Tavstal.Trade.Components;
using Tavstal.Trade.Models;
using UnityEngine;

namespace Tavstal.Trade.Managers
{
    /// <summary>
    /// Manages the vault system for storing and retrieving player-specific data.
    /// </summary>
    public static class VaultManager
    {
        /// <summary>
        /// A dictionary containing all vaults, mapped by their unique <see cref="Guid"/> keys.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private static readonly Dictionary<Guid, VaultStorage> _vaultList = new Dictionary<Guid, VaultStorage>();
        /// <summary>
        /// Gets the dictionary of vaults, where each vault is associated with a unique <see cref="Guid"/> key.
        /// </summary>
        public static Dictionary<Guid, VaultStorage> VaultList => _vaultList;

        /// <summary>
        /// Adds a new vault for the specified <see cref="UnturnedPlayer"/>.
        /// </summary>
        /// <param name="player">The <see cref="UnturnedPlayer"/> for whom the vault will be created.</param>
        /// <returns>A <see cref="VaultStorage"/> instance representing the created vault.</returns>
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
        
        /// <summary>
        /// Opens a vault for the specified <see cref="UnturnedPlayer"/>.
        /// </summary>
        /// <param name="player">The <see cref="UnturnedPlayer"/> accessing the vault.</param>
        /// <param name="guid">The <see cref="Guid"/> of the vault to open.</param>
        /// <param name="edit">
        /// A value indicating whether the vault is being opened in edit mode.
        /// <c>true</c> to enable edit mode; otherwise, <c>false</c>.
        /// </param>
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
        
        /// <summary>
        /// Removes a vault associated with the specified <see cref="UnturnedPlayer"/>.
        /// </summary>
        /// <param name="player">The <see cref="UnturnedPlayer"/> whose vault is being removed.</param>
        /// <param name="id">The <see cref="Guid"/> of the vault to remove.</param>
        /// <param name="keepItems">
        /// A value indicating whether the items in the vault should be retained.
        /// <c>true</c> to keep the items; otherwise, <c>false</c>.
        /// </param>
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