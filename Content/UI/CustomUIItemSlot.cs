using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using Terraria;
using Microsoft.Xna.Framework;

namespace MakeMeAccessory.Content.UI
{
    public class CustomUIItemSlot : UIElement
    {
        private Item[] _itemArray;
        private int _itemIndex;
        private int _itemSlotContext;
        private int _slot;
        private Item[] _onlySlot;

        public CustomUIItemSlot(Item[] itemArray, int itemIndex, int slot, int itemSlotContext)
        {
            _itemArray = itemArray;
            _itemIndex = itemIndex;
            _itemSlotContext = itemSlotContext;
            _slot = slot;
            _onlySlot = new Item[_slot+1];
            _onlySlot[_slot] = _itemArray[_itemIndex];
            Width = new StyleDimension(48f, 0f);
            Height = new StyleDimension(48f, 0f);
        }

        private void HandleItemSlotLogic()
        {
            if (IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
                _onlySlot[_slot] = _itemArray[_itemIndex];
                ItemSlot.OverrideHover(_onlySlot, _itemSlotContext, _slot);
                ItemSlot.LeftClick(_onlySlot, _itemSlotContext, _slot);
                ItemSlot.RightClick(_onlySlot, _itemSlotContext, _slot);
                ItemSlot.MouseHover(_onlySlot, _itemSlotContext, _slot);
                _itemArray[_itemIndex] = _onlySlot[_slot];
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            HandleItemSlotLogic();
            Vector2 position = GetDimensions().Center() + new Vector2(52f, 52f) * -0.5f * Main.inventoryScale;
            ItemSlot.Draw(spriteBatch, _onlySlot, _itemSlotContext, _slot, position);
        }
    }
}
