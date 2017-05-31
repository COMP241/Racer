using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class portalTrigger : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        gameController.Respawn();
    }

}
