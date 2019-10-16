using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Networking;

public class APIController : MonoBehaviour
{
    public string API_KEY = "";
    public string Location = "";
    public string Types = "point_of_interest,establishment,place_of_worship,art_gallery,health,natural_feature,political";
    public bool IsConnected;
    private Texture2D _noImg;

    private void Start()
    {
        _noImg = (Texture2D)Resources.Load("No_Image");
        StartCoroutine(CheckInternetConnection((isConnected) =>
        {
            IsConnected = isConnected;
            if (!isConnected) PrepareOfflineCarrousel();
        }));
    }

    public IEnumerator CheckInternetConnection(Action<bool> syncResult)
    {
        const string echoServer = "http://google.com";

        bool result;
        using (var request = UnityWebRequest.Head(echoServer))
        {
            request.timeout = 5;
            yield return request.SendWebRequest();
            result = !request.isNetworkError && !request.isHttpError && request.responseCode == 200;
        }
        syncResult(result);
    }

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
            $"https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={location}&rankby=distance&type=Types&fields=geometry,id,name,photos,place_id,vicinity&key={API_KEY}");
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException());
        string jsonResponse = reader.ReadToEnd();
        reader.Close();
        var places = JsonConvert.DeserializeObject<PlaceNearyby>(jsonResponse);

        if (!sortforImages) return places;

        var imgResults = new List<Results>();
        var noimgResults = new List<Results>();
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

    public void PrepareOfflineCarrousel()
    {
        var file = ReadFromJson("NearbyPlaces");
        var ui = GameObject.FindGameObjectsWithTag("UI")[0];
        if (file == null) 
        {
            ui.transform.GetChild(4).gameObject.SetActive(false);
            ui.transform.GetChild(8).gameObject.SetActive(true);
            return;
        }

        var places = JsonConvert.DeserializeObject<PlaceNearyby>(file);
        ui.transform.GetChild(4).gameObject.SetActive(false);

        var photos = new Texture2D[places.Results.Length];
        for (int i = 0; i < photos.Length; i++)
        {
            var img = LoadTextureFromFile(i.ToString());
            if (!img)
                img = _noImg;

            if (img == null) continue;

            photos[i] = img;
        }

        var carrousel = transform.GetChild(0);
        
        for (int i = 0; i < carrousel.childCount; i++)
        {
            if (photos[i] != null)
                carrousel.GetChild(i).GetComponent<SpriteRenderer>().sprite = Sprite.Create(photos[i], new Rect(0.0f, 0.0f, photos[i].width, photos[i].height), new Vector2(0.5f, 0.5f), 100.0f);
        }

        carrousel.GetComponent<Carrousel>().Paused = false;
        ui.transform.GetChild(3).gameObject.SetActive(false);


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
        carrousel.GetComponent<Carrousel>().Paused = false;
        for (int i = 0; i < carrousel.childCount; i++)
        {
            carrousel.GetChild(i).GetComponent<SpriteRenderer>().sprite = photos[i] != null ? Sprite.Create(photos[i], new Rect(0.0f, 0.0f, photos[i].width, photos[i].height), new Vector2(0.5f, 0.5f), 100.0f) : Sprite.Create(_noImg, new Rect(0.0f, 0.0f, _noImg.width, _noImg.height), new Vector2(0.5f, 0.5f), 100.0f);
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
}
