using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeHealth : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject healthbar;
    public GameObject bullet;
    public GameObject tree;
    void Start()
    {
        healthbar.SetActive(false);   
    }

    void Update()
    {
        if(this.GetComponent<Health>().GetHealth() <= 0) {
            Destroy(tree);
        }
    }



    public void Hit()
    {
        healthbar.SetActive(true);
    }

}
