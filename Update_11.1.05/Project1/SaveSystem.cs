using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project1.NPC;

namespace Project1
{
    // ---------- DTO ----------
    public class SlotDTO
    {
        public string ItemId { get; set; }
        public int Quantity { get; set; } = 1;
        public int Rotation { get; set; } = 0;
    }

    public class GridSlotDTO : SlotDTO
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class NoteDTO
    {
        public string Id { get; set; }
        public string Text { get; set; }
    }

    public class SaveData
    {
        public int Version { get; set; } = 1;
        public DateTime SavedAt { get; set; } = DateTime.UtcNow;

        // Player
        public int Health { get; set; }
        public float Stamina { get; set; }
        public float Battery { get; set; }
        public int AmmoInMagazine { get; set; }
        public bool HasGun { get; set; }

        // Progress
        public int Day { get; set; } = 1;

        // Campfire
        public int CampfireLevel { get; set; } = 1;
        public float CampfireMaxDuration { get; set; } = 120f;
        public float CampfireRemainingTime { get; set; } = 120f;

        // Inventory
        public int AmmoReserve { get; set; }
        public SlotDTO[] Hotbar { get; set; } = new SlotDTO[4];
        public List<GridSlotDTO> Grid { get; set; } = new List<GridSlotDTO>();
        public List<string> KeyItemIds { get; set; } = new List<string>();

        // Codex
        public List<NoteDTO> PlayerNotes { get; set; } = new List<NoteDTO>();
        public List<string> NPCDialogueLines { get; set; } = new List<string>();
    }

    // ---------- System ----------
    public class SaveSystem
    {
        GameManager gm;
        public string SavePath { get; }
        public bool HasSave => File.Exists(SavePath);

        public SaveSystem(GameManager gm)
        {
            this.gm = gm;
            string baseDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (string.IsNullOrEmpty(baseDir))
                baseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), ".config");
            string dir = Path.Combine(baseDir, "ProjectIsland");
            Directory.CreateDirectory(dir);
            SavePath = Path.Combine(dir, "save.json");
        }

        // ---------- Save ----------
        public bool Save()
        {
            try
            {
                var data = BuildSaveData();
                string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SavePath, json);
                gm.Log("Game saved.");
                return true;
            }
            catch (Exception ex)
            {
                gm.Log("Save failed: " + ex.Message);
                return false;
            }
        }

        SaveData BuildSaveData()
        {
            var d = new SaveData
            {
                Health = gm.Player.Health,
                Stamina = gm.Player.Stamina,
                Battery = gm.Player.Battery,
                AmmoInMagazine = gm.Player.AmmoInMagazine,
                HasGun = gm.Player.HasGun,

                Day = gm.DayManager.Day,

                CampfireLevel = gm.campfire.Level,
                CampfireMaxDuration = gm.campfire.MaxDuration,
                CampfireRemainingTime = gm.campfire.remainingTime,

                AmmoReserve = gm.Inventory.AmmoReserve,
            };

            // Hotbar
            for (int i = 0; i < gm.Inventory.HotbarSize; i++)
            {
                var slot = gm.Inventory.Hotbar[i];
                d.Hotbar[i] = (slot?.Item != null)
                    ? new SlotDTO { ItemId = slot.Item.Id, Quantity = slot.Quantity, Rotation = slot.Rotation }
                    : null;
            }

            // Grid
            for (int x = 0; x < gm.Inventory.GridW; x++)
            {
                for (int y = 0; y < gm.Inventory.GridH; y++)
                {
                    var slot = gm.Inventory.Grid[x, y];
                    if (slot?.Item != null)
                    {
                        d.Grid.Add(new GridSlotDTO
                        {
                            X = x,
                            Y = y,
                            ItemId = slot.Item.Id,
                            Quantity = slot.Quantity,
                            Rotation = slot.Rotation
                        });
                    }
                }
            }

            // KeyItems
            foreach (var k in gm.Inventory.KeyItems)
                d.KeyItemIds.Add(k.Id);

            // Codex
            foreach (var n in gm.Codex.GetPlayerNotesSnapshot())
                d.PlayerNotes.Add(new NoteDTO { Id = n.Id, Text = n.Text });

            foreach (var line in gm.Codex.GetNPCLinesSnapshot())
                d.NPCDialogueLines.Add(line);

            return d;
        }

        // ---------- Load ----------
        public bool Load()
        {
            if (!HasSave) return false;
            try
            {
                string json = File.ReadAllText(SavePath);
                var d = JsonSerializer.Deserialize<SaveData>(json);
                if (d == null) return false;
                ApplySaveData(d);
                gm.Log("Game loaded.");
                return true;
            }
            catch (Exception ex)
            {
                gm.Log("Load failed: " + ex.Message);
                return false;
            }
        }

        void ApplySaveData(SaveData d)
        {
            // เคลียร์ของเดิม
            gm.Player.ResetAll();
            gm.Inventory.Clear();
            gm.Codex.Reset();
            gm.campfire.Reset();

            // Player
            gm.Player.Health = d.Health > 0 ? d.Health : gm.Player.MaxHealth;
            gm.Player.Stamina = d.Stamina > 0 ? d.Stamina : gm.Player.MaxStamina;
            gm.Player.Battery = d.Battery > 0 ? d.Battery : 100f;
            gm.Player.AmmoInMagazine = d.AmmoInMagazine;
            gm.Player.HasGun = d.HasGun;

            // Day
            gm.DayManager.Day = d.Day;

            // Campfire
            gm.campfire.Level = d.CampfireLevel;
            gm.campfire.MaxDuration = d.CampfireMaxDuration;
            gm.campfire.remainingTime = d.CampfireRemainingTime;

            // Inventory
            gm.Inventory.AmmoReserve = d.AmmoReserve;

            for (int i = 0; i < d.Hotbar.Length && i < gm.Inventory.HotbarSize; i++)
            {
                if (d.Hotbar[i] == null) continue;
                var item = CreateItem(d.Hotbar[i].ItemId);
                if (item == null) continue;
                gm.Inventory.Hotbar[i].Item = item;
                gm.Inventory.Hotbar[i].Quantity = d.Hotbar[i].Quantity;
                gm.Inventory.Hotbar[i].Rotation = d.Hotbar[i].Rotation;
            }

            foreach (var g in d.Grid)
            {
                if (g.X < 0 || g.X >= gm.Inventory.GridW) continue;
                if (g.Y < 0 || g.Y >= gm.Inventory.GridH) continue;
                var item = CreateItem(g.ItemId);
                if (item == null) continue;
                gm.Inventory.Grid[g.X, g.Y].Item = item;
                gm.Inventory.Grid[g.X, g.Y].Quantity = g.Quantity;
                gm.Inventory.Grid[g.X, g.Y].Rotation = g.Rotation;
            }

            foreach (var keyId in d.KeyItemIds)
            {
                var item = CreateItem(keyId);
                if (item != null)
                {
                    item.IsKeyItem = true;
                    gm.Inventory.KeyItems.Add(item);
                }
            }

            // Codex
            foreach (var n in d.PlayerNotes)
            {
                var note = new Note(n.Id, n.Text) { Collected = true };
                gm.Codex.UnlockPlayerNote(note);
            }
            foreach (var line in d.NPCDialogueLines)
                gm.Codex.UnlockNote(new Note("NPC01", line) { Collected = false });
        }

        // map item id → PickupItem พร้อม texture
        PickupItem CreateItem(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            Texture2D tex = ResolveTexture(id);
            if (tex == null) return null;

            bool isAxe = id == "axe";
            var item = new PickupItem(gm, Vector2.Zero, tex, id, isKeyItem: false, isNote: false, isAxe: isAxe);
            if (isAxe || id == "gun" || id == "wood")
                item.Scale = new Vector2(2f, 2f);
            return item;
        }

        Texture2D ResolveTexture(string id)
        {
            switch (id)
            {
                case "gun": return gm.TexPistol;
                case "axe": return gm.content.Load<Texture2D>("Sprite_axe");
                case "wood": return gm.content.Load<Texture2D>("Sprite_wood");
                case "ammo": return gm.content.Load<Texture2D>("AmmoPickup");
                default:
                    if (id.StartsWith("note"))
                        return gm.content.Load<Texture2D>(id);
                    return null;
            }
        }

        public void Delete()
        {
            try { if (HasSave) File.Delete(SavePath); }
            catch { }
        }
    }
}
