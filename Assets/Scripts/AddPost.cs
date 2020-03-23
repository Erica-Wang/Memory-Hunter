using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.SceneManagement;

public class post
{
    public string dt;
    public string pst;
    public string user;
    public int likes;
    public float longitude, latitude;

}

public class AddPost : MonoBehaviour
{
    // Start is called before the first frame update
    public InputField post;
    public Text error;
    public GameObject overlay;
    public Text postText;

    string date;
    string pst;
    string User = "default@a.com";

    DatabaseReference reference;

    void Start(){

    	overlay.SetActive(false);
    	if(PlayerPrefs.GetString("User")!=""){
            User = PlayerPrefs.GetString("User");
        }
    	Debug.Log(User);
    	FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://memhunter-c91dd.firebaseio.com/");

	    // Get the root reference location of the database.
	    reference = FirebaseDatabase.DefaultInstance.RootReference;

        post.GetComponent<InputField>().lineType = InputField.LineType.MultiLineNewline;
    }

    public void errok(){
    	overlay.SetActive(false);
    }

    public void updateIndex(string cur){
    	int updated = int.Parse(cur)+1;
    	Dictionary<string,System.Object> update = new Dictionary<string,System.Object>();
    	update["posts"]=updated;
    	reference.Child("indexes").UpdateChildrenAsync(update);
    }

    public void getPostIndex(string json){
    	FirebaseDatabase.DefaultInstance.GetReference("indexes").GetValueAsync().ContinueWith(task => 
    {  
        DataSnapshot snapshot = task.Result;
        Debug.Log(snapshot.Value.ToString());
        string posts = snapshot.Child("posts").Value.ToString();
        reference.Child("posts").Child(posts).SetRawJsonValueAsync(json);
        updateIndex(posts);
    });  
    }

    public void add(){

		/*
		if(err!=""){
			error.text = err;
			overlay.SetActive(true);
			return;
		}*/
    	pst = post.text;
		date = System.DateTime.Now.ToString();


		post p = new post();
		p.dt = date;
		p.pst = pst;
		p.likes = 0;
		p.user = User;
		p.latitude = global.latitude;
		p.longitude = global.longitude;

		string json = JsonUtility.ToJson(p);

		getPostIndex(json);

	}

	public void toMap(){
        Debug.Log("TOMAP");
        Debug.Log(GoogleMapsAPI.prevurl);
		SceneManager.LoadScene("GoogleMaps");
	}
}
