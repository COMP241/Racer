using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinishRace : MonoBehaviour {

    private static FinishRace instance;
	public static float mapTime;

    [SerializeField] private Text timeText;

	// Use this for initialization
	void Start () {
        if (instance == null) instance = GetComponent<FinishRace>();
        else Destroy(gameObject);
        SetInactive();
	}

    public static void SetFinishTime(float s){
		mapTime = s;
		instance.timeText.text = s.ToString("0.00");

	}

    public static void SetInactive(){
        instance.gameObject.SetActive(false);
    }

    public static void SetActive(){
        instance.gameObject.SetActive(true);
    }
}
