using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SubmitScore : MonoBehaviour {

	private string name = null;
	[SerializeField] private Text text;
	private string game = "race";

	public void SetName(string name)
	{
		this.name = name;
	}
	public void submitScore()
	{
		if (!string.IsNullOrEmpty(name))
		{
			text.text = "Submitted";
			StartCoroutine(SendPost());
		}
	}
	private IEnumerator SendPost()
	{
		using (UnityWebRequest www = UnityWebRequest.Post("http://score.papertopixels.tk/a.php", new Dictionary<string, string>
			{
				{  "game", game },
				{  "user", name },
				{  "score", FinishRace.mapTime.ToString()},
				{  "mapid", MapCreate2.mapid.ToString()}

			} ))
		{
			yield return www.Send();
		}
	}
}

