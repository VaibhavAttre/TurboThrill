using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadWorldMenu : MonoBehaviour
{
    [SerializeField] private GameObject inputField;
    [SerializeField] private GameObject dropDown;
    [SerializeField] private GameObject back;

    GameObject create;
    GameObject load;
    GameObject input;
    GameObject drop;
    GameObject backButton;

    public void Awake()
    {
        back.SetActive(false);

    }
    public void onLoad()
    {
        create = GameObject.Find("Create");
        create.SetActive(false);
        GameObject load = GameObject.Find("Load");
        back.SetActive(true);
        load.SetActive(false);
        input = Instantiate(dropDown, this.transform);
    }

    public void onCreate()
    {
        load = GameObject.Find("Load");
        load.SetActive(false);
        GameObject create = GameObject.Find("Create");
        create.SetActive(false);
        //back.SetActive(true);
        drop = Instantiate(inputField, this.transform);
    }

    public void goBack()
    {
        //back.SetActive(false);
        create.SetActive(true);
        load.SetActive(true);
        input.SetActive(false);
        drop.SetActive(false);
    }
}
