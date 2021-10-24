using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[Serializable]
public class User
{
    public string localId;
    public string userName;
    public int userScore;
    public string modelNo;
    public string url;

    public User()
    {
        userName = PlayerScores.playerName;
        userScore = PlayerScores.playerScore;
        localId = PlayerScores.localId;
        modelNo = SearchScript.mNo;
        url = SearchScript.url;
    }
}
