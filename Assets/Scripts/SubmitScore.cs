using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SubmitScore : MonoBehaviour {

	private string name = null;
	[SerializeField] private Text text;
	private string game = "tilt";

	public void SetName(string name)
	{
		Debug.Log("setName");
		this.name = name;
	}
	public void submitScore()
	{
		Debug.Log("submitScore");
		if (!string.IsNullOrEmpty(name))
		{
			text.text = "Submitted";
			StartCoroutine(SendPost());
		}
	}
	private IEnumerator SendPost()
	{

		Debug.Log("sendpost");
		Debug.Log (game);
		Debug.Log (name);
		Debug.Log (MapCreate2.mapid.ToString ());
		Debug.Log (FinishRace.mapTime.ToString("##.00"));
		using (UnityWebRequest www = UnityWebRequest.Post("http://103.208.86.184/a.php", new Dictionary<string, string>
			{
				{  "game", game },
				{  "user", name },
				{  "score", MapCreate2.mapid.ToString("##.00") },
				{  "mapid", FinishRace.mapTime.ToString() }
			} ))
		{
			yield return www.Send();
			Debug.Log (www.responseCode);
		}
	}
}

