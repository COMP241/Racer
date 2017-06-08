using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class portalTrigger : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        //gameController.Respawn();
        if (MapCreate2.checkpointCount == 0) gameController.Lap();
        else gameController.Complete(); // Could add new canvas and failure indication
    }

}
