using System.Collections;
using System.Collections.Generic;
using Proyecto26;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SearchScript : MonoBehaviour
{
    public Text scoreText;
    public InputField nameText;
    public string urlfromserver;
        

    User user = new User();

    public static string url;
    public static string mNo;


    // Start is called before the first frame update
  
    public void OnSubmit()
    {   

        mNo = nameText.text;
        mNo.ToUpper();
        PostToDatabase();
    }

    public void OnGetScore()
    {
        RetrieveFromDatabase();
      
    }


    private void UpdateScore()
    {
        scoreText.text = "URL: " + user.url;
        urlfromserver = user.url;
        PlayerPrefs.SetString("Name", urlfromserver);
        SceneManager.LoadScene(4);
    }

    private void PostToDatabase()
    {
        User user = new User();
        
        RestClient.Put("https://indesar-dc79d.firebaseio.com/" + mNo + ".json", user);
    }

    private void RetrieveFromDatabase()
    {
        RestClient.Get<User>("https://indesar-dc79d.firebaseio.com/" + nameText.text + ".json").Then(response =>
        {
            user = response;
            UpdateScore();
        });
    }
}
