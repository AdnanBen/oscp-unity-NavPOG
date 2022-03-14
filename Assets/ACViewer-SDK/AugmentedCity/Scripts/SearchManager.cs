using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

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
            string s = navScript.texts[i].GetComponent<Text>().text.ToLower();
            string[] words = s.Split(' ');

            LinkedList<string> finWords = new LinkedList<string>(words);
            
            //string[] finWords = new string[words.Length];
            for (int j = 0; j < words.Length; j++)
            {
                LinkedList<string> finWords2 = new LinkedList<string>(finWords);
                //finWords.AddLast(s);
                for(int k = 0; k < j; k++)
                {
                    finWords2.RemoveFirst();
                }
                words[j] = string.Join(" ", finWords2.ToArray());
            }

            /*for (int j = 0; j < words.Length; j++)
            {
                finWords
            }*/

            

            foreach (string word in words)
            {
                if (word.StartsWith(input))
                {
                    // show parent element
                    navScript.parents[i].SetActive(true);
                    break;
                }
                else
                {
                    // hide parent element
                    navScript.parents[i].SetActive(false);
                }
            }
        }
    }
}
