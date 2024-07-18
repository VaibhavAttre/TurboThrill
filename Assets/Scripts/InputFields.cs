using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;

public class InputFields : MonoBehaviour
{
    public static string playerName;
    [SerializeField] private TMPro.TMP_InputField inputField;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("return"))
        {
            //playerName = inputBox.text;
            playerName = inputField.GetComponent<TMPro.TMP_InputField>().text;
            SceneManager.LoadScene(7);
            Debug.Log(playerName);
            WriteName();
        }
    }
    public static void WriteName()
   {
       string path = Application.persistentDataPath + "/Name.txt";
       //Write some text to the test.txt file
       StreamWriter writer = new StreamWriter(path, true);

       //Insert code to save name to WriteLine

       //write name, then continue this code

		/*while (playerName != null) 
		{
            if (Input.GetKeyDown("return"))
            {
                //playerName = inputBox.text;
                SceneManager.LoadScene(0);
                Debug.Log(playerName);
            }

        }*/
       writer.WriteLine(playerName);
       writer.Close();
       StreamReader reader = new StreamReader(path);
       //Print the text from the file
       Debug.Log(reader.ReadToEnd());
       reader.Close();
       
    }
}
