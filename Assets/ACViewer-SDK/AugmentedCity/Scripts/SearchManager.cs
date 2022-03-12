using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchManager : MonoBehaviour
{
    private string input;
    public NavigationManager navScript;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReadInput(string str)
    {
        navScript = GetComponent<NavigationManager>();
        input = str;
        Debug.Log(input);

        for(int i = 0; i < navScript.parents.Count; i++)
        {
            string s = navScript.texts[i].GetComponent<Text>().text;
            if(s.StartsWith(input))
            {
                // show parent element
                navScript.parents[i].SetActive(true);
            }
            else
            {
                // hide parent element
                navScript.parents[i].SetActive(false);
            }
        }
    }
}
