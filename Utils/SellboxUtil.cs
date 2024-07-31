using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RFRocketLibrary.Models;
using Sellbox.Enums;
using Sellbox.Helpers;
using Sellbox.Models;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using RocketExtensions.Models;
using RocketExtensions.Utilities;
using SDG.Unturned;

namespace Sellbox.Utils
{
    internal static class SellboxUtil
    {
        internal static List<Sellbox> GetSellboxs(UnturnedPlayer player)
        {
            try
            {
                return Plugin.Conf.Sellboxs.Where(sellbox => player.HasPermission(sellbox.Permission ?? string.Empty))
                    .ToList();
            }
            catch (Exception e)
            {
                Logger.LogError($"[{Plugin.Inst.Name}] [ERROR] SellboxUtil GetVaults: {e.Message}");
                Logger.LogError($"[{Plugin.Inst.Name}] [ERROR] Details: {e}");
                return new List<Sellbox>();
            }
        }

        internal static bool IsBlacklisted(UnturnedPlayer player, ushort id)
        {
            try
            {
                var blacklist = Plugin.Conf.BlacklistedItems.Any(blacklistedItem => blacklistedItem.Items.Any(
                    blacklistItemId =>
                        blacklistItemId == id && !player.HasPermission(blacklistedItem.BypassPermission)));
                if (!blacklist)
                    return false;
                var itemAsset = (ItemAsset)Assets.find(EAssetType.ITEM, id);
                ChatHelper.Say(player,
                    Plugin.Inst.Translate(EResponse.BLACKLIST.ToString(), itemAsset.itemName, itemAsset.id),
                    Plugin.MsgColor,
                    Plugin.Conf.AnnouncerIconUrl);
                return true;
            }
            catch (Exception e)
            {
                Logger.LogError($"[{Plugin.Inst.Name}] [ERROR] SellboxUtil IsBlacklisted: {e.Message}");
                Logger.LogError($"[{Plugin.Inst.Name}] [ERROR] Details: {e}");
                return false;
            }
        }

        internal static async Task OpenVaultAsync(UnturnedPlayer player, Sellbox sellbox)
        {
            try
            {
                await LoadVaultAsync(player, sellbox);
                if (Plugin.Conf.DebugMode)
                    Logger.LogWarning(
                        $"[{Plugin.Inst.Name}] [DEBUG] {player.CharacterName} is accessing {sellbox.Name} Sellbox");
            }
            catch (Exception e)
            {
                Logger.LogError($"[{Plugin.Inst.Name}] [ERROR] OpenVaultAsync: {e.Message}");
                Logger.LogError($"[{Plugin.Inst.Name}] [ERROR] Details: {e}");
            }
        }

        private static async Task LoadVaultAsync(UnturnedPlayer player, Vault vault)
        {
            IEnumerator SendItemsOverTime(PlayerComponent component, Items items)
            {
                var toRemove = new List<ItemJarWrapper>();
                foreach (var itemJarWrapper in component.PlayerSellbox.SellboxContent.Items)
                {
                    if (items.width == 0 || items.height == 0)
                        goto Break;

                        if (itemJarWrapper.X > sellbox.Width || itemJarWrapper.Y > sellbox.Height)
                        {
                            ItemManager.dropItem(itemJarWrapper.Item.ToItem(), player.Position, true, true, true);
                            toRemove.Add(itemJarWrapper);
                        }
                        else
                            items.addItem(itemJarWrapper.X, itemJarWrapper.Y, itemJarWrapper.Rotation,
                                itemJarWrapper.Item.ToItem());
                    }

                    yield return null;
                }

                component.IsBusy = false;

            Break:
                foreach (var itemJarWrapper in toRemove)
                    component.PlayerSellbox.SellboxContent.Items.Remove(itemJarWrapper);
            }

            try
            {
                var cPlayer = player.GetPlayerComponent();
                cPlayer.IsBusy = true;
                var loadedVault = await cPlayer.SelectedSellbox
                cPlayer.PlayerSellbox = loadedVault;

                await ThreadTool.RunOnGameThreadAsync(() =>
                {
                    var sellboxItems = new Items(7);
                    sellboxItems.resize(vault.Width, vault.Height);
                    cPlayer.PlayerSellboxItems = sellboxItems;
                    player.Player.inventory.isStoring = true;
                    player.Player.inventory.storage = null;
                    player.Player.inventory.updateItems(7, vaultItems);
                    player.Player.inventory.sendStorage();
                    Plugin.Inst.StartCoroutine(SendItemsOverTime(cPlayer, vaultItems));
                });
            }
            catch (Exception e)
            {
                Logger.LogError($"[{Plugin.Inst.Name}] [ERROR] VaultUtil LoadVault: {e.Message}");
                Logger.LogError($"[{Plugin.Inst.Name}] [ERROR] Details: {e}");
            }
        }

        private static void AdminLoadVault(UnturnedPlayer player, PlayerSellbox playerSellbox)
        {
            try
            {
                var cPlayer = player.GetPlayerComponent();

                var sellboxItems = new Items(7);
                sellboxItems.resize(playerSellbox.SellboxContent.Width, playerSellbox.SellboxContent.Height);

                foreach (var itemJarWrapper in playerSellbox.SellboxContent.Items)
                {
                    if (itemJarWrapper.X > playerSellbox.SellboxContent.Width ||
                        itemJarWrapper.Y > playerSellbox.SellboxContent.Height)
                        ItemManager.dropItem(itemJarWrapper.Item.ToItem(), player.Position, true, true, true);
                    else
                        vaultItems.addItem(itemJarWrapper.X, itemJarWrapper.Y, itemJarWrapper.Rotation,
                            itemJarWrapper.Item.ToItem());
                }

                player.Player.inventory.isStoring = true;
                player.Player.inventory.storage = null;
                player.Player.inventory.updateItems(7, sellboxItems);
                player.Player.inventory.sendStorage();
                cPlayer.PlayerSellbox = playerSellbox;
                cPlayer.PlayerSellboxItems = sellboxItems;
            }
            catch (Exception e)
            {
                Logger.LogError($"[{Plugin.Inst.Name}] [ERROR] VaultUtil AdminLoadVault: {e.Message}");
                Logger.LogError($"[{Plugin.Inst.Name}] [ERROR] Details: {e}");
            }
        }

        internal static bool IsVaultBusy(ulong owner, Sellbox sellbox)
        {
            foreach (var steamPlayer in Provider.clients)
            {
                var cPlayer = steamPlayer.player.GetComponent<PlayerComponent>();
                if (cPlayer.PlayerSellbox != null && cPlayer.PlayerSellboxItems != null &&
                    cPlayer.PlayerSellbox.SteamId == owner &&
                    cPlayer.PlayerSellbox.SellboxName == sellbox.Name)
                {
                    return true;
                }
            }

            return false;
        }
    }
}