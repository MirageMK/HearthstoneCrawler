﻿using System;
using System.ComponentModel;
using System.Reflection;

namespace HSCore
{
    public enum SourceEnum
    {
        TempoStorm,
        HearthstoneTopDecks,
        ViciousSyndicate,
        Metabomb,
        DisguisedToast,
        HSReplay
    }

    public enum SetEnum
    {
        [ Description("Basic") ] Basic = 1,
        [ Description("Classic") ] Classic = 2,
        [ Description("Hall of Fame") ] HoF = 3,
        [ Description("Naxxramas") ] Naxx = 4,
        [ Description("Goblins vs Gnomes") ] GvG = 5,
        [ Description("Blackrock Mountain") ] BRM = 6,
        [ Description("The Grand Tournament") ] TGT = 7,
        [ Description("The League of Explorers") ] LoE = 8,
        [ Description("Whispers of the Old Gods") ] WotOG = 9,
        [ Description("One Night in Karazhan") ] Kara = 10,
        [ Description("Mean Streets of Gadgetzan") ] MSoG = 11,
        [ Description("Journey to Un\'Goro") ] JtU = 12,
        [ Description("Knights of the Frozen Throne") ] KotFT = 13,
        [ Description("Kobolds & Catacombs") ] KnC = 14,
        [ Description("The Witchwood")] TW = 15,
        [ Description("The Boomsday Project")] TBP = 16,
        [ Description("Rastakhan's Rumble")] RR = 17,
        [ Description("Rise of Shadows")] RoS = 18,
        [ Description("Saviors of Uldum")] SoU = 19,
        [ Description("Descent of Dragons")] DoD = 20,
        [ Description("Galakrond's Awakening")] GA = 21,
        [ Description("Demon Hunter Initiate")] DHI = 22,
        [ Description("Ashes of Outland")] AoO = 23
    }

    public enum DeckType
    {
        [ Description("Undefined") ] Undefined,
        [ Description("Standard") ] Standard,
        [ Description("Wild") ] Wild
    }

    public static class Enums
    {
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[]) fi.GetCustomAttributes(
                                                                typeof(DescriptionAttribute),
                                                                false);

            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        public static T GetValueFromDescription<T>(string description)
        {
            Type type = typeof(T);
            if(!type.IsEnum) throw new InvalidOperationException();
            foreach(FieldInfo field in type.GetFields())
            {
                DescriptionAttribute attribute = Attribute.GetCustomAttribute(field,
                                                                              typeof(DescriptionAttribute)) as DescriptionAttribute;
                if(attribute != null)
                {
                    if(attribute.Description == description)
                        return (T) field.GetValue(null);
                }
                else
                {
                    if(field.Name == description)
                        return (T) field.GetValue(null);
                }
            }
            return default(T);
        }
    }
}