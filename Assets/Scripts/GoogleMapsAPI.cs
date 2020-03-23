using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;


public class GoogleMapsAPI : MonoBehaviour
{
    public static GoogleMapsAPI Instance {set; get;}

    DatabaseReference reference;
    string User = "default@a.com";
    public static string prevurl = "";
    static string url = "";

    void Start()
    {

        Debug.Log("Maps start");
        User = PlayerPrefs.GetString("User");
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://memhunter-c91dd.firebaseio.com/");

        // Get the root reference location of the database.
        reference = FirebaseDatabase.DefaultInstance.RootReference;

        Instance = this;
        //StartCoroutine(StartLocationService());
        getNewURL();

    }

    void Update(){
        if(prevurl!=url){
            StartCoroutine(updateMaps());
        }
    }


    private IEnumerator StartLocationService(){
        if(!Input.location.isEnabledByUser){
            Debug.Log("User has not enabled GPS");
            yield break;
        }

        Input.location.Start();
        while(Input.location.status == LocationServiceStatus.Initializing){
            yield return new WaitForSeconds(1);
        }

        if(Input.location.status == LocationServiceStatus.Failed){
            Debug.Log("Unable to determine location");
            yield break;
        }
        if(global.latitude!=Input.location.lastData.latitude||global.longitude!=Input.location.lastData.longitude){
            global.latitude = Input.location.lastData.latitude;
            global.longitude = Input.location.lastData.longitude; 
            getNewURL();
        }
    }
    

    
    float degreesToRadians(float deg){
        return deg * Mathf.PI / 180;
    }
    float calculateDistance(float lat1, float lon1, float lat2, float lon2){

        float earthRadius = (float)6371000;

        float dLat = degreesToRadians(lat2-lat1);
        float dLon = degreesToRadians(lon2-lon1);

        lat1 = degreesToRadians(lat1);
        lat2 = degreesToRadians(lat2);

        float a = Mathf.Sin(dLat/2) * Mathf.Sin(dLat/2) +
              Mathf.Sin(dLon/2) * Mathf.Sin(dLon/2) * Mathf.Cos(lat1) * Mathf.Cos(lat2); 
        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1-a)); 
        return earthRadius * c;
    
    }

    string markerText(post p){
        string res = "&markers=color:blue%7Clabel:S%7C";
        res+=p.latitude.ToString();
        res+=",";
        res+=p.longitude.ToString();
        return res;
    }

    void getNewURL(){
        url = "https://maps.googleapis.com/maps/api/staticmap?";
        url += "&zoom=18&size=200x432&maptype=satellite";
        string key = "&key=AIzaSyDzZE_iL05RSddPakPeDCcDR56HdWw34Is";
        string location = "&center="+global.latitude+","+global.longitude;
        url+=key;
        url+=location;

        global.displayedPosts = new List<post>();

        FirebaseDatabase.DefaultInstance.GetReference("posts").GetValueAsync().ContinueWith(task => 
        {  
            DataSnapshot snapshot = task.Result;
            int i = 1;
            while(true){
                if(snapshot.Child(i.ToString()).Child("dt").Value==null){
                    break;
                }
                float markerLat = float.Parse(snapshot.Child(i.ToString()).Child("latitude").Value.ToString());
                float markerLon = float.Parse(snapshot.Child(i.ToString()).Child("longitude").Value.ToString());

                float dist = calculateDistance(global.latitude,global.longitude,markerLat,markerLon);
                Debug.Log(dist);

                post p = new post();
                p.dt = snapshot.Child(i.ToString()).Child("dt").Value.ToString();
                p.pst = snapshot.Child(i.ToString()).Child("pst").Value.ToString();
                p.user = snapshot.Child(i.ToString()).Child("user").Value.ToString();
                p.likes = int.Parse(snapshot.Child(i.ToString()).Child("likes").Value.ToString());
                p.longitude = markerLon;
                p.latitude = markerLat;

                if(dist<=200){
                    url+=markerText(p);
                }
                if(dist<=50){
                    global.displayedPosts.Add(p);
                }

                i++;

            }
            Debug.Log(url);
        });  
    }


    private IEnumerator updateMaps(){
        Debug.Log(url);
        WWW req = new WWW(url);

        // Create a texture in DXT1 format
        GetComponent<Renderer>().material.mainTexture = new Texture2D(512, 512, TextureFormat.DXT1, false);
        
        while (!req.isDone){
            yield return null;
        }
        if (req.error == null){
            req.LoadImageIntoTexture((Texture2D)GetComponent<Renderer>().material.mainTexture);
            prevurl = url;
        }
        
    }
    public void toARView(){
        SceneManager.LoadScene("AR Scene");
    }

    public void toPostView(){
        SceneManager.LoadScene("AddPost");
    }
}
