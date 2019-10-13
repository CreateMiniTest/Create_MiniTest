using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using Newtonsoft.Json;

public class APIController : MonoBehaviour
{
    public string API_KEY = "";
    public string Location = "";
    public List<string> Types;

    public PlaceSearch GetPlaceLocation(string location)
    {
        Location = location;
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
            $"https://maps.googleapis.com/maps/api/place/findplacefromtext/json?input={location}&fields=formatted_address,geometry,name,place_id&inputtype=textquery&key={API_KEY}");
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException());
        string jsonResponse = reader.ReadToEnd();
        reader.Close();
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
        reader.Close();
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
        var image = new Texture2D(2,2);
        byte[] bytes = reader.ReadBytes(1 * 1920 * 1080 * 10);
        reader.Close();
        image.LoadImage(bytes);
        return image;
    }

    public void PrepareCarrousel(location location)
    {
        var places = GetPlacesNearby(location, true);
        string serializedData = JsonConvert.SerializeObject(places);
        SaveToJson(serializedData, "NearbyPlaces");

        var photos = new Texture2D[places.Results.Length];
        for (int i = 0; i < photos.Length; i++)
        {
            if (places.Results[i].Photos == null) continue;
            
            photos[i] = GetPlacePhoto(places.Results[i].Photos[0].Photo_reference);
            SaveTextureToFile(photos[i], i.ToString());
        }

        var carrousel = transform.GetChild(0);
        carrousel.GetComponent<Carrousel>()._Paused = false;
        for (int i = 0; i < carrousel.childCount; i++)
        {
            if (photos[i] != null)
                carrousel.GetChild(i).GetComponent<SpriteRenderer>().sprite = Sprite.Create(photos[i], new Rect(0.0f, 0.0f, photos[i].width, photos[i].height), new Vector2(0.5f, 0.5f), 100.0f);
        }
    }

    public static void SaveTextureToFile(Texture2D texture, string fileName)
    {
        var bytes = texture.EncodeToPNG();
        
        var file = File.Open(Application.dataPath + "/Resources/" + fileName + ".png", FileMode.Create);
        var binary = new BinaryWriter(file);
        binary.Write(bytes);
        binary.Close();
        file.Close();
    }

    public static Texture2D LoadTextureFromFile(string filename)
    {
        if (!File.Exists(Application.dataPath + "/Resources/" + filename + ".png")) return null;

        var fileData = File.ReadAllBytes(Application.dataPath + "/Resources/" + filename + ".png");
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(fileData);
        return tex;
    }

    public static void SaveToJson(string data, string filename)
    {
        StreamWriter writer = new StreamWriter(Application.dataPath + "/Resources/" + filename + ".json");
        writer.Write(data);
        writer.Close();
    }

    public static string ReadFromJson(string filename)
    {
        if (!File.Exists(Application.dataPath + "/Resources/" + filename + ".json")) return null;
        StreamReader reader = new StreamReader(Application.dataPath + "/Resources/" + filename + ".json");
        string str = reader.ReadToEnd();
        reader.Close();
        return str;
    }

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
