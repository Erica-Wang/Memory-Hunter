using Firebase;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class LogIn: MonoBehaviour
{
	public InputField emailField;
	public InputField passwordField;
	public Text errorText;

	string email;
	string password;

	private IEnumerator onRegister;
	public void onLogIn(){
		email = emailField.text;
		password = passwordField.text;

		onRegister = LogInUser(email,password);
		StartCoroutine(onRegister);
	}

	public IEnumerator LogInUser(string email, string password){
		var aut = FirebaseAuth.DefaultInstance;
		var registerTask = aut.SignInWithEmailAndPasswordAsync(email, password);
		yield return new WaitUntil (() => registerTask.IsCompleted);
		//this in array functions exists in outer scope

		if(registerTask.Exception == null){
			Debug.Log($"Successful {registerTask.Result}");
			PlayerPrefs.SetString("User",email);
			SceneManager.LoadScene("GoogleMaps");
		}else{
			ErrorMessage(registerTask.Exception.ToString());
		}
	}

	public void ErrorMessage(string message){
		int startIndex = 0;
		int length = 0;
		string target = "FirebaseException";
		for(int i = 0; i<message.Length; i++){
			if(message.Substring(i,target.Length)==target){
				startIndex = i+target.Length+2;
				break;
			}
		}
		for(int i = startIndex; i<message.Length; i++){
			if(message[i]=='.'){
				break;
			}
			length++;
		}
		errorText.text = message.Substring(startIndex,length);
	}

	public void goToSignUp(){
		SceneManager.LoadScene("SignUp");
	}
}