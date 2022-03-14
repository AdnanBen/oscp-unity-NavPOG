using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using UnityEngine.Networking;
using System.Text;
using System;

public class locationEntry {
            public string name;
            public string placeholder_id;
            public string latitude;
            public string longitude;
        }


[Serializable]
public class JSONform
{
    public List<string> p_placeholder_ids;
    public string p_language;

}

public class NavigationManager : MonoBehaviour
{
    public List<GameObject> texts = new List<GameObject>();
    public List<GameObject> parents = new List<GameObject>();
    public GameObject content;
    public Font font;
    public Sprite locationElement;

    // Start is called before the first frame update
    void Start()
    {

        Input.location.Start();
        //StartCoroutine(getNearPlaceholders("51.531940", "-0.050740", "1000"));
        StartCoroutine(getNearPlaceholders(Input.location.lastData.latitude.ToString(), Input.location.lastData.longitude.ToString(), "100"));

    }

    // Update is called once per frame
    void Update()
    {
            
    }
    
    void add(string title, float latitude, float longitude)
    {
        // location background element
        GameObject locationBg = new GameObject();
        locationBg.transform.parent = content.transform;

        // background image for elements
        Image bg = locationBg.AddComponent<Image>();
        bg.sprite = locationElement;
        bg.color = new Color32(227, 196, 255, 255);

        // size location elements appropriately
        RectTransform rect = locationBg.GetComponent<RectTransform>();
        rect.localScale = new Vector3(1f, 1f, 1f);
        rect.sizeDelta = new Vector2(91, 30);

        // text elements as children of bg
        GameObject location = new GameObject();
        location.transform.parent = locationBg.transform;

        // text styling
        Text locationText = location.AddComponent<Text>();
        locationText.text = title;
        locationText.font = font;
        locationText.color = Color.black;
        locationText.rectTransform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        locationText.rectTransform.localPosition = new Vector3(-14f, 4f, 0f);
        locationText.rectTransform.sizeDelta = new Vector2(100f, 30f);
        locationText.fontSize = 22;
        //locationText.fontStyle = FontStyle.Bold;
        locationText.horizontalOverflow = HorizontalWrapMode.Overflow;

        texts.Add(location);
        parents.Add(locationBg);
    }

    IEnumerator getNearPlaceholders(string latitude, string longitude, string radius) 
    {

        string apiURL = "https://developer.augmented.city/rpc/get_near_placeholders?p_latitude=" + latitude +  "&p_longitude=" + longitude +  "&p_radius=" + radius;

        UnityWebRequest req = UnityWebRequest.Get(apiURL);

        yield return req.SendWebRequest();

        if (req.isNetworkError || req.isHttpError) 
        {
            Debug.LogError(req.error);
            yield break;

        }
        

        JSONNode info = JSON.Parse(req.downloadHandler.text);

        locationEntry[] placeholder_ids = new locationEntry[info.Count];

        List<locationEntry> locationEntries = new List<locationEntry>();

        List<string> placeholder_ids_array = new List<string>();


        for (int i = 0; i < info.Count; i++) {
            if (info[i]["placeholder"]["projections"].Count != 0) {
                locationEntry newEntry = new locationEntry();
                newEntry.name = "default";
                newEntry.placeholder_id = info[i]["placeholder"]["placeholder_id"];
                newEntry.latitude = info[i]["placeholder"]["gps"]["latitude"];
                newEntry.longitude = info[i]["placeholder"]["gps"]["longitude"];
                locationEntries.Add(newEntry);
                placeholder_ids_array.Add(newEntry.placeholder_id);
            }
        }


        JSONform jsonform = new JSONform();
        jsonform.p_placeholder_ids = placeholder_ids_array;
        jsonform.p_language = "en";

        string jsonbody = JsonUtility.ToJson(jsonform);

        Debug.Log(jsonbody);
        

        req = new UnityWebRequest("https://developer.augmented.city/rpc/get_stickers_by_placeholders", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonbody);
        req.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        yield return req.SendWebRequest();

        info = JSON.Parse(req.downloadHandler.text);

        for (int i = 0; i < info.Count; i++) {
            locationEntries[i].name = info[i]["sticker"]["sticker_text"];
        }

        foreach (locationEntry entry in locationEntries) {
            Debug.Log(entry.name);
            Debug.Log(entry.placeholder_id);
            Debug.Log(entry.latitude);
            Debug.Log(entry.longitude);
            add(entry.name, 2.1f, 5.6f);

        }

    }
}
