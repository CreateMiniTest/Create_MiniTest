using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


[Serializable]
public class location
{
    public double Lat;
    public double Lng;
}

[Serializable]
public class viewport
{
    public location Northeast;
    public location Southwest;
}

[Serializable]
    public class geometry
    {
        public location Location;
        public viewport Viewport;
    }
[Serializable]
    public class photos
    {
        public int Height;
        public string Html_attributions;
        public string Photo_reference;
        public int Width;
    }

[Serializable]
public class candidate
{
    public string Formatted_address;
    public geometry Geometry;
    public string Name;
    public string PlaceId;
    public photos Photos;
}

[Serializable]
    public class PlaceSearch
    {
        public candidate[] Candidates;
    }


