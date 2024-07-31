using System;
using System.Linq;
using System.Xml.Serialization;

namespace Sellbox.Models
{
    public class Sellbox
    {
        [XmlAttribute] public string Name = string.Empty;
        [XmlAttribute] public string Permission = string.Empty;
        [XmlAttribute] public byte Width;
        [XmlAttribute] public byte Height;

        public Sellbox()
        {
        }

        public Sellbox(string name, string permission, byte width, byte height)
        {
            Name = name;
            Permission = permission;
            Width = width;
            Height = height;
        }

        public override bool Equals(object obj)
        {
            if (obj is not Sellbox sellbox)
                return false;

            return sellbox.Name == Name;
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        public static Sellbox Parse(string sellboxName)
        {
            return Plugin.Conf.Sellboxs.FirstOrDefault(virtualLocker =>
                string.Equals(virtualLocker.Name, sellboxName, StringComparison.CurrentCultureIgnoreCase));
        }

        public static bool TryParse(string sellboxName, out Sellbox sellbox)
        {
            sellbox = null;
            foreach (var virtualLocker in Plugin.Conf.Sellboxs.Where(virtualLocker =>
                string.Equals(virtualLocker.Name, sellboxName, StringComparison.CurrentCultureIgnoreCase)))
            {
                sellbox = virtualLocker;
                return true;
            }

            return false;
        }
    }
}