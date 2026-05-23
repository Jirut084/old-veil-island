using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project1
{
    public class AmmoPickup : PickupItem
    {
        public int Amount;
        public AmmoPickup(GameManager gm, Vector2 pos, Texture2D tex, string id, int amount)
            : base(gm, pos, tex, id, false)
        {
            Amount = amount;
        }
    }
}
