using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectLevel : MonoBehaviour {

    private string mapID;
    [SerializeField] private Text text;

    public void SetMapId(string id){
        this.mapID = id;
    }

    public void StartMap(){
        MapCreate2.Load(mapID);
    }

}
