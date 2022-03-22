using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEditor;

public class LoginManager : MonoBehaviour
{
    public InputField inputField;
    public string userIDInput;
    public GameObject loginPanel;

    public void guestLogin()
    {
        userIDInput = "guest";
        loginPanel.SetActive(false);
    }
    public void readInput()
    {
        if (inputField.text != "")
        {
            userIDInput = inputField.text;
            // Debug.Log("user input " +userIDInput);
            loginPanel.SetActive(false);

        }
    }


}
