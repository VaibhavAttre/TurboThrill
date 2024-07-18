using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Cinemachine;

public class Menu : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera mainCamera;
    [SerializeField] private CinemachineVirtualCamera garageCamera;
    private void OnEnable()
    {
        if (mainCamera != null || garageCamera != null)
        {
            CameraSwitcher.Register(mainCamera);
            CameraSwitcher.Register(garageCamera);
            CameraSwitcher.SwitchCamera(mainCamera);
        }

    }

    private void OnDisable()
    {
        if(mainCamera != null || garageCamera != null)
        {
            CameraSwitcher.Unregister(mainCamera);
            CameraSwitcher.Unregister(garageCamera);    
        }

    }
    
    //Scene 0 is home, 1 is game, 2 is instructions, 3 is leaderboard, 4 is credits
    public void goHome()
    {
        SceneManager.LoadScene(0);
    }
    public void onPlay()
    {
        if (CameraSwitcher.IsActiveCamera(garageCamera))
        {
            CameraSwitcher.SwitchCamera(mainCamera);
        }
        else if (CameraSwitcher.IsActiveCamera(mainCamera))
        {
            CameraSwitcher.SwitchCamera(garageCamera);
        }
        Invoke("SwitchScenes", 3);
    }

    public void onDrive()
    {
        StartCoroutine(DriveSequence());
    }

    IEnumerator DriveSequence()
    {
        DriveAnimation();
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene(6);
    }

    void DriveAnimation()
    {
        Animator anim = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
        anim.Play("DriveOut");
    }
    private void SwitchScenes()
    {
        SceneManager.LoadScene(1);
    }
    public void onInstructions()
    {
        SceneManager.LoadScene(3);
    }
    public void onLeaderboard()
    {
        SceneManager.LoadScene(4);
    }
    public void onCredits()
    {
        SceneManager.LoadScene(5);
    }

}
