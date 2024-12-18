using Humanizer;
using MakeMeAccessory.Content.UI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace MakeMeAccessory.Content.Items
{
    public class AccessoryBox : ModItem
    {
        public List<Item> items = new();
        public virtual bool isArmor => false;
        public override void SetDefaults()
        {
            Item.accessory = true;
        }
        public virtual int GetCount()
        {
            return items.Count;
        }
        public override LocalizedText Tooltip => LocalizedText.Empty;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Insert(tooltips.FindIndex((l) => l.Name == "Tooltip0"), new TooltipLine(Mod, "TooltipR", this.GetLocalization("Tooltip").Format(GetCount())));
        }
        public override ModItem Clone(Item newEntity)
        {
            var clone = base.Clone(newEntity);
            var insert = new List<Item>();
            foreach (var item in items)
            {
                insert.Add(item.Clone());
            }
            ((AccessoryBox)clone).items = insert;

            return clone;
        }
        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(items.Count);
            foreach (var item in items)
            {
                writer.Write(item.type);
                writer.Write(item.prefix);
                var isMod = item.ModItem != null;
                writer.Write(isMod);
                if (isMod)
                {
                    item.ModItem.NetSend(writer);
                }
            }
        }
        public override void NetReceive(BinaryReader reader)
        {
            items = new List<Item>();
            var count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var item = new Item(reader.ReadInt32(), 1, reader.ReadInt32());
                var isMod = reader.ReadBoolean();
                if (isMod)
                {
                    item.ModItem.NetReceive(reader);
                }
                items.Add(item);
            }
        }
        public override bool CanRightClick()
        {
            return true;
        }
        public override void RightClick(Player player)
        {
            Item.stack += 1;
            if (player != Main.LocalPlayer) return;
            UIDrawStorage.OpenedItem = this;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i] == Item)
                {
                    UIDrawStorage.OpenedItemSlot = i;
                }
            }
            UIDrawStorage.IsArmor = isArmor;
            MakeMeAccessory.CustomUIState.Open();
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (Main.LocalPlayer != player) return;
            foreach (var item in items)
            {
                player.GetModPlayer<EffectPlayer>().EffectAccessories.Add((item, hideVisual));
            }
        }
        public override void SaveData(TagCompound tag)
        {
            tag["items"] = items.ToArray();
        }
        public override void LoadData(TagCompound tag)
        {
            items = tag.Get<Item[]>("items").ToList();
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
