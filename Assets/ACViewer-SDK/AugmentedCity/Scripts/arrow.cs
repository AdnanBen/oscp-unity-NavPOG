using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.Networking;

public class arrow : MonoBehaviour
{
    public GameObject gameObject;
    string apiURL = "http://developer.augmented.city";

  

    void Start()
    {
        Input.compass.enabled = true;
        Input.location.Start();
        //InvokeRepeating("Spin", 1f, 0.5f);
    }

    void Spin()
    {
        //Debug.Log(Time.time);
        //gameObject.transform.rotation = Quaternion.Euler(0f, 0f, Input.compass.trueHeading - 90f);
        
        Debug.Log("true: " + Input.compass.trueHeading + ", magnetic: " + Input.compass.magneticHeading);
        Debug.Log(Input.location.status);
        Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
        //transform.rotation = Quaternion.Euler(0, 0, -Input.compass.trueHeading);
        //gameObject.transform.rotation = transform.rotation;
        //gameObject.transform.Rotate(0f, 0f, -5f);
    }

    // returns bearing from location1 to location2
    float calculateAngleBetweenTwoCoords(float lat1, float long1, float lat2, float long2)
    {
        float long_diff = long2 - long1;
        
        float y = Mathf.Sin(long_diff) * Mathf.Cos(lat2);
        float x = (Mathf.Cos(lat1) * Mathf.Sin(lat2)) - (Mathf.Sin(lat1) * Mathf.Cos(lat2) * Mathf.Cos(long_diff));

        float angle = Mathf.Atan2(y, x);

        angle = angle * Mathf.Rad2Deg;
        if (angle < 360) {
            angle += 360;
        }

        return angle % 360;
    }

    void getNearPlaceholders() 
    {

        string apiURL = "https://developer.augmented.city/rpc/get_near_placeholders?p_latitude=51.531940&p_longitude=-0.050740&p_radius=1000";

        UnityWebRequest req = UnityWebRequest.Get(apiURL);
        req.SendWebRequest();

        if (req.isNetworkError || req.isHttpError) 
        {
            Debug.LogError(req.error);

        }

        JSONNode info = JSON.Parse(req.downloadHandler.text);

        JSONNode placeholders = info["placeholder"];

        string[] placeholder_ids = new string[placeholders.Count];

        for (int i = 0; i < placeholders.Count; i++) {
            placeholder_ids[i] = placeholders[i]["placeholder_id"];   
            Debug.Log(placeholder_ids[i]);
        }



    }



    void Update()
    {
        //transform.rotation = Quaternion.Euler(0, 0, -Input.compass.trueHeading);

        float bearing = calculateAngleBetweenTwoCoords(51.52461870444352f, -0.04846241626718877f, 51.524613297150005f, -0.047158412442516014f);
        gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, Quaternion.Euler(0f, 0f, Input.compass.magneticHeading + 90f - bearing), 0.1f);


        /*TODO: 
         * get coordinates
         * UI for picking location (scrolling, searching)
         * terminating navigation
         */
    }
}
