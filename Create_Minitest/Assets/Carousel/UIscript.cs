using System.Globalization;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class UIscript : MonoBehaviour
{
        private bool _isOptions = false;

        private void Start()
        {
            var ui = transform.GetChild(0);
            if (tag == "UI_Settings")
            {
                var car = GameObject.FindGameObjectsWithTag("Carousel")[0].GetComponent<Carrousel>();
                ui.GetChild(3).GetComponent<Slider>().value = car._NumberOfImages;
                ui.GetChild(3).GetChild(4).GetComponent<Text>().text = car._NumberOfImages.ToString();
                ui.GetChild(4).GetComponent<Slider>().value = car._Radius;
                ui.GetChild(4).GetChild(4).GetComponent<Text>().text = car._Radius.ToString();
                ui.GetChild(5).GetComponent<Slider>().value = car._SpriteOrienataion;
                ui.GetChild(5).GetChild(4).GetComponent<Text>().text = car._SpriteOrienataion.ToString(CultureInfo.InvariantCulture);
            } 
        }


        public void SearchLocation()
        {
           var input = GetTextFromInput();
           var apiControl = GameObject.FindGameObjectsWithTag("ApiController")[0].GetComponent<APIController>();
            
           var place = apiControl.GetPlaceLocation(input);
           var titleText = transform.GetChild(1).transform.GetComponent<Text>();

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
                titleText.text = "Vul een locatie naar keuze in:";
                apiControl.PrepareCarrousel(place.Candidates[0].Geometry.Location);

               transform.gameObject.SetActive(false);
               transform.parent.GetChild(3).gameObject.SetActive(false);
           }
        }

        public void InspectPlace()
        {
            var carScript = GameObject.FindWithTag("Carousel").GetComponent<Carrousel>();
            var index = carScript.FowardIndex();
            carScript._Paused = true;

            var inspector = transform.parent.GetChild(6);
            inspector.gameObject.SetActive(true);
            var img = APIController.LoadTextureFromFile(index.ToString());
            if(!img)
                img = (Texture2D)Resources.Load("No_Image");

            inspector.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Sprite.Create(img, new Rect(0.0f, 0.0f, img.width, img.height), new Vector2(0.5f, 0.5f), 100.0f);

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
            return transform.GetChild(0).transform.GetComponent<InputField>().text;
        }

        public void Settings()
        {
            transform.GetChild(0).gameObject.SetActive(_isOptions = !_isOptions);

           if(_isOptions) return;
           GameObject.FindGameObjectsWithTag("Carousel")[0].GetComponent<Carrousel>().SetUp();
        }

        public void Slider_Images()
        {
            var val = GetComponent<Slider>().value;
            var carScript = GameObject.FindGameObjectsWithTag("Carousel")[0].GetComponent<Carrousel>();
            carScript.GetComponent<Carrousel>()._NumberOfImages = (int)val;
            transform.GetChild(4).GetComponent<Text>().text = val.ToString(CultureInfo.InvariantCulture);
            carScript.GetComponent<Carrousel>().SetUp();
        }   

        public void Slider_Radius()
        {
            var val = GetComponent<Slider>().value;
            var carScript = GameObject.FindGameObjectsWithTag("Carousel")[0].GetComponent<Carrousel>();
            carScript.GetComponent<Carrousel>()._Radius = (int)val;
            transform.GetChild(4).GetComponent<Text>().text = val.ToString(CultureInfo.InvariantCulture);
            carScript.GetComponent<Carrousel>().SetUp();
        }

        public void Slider_Orientation()
        {
            var val = GetComponent<Slider>().value;
            var carScript = GameObject.FindGameObjectsWithTag("Carousel")[0].GetComponent<Carrousel>();
            carScript.GetComponent<Carrousel>()._SpriteOrienataion = val;
            transform.GetChild(4).GetComponent<Text>().text = val.ToString(CultureInfo.InvariantCulture);
        }

        public void Return()
        {          
            var car = GameObject.FindWithTag("Carousel");
            if (car.GetComponent<Carrousel>()._Paused) Application.Quit();
            if (!GameObject.FindGameObjectsWithTag("ApiController")[0].GetComponent<APIController>().IsConnected)
                Application.Quit();
            car.GetComponent<Carrousel>()._Paused = true;
            car.transform.rotation = new Quaternion();
            transform.parent.GetChild(3).gameObject.SetActive(true);
            transform.parent.GetChild(4).gameObject.SetActive(true);
        }
}   