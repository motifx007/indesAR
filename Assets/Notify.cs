using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.SimpleAndroidNotifications
{
    public class Notify : MonoBehaviour
    {
        public void SendNotif()
        {
            NotificationManager.SendWithAppIcon(TimeSpan.FromSeconds(5),
                "Notification",
                "You have Successfully Signed In",
                Color.white,
                NotificationIcon.Message);
        }
    }
}

