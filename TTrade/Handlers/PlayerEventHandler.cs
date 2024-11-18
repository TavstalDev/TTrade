using Rocket.Unturned;
using Rocket.Unturned.Player;
using Tavstal.Trade.Models;

namespace Tavstal.Trade.Handlers
{
    public static class PlayerEventHandler
    {
        private static bool _isAttached;

        public static void AttachEvents()
        {
            if (_isAttached)
                return;

            _isAttached = true;

            U.Events.OnPlayerConnected += OnPlayerConnected;
        }

        public static void DetachEvents()
        {
            if (!_isAttached)
                return;

            _isAttached = false;

            U.Events.OnPlayerConnected -= OnPlayerConnected;
        }

        private static async void OnPlayerConnected(UnturnedPlayer player)
        {
           
        }
    }
}
