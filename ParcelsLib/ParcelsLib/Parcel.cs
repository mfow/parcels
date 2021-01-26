using System;

namespace ParcelsLib
{
    public class Parcel
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public double Depth { get; set; }
        public double Weight { get; set; }
        public Guid Id { get; set; }
    }
}
