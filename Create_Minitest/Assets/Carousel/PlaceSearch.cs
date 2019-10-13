using System;

[Serializable]
public class location
{
    public double Lat { get; set; }
    public double Lng { get; set; }
}

[Serializable]
public class viewport
{
    public location Northeast { get; set; }
    public location Southwest { get; set; }
}

[Serializable]
public class geometry
{
    public location Location { get; set; }
    public viewport Viewport { get; set; }
}
[Serializable]
public class photos
{
    public int Height { get; set; }
    public string[] Html_attributions { get; set; }
    public string Photo_reference { get; set; }
    public int Width { get; set; }
}

[Serializable]
public class candidate
{
    public string Formatted_address { get; set; }
    public geometry Geometry;
    public string Name { get; set; }
    public string Place_id { get; set; }
}

[Serializable]
public class PlaceSearch
{
    public candidate[] Candidates { get; set; }
}


[Serializable]
public class results
{
    public geometry Geometry { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }
    public photos[] Photos { get; set; }
    public string Place_id { get; set; }
    public string Vicinity { get; set; }
}

[Serializable]
public class PlaceNearyby
{
    public string[] Html_attributions { get; set; }
    public string Next_page_token { get; set; }
    public results[] Results { get; set; }
}

