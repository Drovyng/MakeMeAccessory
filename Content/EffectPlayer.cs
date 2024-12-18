using MakeMeAccessory.Content.Buffs;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace MakeMeAccessory.Content
{
    public class EffectPlayer : ModPlayer
    {
        public List<(Item, bool)> EffectAccessories = new();
        public List<(Item, bool)> EffectArmor = new();
        public string SetBonuses = "";

        public override void PreUpdate()
        {
            EffectAccessories.Clear();
            EffectArmor.Clear();
        }
        public override void UpdateEquips()
        {
            for (int i = 0; i < EffectAccessories.Count; i++)
            {
                var item = EffectAccessories[i];
                Player.GrantPrefixBenefits(item.Item1);
                Player.GrantArmorBenefits(item.Item1);
                Player.ApplyEquipFunctional(item.Item1, item.Item2);
            }
            foreach (var item in EffectArmor)
            {
                Player.GrantArmorBenefits(item.Item1);
                Player.ApplyEquipFunctional(item.Item1, item.Item2);
            }
        }
        public override void PostUpdateEquips()
        {
            var head = Player.armor[0];
            var body = Player.armor[1];
            var legs = Player.armor[2];
            var setbonus = Player.setBonus;
            SetBonuses = "";
            bool useBuff = false;
            bool flag = true;
            for (int i = 0; i < EffectArmor.Count; i += 3)  // FUCK IT! I forgot to remove "EffectArmor.Count / 3" and nothing works!
            {
                Player.armor[0] = EffectArmor[i].Item1;
                Player.armor[1] = EffectArmor[i + 1].Item1;
                Player.armor[2] = EffectArmor[i + 2].Item1;
                Player.head = Player.armor[0].headSlot;
                Player.body = Player.armor[1].bodySlot;
                Player.legs = Player.armor[2].legSlot;

                Player.UpdateArmorLights();
                Player.UpdateArmorSets(Player.whoAmI);
                if (Player.setBonus.Length > 0)
                {
                    var color = "\n[c/" + (flag ? "FFFFFF:" : "D0D0D0:");
                    flag = !flag;
                    var split = Player.setBonus.Split("\n");
                    foreach (var item in split)
                    {
                        SetBonuses += color + item + "]";
                    }
                    useBuff = true;
                }
            }
            if (Player.HasBuff<ArmorSetsBuff>())
            {
                if (!useBuff) Player.ClearBuff(ModContent.BuffType<ArmorSetsBuff>());
            }
            else
            {
                if (useBuff) Player.AddBuff(ModContent.BuffType<ArmorSetsBuff>(), 2000000000, false);
            }
            if (SetBonuses.Length > 0) SetBonuses = SetBonuses.Substring(1);
            Player.armor[0] = head;
            Player.armor[1] = body;
            Player.armor[2] = legs;
            Player.head = Player.armor[0].headSlot; // <- FUCK IT!
            Player.body = Player.armor[1].bodySlot; // I forgot to do that
            Player.legs = Player.armor[2].legSlot;
            Player.setBonus = setbonus;
        }
        public override void UpdateVisibleAccessories()
        {
            foreach (var item in EffectAccessories)
            {
                Player.UpdateVisibleAccessories(item.Item1, item.Item2);
            }
        }
    }
}
