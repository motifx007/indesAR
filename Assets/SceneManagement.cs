using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManagement : MonoBehaviour
{
    
    public void sceneChange(int i)
    {
        SceneManager.LoadScene(i);
    }


}
