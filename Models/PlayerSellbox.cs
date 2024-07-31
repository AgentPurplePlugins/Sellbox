using System;
using RFRocketLibrary.Models;

namespace Sellbox.Models
{
    [Serializable]
    public class PlayerSellbox
    {
        public int Id { get; set; }
        public ulong SteamId { get; set; }
        public string SellboxName { get; set; } = string.Empty;
        public ItemsWrapper SellboxContent { get; set; } = new();
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        public PlayerSellbox()
        {
        }

        public Sellbox GetSellbox()
        {
            return Sellbox.Parse(SellboxName);
        }

        public override bool Equals(object obj)
        {
            if (obj is not PlayerSellbox playerSellbox)
                return false;

            return playerSellbox.Id == Id || playerSellbox.SteamId == SteamId && playerSellbox.SellboxName == SellboxName;
        }

        protected bool Equals(PlayerSellbox other)
        {
            return SteamId == other.SteamId && SellboxName == other.SellboxName;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (SteamId.GetHashCode() * 397) ^ (SellboxName != null ? SellboxName.GetHashCode() : 0);
            }
        }

        public static int HashCode(ulong steamId, string sellboxName)
        {
            unchecked
            {
                return (steamId.GetHashCode() * 397) ^ (sellboxName?.GetHashCode() ?? 0);
            }
        }
    }
}