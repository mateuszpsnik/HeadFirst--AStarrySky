using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace HeadFirst__AStarrySky.Model
{
    class Bee
    {
        public Point Location { get; set; }
        public Size Size { get; set; }
        public Rect Position => new Rect(Location, Size);
        public double Width => Position.Width;
        public double Height => Position.Height;

        public Bee(Point location, Size size)
        {
            Location = location;
            Size = size;
        }
    }
}
