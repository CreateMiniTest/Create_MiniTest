using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.Experimental.UIElements.Image;

public class UIscript : MonoBehaviour
    {
        public void SearchLocation()
        {
           var input = GetTextFromInput();
           var apiControl = GameObject.FindGameObjectsWithTag("ApiController")[0].GetComponent<APIController>();
            
           var place = apiControl.GetPlaceLocation(input);
           var titleText = this.transform.GetChild(1).transform.GetComponent<Text>();

           if (input == string.Empty)
           {
               titleText.text = "Vul EERST een locatie naar keuze in:";
           }
            else if (place.Candidates.Length == 0)
           {
               titleText.text = "Locatie niet gevonden! vul een andere locatie naar keuze in:";
           }
           else
           {
                titleText.text = "Verwerken...:";
                apiControl.PrepareCarrousel(place.Candidates[0].Geometry.Location);

               this.transform.gameObject.SetActive(false);
               this.transform.parent.GetChild(3).gameObject.SetActive(false);
           }
        }

        public void InspectPlace()
        {
            var carScript = GameObject.FindWithTag("Carousel").GetComponent<Carrousel>();
            var index = carScript.FowardIndex();
            carScript._Paused = true;

            var inspector = transform.parent.GetChild(5);
            inspector.gameObject.SetActive(true);
            var img = APIController.LoadTextureFromFile(index.ToString());
            if(!img)
                img = (Texture2D)Resources.Load("No_Image");

            inspector.GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(img, new Rect(0.0f, 0.0f, img.width, img.height), new Vector2(0.5f, 0.5f), 100.0f);

            PlaceNearyby places = JsonConvert.DeserializeObject<PlaceNearyby>(APIController.ReadFromJson("NearbyPlaces"));
            inspector.GetChild(1).GetChild(0).GetComponent<Text>().text = places.Results[index].Name;
            inspector.GetChild(1).GetChild(1).GetComponent<Text>().text = places.Results[index].Vicinity;
            string location = "Lat: " + places.Results[index].Geometry.Location.Lat + "\nLng: " + places.Results[index].Geometry.Location.Lng;
            inspector.GetChild(1).GetChild(2).GetComponent<Text>().text = location;
        }

        public void ExitInspect()
        {
            var carScript = GameObject.FindWithTag("Carousel").GetComponent<Carrousel>();
            carScript._Paused = false;
            transform.parent.gameObject.SetActive(false);
        }
        public void NextPicture(bool isRight = true)
        {
            GameObject.FindGameObjectsWithTag("Carousel")[0].GetComponent<Carrousel>().MoveCarouselToNext(isRight);
        }

        private string GetTextFromInput()
        {
            return this.transform.GetChild(0).transform.GetComponent<InputField>().text;
        }
    }   