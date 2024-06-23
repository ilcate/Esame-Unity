using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class ImageScript : MonoBehaviour
{

    public GameObject camera1;
    public GameObject camera2;


    public void activateGameCam()
    {
        camera2.SetActive(true);
        camera1.SetActive(false);
    }
}
