using System.Collections;
using System.Collections.Generic;
using FullSerializer;
using Proyecto26;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using System;
using Assets.SimpleAndroidNotifications;
using UnityEngine.SceneManagement;

public class PlayerScores : MonoBehaviour
{
    public Text scoreText;
    public InputField getScoreText;
    public InputField emailText;
    public InputField usernameText;
    public InputField passwordText;

    string toastString;
    AndroidJavaObject currentActivity;

    private System.Random random = new System.Random(); 

    User user = new User();

    private string databaseURL = "https://indesar-dc79d.firebaseio.com/Username"; 
    private string AuthKey = "AIzaSyAGZ_FBIZuwN5jT4W-QfnVIUBFCM0TVgR4";
    
    public static fsSerializer serializer = new fsSerializer();
    
    
    public static int playerScore;
    public static string playerName;

    private string idToken;
    
    public static string localId;

    private string getLocalId;
    

      

    public void OnSubmit()
    {
        PostToDatabase();
    }
    
    public void OnGetScore()
    {
        GetLocalId();
    }

    private void UpdateScore()
    {
        scoreText.text = "Score: " + user.userScore;
    }

    private void PostToDatabase(bool emptyScore = false)
    {
        User user = new User();

        if (emptyScore)
        {
            user.userScore = 0;
        }
        
        RestClient.Put(databaseURL + "/" + localId + ".json?auth=" + idToken, user);
    }

    private void RetrieveFromDatabase()
    {
        RestClient.Get<User>(databaseURL + "/" + getLocalId + ".json?auth=" + idToken).Then(response =>
            {
                user = response;
                UpdateScore();
            });
    }

    public void SignUpUserButton()
    {
        if(passwordText.text.Length<6)
        {
            showToastOnUiThread("Password length must be atleast 6 characters");
        }
        else if (emailText.text == "" || usernameText.text == "" || passwordText.text == "")
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                showToastOnUiThread("Username or Email or Password Field is/are Empty!");
            } 
        }
        
        else
        {
            SignUpUser(emailText.text, usernameText.text, passwordText.text);
        }
    }

    public void SignInUserButton()
    {
        if (passwordText.text.Length < 6)
        {
            showToastOnUiThread("Password length must be atleast 6 characters");
        }

        if (emailText.text == "" || passwordText.text == "")
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                showToastOnUiThread("Email or Password Field is empty");
            }
        }
        else
        {
            SignInUser(emailText.text, passwordText.text);
        }
    }
    private void SignUpUser(string email, string username, string password)
    {
        string userData = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
        RestClient.Post<SignResponse>("https://www.googleapis.com/identitytoolkit/v3/relyingparty/signupNewUser?key=" + AuthKey, userData).Then(
            response =>
            {
                idToken = response.idToken;
                localId = response.localId;
                playerName = username;
                PostToDatabase(true);
                NotificationManager.SendWithAppIcon(TimeSpan.FromSeconds(0.1),
                "Notification",
                "You have Successfully Signed In",
                Color.white,
                NotificationIcon.Message);
                SceneManager.LoadScene(0);
                showToastOnUiThread("User Registered Successfully!");

            }).Catch(error =>
        {
            showToastOnUiThread("Invalid Email Id");
            Debug.Log(error);
        });
    }
    
    private void SignInUser(string email, string password)
    {
        string userData = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
        RestClient.Post<SignResponse>("https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key=" + AuthKey, userData).Then(
            response =>
            {
                idToken = response.idToken;
                localId = response.localId;
                GetUsername();
                
    
            }).Catch(error =>
        {

    
            showToastOnUiThread("Invalid User");
            Debug.Log(error);
        });
    }

    //Toast Message Code Below
    void showToastOnUiThread(string toastString)
    {
        AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

        currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        this.toastString = toastString;

        currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(showToast));
    }

    void showToast()
    {
        Debug.Log("Running on UI thread");
        AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
        AndroidJavaClass Toast = new AndroidJavaClass("android.widget.Toast");
        AndroidJavaObject javaString = new AndroidJavaObject("java.lang.String", toastString);
        AndroidJavaObject toast = Toast.CallStatic<AndroidJavaObject>("makeText", context, javaString, Toast.GetStatic<int>("LENGTH_SHORT"));
        toast.Call("show");
    }

    private void GetUsername()
    {
        RestClient.Get<User>(databaseURL + "/" + localId + ".json?auth=" + idToken).Then(response =>
        {
            playerName = response.userName;
            PlayerPrefs.SetString("Username", playerName);
            SceneManager.LoadScene(2);
            Debug.Log(playerName);
        });
    }
    
    private void GetLocalId(){
        RestClient.Get(databaseURL + ".json?auth=" + idToken).Then(response =>
        {
            var username = getScoreText.text;
            
            fsData userData = fsJsonParser.Parse(response.Text);
            Dictionary<string, User> users = null;
            serializer.TryDeserialize(userData, ref users);

            foreach (var user in users.Values)
            {
                if (user.userName == username)
                {
                    getLocalId = user.localId;
                    RetrieveFromDatabase();
                    break;
                }
            }
        }).Catch(error =>
        {
            Debug.Log(error);
        });
    }
}
