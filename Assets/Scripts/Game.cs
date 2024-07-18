using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public float score;
    public float time;

    [SerializeField] private TMPro.TextMeshProUGUI scoreText;
    [SerializeField] private TMPro.TextMeshProUGUI timeText;

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        scoreText.text = "Score: " + getScore();
        timeText.text = "Time: " + getMinute() + ":" + getSecond();
    }
    public void incScore(float num){
        score += num;
    }
    public float getScore(){
        return score;
    }
    public string getMinute(){
        return (int)(time/60) + "";
    }
    public string getSecond(){
        string ans = "";
        int seconds = (int)(time % 60);
        if(seconds < 10){
            ans = "0";
        }
        ans += (int)(time % 60) + "";
        return ans;
    }
}
