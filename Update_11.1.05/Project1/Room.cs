using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Project1
{
    public class Room
    {
        public Rectangle Bounds { get; set; }

        public Room(Rectangle bounds)
        {
            Bounds = bounds;
        }
    }
}
