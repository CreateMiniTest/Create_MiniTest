using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UI;

public class APIController : MonoBehaviour
{
    private const string API_KEY = "";
    public string Location = "Paul van Ostaijenlaan 12";
    public Texture2D Image;

    public PlaceSearch GetPlaceLocation(string location)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
            $"https://maps.googleapis.com/maps/api/place/findplacefromtext/json?input={location}&fields=formatted_address,geometry,name,place_id&inputtype=textquery&key={API_KEY}");
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException());
        string jsonResponse = reader.ReadToEnd();
        var places = JsonConvert.DeserializeObject<PlaceSearch>(jsonResponse);
        return places;
    }

    private PlaceNearyby GetPlacesNearby(location loc, bool isOnlyImagedLocations = false)
    {
        string location = loc.Lat + "," + loc.Lng;
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
            $"https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={location}&rankby=distance&type=point_of_interest&fields=geometry,id,name,photos,place_id,vicinity&key={API_KEY}");
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException());
        string jsonResponse = reader.ReadToEnd();
        var places = JsonConvert.DeserializeObject<PlaceNearyby>(jsonResponse);
        
        if (!isOnlyImagedLocations) return places;

        List<results> imgResults = new List<results>();
        foreach (var result in places.Results)
        {
            if (result.Photos != null)
                imgResults.Add(result);
        }
        places.Results = imgResults.ToArray();

        return places;
    }

    private Texture2D GetPlacePhoto(string photoRef)
    {      
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
            $"https://maps.googleapis.com/maps/api/place/photo?maxwidth=1920&photoreference={photoRef}&key={API_KEY}");
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        BinaryReader reader = new BinaryReader(response.GetResponseStream() ?? throw new InvalidOperationException());
        Image = new Texture2D(2,2);
        byte[] bytes = reader.ReadBytes(1 * 1920 * 1080 * 10);
        Image.LoadImage(bytes);
        return Image;
    }

    public void PrepareCarrousel(location location)
    {
        var near = GetPlacesNearby(location);
        //near.Results.Length;
    }
    // Start is called before the first frame update
    void Start()
    {
        //var loc = GetPlaceLocation(Location).Candidates[0].Geometry.Location;
        //var near = GetPlacesNearby(loc);

        //var counter = near.Results.Count(results => results.Photos == null);

        //GetPlacePhoto(
        //    "CmRaAAAAKYMNG1AonRHddLu3s-LzshXvwkegnG-vMP34f2rEwCT4zmLO0ETFxH-r3JX5zzMLadsonBTjA1e4aTKZaTFPyxl6jj_ZwaJb0GOZ2tHuNhksQ9O2d8zAzHtXAy1FrDtBEhBCzNtC8zo57ObvUOy2eAbOGhS69n6sXpjnBSf93iRk4_CzZfFhCA");

        //print(loc.Lat + ", " + loc.Lng + ": " + near.Results[0].Name + " // images found= " + counter);
    }
}
