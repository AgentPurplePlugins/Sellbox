using RFRocketLibrary;
using RFRocketLibrary.Enum;
using RFRocketLibrary.Events;
using RFRocketLibrary.Utils;
using Rocket.API.Collections;
using Rocket.API.Extensions;
using Rocket.Unturned.Chat;
using Sellbox.Utils;
using Sellbox.Models;
using RocketExtensions.Plugins;
using SDG.Unturned;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Sellbox

public class Plugin : ExtendedRocketPlugin<SellboxConfiguration>
{
    private const int Major = 1;
    private const int Minor = 0;
    private const int Patch = 0;

    public static Plugin Inst;
    public static Configuration Conf;
    internal static Color MsgColor;

    protected override void Load()
    {
        Inst = this;
        Conf = Configuration.Instance;
        if (Conf.Enabled)
        {
            MsgColor = UnturnedChat.GetColorFromName(Conf.MessageColor, Color.green);

            DependencyUtil.Load(EDependency.NewtonsoftJson);
            DependencyUtil.Load(EDependency.SystemRuntimeSerialization);
            DependencyUtil.Load(EDependency.LiteDB);
            DependencyUtil.Load(EDependency.LiteDBAsync);
            DependencyUtil.Load(EDependency.Dapper);
            DependencyUtil.Load(EDependency.MySqlData);
            DependencyUtil.Load(EDependency.I18N);
            DependencyUtil.Load(EDependency.I18NWest);
            DependencyUtil.Load(EDependency.SystemManagement);
            DependencyUtil.Load(EDependency.UbietyDnsCore);
            DependencyUtil.Load(EDependency.ZstdNet);

            //Load RFRocketLibrary Events
            Library.AttachEvent(true);
            UnturnedEvent.OnPrePlayerTookItem += PlayerEvent.OnPreItemTook;
            UnturnedPatchEvent.OnPrePlayerDraggedItem += PlayerEvent.OnPreItemDragged;
            UnturnedPatchEvent.OnPrePlayerSwappedItem += PlayerEvent.OnPreItemSwapped;
#if RF
            Level.onPostLevelLoaded += OnPostLevelLoaded;
#endif

            if (Level.isLoaded)
            {
#if RF
                OnPostLevelLoaded(0);
#endif
                foreach (var steamPlayer in Provider.clients)
                    steamPlayer.player.gameObject.TryAddComponent<PlayerComponent>();
            }
        }
        else
            Logger.LogError($"[{Name}] Plugin: DISABLED");

        Logger.LogWarning($"[{Name}] Plugin loaded successfully!");
        Logger.LogWarning($"[{Name}] {Name} v{Major}.{Minor}.{Patch}");
        Logger.LogWarning($"[{Name}] Made by Agent Purple reworked from RFVaults");
    }

    protected override void Unload()
    {
        if (Conf.Enabled)
        {
            UnturnedEvent.OnPrePlayerTookItem -= PlayerEvent.OnPreItemTook;
            UnturnedPatchEvent.OnPrePlayerDraggedItem -= PlayerEvent.OnPreItemDragged;
            UnturnedPatchEvent.OnPrePlayerSwappedItem -= PlayerEvent.OnPreItemSwapped;
#if RF
            Level.onPostLevelLoaded -= OnPostLevelLoaded;
#endif
            Library.DetachEvent(true);
#if RF
            Library.Uninitialize();
#endif

            foreach (var steamPlayer in Provider.clients)
                steamPlayer.player.gameObject.TryRemoveComponent<PlayerComponent>();
        }

        Inst = null;
        Conf = null;

        Logger.LogWarning($"[{Name}] Plugin unloaded successfully!");
    }

    public override TranslationList DefaultTranslations => new()
        {
            {$"{EResponse.BLACKLIST}", "[Sellbox] BLACKLIST: {0} ({1})"},
            {$"{EResponse.INVALID_PARAMETER}", "[Sellbox] Invalid parameter! Usage: {0}"},
            {$"{EResponse.IN_VEHICLE}", "[Sellbox] Accessing Vault while in a vehicle is not allowed!"},
            {$"{EResponse.NO_PERMISSION}", "[Sellbox] You don't have permission to access {0} Vault!"},
            {$"{EResponse.NO_PERMISSION_ALL}", "[Sellbox] You don't have permission to access any Vault!"},
            {$"{EResponse.VAULT_NOT_FOUND}", "[Sellbox] Vault not found!"},
            {
                $"{EResponse.VAULT_NOT_SELECTED}",
                "[Sellbox] Please set default Vault first! /vset <vaultName> or /vault <vaultName>"
            },
            {$"{EResponse.VAULT_PROCESSING}", "[Sellbox] Processing vault. Please wait..."},
            {$"{EResponse.VAULTS}", "[Sellbox] Available Vaults: {0}"},
            {$"{EResponse.VAULTSET}", "[Sellbox] Successfully set {0} Vault as default Vault!"},
            {$"{EResponse.SAME_DATABASE}", "[Sellbox] You can't run migrate to the same database!"},
            {$"{EResponse.MIGRATION_START}", "[Sellbox] Starting migration from {0} to {1}..."},
            {$"{EResponse.MIGRATION_FINISH}", "[Sellbox] Migration finished!"},
            {$"{EResponse.DATABASE_NOT_READY}", "[Sellbox] Database is not ready. Please wait..."},
            {$"{EResponse.PLAYER_VAULT_NOT_FOUND}", "[Sellbox] {0} doesn't have {1} Vault!"},
            {$"{EResponse.ADMIN_VAULT_CLEAR}", "[Sellbox] Successfully cleared {0}'s {1} Vault"},
            {$"{EResponse.VAULT_CLEAR}", "[Sellbox] Successfully cleared {0} Vault!"},
            {$"{EResponse.VAULT_BUSY}", "[Sellbox] Someone is using this vault! Please wait until they are finished!"},
            {$"{EResponse.VAULT_SYSTEM_BUSY}", "[Sellbox] Try again later. Vault system is busy..."},
            {$"{EResponse.PLAYER_NOT_FOUND}", "[Sellbox] Can't find player under name {0}!"},
        };
}