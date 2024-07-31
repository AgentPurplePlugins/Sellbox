using System.Linq;
using System.Threading.Tasks;
using RFRocketLibrary.Models;
using RFRocketLibrary.Utils;
using Sellbox.Utils;
using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace Sellbox
{
    public class PlayerComponent : UnturnedPlayerComponent
    {
        internal bool IsBusy { get; set; }
        internal Sellbox SelectedSellbox { get; set; }
        internal PlayerSellbox PlayerSellbox { get; set; }
        internal Items PlayerSellboxItems { get; set; }

        protected override void Load()
        {
            var sellbox = SellboxUtil.GetSellboxs(Player).FirstOrDefault();
            if (sellbox != null)
                SelectedSellbox = sellbox;

            Player.Player.inventory.onInventoryResized += OnInventoryResized;
        }

        protected override void Unload()
        {
            Player.Player.inventory.onInventoryResized -= OnInventoryResized;

            if (PlayerSellboxItems != null)
                OnInventoryResized(PlayerInventory.STORAGE, 0, 0);
        }

        private void OnInventoryResized(byte page, byte newwidth, byte newheight)
        {
            if (Plugin.Conf.DebugMode)
                Logger.LogWarning(
                    $"[{Plugin.Inst.Name}] [DEBUG] OnInventoryResized {Player.CharacterName} page:{page} newwidth:{newwidth} newheight:{newheight}");

            if (page == PlayerInventory.STORAGE && newwidth == 0 && newheight == 0 && PlayerSellbox != null &&
                PlayerSellboxItems != null)
            {
                if (!IsBusy)
                {
                    var itemsWrapper = ItemsWrapper.Create(PlayerSellboxItems);
                    PlayerSellbox.SellboxContent = itemsWrapper;
                }

                IsBusy = true;
            }
        }
    }
}