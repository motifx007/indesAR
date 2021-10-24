using UnityEngine;
using System.Collections;
using ZXing;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class ScanQRCode : MonoBehaviour 
{
    public Text Showresult;
	private WebCamTexture webCamTexture;
	public string resultText;
	private Material quadMat;
    private string text;

    //public static ScanQRCode instance;

    //Awake is always called before any Start functions
  

        /*
        public static ScanQRCode instance = null;

        //Awake is always called before any Start functions
        void Awake()
        {
            //Check if instance already exists
            if (instance == null)
            {
                //if not, set instance to this
                instance = this;
            }
            //If instance already exists and it's not this:
            else if (instance != this)
            {
                Destroy(transform.gameObject);
            }
            //Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(transform.gameObject);

        }
        */

        // Use this for initialization
        void Start () 
	{
		webCamTexture = new WebCamTexture (1080,2160);//4096,4096
		WebCamDevice[] devices = WebCamTexture.devices;
		webCamTexture.deviceName = devices[0].name;
		quadMat = GetComponent<Renderer>().material;
		quadMat.mainTexture = webCamTexture;
		webCamTexture.Play ();
        //b1.onClick.AddListener(Scenechange);
		InvokeRepeating ("Scan", 1f, 1f);
	}

	void Update()
	{
		transform.rotation = Quaternion.AngleAxis(webCamTexture.videoRotationAngle, -Vector3.forward);

		var screenAspect = (float)Screen.width / Screen.height;
		var webCamAspect = (float)webCamTexture.width / webCamTexture.height;
		Debug.Log ("screenAspect: " + screenAspect + " " + "webCamAspect " + webCamAspect);
		Debug.Log ("webCam.width: "+webCamTexture.width+" "+"webCam.height: "+webCamTexture.height);

		var rot90 = (webCamTexture.videoRotationAngle / 90) % 2 != 0;
		//Debug.Log ("rot90: " + rot90);
		if (rot90) 
		{
			webCamAspect = 1.0f / webCamAspect;
		}
		//Debug.Log ("webCamAspect " + webCamAspect); //0.5625

		float sx, sy;
		if (webCamAspect < screenAspect) //0.5625 > 0.5622189
		{
			sx = webCamAspect;
			sy = 1.0f;
		}
		else
		{
			sx = screenAspect;
			sy = screenAspect / webCamAspect;
		}
		//Debug.Log ("sx "+sx+" sy "+sy);

		if (rot90) 
		{
			transform.localScale = new Vector3 (sy, sx, 1);
		} 
		else 
		{
			transform.localScale = new Vector3 (1,1, 1);
		}
			
		//var mirror = webCamTexture.videoVerticallyMirrored;
		//Debug.Log ("mirror " + mirror);
		//quadMat.mainTextureOffset = new Vector2(0, mirror ? 1 : 0);
		//quadMat.mainTextureScale = new Vector2(1, mirror ? -1 : 1);
        //Debug.Log("Offset"+quadMat.mainTextureOffset);
        //Debug.Log("Scale"+quadMat.mainTextureScale);
    }

	private void Scan()
	{
		if (webCamTexture != null && webCamTexture.width > 100) 
		{
			resultText = Decode(webCamTexture.GetPixels32 (), webCamTexture.width, webCamTexture.height);
			//Showresult.text=resultText;
            if(resultText!=null)
            {
                //webCamTexture.Stop();
                //CancelInvoke();
                PlayerPrefs.SetString("Name", resultText);
                Scenechange(2);
            }
            //webCamTexture.Stop();
        }
        //webCamTexture.Stop();
	}
    public void Scenechange(int i)
    {

        webCamTexture.Stop();
        CancelInvoke();
        SceneManager.LoadScene(i);

    }
	public string Decode(Color32[] colors, int width, int height)
	{
		BarcodeReader reader = new BarcodeReader ();
		var result = reader.Decode (colors, width, height);
		if (result != null) 
		{
			return result.Text;
            //webCamTexture.Stop();
        }
		return null;
	}

    /*
	void OnGUI()
	{
		var text = "web cam size = " + webCamTexture.width + " x " + webCamTexture.height;
		text += "\nrotation = " + webCamTexture.videoRotationAngle;
		text += "\nscreen size = " + Screen.width + " x " + Screen.height;
        text = "\nresultText = " + resultText;
		GUI.Label(new Rect(0, 0, Screen.width, Screen.height), text);
	}
    */
    
}
