using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

public class MapCreate : MonoBehaviour {


	private ImageMap map;

	// Generated Fields
	private float horizontalScale;
	private float verticalScale;
	private Vector3 adjust;

	// Editor Fields
	[Header("Function")]
	[SerializeField] private Transform playContainer;
	[SerializeField] private float allScale = 10f;

	[Header("Aesthetic")]
	[SerializeField] private Material lineMaterial;

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

        adjust = new Vector3(-horizontalScale * allScale / 2f, 0, verticalScale * allScale / 2f);

       // Line spawnLine = map.Lines.First(l => l.Color == MapColor.Blue);
        //player.transform.position = PointToWorldSpace(spawnLine.AveragePoint()) + Vector3.up * 0.5f;
        // TODO: Scaling player / map appropriately
    }

    private void GenerateLevel(){
        SetConstants();
		MakeTrack();
		//MakeFloor ();
	}
	/*
	// Update is called once per frame
	void Update () {
		
	}
	*/
	private void MakeFloor()
	{
		GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
		floor.transform.parent = playContainer;
		floor.transform.position = Vector3.zero;
		floor.transform.localScale = new Vector3(horizontalScale * allScale / 10f, 1, verticalScale * allScale / 10f);
	}


    private Vector3 PointToWorldSpace(Point p)
    {
        return new Vector3(p.X * horizontalScale * allScale, 0f, -p.Y * verticalScale * allScale) + adjust;
    }

    private void MakeTrack(){


        foreach (Line line in map.Lines) {


            GameObject track = new GameObject("Track");//NEED TO GIVE FILTER ETC
            Mesh mesh = new Mesh();
            MeshFilter mf = track.AddComponent<MeshFilter>();
            mf.sharedMesh = mesh;
            MeshRenderer mr = track.AddComponent<MeshRenderer>();
            mr.material = lineMaterial;

            track.transform.parent = playContainer;
            track.transform.position = Vector3.zero;



            Vector3[] vertices = new Vector3[line.Points.Length * 2];
			int[] triangles = new int[(vertices.Length-2)*6];



			Point[] point = line.Points;

            //CAN TAKE A POINT, TURN IT TO A VECTOR, FIGURE OUT DX DY, THEN NORMALISE IT

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
            
            vertices[0] = vp1 + dr*.5f;
            vertices[1] =vp1 - dr*.5f;

			//create remaining vertices
			for (int i = 0; i < point.Length-1; i++) {
				Point p1 = point [i];
				Point p2 = point [i + 1];
				float dx = p2.X - p1.X;
				float dy = p2.Y - p1.Y;

                Point derivative = new Point(-dy, dx);
                Vector3 deriv = PointToWorldSpace(derivative);
                deriv = deriv.normalized;

                Vector3 point1 = PointToWorldSpace(p1);
                Vector3 point2 = PointToWorldSpace(p2);
                /*
				Point np1 = new Point (p2.X-dx, p2.Y+dy);
				Point np2 = new Point (p2.X-dx, p2.X-dy);
                */

                // //Vector3 v1 = new Vector3 (np1.X*3f, 3f, np1.Y*3f);
                //Vector3 v2 = new Vector3 (np2.X*3f,3f, np1.Y*3f);
                Vector3 fv1 = point2 + deriv*.5f;//PointToWorldSpace(np1);
                Vector3 fv2 = point2 - deriv*.5f;//PointToWorldSpace(np2);
                vertices [i*2+2] = fv1;
				vertices [i*2+3] = fv2;
			
			}



			//create triangles
			for(int j = 0; j < triangles.Length; j+=12){
				int start = j / 6;

				triangles[j] = start;
				triangles [j + 1] = start + 1;
				triangles [j + 2] = start + 2;

				triangles [j + 3] = start + 2;
				triangles [j + 4] = start + 1;
				triangles [j + 5] = start + 3;

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
}
