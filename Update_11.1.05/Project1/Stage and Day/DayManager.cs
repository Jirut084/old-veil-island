using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Project1
{
    public class DayManager
    {
        GameManager gm;
        public int Day = 1;
        public bool DayCompleted = false;
        public bool IsGameOver = false;
        HashSet<string> CollectedParts = new HashSet<string>();

        public DayManager(GameManager gm) { this.gm = gm; }

        public bool HasAllKeyItems() => gm.Inventory.KeyItems.Count >= 5;
        public int CollectedKeyItemCount => gm.Inventory.KeyItems.Count;

        public void StartDay()
        {
            DayCompleted = false;
            CollectedParts.Clear();

            // reset campfire → เริ่มต้น 5 นาที
            //gm.campfire.Reset();

        }

        public void Update(GameTime gt)
        {
            //if (IsGameOver) return;

            //// ถ้า Campfire ดับ → GameOver
            //if (gm.campfire.LightAlpha <= 0)
            //{
            //    gm.GameOver.Start();
            //}

            // แค่เช็คว่าเก็บครบ key items แต่ไม่ NextDay
            if (HasAllKeyItems())
            {
                DayCompleted = true;
            }
        }



        public void NextDay()
        {
            Day++;
            StartDay();

            if (Day == 2)
            {
                //gm.Level = new ProceduralLevel2(gm);
                gm.Level.GenerateLevelSeed(Day);
            }
        }


        public void ResetAll()
        {
            Day = 1;
            IsGameOver = false;
            StartDay();
        }

        public void CollectKeyPart(string id)
        {
            if (!CollectedParts.Contains(id))
            {
                CollectedParts.Add(id);
            }
        }

        public string GetTimeString()
        {
            return $"Day: {Day} | Timer: {Math.Ceiling(gm.campfire.remainingTime):000}s";
        }

    }
}
