using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCountdown : MonoBehaviour
{
    public Player player;
    private Rigidbody rb;
    public void SetCountedDown(){
        player = GameObject.Find("Car").GetComponent<Player>();
        rb = player.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        gameController.EnableCountedDown();
    }
}
 