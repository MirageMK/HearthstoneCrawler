using System;
using System.ComponentModel;
using System.Reflection;

namespace HSCore
{
    public enum SourceEnum
    {
        TempoStorm,
        HearthstoneTopDecks,
        ViciousSyndicate,
        Metabomb
    }

    public enum SetEnum
    {
        [Description("Basic")]
        Basic = 1,
        [Description("Classic")]
        Classic = 2,
        [Description("Promo")]
        Promo = 3,
        [Description("Reward")]
        Reward = 4,
        [Description("Naxxramas")]
        Naxx = 5,
        [Description("Goblins vs Gnomes")]
        GvG = 6,
        [Description("Blackrock Mountain")]
        BRM = 7,
        [Description("The Grand Tournament")]
        TGT = 8,
        [Description("The League of Explorers")]
        LoE = 9,
        [Description("Whispers of the Old Gods")]
        WotOG = 10,
        [Description("Karazhan")]
        Kara = 11,
        [Description("Mean Streets of Gadgetzan")]
        MSoG = 12
    }

    public enum DeckType
    {
        [Description("Undefined")]
        Undefined,
        [Description("Standard")]
        Standard,
        [Description("Wild")]
        Wild
    }

    public static class Enums
    {
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                                                                typeof(DescriptionAttribute),
                                                                false);

            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        public static T GetValueFromDescription<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
            return default(T);
        }
    }
}
