using Terraria;
using Terraria.ModLoader;

namespace MakeMeAccessory.Content.Buffs
{
    public class ArmorSetsBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override bool RightClick(int buffIndex)
        {
            return false;
        }
        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            tip = Main.LocalPlayer.GetModPlayer<EffectPlayer>().SetBonuses;
        }
    }
}
