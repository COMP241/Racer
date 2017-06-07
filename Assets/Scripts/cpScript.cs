using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cpScript : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
        //Debug.Log(MapCreate2.checkpointCount.ToString());
        MapCreate2.checkpointCount = MapCreate2.checkpointCount - 1;
	}
}
