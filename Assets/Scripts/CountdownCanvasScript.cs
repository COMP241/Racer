using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownCanvasScript : MonoBehaviour
{

	private static CountdownCanvasScript instance;

	// Use this for initialization
	void Start()
	{
		if (instance == null) instance = GetComponent<CountdownCanvasScript>();
		else Destroy(gameObject);
		SetInactive();
	}

	public static void SetInactive()
	{
		instance.gameObject.SetActive(false);
	}

	public static void SetActive()
	{
		instance.gameObject.SetActive(true);

	}

	public static void SetCountedDown()
	{
        gameController.EnableCountedDown();
	}
}
