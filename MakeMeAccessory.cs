using MakeMeAccessory.Content.UI;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace MakeMeAccessory
{
	public class MakeMeAccessory : Mod
	{
		public static UserInterface CustomUI;
        public static UIDrawStorage CustomUIState;
        public override void Load()
        {
            if (!Main.dedServ)
            {
                CustomUI = new UserInterface();
                CustomUIState = new UIDrawStorage();
                CustomUIState.Initialize();
                CustomUI.SetState(CustomUIState);
            }
        }
    }
}
