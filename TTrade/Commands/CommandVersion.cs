﻿using Rocket.API;
using System.Collections.Generic;
using System.Reflection;
using Tavstal.TLibrary.Helpers.Unturned;

namespace Tavstal.Trade.Commands
{
    public class CommandVersion : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => ("v" + Assembly.GetExecutingAssembly().GetName().Name);
        public string Help => "Gets the version of the plugin";
        public string Syntax => "";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "ttrade.command.version" };


        public void Execute(IRocketPlayer caller, string[] command)
        {
            // Please do not remove this region and its code, because the license require credits to the author.
            #region Credits to Tavstal
            TTrade.Instance.SendPlainCommandReply(caller, "#########################################");
            TTrade.Instance.SendPlainCommandReply(caller, $"# This plugin uses TLibrary.");
            TTrade.Instance.SendPlainCommandReply(caller, $"# TLibrary Created By: Tavstal");
            TTrade.Instance.SendPlainCommandReply(caller, $"# Github: https://github.com/TavstalDev/TLibrary/tree/master");
            #endregion
            TTrade.Instance.SendPlainCommandReply(caller, "#########################################");
            TTrade.Instance.SendPlainCommandReply(caller, $"# Build Version: {TTrade.Version}");
            TTrade.Instance.SendPlainCommandReply(caller, $"# Build Date: {TTrade.BuildDate}");
            TTrade.Instance.SendPlainCommandReply(caller, "#########################################");
        }
    }
}
