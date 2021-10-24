//-----------------------------------------------------------------------
// <copyright file="AndyPlacementManipulator.cs" company="Google">
//
// Copyright 2018 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCore.Examples.ObjectManipulation
{
    using GoogleARCore;
    using UnityEngine;
    using UnityEngine.Networking;
    using System.Collections;
    using UnityEngine.UI;
    using System;
    using UnityEngine.SceneManagement;
    using System.IO;
    using System.Linq;


    /// <summary>
    /// Controls the placement of Andy objects via a tap gesture.
    /// </summary>
    public class AndyPlacementManipulator : Manipulator
    {
        /// <summary>
        /// The first-person camera being used to render the passthrough camera image (i.e. AR background).
        /// </summary>
        public Camera FirstPersonCamera;

        /// <summary>
        /// Manipulator prefab to attach placed objects to.
        /// </summary>
        public GameObject ManipulatorPrefab;

        public GameObject other;
        public string scr;
        public GameObject Arcprefab;
        AssetBundle bundle;
        UnityWebRequest www;
        public GameObject Sofa;
        GameObject andyObject;
        Anchor anchor;
        GameObject manipulator;
        public ARCoreSession Ar;
        int flag = 0;
        public Text TextHeader;
        public string geturl;
        public GameObject[] ch;
        bool isdown;
        bool ison;
        public SimpleHealthBar healthBar;
        public GameObject barswitch;
        public GameObject imagedispanel;
        public Button bt1;
        Texture2D ss;
        string path;
        public RawImage img1;
        public Text text1;
        public DateTime LastWriteTime;
        public Button bt2;
        static public string urlofindes;

        private void Start()   
        {
            Debug.Log("Hello");
            geturl = PlayerPrefs.GetString("Name"); //for getting the saved url of the model obtained from
                                                    //the Qrcode/Model Number
            Debug.Log("URL : " + geturl);
            ison = true; //a variable set as true for checkInternetConnection function

            ////A coroutine function which executes parallely for checking the internet connection
            StartCoroutine(checkInternetConnection((isConnected) => {
            }));
        }

        IEnumerator checkInternetConnection(Action<bool> action)
        {
            while (ison)
            {
                //for checking the connectivity by requesting the page "https://www.google.com/"
                UnityWebRequest www = new UnityWebRequest("https://www.google.com/");
                yield return www.SendWebRequest();

                if (www.error != null)
                {
                    action(false);
                    Debug.Log("Not Reachable");
                }
                else
                {
                    action(true);
                    Debug.Log("Reachable");
                    StartCoroutine(GetAssetBundle()); //coroutine for downloading asset bundle
                    yield break;
                }
            }
        }

        IEnumerator GetAssetBundle()
        {
            //download the asset bundle 
            //geturl is the url of the corresponding furniture model selected by the user

            www = UnityWebRequestAssetBundle.GetAssetBundle(geturl);
            barswitch.SetActive(true); //progressbar is set active
            isdown = true; 
            StartCoroutine(Progresscheck()); //function that updates the progres bar by referring to the downloader code
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("success");
                yield return new WaitForSeconds(1);
                barswitch.SetActive(false); //barswitch is deactivated after the model is downloaded
                bundle = DownloadHandlerAssetBundle.GetContent(www); //Assetbundle is saved to bundle
                Sofa = bundle.LoadAsset("q3") as GameObject; //extracts the prefab to the gameobject “sofa”  
            }
        }

        IEnumerator Progresscheck()
        {
            while (isdown)
            {
                var down = www.downloadProgress; //for getting the download progresss
                healthBar.UpdateBar(down, 1); //updates the progress bar with the value of the variable “down”
                Debug.Log(down);
                yield return new WaitForSeconds(0.1f);
                if(down==1)
                {
                    isdown = false;
                }
            }
        }

        IEnumerator Example()
        {
            yield return new WaitForSeconds(2);
        }

        /// <summary>
        /// Returns true if the manipulation can be started for the given gesture.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <returns>True if the manipulation can be started.</returns>
        protected override bool CanStartManipulationForGesture(TapGesture gesture)
        {
            if (gesture.TargetObject == null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Function called when the manipulation is ended.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        protected override void OnEndManipulation(TapGesture gesture)
        {
            if (gesture.WasCancelled)
            {
                return;
            }

            // If gesture is targeting an existing object we are done.
            if (gesture.TargetObject != null)
            {
                return;
            }

            if (flag != 1)
            { 
                // Raycast against the location the player touched to search for planes.
                TrackableHit hit;
                TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon;

                if (Frame.Raycast(gesture.StartPosition.x, gesture.StartPosition.y, raycastFilter, out hit))
                {
                    // Use hit pose and camera pose to check if hittest is from the
                    // back of the plane, if it is, no need to create the anchor.
                    if ((hit.Trackable is DetectedPlane) &&
                    Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position,
                        hit.Pose.rotation * Vector3.up) < 0)
                    {
                        Debug.Log("Hit at back of the current DetectedPlane");
                    }
                    else
                    {
                        // Instantiate Andy model at the hit pose.
                        andyObject = Instantiate(Sofa, hit.Pose.position, hit.Pose.rotation * Quaternion.Euler(0f, 180f, 0f));

                        // Instantiate manipulator.
                        manipulator = Instantiate(ManipulatorPrefab, hit.Pose.position, hit.Pose.rotation);

                        // Make Andy model a child of the manipulator.
                        andyObject.transform.parent = manipulator.transform;

                        // Create an anchor to allow ARCore to track the hitpoint as understanding of the physical
                        // world evolves.
                        anchor = hit.Trackable.CreateAnchor(hit.Pose);

                        // Make manipulator a child of the anchor.
                        manipulator.transform.parent = anchor.transform;

                        // Select the placed object.
                        manipulator.GetComponent<Manipulator>().Select();

                        //DeleteonClick(manipulator);

                        flag = 1; 

                    }
                }
            }
        }

        //function for scene change
        public void Scenechange(int i)
        {
            bundle.Unload(false); //unload the bundle after usage to save memory
            SceneManager.LoadScene(i);

        }

        //function for capturing of screenshot of the model after placing in the real world
        public void Capscreen1()
        {
            Arcprefab.GetComponent<ARCoreSession>().enabled = false;
            ch = GameObject.FindGameObjectsWithTag("ARCORE"); //find all the objects having the tag as “ARCORE
            Disable(); //for disabling the objects having the tag “ARCORE” during capturing of screenshot
            StartCoroutine(TakeScreenshotAndSave());
            Arcprefab.GetComponent<ARCoreSession>().enabled = true;

        }

        public void Disable()
        {
            for (int i =0; i < ch.Length; i++)
            {
                ch[i].SetActive(false); //for making the objects inactive one by one
            }
        }

        public void Enable()
        {
            for (int i = 0; i < ch.Length; i++)
            {
                ch[i].SetActive(true); //for making the objects active one by one
            }
        }

        private IEnumerator TakeScreenshotAndSave()
        {
            yield return new WaitForEndOfFrame();

            //calculation of screen width, height etc for saving the image
            ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            ss.Apply();

            //for saving the image to gallery
            var someth =NativeGallery.SaveImageToGallery(ss, "GalleryTest", "My img {0}.png");

            // To avoid memory leaks
            Destroy(ss);
            Enable();
            StartCoroutine(PickImage());
        }

        //selection of the image that was previously saved for changing the image on the button
        IEnumerator PickImage()
        {
            var path1 = "/storage/emulated/0/GalleryTest";
            var some = Directory.GetFiles(path1, "*.*").Select(f => new FileInfo(f)).OrderByDescending(f => f.CreationTime).ToArray();
            path = path1 + "/" + some[0].Name; //name of the last saved image
            Texture2D text = NativeGallery.LoadImageAtPath(path, 400); //for loading the image from path and set it's size to 400
            Rect rec = new Rect(0, 0, text.width, text.height/4); //cropping of the image having size referened from the button
            bt1.image.sprite = Sprite.Create(text, rec, new Vector2(0.5f, 0.5f), 400); //the button’s image is changed to that of the image
            yield break;
        }

        //selection of the image that was previously saved and user have the option to share it
        public void DisplayImage()
        {
            imagedispanel.SetActive(true); //activate the panel for displaying the image and share button
            Texture2D text = NativeGallery.LoadImageAtPath(path, 1024);
            var w=text.width/1.5f;
            var h=text.height/1.5f;
            img1.rectTransform.sizeDelta = new Vector2(w,h); //setting the transform for 
            img1.texture = text; //displaying of the image
            bt2.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);       
        }

        public void Shareimage()
        {
            Debug.Log("hello");
            Arcprefab.GetComponent<ARCoreSession>().enabled = false;
            StartCoroutine(ShareScreenshot(path));

            Arcprefab.GetComponent<ARCoreSession>().enabled = true;
        }


        private IEnumerator ShareScreenshot(string destination)
        {
            Debug.Log("Hi");
            string ShareSubject = "Picture Share";
            string shareLink = " Made By Team IndesAR";
            string textToShare = "Made with Google ARCore";

            Debug.Log(destination);


            if (!Application.isEditor)
            {

                AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
                AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
                intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
                AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
                AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", /* "file://" +*/ destination);

                intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
                intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), textToShare + shareLink);
                intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), ShareSubject);
                intentObject.Call<AndroidJavaObject>("setType", "image/png");
                AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
                currentActivity.Call("startActivity", intentObject);
            }
            yield return null;
        }
    }
}
