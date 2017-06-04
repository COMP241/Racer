using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainOverlay : MonoBehaviour {
    private static MainOverlay instance;

    [SerializeField]private Text timeText;

	// Use this for initialization
	void Start () {
        if (instance == null) instance = GetComponent<MainOverlay>();
        else Destroy(gameObject);
        SetInactive();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static void SetTime(float seconds){
        instance.timeText.text = seconds.ToString("0.00");
    }

    public static void SetInactive(){
        instance.gameObject.SetActive(false);
    }
    public static void SetActive(){
        instance.gameObject.SetActive(true);
    }
}
