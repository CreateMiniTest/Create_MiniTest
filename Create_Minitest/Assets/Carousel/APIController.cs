using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using Newtonsoft.Json;

public class APIController : MonoBehaviour
{
    private const string API_KEY = "AIzaSyAjQ1IDr8qj9m8hWKO1VRs1hB9FJC1DibM";
    public string Location = "Paul van Ostaijenlaan 12";


    private PlaceSearch GetPlaceLocation()
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
            $"https://maps.googleapis.com/maps/api/place/findplacefromtext/json?input={Location}&fields=place_id,photos,formatted_address,geometry,name&inputtype=textquery&key={API_KEY}");
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException());
        string jsonResponse = reader.ReadToEnd();
        var place = JsonConvert.DeserializeObject<PlaceSearch>(jsonResponse);
        return place;
    }

    private void GetPlacesNearby(location loc)
    {
        string location = loc.Lat + "," + loc.Lng;
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
            $"https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={location}&rankby=distance&type=point_of_interest&fields=geometry,id,name,photos,place_id,vicinity&key={API_KEY}");
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException());
        string jsonResponse = reader.ReadToEnd();
        var place = JsonConvert.DeserializeObject<PlaceNearyby>(jsonResponse);
        return;
    }
    // Start is called before the first frame update
    void Start()
    {
        var loc = GetPlaceLocation().Candidates[0].Geometry.Location;
        print(loc.Lat + ", " + loc.Lng);
        GetPlacesNearby(loc);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
