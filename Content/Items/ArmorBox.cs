﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MakeMeAccessory.Content.Items
{
    public class ArmorBox : AccessoryBox
    {
        public override bool isArmor => true;

        public override int GetCount()
        {
            return (items.Count - 1) / 3;
        }
        public override int GetMaxCount()
        {
            return ModContent.GetInstance<MMAConfig>().MaxArmorSets;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (Main.LocalPlayer != player) return;
            foreach (var item in items)
            {
                player.GetModPlayer<EffectPlayer>().EffectArmor.Add((item, hideVisual, AllowEffects, AllowProtection));
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddRecipeGroup(RecipeGroupID.IronBar, 15)
                .AddRecipeGroup(RecipeGroupID.Wood, 40)
                .AddTile(TileID.Anvils)
                .Register();

        }
    }
}
