using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace HeadFirst__AStarrySky.Model
{
    class Star
    {
        public Point Location { get; set; }

        public Star(Point location)
        {
            Location = location;
        }
    }
}
