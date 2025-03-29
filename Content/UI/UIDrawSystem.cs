using MakeMeAccessory.Content.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace MakeMeAccessory.Content.UI
{
    public class UIDrawStorage : UIState
    {
        public static AccessoryBox OpenedItem;
        public static bool IsArmor;
        public static int OpenedItemSlot;


        public List<CustomUIItemSlot> slots = new();
        public UIPanel panel;
        public UIPanel buttonEffects;
        public UIPanel buttonProtection;
        public Item[] items = [];
        public UIDrawStorage() { }
        public Color RedOrGreen(Color value, bool active)
        {
            if (active)
            {
                var g = value.G;
                value.G = value.B;
                value.B = g;
                return value;
            }
            var r = value.R;
            value.R = value.B;
            value.B = r;
            return value;
        }
        public void Open()
        {
            panel = new();
            buttonEffects = new()
            {
                Width = { Precent = 0.5f, Pixels = -8 }, Height = { Pixels = 32 },
            };
            buttonEffects.WithFadedMouseOver();
            buttonEffects.SetPadding(0);
            buttonEffects.Append(new UIText(Language.GetText("Mods.MakeMeAccessory.ButtonEffects"), 0.95f){
                Width = { Precent = 1 }, Height = { Precent = 1 }, TextOriginX = 0.5f, TextOriginY = 0.5f
            });
            buttonEffects.OnMouseOver += (_, _) => { buttonEffects.BackgroundColor = RedOrGreen(buttonEffects.BackgroundColor, OpenedItem.AllowEffects); };
            buttonEffects.OnMouseOut += (_, _) => { buttonEffects.BackgroundColor = RedOrGreen(buttonEffects.BackgroundColor, OpenedItem.AllowEffects); };
            buttonEffects.OnLeftClick += (_, _) => { OpenedItem.AllowEffects = !OpenedItem.AllowEffects; buttonEffects.MouseOver(null); };
            buttonEffects.MouseOut(null);
            panel.Append(buttonEffects);

            buttonProtection = new()
            {
                Width = { Precent = 0.5f, Pixels = 8 }, Height = { Pixels = 32 }, Left = { Precent = 0.5f, Pixels = -8 }
            };
            buttonProtection.WithFadedMouseOver();
            buttonProtection.SetPadding(0);
            buttonProtection.Append(new UIText(Language.GetText("Mods.MakeMeAccessory.ButtonProtection"), 0.85f)
            {
                Width = { Precent = 1 }, Height = { Precent = 1 }, TextOriginX = 0.5f, TextOriginY = 0.5f
            });
            buttonProtection.OnMouseOver += (_, _) => { buttonProtection.BackgroundColor = RedOrGreen(buttonProtection.BackgroundColor, OpenedItem.AllowProtection); };
            buttonProtection.OnMouseOut += (_, _) => { buttonProtection.BackgroundColor = RedOrGreen(buttonProtection.BackgroundColor, OpenedItem.AllowProtection); };
            buttonProtection.OnLeftClick += (_, _) => { OpenedItem.AllowProtection = !OpenedItem.AllowProtection; buttonProtection.MouseOver(null); };
            buttonProtection.MouseOut(null);
            panel.Append(buttonProtection);

            holding = false;

            Main.playerInventory = true;
            RemoveAllChildren();

            var max = IsArmor ? ModContent.GetInstance<MMAConfig>().MaxArmorSets * 3 : ModContent.GetInstance<MMAConfig>().MaxAccessories;

            Append(panel);
            panel.OnLeftMouseDown += _LeftMouseDown;
            panel.OnLeftMouseUp += _LeftMouseUp;
            slots = new();
            var itemsCopy = OpenedItem.items.ToArray().ToList();
            while ((itemsCopy.Count % 3 != 0 || itemsCopy.Count < 9 || itemsCopy.Count < OpenedItem.items.Count + 3) && itemsCopy.Count < max) itemsCopy.Add(new Item());
            items = itemsCopy.ToArray();
            for (int i = 0; i < itemsCopy.Count; i++)
            {
                var slot = new CustomUIItemSlot(items, i, i % 3, IsArmor ? 8 : 10);
                slot.Left.Pixels = i % 3 * 48;
                slot.Top.Pixels = i / 3 * 48 + 32;
                slot.OnLeftMouseDown += (_, _) => Save();
                slot.OnLeftMouseUp += (_, _) => Save();
                slots.Add(slot);
                panel.Append(slot);
            }
            panel.Width.Pixels = 170;
            panel.Height.Pixels = itemsCopy.Count / 3 * 50 + 20 + 32;
            panel.Left.Pixels = (int)(Main.screenWidth * 0.5f - panel.Width.Pixels * 0.5f);
            panel.Top.Pixels = (int)(Main.screenHeight * 0.5f - panel.Height.Pixels * 0.5f);
            panel.Recalculate();

        }
        public bool holding;
        public Vector2 lastPos;
        public void _LeftMouseDown(UIMouseEvent evt, UIElement listeningElement)
        {
            if (evt.Target == listeningElement && !Main.LocalPlayer.mouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
                holding = true;
                lastPos = evt.MousePosition;
            }
        }
        public override void LeftMouseUp(UIMouseEvent evt)
        {
            holding = false;
        }
        public void Save()
        {
            var list = items.ToList();
            if (IsArmor)
            {
                for (int i = 0; i < list.Count - 3; i+=3)
                {
                    if (list[i].IsAir && list[i+1].IsAir && list[i+2].IsAir)
                    {
                        list.RemoveAt(i);
                        list.RemoveAt(i);
                        list.RemoveAt(i);
                        i -= 3;
                    }
                }
            }
            else
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].IsAir)
                    {
                        list.RemoveAt(i);
                        i--;
                    }
                }
            }
            OpenedItem.items = list;
            if (list.Count + 3 > items.Length || list.Count - 3 <= items.Length)
            {
                var left = panel.Left.Pixels;
                var top = panel.Top.Pixels;
                Open();
                panel.Left.Pixels = left;
                panel.Top.Pixels = top;
                panel.Recalculate();
            }
        }
        public void _LeftMouseUp(UIMouseEvent evt, UIElement listeningElement)
        {
            holding = false;
        }
        public override void Update(GameTime gameTime)
        {
            if (!Main.playerInventory || Main.LocalPlayer == null || Main.LocalPlayer.inventory[OpenedItemSlot] != OpenedItem.Item)
            {
                OpenedItem = null;
            }
            Main.LocalPlayer.mouseInterface = IsMouseHovering;
            if (holding)
            {
                Main.LocalPlayer.mouseInterface = true;
                var pos = new Vector2(Main.mouseX, Main.mouseY);
                panel.Left.Pixels += pos.X - lastPos.X;
                panel.Top.Pixels += pos.Y - lastPos.Y;
                panel.Recalculate();
                lastPos = pos;
            }
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (holding) Main.LocalPlayer.mouseInterface = true;
            Main.inventoryScale = 0.85f;
            base.Draw(spriteBatch);
        }
    }
    public class UIDrawSystem : ModSystem
    {
        public override void OnWorldUnload()
        {
            UIDrawStorage.OpenedItem = null;
        }
        public override void ClearWorld()
        {
            UIDrawStorage.OpenedItem = null;
        }
        public override void UpdateUI(GameTime gameTime)
        {
            if (UIDrawStorage.OpenedItem != null) MakeMeAccessory.CustomUI.Update(gameTime);
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            layers.Insert(layers.FindIndex(layer => layer.Name == "Vanilla: Mouse Text"), new LegacyGameInterfaceLayer(
                "MakeMeAccessory: Storage",
                delegate
                {
                    if (UIDrawStorage.OpenedItem != null) MakeMeAccessory.CustomUIState.Draw(Main.spriteBatch);
                    return true;
                }, InterfaceScaleType.UI)
            );
        }
    }
}
