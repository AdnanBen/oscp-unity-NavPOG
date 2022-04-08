using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using UnityEngine.Networking;
using System.Text;
using System;
using System.Linq;
using TMPro;

public class locationEntry
{
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
    public List<locationEntry> locationEntries = new List<locationEntry>();
    public GameObject content;
    public Sprite locationElement;
    public locationEntry destination;
    public GameObject navigationPanel;
    public GameObject arrowObject;

    // Start is called before the first frame update
    void Start()
    {
        Input.location.Start();
    }

    void OnEnable()
    {
        Debug.Log("Navigation manager started");
        Input.location.Start();
        //StartCoroutine(getNearPlaceholders("51.531940", "-0.050740", "10000"));
        StartCoroutine(getNearPlaceholders(Input.location.lastData.latitude.ToString(), Input.location.lastData.longitude.ToString(), "10000"));

    }

    void OnDisable()
    {
        foreach (GameObject parent in parents)
        {
            Destroy(parent);
        }
        parents = new List<GameObject>();
        texts = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReadInput(string input)
    {
        int radius;
        if (int.TryParse(input, out radius))
        {
            foreach (GameObject parent in parents)
            {
                Destroy(parent);
            }
            parents = new List<GameObject>();
            texts = new List<GameObject>();

            Input.location.Start();
            //StartCoroutine(getNearPlaceholders("51.531940", "-0.050740", input));
            StartCoroutine(getNearPlaceholders(Input.location.lastData.latitude.ToString(), Input.location.lastData.longitude.ToString(), input));
        }
    }

    decimal[] CalculateDistanceBetweenCoords(float lat1, float lon1, float lat2, float lon2)
    {
        const float EARTH_RADIUS = 6371000f;
        float tempLat = lat1 * Mathf.PI / 180;
        float tempLat2 = lat2 * Mathf.PI / 180;
        float deltaLat = (lat2 - lat1) * Mathf.PI / 180;
        float deltaLon = (lon2 - lon1) * Mathf.PI / 180;

        float a = Mathf.Sin(deltaLat / 2) * Mathf.Sin(deltaLat / 2) + Mathf.Cos(tempLat) * Mathf.Cos(tempLat2) * Mathf.Sin(deltaLon / 2) * Mathf.Sin(deltaLon / 2);
        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));

        decimal roundedMetres = (decimal)(EARTH_RADIUS * c);
        roundedMetres = Math.Round(roundedMetres, 0);

        decimal roundedKm = (decimal)(EARTH_RADIUS * c / 1000);
        roundedKm = Math.Round(roundedKm, 2);

        if (roundedKm < 1)
        {
            return new decimal[] { roundedMetres, 1 };
        }

        return new decimal[] { roundedKm, 0 };
    }

    void add(string title, float latitude, float longitude)
    {
        // location background element
        GameObject locationBg = new GameObject();
        locationBg.transform.parent = content.transform;

        // background image for elements
        Image bg = locationBg.AddComponent<Image>();

        Shadow shadow = locationBg.AddComponent<Shadow>();
        shadow.effectDistance = new Vector2(-1f, -1f);
        shadow.effectColor = new Color32(0xC6, 0xC6, 0xC6, 0xFF);

        // size location elements appropriately
        RectTransform rect = locationBg.GetComponent<RectTransform>();
        rect.localScale = new Vector3(1f, 1f, 1f);
        rect.sizeDelta = new Vector2(91, 15);

        // text elements as children of bg
        GameObject location = new GameObject();
        location.transform.parent = locationBg.transform;

        // text styling
        TextMeshProUGUI locationText = location.AddComponent<TextMeshProUGUI>();
        locationText.text = char.ToUpper(title[0]) + title.Substring(1).ToLower();
        locationText.color = Color.black;
        locationText.rectTransform.localScale = new Vector3(1f, 1f, 1f);
        locationText.rectTransform.localPosition = new Vector3(0f, 0f, 0f);
        locationText.rectTransform.sizeDelta = new Vector2(80f, 10f);
        locationText.fontSize = 10;
        locationText.enableWordWrapping = true;
        locationText.enableAutoSizing = true;
        locationText.fontSizeMin = 0;
        locationText.alignment = TextAlignmentOptions.BottomLeft;

        location.AddComponent<LocationData>();
        LocationData dataScript = location.GetComponent<LocationData>();
        dataScript.latitude = latitude;
        dataScript.longitude = longitude;

        location.AddComponent<Button>();
        location.GetComponent<Button>().onClick.AddListener(() => OnLocationClick(dataScript.latitude, dataScript.longitude));

        texts.Add(location);
        parents.Add(locationBg);
    }

    void OnLocationClick(float latitude, float longitude)
    {
        navigationPanel.SetActive(false);
        arrow arrowScript = arrowObject.GetComponent<arrow>();
        arrowScript.latitude = latitude;
        arrowScript.longitude = longitude;
        arrowScript.gameObject.SetActive(true);
    }

    IEnumerator getNearPlaceholders(string latitude, string longitude, string radius)
    {
        locationEntries = new List<locationEntry>();

        string apiURL = "https://developer.augmented.city/rpc/get_near_placeholders?p_latitude=" + latitude + "&p_longitude=" + longitude + "&p_radius=" + radius;

        UnityWebRequest req = UnityWebRequest.Get(apiURL);

        yield return req.SendWebRequest();

        if (req.isNetworkError || req.isHttpError)
        {
            Debug.LogError(req.error);
            yield break;

        }


        JSONNode info = JSON.Parse(req.downloadHandler.text);

        locationEntry[] placeholder_ids = new locationEntry[info.Count];

        List<string> placeholder_ids_array = new List<string>();


        for (int i = 0; i < info.Count; i++)
        {
            {
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
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        yield return req.SendWebRequest();

        info = JSON.Parse(req.downloadHandler.text);

        for (int i = 0; i < info.Count; i++)
        {
            locationEntries[i].name = info[i]["sticker"]["sticker_text"];
        }

        var locationDistancePairs = new List<(string, decimal, locationEntry)>();
        var locationDistancePairs2 = new List<(string, decimal, locationEntry)>();

        foreach (locationEntry entry in locationEntries)
        {
            Debug.Log(entry.name);
            Debug.Log(entry.placeholder_id);
            Debug.Log(entry.latitude);
            Debug.Log(entry.longitude);
            decimal[] distanceArray = CalculateDistanceBetweenCoords(float.Parse(entry.latitude), float.Parse(entry.longitude), float.Parse(latitude), float.Parse(longitude));
            decimal distance = distanceArray[0];
            string printString = entry.name + " " + distance.ToString() + "m";
            if (distanceArray[1] == 0)
            {
                printString = entry.name + " " + distance.ToString() + "km";
                locationDistancePairs2.Add((printString, distance, entry));
                continue;
            }

            locationDistancePairs.Add((printString, distance, entry));

            //add(printString, 2.1f, 5.6f);
        }

        locationDistancePairs.Sort((a, b) => a.Item2.CompareTo(b.Item2));
        locationDistancePairs2.Sort((a, b) => a.Item2.CompareTo(b.Item2));

        foreach (var item in locationDistancePairs)
        {
            string name = item.Item1;
            add(name, float.Parse(item.Item3.latitude), float.Parse(item.Item3.longitude));
        }

        foreach (var item in locationDistancePairs2)
        {
            string name = item.Item1;
            add(name, float.Parse(item.Item3.latitude), float.Parse(item.Item3.longitude));
        } 
    }
}

/* TODO:
 * make distances stick to right side
 * navigation logic
 * investigate search bug
 */