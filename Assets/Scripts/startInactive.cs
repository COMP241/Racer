using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class startInactive : MonoBehaviour {

    private void Awake()
    {
        gameObject.SetActive(false);
    }
}
