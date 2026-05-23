using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project1
{
    public class InventorySlot
    {
        public PickupItem Item = null;
        public int Rotation = 0; // 0,90,180,270
        public int Quantity = 1; // for stackable items like ammo
    }

    public class Inventory
    {
        GameManager gm;
        public List<PickupItem> Items = new List<PickupItem>();
        public List<PickupItem> KeyItems = new List<PickupItem>();
        public int HotbarSize = 4;
        public InventorySlot[] Hotbar;
        public bool Open = false;
        public int SelectedIndex = 0;

        public int GridW = 12, GridH = 12;
        public InventorySlot[,] Grid;

        InventorySlot HeldSlot = null;
        Point HeldFrom = Point.Zero;
        bool HoldingFromHotbar = false;

        public int AmmoReserve = 0;

        public Inventory(GameManager gm)
        {
            this.gm = gm;
            Hotbar = new InventorySlot[HotbarSize];
            Grid = new InventorySlot[GridW, GridH];
            for (int i = 0; i < HotbarSize; i++) Hotbar[i] = new InventorySlot();
            for (int x = 0; x < GridW; x++)
                for (int y = 0; y < GridH; y++)
                    Grid[x, y] = new InventorySlot();
        }

        public bool AddItem(PickupItem it)
        {
            if (it.IsKeyItem)
            {
                if (!KeyItems.Any(k => k.Id == it.Id))
                {
                    KeyItems.Add(it);
                    return true;
                }
                else
                {
                    gm.Log($"{it.Id} already collected!");
                    return false;
                }
            }

            if (it.Id == "ammo")
            {
                AmmoReserve += (it is AmmoPickup ap ? ap.Amount : 1);
                return true;
            }

            if (it.Id.StartsWith("herb"))
            {
                for (int i = 0; i < HotbarSize; i++)
                {
                    if (Hotbar[i].Item != null && Hotbar[i].Item.Id == it.Id)
                    {
                        Hotbar[i].Quantity++;
                        return true;
                    }
                }
                for (int x = 0; x < GridW; x++)
                {
                    for (int y = 0; y < GridH; y++)
                    {
                        if (Grid[x, y].Item != null && Grid[x, y].Item.Id == it.Id)
                        {
                            Grid[x, y].Quantity++;
                            return true;
                        }
                    }
                }
            }

            for (int i = 0; i < HotbarSize; i++)
            {
                if (Hotbar[i].Item == null)
                {
                    Hotbar[i].Item = it;
                    Hotbar[i].Quantity = 1;
                    return true;
                }
            }

            for (int x = 0; x < GridW; x++)
            {
                for (int y = 0; y < GridH; y++)
                {
                    if (Grid[x, y].Item == null)
                    {
                        Grid[x, y].Item = it;
                        Grid[x, y].Quantity = 1;
                        return true;
                    }
                }
            }

            gm.Log("Inventory full!");
            return false;
        }

        public void ToggleOpen() => Open = !Open;

        public void Update(GameTime gt)
        {
            if (!Open) return;
            HandleMouseInventory(new Vector2(200, 220), 32);
        }

        public void Draw(SpriteBatch sb)
        {
            int slotSize = 40;
            int padding = 8;
            int screenHeight = gm.graphics.Viewport.Height;

            // --- Draw Hotbar ---
            // --- Draw Hotbar ---
            for (int i = 0; i < HotbarSize; i++)
            {
                var pos = new Vector2(20 + i * (slotSize + padding), screenHeight - slotSize - 25);
                var slotRect = new Rectangle((int)pos.X - 2, (int)pos.Y - 2, slotSize, slotSize);

                // ช่องพื้นหลัง
                sb.Draw(gm.WhitePixel, slotRect, Color.Gray * 0.5f);

                // วาดไอเท็มตรงกลาง
                var s = Hotbar[i];
                if (s.Item != null && s.Item.Texture != null)
                {
                    Texture2D tex = s.Item.Texture;

                    float scale = 1.1f;
                    float maxDim = Math.Max(tex.Width, tex.Height);
                    if (maxDim > slotSize * 0.9f)
                        scale = (slotSize * 0.9f) / maxDim;

                    Vector2 texPos = pos + new Vector2(slotSize / 2f-2, slotSize / 2f -2);
                    Vector2 origin = new Vector2(tex.Width / 2f, tex.Height / 2f);

                    sb.Draw(tex, texPos, null, Color.White, MathHelper.ToRadians(s.Rotation),
                            origin, scale, SpriteEffects.None, 0f);
                }

                // ตัวเลขช่อง
                sb.DrawString(gm.DefaultFont, (i + 1).ToString(),
                    pos + new Vector2(14, slotSize + 4), Color.Yellow, 0f, Vector2.Zero, 0.82f, SpriteEffects.None, 0f);

                // ขอบช่องที่เลือก
                if (i == SelectedIndex)
                    sb.Draw(gm.WhitePixel, slotRect, Color.White * 0.2f);
            }


            if (!Open) return;

            // --- Draw Grid ---
            Vector2 basePos = new Vector2(200, 220);
            int gridSlotSize = 32;

            for (int y = 0; y < GridH; y++)
            {
                for (int x = 0; x < GridW; x++)
                {
                    Vector2 slotPos = basePos + new Vector2(x * (gridSlotSize + 4), y * (gridSlotSize + 4));
                    sb.Draw(gm.WhitePixel, new Rectangle((int)slotPos.X, (int)slotPos.Y, gridSlotSize, gridSlotSize), Color.Gray * 0.4f);

                    var s = Grid[x, y];
                    if (s.Item != null)
                    {
                        var tex = s.Item.Texture;
                        Vector2 texPos = slotPos + new Vector2((gridSlotSize - tex.Width) / 2f, (gridSlotSize - tex.Height) / 2f);
                        sb.Draw(tex, texPos, null, Color.White, MathHelper.ToRadians(s.Rotation), Vector2.Zero, 1f, SpriteEffects.None, 0f);

                        if (s.Quantity > 1)
                            sb.DrawString(gm.DefaultFont, s.Quantity.ToString(), slotPos + new Vector2(gridSlotSize - 14, gridSlotSize - 14), Color.White);
                    }
                }
            }

            // --- Draw Held Item ---
            if (HeldSlot != null && HeldSlot.Item != null)
            {
                Vector2 mousePos = InputManager.MouseState.Position.ToVector2();
                var tex = HeldSlot.Item.Texture;
                Vector2 texPos = mousePos - new Vector2(tex.Width / 2f, tex.Height / 2f);
                sb.Draw(tex, texPos, null, Color.White, MathHelper.ToRadians(HeldSlot.Rotation), Vector2.Zero, 1f, SpriteEffects.None, 0f);

                if (HeldSlot.Quantity > 1)
                    sb.DrawString(gm.DefaultFont, HeldSlot.Quantity.ToString(), mousePos + new Vector2(8, 8), Color.White);
            }
        }

        void HandleMouseInventory(Vector2 basePos, int slotSize)
        {
            var ms = InputManager.MouseState;
            Vector2 m = ms.Position.ToVector2();

            int hotbarSlotSize = 40;
            int hotbarPadding = 8;

            // Hotbar
            for (int i = 0; i < HotbarSize; i++)
            {
                var pos = new Vector2(20 + i * (hotbarSlotSize + hotbarPadding), gm.graphics.Viewport.Height - hotbarSlotSize - 25);
                Rectangle r = new Rectangle((int)pos.X - 2, (int)pos.Y - 2, hotbarSlotSize, hotbarSlotSize);

                if (r.Contains(ms.Position))
                {
                    if (InputManager.MouseLeftPressed())
                        PickupOrSwap(ref Hotbar[i], i, -1, true);

                    if (InputManager.MouseRightPressed() && Hotbar[i].Item != null)
                        Hotbar[i].Rotation = (Hotbar[i].Rotation + 90) % 360;
                }
            }

            // Grid
            for (int y = 0; y < GridH; y++)
            {
                for (int x = 0; x < GridW; x++)
                {
                    Vector2 slotPos = basePos + new Vector2(x * (slotSize + 4), y * (slotSize + 4));
                    Rectangle r = new Rectangle((int)slotPos.X, (int)slotPos.Y, slotSize, slotSize);

                    if (r.Contains(ms.Position))
                    {
                        if (InputManager.MouseLeftPressed())
                            PickupOrSwap(ref Grid[x, y], x, y, false);

                        if (InputManager.MouseRightPressed() && Grid[x, y].Item != null)
                            Grid[x, y].Rotation = (Grid[x, y].Rotation + 90) % 360;
                    }
                }
            }

            if (HeldSlot != null && InputManager.MouseLeftReleased())
                TryPlaceHeldSlot(basePos, slotSize);
        }

        void PickupOrSwap(ref InventorySlot slot, int x, int y, bool isHotbar)
        {
            if (HeldSlot == null && slot.Item != null)
            {
                HeldSlot = new InventorySlot { Item = slot.Item, Rotation = slot.Rotation, Quantity = slot.Quantity };
                slot.Item = null;
                HeldFrom = new Point(x, y);
                HoldingFromHotbar = isHotbar;
            }
            else if (HeldSlot != null)
            {
                var tmp = slot;
                slot = HeldSlot;
                HeldSlot = tmp;
            }
        }

        void TryPlaceHeldSlot(Vector2 basePos, int slotSize)
        {
            var msPos = InputManager.MouseState.Position;

            int hotbarSlotSize = 40;
            int hotbarPadding = 8;

            // Hotbar
            for (int i = 0; i < HotbarSize; i++)
            {
                var pos = new Vector2(20 + i * (hotbarSlotSize + hotbarPadding), gm.graphics.Viewport.Height - hotbarSlotSize - 25);
                Rectangle r = new Rectangle((int)pos.X - 2, (int)pos.Y - 2, hotbarSlotSize, hotbarSlotSize);
                if (r.Contains(msPos))
                {
                    Hotbar[i] = HeldSlot;
                    HeldSlot = null;
                    return;
                }
            }

            // Grid
            for (int y = 0; y < GridH; y++)
            {
                for (int x = 0; x < GridW; x++)
                {
                    Vector2 slotPos = basePos + new Vector2(x * (slotSize + 4), y * (slotSize + 4));
                    Rectangle r = new Rectangle((int)slotPos.X, (int)slotPos.Y, slotSize, slotSize);
                    if (r.Contains(msPos))
                    {
                        Grid[x, y] = HeldSlot;
                        HeldSlot = null;
                        return;
                    }
                }
            }

            HeldSlot = null;
        }

        public void UseSelectedItem()
        {
            var itSlot = Hotbar[SelectedIndex];
            if (itSlot == null || itSlot.Item == null) return;
            var it = itSlot.Item;

            if (!it.IsKeyItem)
            {
                if (it.Id.StartsWith("herb"))
                {
                    gm.Player.Health = Math.Min(gm.Player.MaxHealth, gm.Player.Health + 40);
                    itSlot.Item = null;
                }
            }
        }

        public void Clear()
        {
            Items.Clear();
            for (int i = 0; i < HotbarSize; i++) Hotbar[i] = new InventorySlot();
            for (int x = 0; x < GridW; x++)
                for (int y = 0; y < GridH; y++)
                    Grid[x, y] = new InventorySlot();
            AmmoReserve = 0;
        }
    }
}
