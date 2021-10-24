using System;
using UnityEngine;
using UnityEngine.UI;

public class UIControllerScript : MonoBehaviour
{
    public InputField passwordInput = null;

    public void ToggleInputType()
    {
        if (this.passwordInput != null)
        {
            if (this.passwordInput.contentType == InputField.ContentType.Password)
            {
                this.passwordInput.contentType = InputField.ContentType.Standard;
            }
            else
            {
                this.passwordInput.contentType = InputField.ContentType.Password;
            }

            this.passwordInput.ForceLabelUpdate();
        }
    }
}