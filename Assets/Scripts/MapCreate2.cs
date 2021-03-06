﻿﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

public class MapCreate2 : MonoBehaviour
{
    private static MapCreate2 instance;

    private ImageMap map;
    private bool loading;
    private static AugLine[] aLines;
    private static CheckPoint[] cPointArray;

    public static int checkpointCount;
	public static int mapid;

    // Generated Fields
    private static float horizontalScale;
    private static float verticalScale;
    //private Vector3 adjust;
    private static float width;
    private static Line trackLine;






    // Editor Fields
    [Header("Function")]

    [SerializeField]
    private GameObject playContainer;
    [SerializeField] 
    private static float allScale = 40f;
    [SerializeField]
    private float setwidth = 1;
    [SerializeField]
    private GameObject carObject;
    [SerializeField]
    private GameObject idcanvas;
    [SerializeField]
    private static Transform levelContainer;


    [Header("Aesthetic")]
    [SerializeField]
    private Material lineMaterial;

    private void Start()
    {
        if (instance == null) instance = GetComponent<MapCreate2>();
        else Destroy(gameObject);
        SetInactive();
    }

    public static void Load(string id)
    {
        if (instance.loading) return;

        try{
            instance.StartCoroutine(instance.LoadLevel(int.Parse(id)));
        }
        catch (FormatException)
        {
            
        }
    }


    private IEnumerator LoadLevel(int id)
    {
        loading = true;
        using (UnityWebRequest www = UnityWebRequest.Get("http://papermap.tk/api/map/" + id))
        {
            yield return www.Send();

            if (!www.isError)
            {
                map = ImageMap.FromJson(www.downloadHandler.text);
                GenerateLevel();
            }
            else
            {
                Debug.Log(www.error);
            }
        }
        loading = false;
    }

    private void SetConstants()
    {
		mapid = map.Id;
        if (map.Ratio >= 1f)
        {
            horizontalScale = map.Ratio;
            verticalScale = 1f;
        }
        else
        {
            horizontalScale = 1f;
            verticalScale = 1f / map.Ratio;
        }


        width = setwidth*1f;

		trackLine = map.Lines.First(l => l.Color == MapColor.Black);

        gameController.SetSpawnPoint(PointToWorldSpace(trackLine.Points[0]));




    }

    public static void SetCheckpoints(){
        checkpointCount = cPointArray.Length;
        checkpointCount *= 3;
    }


    private void SetAugLines(){
		//Set up augLines
		aLines = new AugLine[trackLine.Points.Length - 1];
        Vector3[] worldPoint = new Vector3[trackLine.Points.Length];
        for (int z = 0; z < trackLine.Points.Length - 1; z++){
            worldPoint[z] = PointToWorldSpace(trackLine.Points[z]);
        }

        for (int i = 0; i < trackLine.Points.Length - 1; i++)
        {
            aLines[i] = new AugLine(worldPoint[i], worldPoint[i + 1]);
        }

        //TODO: NEED TO FIX SPAWN
        Quaternion rt = Quaternion.FromToRotation(Vector3.forward, aLines[0].normPerp);
        rt = rt *= Quaternion.Euler(0, 90, 0);
		gameController.SetRotation(rt);
    }

    private void GenerateLevel()
    {
        SetConstants();
        SetAugLines();
		MakeFloor();
        MakeTrack();
        CreatePortal();
        CreateCheckPoints();
        SetCheckpoints();
		Spawn();
        playContainer.gameObject.SetActive(true);
        MainOverlay.SetActive();
        idcanvas.SetActive(false);
        CountdownCanvasScript.SetActive();
       



    }


    private void ScaleMap(GameObject go, float f){
        go.transform.localScale += new Vector3(f, 0, f);
    }



    private void MakeFloor()
    {
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.transform.parent = levelContainer;
        floor.transform.position = Vector3.zero;
        floor.transform.localScale = new Vector3(horizontalScale * allScale / 10f, 1, verticalScale * allScale / 10f);
        Renderer rn = floor.GetComponent<Renderer>();
        Material grass = (Material)Resources.Load(("Materials/SquareGrass"));
        rn.material = grass;
        Rigidbody rbd = floor.GetComponent<Rigidbody>();
        MeshCollider mc = floor.GetComponent<MeshCollider>();
        PhysicMaterial physMat = new PhysicMaterial();
        physMat.dynamicFriction = 0f;
        physMat.staticFriction = 0f;
        mc.material = physMat;
		ScaleMap(floor, 10);
 

    }

    private void Spawn()
    {
       
        //Vector3 startPos = PointToWorldSpace(trackLine.Points[0]);
        gameController.Respawn();
    }


    private void CreatePortal(){
        GameObject portal = (GameObject)Resources.Load("Models/FinishLine");
        Vector3 position = PointToWorldSpace(trackLine.Points[trackLine.Points.Length-1]);
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, aLines[aLines.Length - 1].normPerp);
        //rotation *= Quaternion.Euler(0, 90, 0);
        portal = Instantiate(portal, position, rotation); 
        portal.transform.parent = levelContainer;


    }

    private void CreateCheckPoints()
    {
        if (aLines.Length < 12) {
			cPointArray = new CheckPoint[1];
			for (int i = 0; i < 1; i++)
			{
				/*
                GameObject cp = (GameObject)Resources.Load(("Models/CheckPoint"));
                Vector3 position = PointToWorldSpace(trackLine.Points[i]);
                cp = Instantiate(cp, position, Quaternion.FromToRotation(Vector3.up, aLines[i].normPerp));
                cp.transform.parent = levelContainer;
                */
				CheckPoint cP = new CheckPoint(trackLine, aLines.Length / 2 * i + 1, i);

				cPointArray[i] = cP;
			}

        }
        else
        {
            //int scale = aLines.Length / 5;
            cPointArray = new CheckPoint[5];
            for (int i = 0; i < 5; i++)
            {
                /*
                GameObject cp = (GameObject)Resources.Load(("Models/CheckPoint"));
                Vector3 position = PointToWorldSpace(trackLine.Points[i]);
                cp = Instantiate(cp, position, Quaternion.FromToRotation(Vector3.up, aLines[i].normPerp));
                cp.transform.parent = levelContainer;
                */
                CheckPoint cP = new CheckPoint(trackLine, aLines.Length / 5 * i + 2, i);
                cPointArray[i] = cP;
            }
        }
        /*
        else
        {
            int scale = aLines.Length / 1;
            cPointArray = new CheckPoint[3];
            for (int i = 0; i < 3; i++)
            {
                /*
                GameObject cp = (GameObject)Resources.Load(("Models/CheckPoint"));
                Vector3 position = PointToWorldSpace(trackLine.Points[i]);
                cp = Instantiate(cp, position, Quaternion.FromToRotation(Vector3.up, aLines[i].normPerp));
                cp.transform.parent = levelContainer;

                CheckPoint cP = new CheckPoint(trackLine, i * scale + 2);
                cPointArray[i] = cP;
            }
        }
        */
    }

    private static Vector3 PointToWorldSpace(Point p)
    {
        return new Vector3(p.X * horizontalScale * allScale, 0f, -p.Y * verticalScale * allScale);
    }

    private void MakeTrack()
	{
		GameObject track = new GameObject("Track");
		Mesh mesh = new Mesh();
		MeshFilter mf = track.AddComponent<MeshFilter>();
		mf.sharedMesh = mesh;
		MeshRenderer mr = track.AddComponent<MeshRenderer>();
		Material mat = (Material)Resources.Load("Materials/SquareRoad");
		mr.material = mat;
        MeshCollider mc = track.AddComponent<MeshCollider>();
        PhysicMaterial pMat = new PhysicMaterial();
        pMat.dynamicFriction = 1.0f;
        pMat.staticFriction = 1.0f;
        mc.material = pMat;

		track.transform.parent = levelContainer;
		track.transform.position = track.transform.position + Vector3.up * 0.001f;

		Vector3[] tempvert = new Vector3[(trackLine.Points.Length * 4) - 4];
		Vector3[] vertices = new Vector3[trackLine.Points.Length * 2];
		int[] triangles = new int[(vertices.Length - 2) * 6];

		Point[] point = trackLine.Points;

		for (int i = 0; i < point.Length - 1; i++)
            {
                Point p1 = point[i];
                Point p2 = point[i + 1];
                float dx = p2.X - p1.X;
                float dy = p2.Y - p1.Y;




                Point derivative = new Point(-dy, dx);
                Vector3 deriv = PointToWorldSpace(derivative);
                deriv = deriv.normalized;

                Vector3 point1 = PointToWorldSpace(p1);
                Vector3 point2 = PointToWorldSpace(p2);

                Vector3 n = point2 - point1;
                Vector3 norm = new Vector3(-n.z, n.y, n.x);
                norm = norm.normalized;


                Vector3 fv1 = point2 + norm * width;
                Vector3 fv2 = point2 - norm * width;
                Vector3 fv3 = point1 + norm * width;
                Vector3 fv4 = point1 - norm * width;


				tempvert[i * 4 + 0] = fv3;
                tempvert[i * 4 + 1] = fv4;
                tempvert[i * 4 + 2] = fv1;
                tempvert[i * 4 + 3] = fv2;



            }

        /*
        for (int i = 0; i < aLines.Length - 1; i++){
            tempvert[i * 4 + 0] = aLines[0].verticeOne;
			tempvert[i * 4 + 1] = aLines[0].verticeTwo; 
            tempvert[i * 4 + 2] = aLines[0].verticeThree;
			tempvert[i * 4 + 3] = aLines[0].verticeFour;
        }
*/

            //straight copy over first points
            vertices[0] = tempvert[0];
            vertices[1] = tempvert[1];

            for (int k = 0; k < point.Length - 2; k++)
            {
                //take k and k+ 2 and merge
                Vector3 kp = tempvert[k*4 + 2];
                Vector3 kp2 = tempvert[k*4 + 4];

                Vector3 a = new Vector3((kp.x + kp2.x) / 2, 0f, (kp.z + kp2.z) / 2);

                //take k+1 and k+3 and merge
                Vector3 kp1 = tempvert[k * 4 + 3];
                Vector3 kp3 = tempvert[k * 4 + 5];

                Vector3 b = new Vector3((kp1.x + kp3.x) / 2, 0f, (kp1.z + kp3.z) / 2);
				
				vertices[k * 2 + 2] = a;
				vertices[k * 2 + 3] = b;
                
			}


            vertices[vertices.Length - 2] = tempvert[tempvert.Length - 2];
            vertices[vertices.Length - 1] = tempvert[tempvert.Length - 1];


            //Loop to create conatant width on corners
            /*
            for (int p = 0; p < point.Length - 2; p++)
            {
                Point tempp = point[p + 1];
                Vector3 temp = vertices[p * 2 + 2];
                Vector3 temp2 = vertices[p * 2 + 3];
                Vector3 pnt = PointToWorldSpace(point[p + 1]);
                temp = temp-pnt;
                temp2 = temp2-pnt;

                temp = pnt + temp.normalized * width;
                temp2 = pnt + temp.normalized * width;

                vertices[p * 2 + 2] = temp;
                vertices[p * 2 + 2] = temp2;
               
            }
            
            */
            //create triangles
                for (int j = 0; j < triangles.Length; j += 12)
                {
                    int start = j / 6;

                    triangles[j] = start;
                    triangles[j + 1] = start + 1;
                    triangles[j + 2] = start + 2;

                    triangles[j + 3] = start + 2;
                    triangles[j + 4] = start + 1;
                    triangles[j + 5] = start + 3;

                    triangles[j + 6] = triangles[j + 2];
                    triangles[j + 7] = triangles[j + 1];
                    triangles[j + 8] = triangles[j];

                    triangles[j + 9] = triangles[j + 5];
                    triangles[j + 10] = triangles[j + 4];
                    triangles[j + 11] = triangles[j + 3];

                }

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i++){
            uvs[i] = new Vector2(vertices[i].x, vertices[i].y);
        }


        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
    }

    public class CheckPoint{
        GameObject model;
        Vector3 position;
        int objectName;

        public CheckPoint(Line line, int index, int i){
            objectName = i;
			GameObject cp = (GameObject)Resources.Load(("Models/CheckpointLine"));
            Vector3 pos = aLines[index].pointOne + new Vector3(0f, 0.002f, 0f);//PointToWorldSpace(line.Points[index]);
            Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, aLines[index].normPerp);
            rotation *= Quaternion.Euler(0, 90, 0);
			cp = Instantiate(cp, pos, rotation);
            cp.name = "CheckPoint" + i;
			cp.transform.parent = levelContainer;

		}

    }


    public class AugLine{
        public Vector3 verticeOne;
        public Vector3 verticeTwo;
        public Vector3 verticeThree;
        public Vector3 verticeFour;
        public Vector3 pointOne;
        public Vector3 pointTwo;
        public Vector3 normPerp;

        public AugLine (Vector3 point1, Vector3 point2){
            pointOne = point1;
            pointTwo = point2;
            Vector3 p1p2 = point2 - point1;
            normPerp = new Vector3(-p1p2.z, p1p2.y, p1p2.x);
            normPerp = normPerp.normalized;
            verticeOne = pointOne + normPerp * width;
            verticeTwo = pointOne - normPerp * width;
            verticeThree = pointTwo + normPerp * width;
            verticeFour = pointTwo - normPerp * width;
        }
    }
    /*
    //PASS augLine.normPerp to this method to get the 
    //quaternion for the rotation
    private Quaternion vectorToQuaternion (Vector3 v){
        Vector3 s;
        Vector3 d;
        Vector3 u;
        Vector3 matrix;
        s = Vector3.Cross(Vector3.up, v);
        d = v;
        u = Vector3.Cross(d, s);
        matrix = new Vector3(d, u, s);
        Quaternion q = Quaternion.Eular(matrix);
    }
    */


    public static void SetInactive(){
        instance.playContainer.SetActive(false);
    }
    public static void SetActive(){
        instance.playContainer.SetActive(true);
        
    }
}

