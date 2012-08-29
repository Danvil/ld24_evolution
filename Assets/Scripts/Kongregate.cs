using UnityEngine;
using System.Collections;

public class Kongregate : MonoBehaviour {

	void Awake() {
		Globals.Kongregate = this;
		Application.ExternalEval(
			"if(typeof(kongregateUnitySupport) != 'undefined') {" +
			" kongregateUnitySupport.initAPI('Kongregate', 'OnKongregateAPILoaded');" +
			"};"
		);
	}

	bool isKongregate = false;
	int userId = 0;
	string username = "Guest";
	string gameAuthToken = "";

	void OnKongregateAPILoaded(string userInfoString) {
		try {
			// Split the user info up into tokens
			var p = userInfoString.Split('|');
			userId = int.Parse(p[0]);
			username = p[1];
			gameAuthToken = p[2];
			// We now know we're on Kongregate
			isKongregate = true;
		}
		catch {
			isKongregate = false;
		}
	}

	public void Gameover(int time_survived) {
		if(isKongregate) {
			Application.ExternalCall("kongregate.stats.submit", "Time survived", time_survived);
		}
	}

	void OnGUI() {
		GUI.Box(new Rect(0, 0, 100, 20), isKongregate.ToString());
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
