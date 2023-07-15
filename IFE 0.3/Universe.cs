using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

enum UniverseState
{
    normal,
    abyss,
    target
}

namespace IFE_0._3
{
    internal


    class Universe
    {
        public Universe(int coord)
        {
            coordinate = coord;
        }

        public UniverseState state = UniverseState.normal;
        public int coordinate;
    }
}
