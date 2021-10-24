using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Permissionstogrant : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //#if PLATFORM_ANDROID
        AndroidRuntimePermissions.Permission[] result = AndroidRuntimePermissions.RequestPermissions("android.permission.WRITE_EXTERNAL_STORAGE", "android.permission.CAMERA");
        if( result[0] == AndroidRuntimePermissions.Permission.Granted && result[1] == AndroidRuntimePermissions.Permission.Granted )
        	Debug.Log( "We have all the permissions!" );
        else
        	Debug.Log( "Some permission(s) are not granted..." );
        //#endif
    }
}
