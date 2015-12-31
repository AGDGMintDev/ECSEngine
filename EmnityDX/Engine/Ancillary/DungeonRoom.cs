using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmnityDX.Engine.Ancillary
{
    public class DungeonRoom
    {
        public Guid Id { get; private set; }
        public Rectangle Room { get; set; }
        public bool Connected { get; set; }
        public List<DungeonRoom> ConnectedRooms { get; set; }

        public DungeonRoom()
        {
            Id = Guid.NewGuid();
            ConnectedRooms = new List<DungeonRoom>();
            Connected = false;
        }
    }
}
