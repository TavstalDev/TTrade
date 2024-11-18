using System;
using System.Collections.Generic;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Tavstal.TLibrary.Helpers.Unturned;
using Tavstal.Trade.Components;
using Tavstal.Trade.Models;
using UnityEngine;

namespace Tavstal.Trade.Managers
{
    public static class VaultManager
    {
        private static Dictionary<Guid, VaultStorage> _vaultList = new Dictionary<Guid, VaultStorage>();
        public static Dictionary<Guid, VaultStorage> VaultList => _vaultList;

        public static bool SendTradeRequest(UnturnedPlayer tradeStarter, UnturnedPlayer tradeReceiver)
        {
            bool success = false;

            try
            {
                float distance = Vector3.Distance(tradeStarter.Position, tradeReceiver.Position);
                if (distance > TTrade.Instance.Config.Distance)
                {
                    TTrade.Instance.SendCommandReply(tradeStarter, "error_trade_distance", distance, TTrade.Instance.Config.Distance);
                    return false;
                }

                if (tradeStarter.Id == tradeReceiver.Id)
                {
                    TTrade.Instance.SendCommandReply(tradeStarter, "error_self_trade");
                    return false;
                }

                TradeComponent startetComp = tradeStarter.GetComponent<TradeComponent>();
                TradeComponent receiverComp = tradeReceiver.GetComponent<TradeComponent>();
                if (!(startetComp.State == ETradeState.None || startetComp.State == ETradeState.Pending))
                {
                    TTrade.Instance.SendCommandReply(tradeStarter, "error_already_trading");
                    return false;
                }

                if (!(receiverComp.State == ETradeState.None || receiverComp.State == ETradeState.Pending))
                {
                    TTrade.Instance.SendCommandReply(tradeStarter, "error_trade_target_busy", tradeReceiver.CharacterName);
                    return false;
                }

                if (receiverComp.TradeRequests.Contains(tradeStarter.CSteamID.m_SteamID))
                {
                    TTrade.Instance.SendCommandReply(tradeStarter, "error_trade_request_already_sent", tradeReceiver.CharacterName);
                    return false;
                }
                    
                startetComp.State = ETradeState.Pending;
                receiverComp.TradeRequests.Add(tradeStarter.CSteamID.m_SteamID);
                success = true;

                TTrade.Instance.SendCommandReply(tradeStarter, "success_trade_request_sent", tradeReceiver.CharacterName);
                TTrade.Instance.SendCommandReply(tradeReceiver, "success_trade_request_received", tradeStarter.CharacterName);
            }
            catch (Exception ex)
            {
                TTrade.Logger.LogException("Error in SendTradeRequest:");
                TTrade.Logger.LogError(ex);
            }

            return success;
        }
        
        public static void RemoveVault(Guid id)
        {
            if (_vaultList.TryGetValue(id, out VaultStorage vault))
            {
                BarricadeDrop drop = BarricadeManager.FindBarricadeByRootTransform(vault.LockerTransform);
                if (drop == null)
                    return;

                BarricadeData barricadeData = drop.GetServersideData();

                InteractableStorage interactableStorage = (InteractableStorage)drop.interactable;
                interactableStorage.items.clear();
                barricadeData.barricade.askDamage(ushort.MaxValue);
                _vaultList.Remove(id);
            }
        }
    }
}