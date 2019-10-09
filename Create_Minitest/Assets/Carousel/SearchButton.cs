using UnityEngine;
using UnityEngine.UI;

namespace Assets.Carousel
{
    public class SearchButton : MonoBehaviour
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
               this.transform.parent.GetChild(0).gameObject.SetActive(false);
           }

            

        }

        private string GetTextFromInput()
        {
            return this.transform.GetChild(0).transform.GetComponent<InputField>().text;
        }
    }
}
