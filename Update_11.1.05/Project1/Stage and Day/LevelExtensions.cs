using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Project1
{
    public static class LevelExtensions
    {
        public static bool IsPlayerHidden(this ProceduralLevel level, Vector2 pos)
        {
            // simplistic: if standing on grass tile (we assume many grass tiles drawn), treat as hidden based on coordinates
            // Real implementation should use tilemap checks — for prototype we use modulo

            return ((int)pos.X / 64 + (int)pos.Y / 64) % 7 == 0; // pseudo-random grass pattern
        }


        public static List<Vector2> GetPickupPositions(this ProceduralLevel level)
        {
            // expose pickup positions for minimap
            var list = new List<Vector2>();
            // reflect into internal list via simple API: a real implementation would expose it
            // Here we search pickups using reflection? But to keep simple, we will assume pickups available through gm.LevelPickupPositions placeholder
            return list;
        }
    }
}
