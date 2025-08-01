﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Rocket.API;
using Rocket.Core;
using Steamworks;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Models.Economy;
using Tavstal.TLibrary.Models.Hooks;

namespace Tavstal.Trade.Hooks
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class UconomyHook : Hook, IEconomyProvider
    {
        private MethodInfo _getBalanceMethod;
        private MethodInfo _increaseBalanceMethod;
        private MethodInfo _getTranslation;
        //private EventInfo _onPlayerPayMethod;
        //private EventInfo _onBalanceUpdateMethod;
        private object _databaseInstance;
        private object _pluginInstance;
        private object _uconomyConfig;


        public UconomyHook() : base(TTrade.Instance, "thook_uconomy", true) { }

        public override void OnLoad()
        {
            try
            {

                TTrade.Logger.Log("Loading Uconomy hook...");
                IRocketPlugin plugin = R.Plugins.GetPlugins().FirstOrDefault(c => c.Name.EqualsIgnoreCase("uconomy"));
                if (plugin == null)
                    throw new Exception("Could not find plugin.");

                Type pluginType = plugin.GetType().Assembly.GetType("fr34kyn01535.Uconomy.Uconomy");
                if (pluginType == null)
                    throw new Exception("Could not get plugin type.");
                
                _pluginInstance = pluginType.GetField("Instance", BindingFlags.Static | BindingFlags.Public)?.GetValue(plugin);
                if (_pluginInstance == null)
                    throw new Exception("Could not get plugin instance.");
                Type pluginInstanceType = _pluginInstance.GetType();
                
                object uconomyConfigInst = pluginType.GetProperty("Configuration")?.GetValue(plugin);
                if (uconomyConfigInst == null)
                    throw new Exception("Could not get plugin configuration field.");

                _uconomyConfig = uconomyConfigInst.GetType().GetProperty("Instance")?.GetValue(uconomyConfigInst);
                if (_uconomyConfig == null)
                    throw new Exception("Could not get plugin configuration instance.");
                
                _databaseInstance = pluginInstanceType.GetField("Database").GetValue(_pluginInstance);
                if (_databaseInstance == null)
                    throw new Exception("Failed to get the plugin database instance.");

                _getBalanceMethod = _databaseInstance.GetType().GetMethod(
                    "GetBalance", new[] { typeof(string) });

                _increaseBalanceMethod = _databaseInstance.GetType().GetMethod(
                    "IncreaseBalance", new[] { typeof(string), typeof(decimal) });
                if (pluginInstanceType.GetMethods().Any(x => x.Name == "Localize"))
                    _getTranslation = pluginInstanceType.GetMethod("Localize", new[] { typeof(string), typeof(object[]) });
                else
                    _getTranslation = pluginInstanceType.GetMethod("Translate", new[] { typeof(string), typeof(object[]) });
                
                #region Create Event Delegates
                /* Added because it might be needed in the future
                var parentPlugin = ExampleMain.Instance;
                var parentPluginType = parentPlugin.GetType().Assembly.GetType("Tavstal.ExampleMain.TShop");
                var parentPluginInstance = parentPluginType.GetField("Instance", BindingFlags.Static | BindingFlags.Public).GetValue(parentPlugin);

                try
                {
                    _onPlayerPayMethod = _pluginInstance.GetType().GetEvent("OnPlayerPay"); // Event in Uconomy
                    // Event handling method in this plugin
                    Delegate handler = Delegate.CreateDelegate(_onPlayerPayMethod.EventHandlerType, parentPlugin, parentPluginInstance.GetType().GetMethod("Event_Uconomy_OnPlayerPay"), true);
                    _onPlayerPayMethod.AddEventHandler(_pluginInstance, handler);

                }
                catch (Exception ex)
                {
                    ExampleMain.Logger.Error("Uconomy hook onPlayerPay delegate error:");
                    ExampleMain.Logger.Error(ex.ToString());
                }

                try
                {
                    _onBalanceUpdateMethod = _pluginInstance.GetType().GetEvent("OnBalanceUpdate"); // Event in Uconomy
                    // Event handling method in this plugin
                    Delegate handler = Delegate.CreateDelegate(_onBalanceUpdateMethod.EventHandlerType, parentPlugin, parentPluginInstance.GetType().GetMethod("Event_Uconomy_OnPlayerBalanceUpdate"), true);
                    _onBalanceUpdateMethod.AddEventHandler(_pluginInstance, handler);
                }
                catch (Exception ex)
                {
                    ExampleMain.Logger.Error("Uconomy hook onBalanceUpdate delegate error:");
                    ExampleMain.Logger.Error(ex.ToString());
                }*/
                #endregion

                TTrade.Logger.Exception("Currency Name >> " + GetCurrencyName());
                TTrade.Logger.Exception("Initial Balance >> " + GetConfigValue<decimal>("InitialBalance"));
                TTrade.Logger.Log("Uconomy hook loaded.");
            }
            catch (Exception e)
            {
                TTrade.Logger.Error("Failed to load Uconomy hook");
                TTrade.Logger.Error(e.ToString());
            }
        }

        public override void OnUnload() { }

        public override bool CanBeLoaded()
        {
            return R.Plugins.GetPlugins().Any(c => c.Name.EqualsIgnoreCase("uconomy"));
        }

        #region IPluginProvider Methods
        public T GetConfigValue<T>(string variableName)
        {
            try
            {
                return (T)Convert.ChangeType(_uconomyConfig.GetType().GetField(variableName).GetValue(_uconomyConfig), typeof(T));
            }
            catch
            {
                try
                {
                    // ReSharper disable PossibleNullReferenceException
                    return (T)Convert.ChangeType(_uconomyConfig.GetType().GetProperty(variableName).GetValue(_uconomyConfig), typeof(T));
                    // ReSharper restore PossibleNullReferenceException
                }
                catch
                {
                    TTrade.Logger.Error($"Failed to get '{variableName}' variable!");
                    return default;
                }
            }
        }

        public JObject GetConfig()
        {
            try
            {
                return JObject.FromObject(_uconomyConfig.GetType());
            }
            catch
            {
                TTrade.Logger.Error($"Failed to get config jobj.");
                return null;
            }
        }

        public string Localize(string translationKey, params object[] placeholder)
        {
            return Localize(false, translationKey, placeholder);
        }

        public string Localize(bool addPrefix, string translationKey, params object[] placeholder)
        {
            return ((string)_getTranslation.Invoke(_pluginInstance, new object[] { translationKey, placeholder }));
        }
        #endregion

        #region Economy Methods
        public decimal Withdraw(CSteamID player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK_ACCOUNT)
        {
            return (decimal)_increaseBalanceMethod.Invoke(_databaseInstance, new object[] {
                player.ToString(), -amount
            });
        }

        public decimal Deposit(CSteamID player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK_ACCOUNT)
        {
            return (decimal)_increaseBalanceMethod.Invoke(_databaseInstance, new object[] {
                player.ToString(), amount
            });
        }

        public decimal GetBalance(CSteamID player, EPaymentMethod method = EPaymentMethod.BANK_ACCOUNT)
        {
            return (decimal)_getBalanceMethod.Invoke(_databaseInstance, new object[] {
                player.ToString()
            });
        }

        public bool Has(CSteamID player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK_ACCOUNT)
        {
            if (amount >= 0)
                return (GetBalance(player) - amount) >= 0;
            else
                return (GetBalance(player) - Math.Abs(amount)) >= 0;
        }

        public async Task<decimal> WithdrawAsync(CSteamID player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK_ACCOUNT)
        {
            var taskCompletionSource = new TaskCompletionSource<decimal>();

            await Task.Run(() =>
            {
                try
                {
                    decimal result = (decimal)_increaseBalanceMethod.Invoke(_databaseInstance, new object[] {
                        player.ToString(), -amount
                    });
                    taskCompletionSource.SetResult(result);
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            });

            return await taskCompletionSource.Task;
        }

        public async Task<decimal> DepositAsync(CSteamID player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK_ACCOUNT)
        {
            var taskCompletionSource = new TaskCompletionSource<decimal>();

            await Task.Run(() =>
            {
                try
                {
                    decimal result = (decimal)_increaseBalanceMethod.Invoke(_databaseInstance, new object[] {
                        player.ToString(), amount
                    });
                    taskCompletionSource.SetResult(result);
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            });

            return await taskCompletionSource.Task;
        }

        public async Task<decimal> GetBalanceAsync(CSteamID player, EPaymentMethod method = EPaymentMethod.BANK_ACCOUNT)
        {
            var taskCompletionSource = new TaskCompletionSource<decimal>();

            await Task.Run(() =>
            {
                try
                {
                    decimal result = (decimal)_getBalanceMethod.Invoke(_databaseInstance, new object[] {
                        player.ToString()
                    });
                    taskCompletionSource.SetResult(result);
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            });

            return await taskCompletionSource.Task;
        }

        public async Task<bool> HasAsync(CSteamID player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK_ACCOUNT)
        {
            if (amount >= 0)
                return (await GetBalanceAsync(player) - amount) >= 0;
            else
                return (await GetBalanceAsync(player) - Math.Abs(amount)) >= 0;
        }

        public string GetCurrencyName()
        {
            string value = "Credits";
            try
            {
                value = GetConfigValue<string>("MoneyName");
            }
            catch { /* ignore */ }
            return value;
        }
        #endregion

        #region TEconomy Methods
        public bool HasTransactionSystem()
        {
            return false;
        }

        public bool HasBankCardSystem()
        {
            return false;
        }
        public void AddTransaction(CSteamID player, ITransaction transaction)
        {
            throw new NotImplementedException($"Transaction system is not supported by the current economy plugin.");
        }

        public List<ITransaction> GetTransactions(CSteamID player)
        {
            throw new NotImplementedException($"Transaction system is not supported by the current economy plugin.");
        }

        public void AddBankCard(CSteamID steamID, IBankCard newCard)
        {
            throw new NotImplementedException($"Bank card system is not supported by the current economy plugin.");
        }

        public void UpdateBankCard(string cardId, decimal limitUsed, bool isActive)
        {
            throw new NotImplementedException($"Bank card system is not supported by the current economy plugin.");
        }

        public void RemoveBankCard(string cardId)
        {
            throw new NotImplementedException($"Bank card system is not supported by the current economy plugin.");
        }

        public List<IBankCard> GetBankCardsByPlayer(CSteamID steamID)
        {
            throw new NotImplementedException($"Bank card system is not supported by the current economy plugin.");
        }

        public IBankCard GetBankCardById(string cardId)
        {
            throw new NotImplementedException($"Bank card system is not supported by the current economy plugin.");
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task AddTransactionAsync(CSteamID player, ITransaction transaction)
        {
            throw new NotImplementedException($"Transaction system is not supported by the current economy plugin.");
        }

        public async Task<List<ITransaction>> GetTransactionsAsync(CSteamID player)
        {
            throw new NotImplementedException($"Transaction system is not supported by the current economy plugin.");
        }

        public async Task AddBankCardAsync(CSteamID steamID, IBankCard newCard)
        {
            throw new NotImplementedException($"Bank card system is not supported by the current economy plugin.");
        }

        public async Task UpdateBankCardAsync(string cardId, decimal limitUsed, bool isActive)
        {
            throw new NotImplementedException($"Bank card system is not supported by the current economy plugin.");
        }

        public async Task RemoveBankCardAsync(string cardId)
        {
            throw new NotImplementedException($"Bank card system is not supported by the current economy plugin.");
        }

        public async Task<List<IBankCard>> GetBankCardsByPlayerAsync(CSteamID steamID)
        {
            throw new NotImplementedException($"Bank card system is not supported by the current economy plugin.");
        }

        public async Task<IBankCard> GetBankCardByIdAsync(string cardId)
        {
            throw new NotImplementedException($"Bank card system is not supported by the current economy plugin.");
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        #endregion
    }
}
