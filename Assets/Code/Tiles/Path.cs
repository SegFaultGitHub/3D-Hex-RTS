using System;
using System.Collections.Generic;

namespace Code.Tiles {
    [Serializable]
    public struct Path {
        public Tile Destination;
        public List<Tile> Tiles;
        public bool Complete;
        public int RenewAt;
    }
}
