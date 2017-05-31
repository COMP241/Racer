using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

public class MapCreate2 : MonoBehaviour
{


    private ImageMap map;

    // Generated Fields
    private float horizontalScale;
    private float verticalScale;
    private Vector3 adjust;
    private float width;
    private Line trackLine;





    // Editor Fields
    [Header("Function")]
    [SerializeField]
    private Transform playContainer;
    [SerializeField] private float allScale = 10f;
    [SerializeField]
    private float setwidth = 1;
    [SerializeField]
    private GameObject carObject;

    [Header("Aesthetic")]
    [SerializeField]
    private Material lineMaterial;


    private IEnumerator Start()
    {
        int id = 2;
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
    }

    private void SetConstants()
    {
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

        //adjust = new Vector3(-horizontalScale * allScale / 2f, 0, verticalScale * allScale / 2f);

        width = setwidth*.2f;

		trackLine = map.Lines.First(l => l.Color == MapColor.Black);

    }

    private void GenerateLevel()
    {
        SetConstants();
		MakeFloor();
        MakeTrack();
        Spawn();
        CreatePortal(); 


 
    }

    private void ScaleMap(GameObject go, float f){

        go.transform.localScale += new Vector3(f, 0, f);
    }

    private void MakeFloor()
    {
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.transform.parent = playContainer;
        floor.transform.position = Vector3.zero;
        floor.transform.localScale = new Vector3(horizontalScale * allScale / 10f, 1, verticalScale * allScale / 10f);
        Renderer rn = floor.GetComponent<Renderer>();
        Material grass = (Material)Resources.Load(("Grass"));
        rn.material = grass;
        Rigidbody rbd = floor.GetComponent<Rigidbody>();
		ScaleMap(floor, 10);
        floor.transform.parent = playContainer;

        
    }


    private void Spawn()
    {
        Vector3 spawnPoint;
        spawnPoint = Vector3.zero;//(PointToWorldSpace(map.Lines[0].Points[1]) + PointToWorldSpace(map.Lines[0].Points[1]) / 4) + PointToWorldSpace(map.Lines[0].Points[0]);
        //SET PLAYER TO GO TO SPAWN POINT
        Vector3 startPos = PointToWorldSpace(trackLine.Points[0]);
        GameObject car = Instantiate(carObject, startPos, Quaternion.Euler(90, 90, 90));
    }


    private void CreatePortal(){
        GameObject portal = (GameObject)Resources.Load("Models/Portal");
        Vector3 position = PointToWorldSpace(trackLine.Points[trackLine.Points.Length-1]);//vertices[vertices.Length - 1] + vertices[vertices.Length - 2]/2;
        portal = Instantiate(portal, position, Quaternion.Euler(90, 90, 90)); //NEED TO GET D VEC FROM 2 VERT//+= new Vector3(4f, 0, 4f), Quaternion.Euler(90, 90, 90));
        portal.transform.parent = playContainer;


    }

    private Vector3 PointToWorldSpace(Point p)
    {
        return new Vector3(p.X * horizontalScale * allScale * 4f, 0f, -p.Y * verticalScale * allScale * 4f);// + adjust;
    }

    private void MakeTrack()
    {

        //Change so only takes black lines



            
            GameObject track = new GameObject("Track");
            Mesh mesh = new Mesh();
            MeshFilter mf = track.AddComponent<MeshFilter>();
            mf.sharedMesh = mesh;
            MeshRenderer mr = track.AddComponent<MeshRenderer>();
            Material mat = (Material)Resources.Load("asphalt");
            mr.material = mat;


            //ScaleMap(track, 4);
            //track.transform.position = new Vector3(0, 0, 0);
            track.transform.parent = playContainer;
            track.transform.position = track.transform.position + Vector3.up * 0.001f;

			Vector3[] tempvert = new Vector3[(trackLine.Points.Length * 4) - 4];
			Vector3[] vertices = new Vector3[trackLine.Points.Length * 2];
			int[] triangles = new int[(vertices.Length - 2) * 6];

            /*
			tempvert = new Vector3[(line.Points.Length * 4) - 4];
            vertices = new Vector3[line.Points.Length * 2];
            triangles = new int[(vertices.Length - 2) * 6];
            */


            Point[] point = trackLine.Points;

            

            //CAN TAKE A POINT, TURN IT TO A VECTOR, FIGURE OUT DX DY, THEN NORMALISE IT

            /*
            //SET UP FIRST TWO VERTICES FROM FIRST POINT
            Point v1 = point[0];
            Point v2 = point[1];
            Vector3 vp1 = PointToWorldSpace(v1);
            Vector3 vp2 = PointToWorldSpace(v2);
            float derx = v2.X - v1.X;
            float dery = v2.Y - v1.Y;
            Point der = new Point(-dery, derx);
            Vector3 dr = PointToWorldSpace(der);
            dr = dr.normalized;

            vertices[0] = vp1 + dr * .5f;
            vertices[1] = vp1 - dr * .5f;
            */
            //create remaining vertices


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


                fv1 = fv1 + new Vector3(1f, 0, 1f);
                fv2 = fv2 + new Vector3(1f, 0, 1f);
                fv3 = fv3 + new Vector3(1f, 0, 1f);
                fv4 = fv4 + new Vector3(1f, 0, 1f);


				tempvert[i * 4 + 0] = fv3;
                tempvert[i * 4 + 1] = fv4;
                tempvert[i * 4 + 2] = fv1;
                tempvert[i * 4 + 3] = fv2;



            }

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

            //straight copy over last points
            vertices[vertices.Length - 2] = tempvert[tempvert.Length - 2];
            vertices[vertices.Length - 1] = tempvert[tempvert.Length - 1];


            //Loop to create contant width on corners
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



            mesh.vertices = vertices;
            mesh.triangles = triangles;





    }
}

