using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject sprite;
    [SerializeField] private GameObject player;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        sprite.transform.position = player.transform.position;
        sprite.transform.position = new Vector3(sprite.transform.position.x, player.transform.position.y+20, sprite.transform.position.z);
        
    }
}
