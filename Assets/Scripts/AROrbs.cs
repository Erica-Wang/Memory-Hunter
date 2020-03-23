using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AROrbs : MonoBehaviour
{

    public GameObject orb;
    public Button floatButton;
    public Canvas canvas;
    public Camera camera;

    public GameObject postDetails;
    public Text postText;

	List<GameObject> orbs = new List<GameObject>();
    List<Button> buttons = new List<Button>();
	List<post> posts;

    void Start()
    {
        postDetails.SetActive(false);
        Input.location.Start();
        Input.compass.enabled = true;
        StartCoroutine(LoadOrbs());
    	posts = global.displayedPosts;
    }

    bool IsVisibleFrom(Renderer renderer, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }
	
	void OnGUI()
	 {
	 	for (int i = 0; i<orbs.Count; i++){

            Vector3 pos = Camera.main.WorldToScreenPoint(orbs[i].transform.position);
            pos.y-=100;
            buttons[i].transform.position = pos;
            if (IsVisibleFrom(orbs[i].GetComponentInChildren<Renderer>(),camera)){
                if(!postDetails.activeSelf){
                    buttons[i].gameObject.SetActive(true);
                }
            }else{
                buttons[i].gameObject.SetActive(false);
            }
            
		}
	 }

    void displayPost(post p){
        postDetails.SetActive(true);
        postText.text = p.pst;
        foreach(Button button in buttons){
            Debug.Log(button);
            button.gameObject.SetActive(false);
        }
        foreach(Button button in buttons){
            Debug.Log(button.gameObject.activeSelf);
        }
    }
    
    IEnumerator LoadOrbs(){
    	if(!Input.compass.enabled){
    		yield return new WaitForSeconds(1);
    	}

    	float curLat = global.latitude;
    	float curLon = global.longitude;

    	
    	for(int i = 0; i<posts.Count; i++){

            post post = posts[i];

    		float deg = calculateAngle(curLat,curLon,post.latitude,post.longitude);
    		float dist = calculateDistance(curLat,curLon,post.latitude,post.longitude);
    		
    		float xval = dist*Mathf.Cos(deg);
    		float yval = dist*Mathf.Sin(deg);

            orbs.Add((GameObject)Instantiate(orb,new Vector3(xval,0,yval),Quaternion.identity));
            
            Vector3 pos = Camera.main.WorldToScreenPoint(orbs[i].transform.position);

            //add a button for each post


            var button = Instantiate(floatButton, new Vector3(0,0,0), Quaternion.identity) as Button;
            button.transform.position = pos;
            button.GetComponent<RectTransform>().SetParent(canvas.transform);
            button.onClick.AddListener(()=>{displayPost(post);});
            buttons.Add(button);

    	}
    }

    float degreesToRadians(float deg){
    	return deg * Mathf.PI / 180;
    }


    float calculateAngle(float lat1, float lon1, float lat2, float lon2){

        float bearing = Input.compass.trueHeading;

	  	float dLat = degreesToRadians(lat2-lat1);
		float dLon = degreesToRadians(lon2-lon1);
    	float degToNorth = Mathf.Atan2(dLon,dLat);
    	if(degToNorth>0&&dLat<0){
    		degToNorth -= 2*Mathf.PI;
    	}else if(degToNorth<0&&dLat<0){
    		degToNorth += 2*Mathf.PI;
    	}

    	//adjust to current user facing direction
    	degToNorth += bearing;
    	//now represents the counterclockwise angle from 
    	//the x axis if the direction user is facing is the y axis

    	return (float)degToNorth;
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

    public void closeOverlay(){
        postDetails.SetActive(false);
    }
    public void toMapView(){
        
		SceneManager.LoadScene("GoogleMaps");
	}

}
