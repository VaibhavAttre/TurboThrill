using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Leaderboard : MonoBehaviour
{
    // Start is called before the first frame update
    public static float[] scoreTime;
    public static string[] scoreName;
    [SerializeField] private TMPro.TextMeshProUGUI score1;
    [SerializeField] private TMPro.TextMeshProUGUI score2;
    [SerializeField] private TMPro.TextMeshProUGUI score3;
    [SerializeField] private TMPro.TextMeshProUGUI score4;
    [SerializeField] private TMPro.TextMeshProUGUI score5;
    void Start()
    {
        scoreTime = new float[5];
        scoreName = new string[5];
        ReadString();
        Debug.Log(scoreName[0]);
        score1.text = scoreName[0] + " " + convertTime(scoreTime[0]);
        score2.text = scoreName[1] + " " + convertTime(scoreTime[1]);
        score3.text = scoreName[2] + " " + convertTime(scoreTime[2]);
        score4.text = scoreName[3] + " " + convertTime(scoreTime[3]);
        score5.text = scoreName[4] + " " + convertTime(scoreTime[4]);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public static void ReadString()
   {
       string path = Application.persistentDataPath + "/Name.txt";
        Debug.Log(path);
       string path2 = Application.persistentDataPath + "/Time.txt";
       //Read the text from directly from the test.txt file
       StreamReader reader = new StreamReader(path);
       StreamReader reader2 = new StreamReader(path2);
       //Debug.Log(reader.ReadToEnd());
       string line = reader.ReadLine();
       string line2 = reader2.ReadLine();
       string[] temp = new string[2];
       do{
            temp[0] = line;
            temp[1] = line2;
        int index = 0;
        float val = float.Parse(temp[1]);
            do
            {
                if (scoreName[index] == null)
                {
                    scoreName[index] = temp[0];
                    scoreTime[index] = val;
                    index = 5;
                }
                else if (val < scoreTime[index])
                {
                    updateList(index);
                    scoreName[index] = temp[0];
                    scoreTime[index] = val;
                    index = 5;
                }
                else
                {
                    index += 1;
                }
            }
            while (index < 5);
        line = reader.ReadLine();
        line2 = reader2.ReadLine();
       }
       while(line != null);
       reader.Close();
   }
   //Pushes everything over
   public static void updateList(int index){
        for(int i = 3; i > index - 1; i--){
            scoreTime[i + 1] = scoreTime[i];
            scoreName[i + 1] = scoreName[i];
        }
   }
   public static string convertTime(float num)
    {
        string temp = "";
        if ((num % 60) < 10)
        {
            temp = "" + (int)(num / 60) + ":";
            temp += "0" + (int)(num % 60);
        }
        else
        {
            temp = "" + (int)(num / 60) + ":" + (int)(num % 60);
        }
        return temp;
    }
}
