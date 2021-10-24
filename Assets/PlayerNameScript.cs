using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameScript : MonoBehaviour
{
    public Text getPlayerName;

    // public string playerNameText;
    // Start is called before the first frame update
    void Start()
    {
        var str = PlayerPrefs.GetString("Username");
        getPlayerName.text = "   You are signed in as "+str+" !";
    }
    
}
