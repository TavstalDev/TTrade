using System;
using Rocket.Unturned.Player;
using Steamworks;
using Tavstal.TLibrary.Helpers.Unturned;
using Tavstal.Trade.Components;
using Tavstal.Trade.Models;
using UnityEngine;

namespace Tavstal.Trade.Utils.Managers
{
    /// <summary>
    /// Provides functionality for managing trade interactions between players.
    /// </summary>
    public static class TradeManager
    {
        /// <summary>
        /// Sends a trade request from one <see cref="UnturnedPlayer"/> to another.
        /// </summary>
        /// <param name="tradeStarter">The <see cref="UnturnedPlayer"/> initiating the trade request.</param>
        /// <param name="tradeReceiver">The <see cref="UnturnedPlayer"/> receiving the trade request.</param>
        /// <returns>
        /// <c>true</c> if the trade request was successfully sent; otherwise, <c>false</c>.
        /// </returns>
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

                TradeComponent starterComp = tradeStarter.GetComponent<TradeComponent>();
                TradeComponent receiverComp = tradeReceiver.GetComponent<TradeComponent>();
                if (starterComp.State != ETradeState.None)
                {
                    TTrade.Instance.SendCommandReply(tradeStarter, "error_already_trading");
                    return false;
                }

                if (receiverComp.State != ETradeState.None)
                {
                    TTrade.Instance.SendCommandReply(tradeStarter, "error_trade_target_busy", tradeReceiver.CharacterName);
                    return false;
                }

                if (receiverComp.TradeRequests.Contains(tradeStarter.CSteamID.m_SteamID))
                {
                    TTrade.Instance.SendCommandReply(tradeStarter, "error_trade_request_already_sent", tradeReceiver.CharacterName);
                    return false;
                }
                
                receiverComp.TradeRequests.Add(tradeStarter.CSteamID.m_SteamID);
                success = true;

                TTrade.Instance.SendCommandReply(tradeStarter, "success_trade_request_sent", tradeReceiver.CharacterName);
                TTrade.Instance.SendCommandReply(tradeReceiver, "success_trade_request_received", tradeStarter.CharacterName);
            }
            catch (Exception ex)
            {
                TTrade.Logger.Exception("Error in SendTradeRequest:");
                TTrade.Logger.Error(ex);
            }

            return success;
        }

        /// <summary>
        /// Accepts a trade request between two <see cref="UnturnedPlayer"/> instances.
        /// </summary>
        /// <param name="tradeReceiver">The <see cref="UnturnedPlayer"/> who is accepting the trade request.</param>
        /// <param name="tradeSender">The <see cref="UnturnedPlayer"/> who initiated the trade request.</param>
        /// <returns>
        /// <c>true</c> if the trade request was successfully accepted; otherwise, <c>false</c>.
        /// </returns>
        public static bool AcceptTradeRequest(UnturnedPlayer tradeReceiver, UnturnedPlayer tradeSender)
        {
            bool success = false;

            try
            {
                float distance = Vector3.Distance(tradeSender.Position, tradeReceiver.Position);
                if (distance > TTrade.Instance.Config.Distance)
                {
                    TTrade.Instance.SendCommandReply(tradeReceiver, "error_trade_distance", distance, TTrade.Instance.Config.Distance);
                    return false;
                }
                
                TradeComponent senderComp = tradeSender.GetComponent<TradeComponent>();
                TradeComponent receiverComp = tradeReceiver.GetComponent<TradeComponent>();
                if (!receiverComp.TradeRequests.Contains(tradeSender.CSteamID.m_SteamID))
                {
                    TTrade.Instance.SendCommandReply(tradeReceiver, "error_trade_request_not_sent", tradeSender.CharacterName);
                    return false;
                }
                
                if (senderComp.State != ETradeState.None)
                {
                    TTrade.Instance.SendCommandReply(tradeReceiver, "error_trade_target_busy", tradeSender.CharacterName);
                    return false;
                }
                
                if (receiverComp.State != ETradeState.None)
                {
                    TTrade.Instance.SendCommandReply(tradeReceiver, "error_already_trading");
                    return false;
                }
                
                receiverComp.TradePartner = tradeSender.CSteamID.m_SteamID;
                receiverComp.State = ETradeState.Active;
                senderComp.TradePartner = tradeReceiver.CSteamID.m_SteamID;
                senderComp.State = ETradeState.Active;
                receiverComp.TradeRequests.Remove(tradeSender.CSteamID.m_SteamID);
                TTrade.Instance.SendCommandReply(tradeReceiver, "success_trade_accept", tradeSender.CharacterName);
                TTrade.Instance.SendCommandReply(tradeSender, "success_trade_accept_send", tradeReceiver.CharacterName);
                success = true;
            }
            catch (Exception ex)
            {
                TTrade.Logger.Exception("Error in AcceptTradeRequest:");
                TTrade.Logger.Error(ex);
            }
            
            return success;
        }
        
        /// <summary>
        /// Denies a trade request between two <see cref="UnturnedPlayer"/> instances.
        /// </summary>
        /// <param name="tradeReceiver">The <see cref="UnturnedPlayer"/> who is denying the trade request.</param>
        /// <param name="tradeSender">The <see cref="UnturnedPlayer"/> who initiated the trade request.</param>
        /// <returns>
        /// <c>true</c> if the trade request was successfully denied; otherwise, <c>false</c>.
        /// </returns>
        public static bool DenyTradeRequest(UnturnedPlayer tradeReceiver, UnturnedPlayer tradeSender)
        {
            bool success = false;

            try
            {
                float distance = Vector3.Distance(tradeSender.Position, tradeReceiver.Position);
                if (distance > TTrade.Instance.Config.Distance)
                {
                    TTrade.Instance.SendCommandReply(tradeReceiver, "error_trade_distance", distance, TTrade.Instance.Config.Distance);
                    return false;
                }
                
                TradeComponent senderComp = tradeSender.GetComponent<TradeComponent>();
                TradeComponent receiverComp = tradeReceiver.GetComponent<TradeComponent>();
                if (!receiverComp.TradeRequests.Contains(tradeSender.CSteamID.m_SteamID))
                {
                    TTrade.Instance.SendCommandReply(tradeReceiver, "error_trade_request_not_sent", tradeSender.CharacterName);
                    return false;
                }
                
                if (senderComp.State != ETradeState.None)
                {
                    TTrade.Instance.SendCommandReply(tradeReceiver, "error_trade_target_busy", tradeSender.CharacterName);
                    return false;
                }
                
                if (receiverComp.State != ETradeState.None)
                {
                    TTrade.Instance.SendCommandReply(tradeReceiver, "error_already_trading");
                    return false;
                }
                
                receiverComp.TradeRequests.Remove(tradeSender.CSteamID.m_SteamID);
                TTrade.Instance.SendCommandReply(tradeReceiver, "success_trade_deny", tradeSender.CharacterName);
                TTrade.Instance.SendCommandReply(tradeSender, "success_trade_deny_send", tradeReceiver.CharacterName);
                success = true;
            }
            catch (Exception ex)
            {
                TTrade.Logger.Exception("Error in DenyTradeRequest:");
                TTrade.Logger.Error(ex);
            }
            
            return success;
        }
        
        /// <summary>
        /// Cancels an ongoing trade involving the specified <see cref="UnturnedPlayer"/>.
        /// </summary>
        /// <param name="player">The <see cref="UnturnedPlayer"/> who is canceling the trade.</param>
        /// <returns>
        /// <c>true</c> if the trade was successfully canceled; otherwise, <c>false</c>.
        /// </returns>
        public static bool CancelTrade(UnturnedPlayer player)
        {
            bool success = false;

            try
            {
                TradeComponent callerComp = player.GetComponent<TradeComponent>();
                if (callerComp.State == ETradeState.None)
                {
                    TTrade.Instance.SendCommandReply(player, "error_not_in_trade");
                    return false;
                }
                
                UnturnedPlayer partnerPlayer = UnturnedPlayer.FromCSteamID((CSteamID)callerComp.TradePartner);
                if (partnerPlayer == null)
                {
                    TTrade.Instance.SendCommandReply(player, "error_trade_partner_not_found");
                    return false;
                }
                
                TradeComponent partnerComp = partnerPlayer.GetComponent<TradeComponent>();

                callerComp.State = ETradeState.None;
                callerComp.TradePartner = 0;
                player.Inventory.closeStorage();
                partnerComp.State = ETradeState.None;
                partnerComp.TradePartner = 0;
                partnerPlayer.Inventory.closeStorage();
                
                VaultManager.RemoveVault(partnerPlayer, partnerComp.VaultId, true);
                VaultManager.RemoveVault(player, callerComp.VaultId, true);

                partnerComp.VaultId = Guid.Empty;
                callerComp.VaultId = Guid.Empty;
                
                TTrade.Instance.SendCommandReply(player, "success_trade_cancel");
                TTrade.Instance.SendCommandReply(partnerPlayer, "success_trade_cancel_send", player.CharacterName);
                success = true;
            }
            catch (Exception ex)
            {
                TTrade.Logger.Exception("Error in CancelTrade:");
                TTrade.Logger.Error(ex);
            }
            
            return success;
        }
        
        /// <summary>
        /// Approves an ongoing trade involving the specified <see cref="UnturnedPlayer"/>.
        /// </summary>
        /// <param name="player">The <see cref="UnturnedPlayer"/> who is approving the trade.</param>
        /// <returns>
        /// <c>true</c> if the trade was successfully approved; otherwise, <c>false</c>.
        /// </returns>
        public static bool ApproveTrade(UnturnedPlayer player)
        {
            bool success = false;

            try
            {
                TradeComponent callerComp = player.GetComponent<TradeComponent>();
                if (callerComp.State != ETradeState.Active)
                {
                    TTrade.Instance.SendCommandReply(player, "error_not_in_trade");
                    return false;
                }
                
                UnturnedPlayer partnerPlayer = UnturnedPlayer.FromCSteamID((CSteamID)callerComp.TradePartner);
                if (partnerPlayer == null)
                {
                    TTrade.Instance.SendCommandReply(player, "error_trade_partner_not_found");
                    return false;
                }
                
                callerComp.State = ETradeState.Approved;
                
                TTrade.Instance.SendCommandReply(player, "success_trade_approve");
                TTrade.Instance.SendCommandReply(partnerPlayer, "success_trade_approve_send", player.CharacterName);
                success = true;
            }
            catch (Exception ex)
            {
                TTrade.Logger.Exception("Error in ApproveTrade:");
                TTrade.Logger.Error(ex);
            }
            
            return success;
        }
        
        /// <summary>
        /// Finalizes an ongoing trade involving the specified <see cref="UnturnedPlayer"/>.
        /// </summary>
        /// <param name="player">The <see cref="UnturnedPlayer"/> who is completing the trade.</param>
        /// <returns>
        /// <c>true</c> if the trade was successfully finalized; otherwise, <c>false</c>.
        /// </returns>
        public static bool FinishTrade(UnturnedPlayer player)
        {
            bool success = false;

            try
            {
                TradeComponent callerComp = player.GetComponent<TradeComponent>();
                if (callerComp.State != ETradeState.Approved)
                {
                    TTrade.Instance.SendCommandReply(player, "error_trade_not_approved");
                    return false;
                }
                
                UnturnedPlayer partnerPlayer = UnturnedPlayer.FromCSteamID((CSteamID)callerComp.TradePartner);
                if (partnerPlayer == null)
                {
                    TTrade.Instance.SendCommandReply(player, "error_trade_partner_not_found");
                    return false;
                }
                TradeComponent partnerComp = partnerPlayer.GetComponent<TradeComponent>();
                
                if (partnerComp.State == ETradeState.Finished)
                {
                    VaultManager.RemoveVault(player, partnerComp.VaultId, true);
                    VaultManager.RemoveVault(partnerPlayer, callerComp.VaultId, true);

                    partnerComp.VaultId = Guid.Empty;
                    callerComp.VaultId = Guid.Empty;
                    
                    TTrade.Instance.SendCommandReply(player, "success_trade_complete");
                    TTrade.Instance.SendCommandReply(partnerPlayer, "success_trade_complete");
                    
                    callerComp.State = ETradeState.None;
                    callerComp.TradePartner = 0;
                    player.Inventory.closeStorage();
                    partnerComp.State = ETradeState.None;
                    partnerComp.TradePartner = 0;
                    partnerPlayer.Inventory.closeStorage();
                }
                else
                {
                    callerComp.State = ETradeState.Finished;
                    TTrade.Instance.SendCommandReply(player, "success_trade_finish");
                    TTrade.Instance.SendCommandReply(partnerPlayer, "success_trade_finish_send", player.CharacterName);
                }
                success = true;
            }
            catch (Exception ex)
            {
                TTrade.Logger.Exception("Error in FinishTrade:");
                TTrade.Logger.Error(ex);
            }
            
            return success;
        }
    }
}