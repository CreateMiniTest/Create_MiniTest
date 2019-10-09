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
    public string Location = "Paul van Ostaijenlaan 12";
    public Texture2D Image;
    public List<string> Types;

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

    private PlaceNearyby GetPlacesNearby(location loc, bool sortforImages = false)
    {
        string location = loc.Lat + "," + loc.Lng;
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
            $"https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={location}&rankby=distance&type=point_of_interest&fields=geometry,id,name,photos,place_id,vicinity&key={API_KEY}");
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException());
        string jsonResponse = reader.ReadToEnd();
        var places = JsonConvert.DeserializeObject<PlaceNearyby>(jsonResponse);

        if (!sortforImages) return places;

        var imgResults = new List<results>();
        var noimgResults = new List<results>();
        foreach (var result in places.Results)
        {
            if (result.Photos != null)
            {
                if (result.Photos.Length > 0)
                    imgResults.Add(result);
                else
                    noimgResults.Add(result);
            }
            else
            {
                noimgResults.Add(result);
            }
        }

        int toAdd = places.Results.Length - imgResults.Count;
        for (int i = 0; i < toAdd; i++)
            imgResults.Add(noimgResults[i]);

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
        var places = GetPlacesNearby(location);

        //near.Results.Length;
    }

    public void SaveTextureToFile(Texture2D texture, string fileName)
    {
        var bytes = texture.EncodeToPNG();
        var file = File.Open(Application.dataPath + "/" + fileName, FileMode.Create);
        var binary = new BinaryWriter(file);
        binary.Write(bytes);
        file.Close();
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
