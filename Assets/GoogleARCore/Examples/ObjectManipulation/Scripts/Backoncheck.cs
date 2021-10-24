using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backoncheck : MonoBehaviour
{
    public GameObject panel;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            panel.SetActive(false);
        }
    }
}
