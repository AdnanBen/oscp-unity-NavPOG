using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;

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

        for (int i = 0; i < navScript.parents.Count; i++)
        {
            string s = navScript.texts[i].GetComponent<TextMeshProUGUI>().text.ToLower();
            string[] words = s.Split(' ');

            LinkedList<string> wordsLL = new LinkedList<string>(words);
            for (int j = 0; j < words.Length; j++)
            {
                LinkedList<string> wordsCopy = new LinkedList<string>(wordsLL);
                for(int k = 0; k < j; k++)
                {
                    wordsCopy.RemoveFirst();
                }
                words[j] = string.Join(" ", wordsCopy.ToArray());
            }

            foreach (string word in words)
            {
                if (word.StartsWith(input))
                {
                    navScript.parents[i].SetActive(true);
                    break;
                }
                else
                {
                    navScript.parents[i].SetActive(false);
                }
            }
        }
    }
}
