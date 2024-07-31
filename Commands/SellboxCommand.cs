using System;
using System.Linq;
using System.Threading.Tasks;
using Sellbox.Enums;
using Sellbox.Models;
using Sellbox.Utils;
using Rocket.API;
using Rocket.Unturned.Player;
using RocketExtensions.Models;
using RocketExtensions.Plugins;
using RocketExtensions.Utilities;

namespace Sellbox.Commands
{
    [CommandActor(AllowedCaller.Player)]
    [CommandPermissions("sellbox")]
    [CommandAliases("locker")]
    [CommandInfo("Open a virtual sellbox storage.", "/sellbox | /sellbox <sellboxName>", AllowSimultaneousCalls = false)]
    public class SellboxCommand : RocketCommand
    {
        public override async Task Execute(CommandContext context)
        {
            if (context.CommandRawArguments.Length > 1)
            {
                await context.ReplyAsync(
                    Sellbox.Plugin.Inst.Translate(EResponse.INVALID_PARAMETER.ToString(), Syntax), Sellbox.Plugin.MsgColor,
                    Sellbox.Plugin.Conf.AnnouncerIconUrl);
                return;
            }

            var player = (UnturnedPlayer)context.Player;
            var cPlayer = player.GetPlayerComponent();

            if (cPlayer.IsBusy)
            {
                await context.ReplyAsync(Sellbox.Plugin.Inst.Translate(EResponse.VAULT_SYSTEM_BUSY.ToString()), Sellbox.Plugin.MsgColor,
                    Sellbox.Plugin.Conf.AnnouncerIconUrl);
                return;
            }

            if (player.IsInVehicle)
            {
                await context.ReplyAsync(Sellbox.Plugin.Inst.Translate(EResponse.IN_VEHICLE.ToString()), Sellbox.Plugin.MsgColor,
                    Sellbox.Plugin.Conf.AnnouncerIconUrl);
                return;
            }

            if (context.CommandRawArguments.Length == 0)
            {
                if (cPlayer.SelectedSellbox == null)
                {
                    if (SellboxUtil.GetSellboxs(player).Count == 0)
                    {
                        await context.ReplyAsync(
                            Sellbox.Plugin.Inst.Translate(EResponse.NO_PERMISSION_ALL.ToString()), Sellbox.Plugin.MsgColor,
                            Sellbox.Plugin.Conf.AnnouncerIconUrl);
                        return;
                    }

                    await context.ReplyAsync(
                        Sellbox.Plugin.Inst.Translate(EResponse.VAULT_NOT_SELECTED.ToString()), Sellbox.Plugin.MsgColor,
                        Sellbox.Plugin.Conf.AnnouncerIconUrl);
                    return;
                }

                if (!player.HasPermission(cPlayer.SelectedSellbox.Permission))
                {
                    await context.ReplyAsync(
                        Sellbox.Plugin.Inst.Translate(EResponse.NO_PERMISSION.ToString(), cPlayer.SelectedSellbox.Name),
                        Sellbox.Plugin.MsgColor, Sellbox.Plugin.Conf.AnnouncerIconUrl);
                    return;
                }

                if (SellboxUtil.IsSellboxBusy(player.CSteamID.m_SteamID, cPlayer.SelectedSellbox))
                {
                    await context.ReplyAsync(
                        Sellbox.Plugin.Inst.Translate(EResponse.VAULT_BUSY.ToString()), Sellbox.Plugin.MsgColor,
                        Sellbox.Plugin.Conf.AnnouncerIconUrl);
                    return;
                }

                await SellboxUtil.OpenVaultAsync(player, cPlayer.SelectedSellbox);
            }

            if (context.CommandRawArguments.Length == 1)
            {
                var sellbox = Sellbox.Parse(context.CommandRawArguments[0]);
                if (sellbox == null)
                {
                    await context.ReplyAsync(
                        Sellbox.Plugin.Inst.Translate(EResponse.VAULT_NOT_FOUND.ToString()), Sellbox.Plugin.MsgColor,
                        Sellbox.Plugin.Conf.AnnouncerIconUrl);
                    return;
                }

                if (!player.HasPermission(sellbox.Permission))
                {
                    await context.ReplyAsync(
                        Sellbox.Plugin.Inst.Translate(EResponse.NO_PERMISSION.ToString(), sellbox.Name), Sellbox.Plugin.MsgColor,
                        Sellbox.Plugin.Conf.AnnouncerIconUrl);
                    return;
                }

                if (cPlayer.PlayerSellboxItems != null)
                {
                    await context.ReplyAsync(
                        Sellbox.Plugin.Inst.Translate(EResponse.VAULT_PROCESSING.ToString()), RFVault.Plugin.MsgColor,
                        Sellbox.Plugin.Conf.AnnouncerIconUrl);
                    return;
                }

                if (SellboxUtil.IsSellboxBusy(player.CSteamID.m_SteamID, sellbox))
                {
                    await context.ReplyAsync(
                        Sellbox.Plugin.Inst.Translate(EResponse.VAULT_BUSY.ToString()), Sellbox.Plugin.MsgColor,
                        Sellbox.Plugin.Conf.AnnouncerIconUrl);
                    return;
                }

                await SellboxUtil.OpenVaultAsync(player, sellbox);
            }
        }
    }
}