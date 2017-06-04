using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameController : MonoBehaviour {

    private static gameController instance;


    [SerializeField]
    private GameObject player;
    private static Vector3 spawnPoint;
    private static Quaternion rotation;
    private static float time = 0f;
    private static bool activeTimer = false;

	// Use this for initialization
	void Start () {
		if (instance == null)
			instance = GetComponent<gameController>();
		else
			Destroy(gameObject);
	}

    // Update is called once per frame
    void Update()
    {
        if (activeTimer) {
            time += Time.deltaTime;
            MainOverlay.SetTime(time);
    }
	}

    public static void SetSpawnPoint(Vector3 p){
        spawnPoint = p;
    }

    public static void SetRotation(Quaternion q){
        rotation = q;
    }

    public static void Respawn(){
        instance.player.transform.position = spawnPoint;
        instance.player.transform.rotation = rotation;
        activeTimer = true;
    }

    public static void Lap(){
        activeTimer = false;

    }


}
