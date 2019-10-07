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
        public string[] Html_attributions;
        public string Photo_reference;
        public int Width;
    }

[Serializable]
public class candidate
{
    public string Formatted_address;
    public geometry Geometry;
    public string Name;
    public string Place_id;
}

[Serializable]
    public class PlaceSearch
    {
        public candidate[] Candidates;
    }


[Serializable]
public class results
{
    public geometry Geometry;
    public string Id;
    public string Name;
    public photos[] Photos;
    public string Place_id;
    public string Vicinity;
}

[Serializable]
public class PlaceNearyby
{
    public string[] Html_attributions;
    public string Next_page_token;
    public results[] Results;
}

[Serializable]
public class Attributes
{
    public string width { get; set; }
    public string height { get; set; }
}

[Serializable]
public class Attributes2
{
    public string id { get; set; }
    public string width { get; set; }
    public string height { get; set; }
    public string x { get; set; }
    public string y { get; set; }
    public string href { get; set; }
}

[Serializable]
public class Image
{
    public Attributes2 attributes { get; set; }
}

[Serializable]
public class RootObject
{
    public Attributes attributes { get; set; }
    public Image image { get; set; }
}