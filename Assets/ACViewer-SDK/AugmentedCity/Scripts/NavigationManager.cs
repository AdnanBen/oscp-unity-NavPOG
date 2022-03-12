using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        add("main", 2.1f, 5.6f);
        add("main2", 2.1f, 5.6f);
        add("main3", 2.1f, 5.6f);
        add("main4", 2.1f, 5.6f);
        add("main5", 2.1f, 5.6f);
        add("main6", 2.1f, 5.6f);
        add("main7", 2.1f, 5.6f);
        add("main8", 2.1f, 5.6f);
        add("main9", 2.1f, 5.6f);
        add("main10", 2.1f, 5.6f);
        add("main11", 2.1f, 5.6f);
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
}
