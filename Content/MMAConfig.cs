using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace MakeMeAccessory.Content
{
    public class MMAConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;
        [DefaultValue(30)]
        [Range(9, 90)]
        [Increment(3)]
        public int MaxAccessories;
        [DefaultValue(10)]
        [Range(3, 30)]
        public int MaxArmorSets;
    }
}
