using System;

[Serializable]
public class location
{
    public double Lat { get; set; }
    public double Lng { get; set; }
}

[Serializable]
public class Viewport
{
    public location Northeast { get; set; }
    public location Southwest { get; set; }
}

[Serializable]
public class Geometry
{
    public location Location { get; set; }
    public Viewport Viewport { get; set; }
}
[Serializable]
public class Photos
{
    public int Height { get; set; }
    public string[] Html_attributions { get; set; }
    public string Photo_reference { get; set; }
    public int Width { get; set; }
}

[Serializable]
public class Candidate
{
    public string Formatted_address { get; set; }
    public Geometry Geometry;
    public string Name { get; set; }
    public string Place_id { get; set; }
}

[Serializable]
public class PlaceSearch
{
    public Candidate[] Candidates { get; set; }
}


[Serializable]
public class Results
{
    public Geometry Geometry { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }
    public Photos[] Photos { get; set; }
    public string Place_id { get; set; }
    public string Vicinity { get; set; }
}

[Serializable]
public class PlaceNearyby
{
    public string[] Html_attributions { get; set; }
    public string Next_page_token { get; set; }
    public Results[] Results { get; set; }
}

