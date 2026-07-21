using System;
using Rocket.Unturned.Player;
using Steamworks;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Helpers.Unturned;
using Tavstal.TTrade.Components;
using Tavstal.TTrade.Models;
using UnityEngine;

namespace Tavstal.TTrade.Utils.Managers
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
                    TTrade.Instance.SendCommandReply(tradeStarter, "error_trade_distance", TTrade.Instance.Config.General.MessageIcon, distance, TTrade.Instance.Config.Distance);
                    return false;
                }

                if (tradeStarter.Id == tradeReceiver.Id)
                {
                    TTrade.Instance.SendCommandReply(tradeStarter, "error_self_trade", TTrade.Instance.Config.General.MessageIcon);
                    return false;
                }

                TradeComponent starterComp = tradeStarter.GetComponent<TradeComponent>();
                TradeComponent receiverComp = tradeReceiver.GetComponent<TradeComponent>();
                if (starterComp.State != ETradeState.NONE)
                {
                    TTrade.Instance.SendCommandReply(tradeStarter, "error_already_trading", TTrade.Instance.Config.General.MessageIcon);
                    return false;
                }

                if (receiverComp.State != ETradeState.NONE)
                {
                    TTrade.Instance.SendCommandReply(tradeStarter, "error_trade_target_busy", TTrade.Instance.Config.General.MessageIcon, tradeReceiver.CharacterName);
                    return false;
                }

                if (receiverComp.TradeRequests.Contains(tradeStarter.CSteamID.m_SteamID))
                {
                    TTrade.Instance.SendCommandReply(tradeStarter, "error_trade_request_already_sent", TTrade.Instance.Config.General.MessageIcon, tradeReceiver.CharacterName);
                    return false;
                }
                
                receiverComp.TradeRequests.Add(tradeStarter.CSteamID.m_SteamID);
                success = true;

                TTrade.Instance.SendCommandReply(tradeStarter, "success_trade_request_sent", TTrade.Instance.Config.General.MessageIcon, tradeReceiver.CharacterName);
                TTrade.Instance.SendCommandReply(tradeReceiver, "success_trade_request_received", TTrade.Instance.Config.General.MessageIcon, tradeStarter.CharacterName);
            }
            catch (Exception ex)
            {
                TTrade.Logger.Error("Error in SendTradeRequest:", ex);
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
                    TTrade.Instance.SendCommandReply(tradeReceiver, "error_trade_distance", TTrade.Instance.Config.General.MessageIcon, distance, TTrade.Instance.Config.Distance);
                    return false;
                }
                
                TradeComponent senderComp = tradeSender.GetComponent<TradeComponent>();
                TradeComponent receiverComp = tradeReceiver.GetComponent<TradeComponent>();
                if (!receiverComp.TradeRequests.Contains(tradeSender.CSteamID.m_SteamID))
                {
                    TTrade.Instance.SendCommandReply(tradeReceiver, "error_trade_request_not_sent", TTrade.Instance.Config.General.MessageIcon, tradeSender.CharacterName);
                    return false;
                }
                
                if (senderComp.State != ETradeState.NONE)
                {
                    TTrade.Instance.SendCommandReply(tradeReceiver, "error_trade_target_busy", TTrade.Instance.Config.General.MessageIcon, tradeSender.CharacterName);
                    return false;
                }
                
                if (receiverComp.State != ETradeState.NONE)
                {
                    TTrade.Instance.SendCommandReply(tradeReceiver, "error_already_trading", TTrade.Instance.Config.General.MessageIcon);
                    return false;
                }
                
                receiverComp.TradePartner = tradeSender.CSteamID.m_SteamID;
                receiverComp.State = ETradeState.ACTIVE;
                senderComp.TradePartner = tradeReceiver.CSteamID.m_SteamID;
                senderComp.State = ETradeState.ACTIVE;
                receiverComp.TradeRequests.Remove(tradeSender.CSteamID.m_SteamID);
                TTrade.Instance.SendCommandReply(tradeReceiver, "success_trade_accept", TTrade.Instance.Config.General.MessageIcon, tradeSender.CharacterName);
                TTrade.Instance.SendCommandReply(tradeSender, "success_trade_accept_send", TTrade.Instance.Config.General.MessageIcon, tradeReceiver.CharacterName);
                success = true;
            }
            catch (Exception ex)
            {
                TTrade.Logger.Error("Error in AcceptTradeRequest:", ex);
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
                    TTrade.Instance.SendCommandReply(tradeReceiver, "error_trade_distance", TTrade.Instance.Config.General.MessageIcon, distance, TTrade.Instance.Config.Distance);
                    return false;
                }
                
                TradeComponent senderComp = tradeSender.GetComponent<TradeComponent>();
                TradeComponent receiverComp = tradeReceiver.GetComponent<TradeComponent>();
                if (!receiverComp.TradeRequests.Contains(tradeSender.CSteamID.m_SteamID))
                {
                    TTrade.Instance.SendCommandReply(tradeReceiver, "error_trade_request_not_sent", TTrade.Instance.Config.General.MessageIcon, tradeSender.CharacterName);
                    return false;
                }
                
                if (senderComp.State != ETradeState.NONE)
                {
                    TTrade.Instance.SendCommandReply(tradeReceiver, "error_trade_target_busy", TTrade.Instance.Config.General.MessageIcon, tradeSender.CharacterName);
                    return false;
                }
                
                if (receiverComp.State != ETradeState.NONE)
                {
                    TTrade.Instance.SendCommandReply(tradeReceiver, "error_already_trading", TTrade.Instance.Config.General.MessageIcon);
                    return false;
                }
                
                receiverComp.TradeRequests.Remove(tradeSender.CSteamID.m_SteamID);
                TTrade.Instance.SendCommandReply(tradeReceiver, "success_trade_deny", TTrade.Instance.Config.General.MessageIcon, tradeSender.CharacterName);
                TTrade.Instance.SendCommandReply(tradeSender, "success_trade_deny_send", TTrade.Instance.Config.General.MessageIcon, tradeReceiver.CharacterName);
                success = true;
            }
            catch (Exception ex)
            {
                TTrade.Logger.Error("Error in DenyTradeRequest:", ex);
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
                if (callerComp.State == ETradeState.NONE)
                {
                    TTrade.Instance.SendCommandReply(player, "error_not_in_trade", TTrade.Instance.Config.General.MessageIcon);
                    return false;
                }
                
                UnturnedPlayer partnerPlayer = UnturnedPlayer.FromCSteamID((CSteamID)callerComp.TradePartner);
                if (partnerPlayer == null)
                {
                    TTrade.Instance.SendCommandReply(player, "error_trade_partner_not_found", TTrade.Instance.Config.General.MessageIcon);
                    return false;
                }
                
                TradeComponent partnerComp = partnerPlayer.GetComponent<TradeComponent>();

                callerComp.State = ETradeState.NONE;
                callerComp.TradePartner = 0;
                player.Inventory.closeStorage();
                partnerComp.State = ETradeState.NONE;
                partnerComp.TradePartner = 0;
                partnerPlayer.Inventory.closeStorage();
                
                VaultManager.RemoveVault(partnerPlayer, partnerComp.VaultId, true);
                VaultManager.RemoveVault(player, callerComp.VaultId, true);

                partnerComp.VaultId = Guid.Empty;
                callerComp.VaultId = Guid.Empty;
                
                TTrade.Instance.SendCommandReply(player, "success_trade_cancel", TTrade.Instance.Config.General.MessageIcon);
                TTrade.Instance.SendCommandReply(partnerPlayer, "success_trade_cancel_send", TTrade.Instance.Config.General.MessageIcon, player.CharacterName);
                success = true;
            }
            catch (Exception ex)
            {
                TTrade.Logger.Error("Error in CancelTrade:", ex);
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
                if (callerComp.State != ETradeState.ACTIVE)
                {
                    TTrade.Instance.SendCommandReply(player, "error_not_in_trade", TTrade.Instance.Config.General.MessageIcon);
                    return false;
                }
                
                UnturnedPlayer partnerPlayer = UnturnedPlayer.FromCSteamID((CSteamID)callerComp.TradePartner);
                if (partnerPlayer == null)
                {
                    TTrade.Instance.SendCommandReply(player, "error_trade_partner_not_found", TTrade.Instance.Config.General.MessageIcon);
                    return false;
                }
                
                callerComp.State = ETradeState.APPROVED;
                
                TTrade.Instance.SendCommandReply(player, "success_trade_approve", TTrade.Instance.Config.General.MessageIcon);
                TTrade.Instance.SendCommandReply(partnerPlayer, "success_trade_approve_send", TTrade.Instance.Config.General.MessageIcon, player.CharacterName);
                success = true;
            }
            catch (Exception ex)
            {
                TTrade.Logger.Error("Error in ApproveTrade:", ex);
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
                if (callerComp.State != ETradeState.APPROVED)
                {
                    TTrade.Instance.SendCommandReply(player, "error_trade_not_approved", TTrade.Instance.Config.General.MessageIcon);
                    return false;
                }
                
                UnturnedPlayer partnerPlayer = UnturnedPlayer.FromCSteamID((CSteamID)callerComp.TradePartner);
                if (partnerPlayer == null)
                {
                    TTrade.Instance.SendCommandReply(player, "error_trade_partner_not_found", TTrade.Instance.Config.General.MessageIcon);
                    return false;
                }
                TradeComponent partnerComp = partnerPlayer.GetComponent<TradeComponent>();
                
                if (partnerComp.State == ETradeState.FINISHED)
                {
                    VaultManager.RemoveVault(player, partnerComp.VaultId, true);
                    VaultManager.RemoveVault(partnerPlayer, callerComp.VaultId, true);

                    partnerComp.VaultId = Guid.Empty;
                    callerComp.VaultId = Guid.Empty;
                    
                    TTrade.Instance.SendCommandReply(player, "success_trade_complete", TTrade.Instance.Config.General.MessageIcon);
                    TTrade.Instance.SendCommandReply(partnerPlayer, "success_trade_complete", TTrade.Instance.Config.General.MessageIcon);
                    
                    callerComp.State = ETradeState.NONE;
                    callerComp.TradePartner = 0;
                    player.Inventory.closeStorage();
                    partnerComp.State = ETradeState.NONE;
                    partnerComp.TradePartner = 0;
                    partnerPlayer.Inventory.closeStorage();
                }
                else
                {
                    callerComp.State = ETradeState.FINISHED;
                    TTrade.Instance.SendCommandReply(player, "success_trade_finish", TTrade.Instance.Config.General.MessageIcon);
                    TTrade.Instance.SendCommandReply(partnerPlayer, "success_trade_finish_send", TTrade.Instance.Config.General.MessageIcon, player.CharacterName);
                }
                success = true;
            }
            catch (Exception ex)
            {
                TTrade.Logger.Error("Error in FinishTrade:", ex);
            }
            
            return success;
        }
    }
}