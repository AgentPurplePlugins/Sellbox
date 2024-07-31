using System.Collections.Generic;
using Sellbox.Utils;
using Rocket.API;

namespace Sellbox
{
    public class Configuration : IRocketPluginConfiguration
    {
        public bool Enabled;
        public bool DebugMode;
        public string MySqlConnectionString;
        public string MessageColor;
        public string AnnouncerIconUrl;
        public HashSet<Sellbox> Sellboxs;
        public HashSet<Blacklist> BlacklistedItems;

        public void LoadDefaults()
        {
            Enabled = true;
            DebugMode = false;
            MessageColor = "magenta";
            AnnouncerIconUrl = "https://cdn.jsdelivr.net/gh/RiceField-Plugins/UnturnedImages@images/plugin/RFVault/RFVault.png";
            Sellboxs = new HashSet<Sellbox>
            {
                new("Sellbox", "sellbox", 5, 5),
            };
            BlacklistedItems = new HashSet<Blacklist>
            {
                new("vaultbypass.example", new List<ushort> {1, 2}),
                new("vaultbypass.example1", new List<ushort> {3, 4}),
            };
        }
    }