using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenechange : MonoBehaviour
{
    // Start is called before the first frame update
    public void Sceneswitch(int i)
    {
        SceneManager.LoadScene(i);
    }
}
