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


    private PlaceSearch GetPlace()
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
            string.Format("https://maps.googleapis.com/maps/api/place/findplacefromtext/json?input={0}&fields=place_id,photos,formatted_address,geometry,name&inputtype=textquery&key={1}", Location, API_KEY));
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException());
        string jsonResponse = reader.ReadToEnd();
        var place = JsonConvert.DeserializeObject<PlaceSearch>(jsonResponse);
        return place;
    }

    // Start is called before the first frame update
    void Start()
    {
        var loc = GetPlace().Candidates[0].Geometry.Location;
        print(loc.Lat + ", " + loc.Lng);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
