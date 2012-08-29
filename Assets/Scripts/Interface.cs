using UnityEngine;
using System.Collections;

public class Interface : MonoBehaviour {

	bool isStartup = true;

	void Awake() {
		MyTime.Pause = true;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}

	public GUIStyle myStyle = new GUIStyle();

	void OnGUI()
	{
		if(isStartup) {
			int w = Screen.width / 2;
			int h = 200;
			GUI.Box(new Rect(Screen.width/2 - w/2, Screen.height/2 - h/2 - 50, w, 30),
				"EVOLUTION HATES YOU"
				//,myStyle
			);
			GUI.Box(new Rect(Screen.width/2 - w/2, Screen.height/2 - h/2, w, h),
				"\n"+
				"Try to survive as long as possible.\n"+
				"Kill plants and animals and collect green bubbles to regain health.\n"+
				"Avoid poisonous red bubbles!\n"+
				"Beware, the environment evolves!\n"+
				"If you are careless everyone will hate you.\n"+
				"\n"+
				"*** Controls ***\n"+
				"Movement: point with mouse\n"+
				"Attack: left mouse button or space\n"+
				"Sprint: shift key"
				//,myStyle
			);
			w = 100;
			bool pressed = GUI.Button(new Rect(Screen.width/2 - w/2, Screen.height/2 + h/2 + 20, w, 30),
				"Start Game"
				//,myStyle
			);
			if(pressed) {
				isStartup = false;
				MyTime.Pause = false;
				Globals.startTime = MyTime.time;
			}
		}

		if(Globals.Player.GetComponent<PlayerInteract>().IsDead) {
			int w = Screen.width / 2;
			int h = 200;
			GUI.Box(new Rect(Screen.width/2 - w/2, Screen.height/2 - h/2, w, h),
				string.Format("\nGame Over!\n\n"+
				"The world evolved to fast for you ...\n"+
				"\n"+
				"You only survived for {0:D} seconds.", Globals.Player.GetComponent<PlayerInteract>().TimeSurvived)
				//,myStyle
			);
		}
		
		string darwinSays = string.Format(
			"Darwin says:\n"+
			"Number of individuals: {0}\n"+
			"Number of yummy species: {1}\n"+
			"Number of poisonous species: {2}\n"+
			"Strongest species: {3}\n",
			Globals.BlobManager.BlobGenotypes.Length,
			Globals.Darwin.NumberOfSpeciesAliveYummy,
			Globals.Darwin.NumberOfSpeciesAlivePoisonous,
			Globals.Darwin.GetSpeciesCount(Globals.Darwin.FindDominatingSpecies())
		);
		Vector2 labsize = GUI.skin.GetStyle("label").CalcSize(new GUIContent(darwinSays));
		GUI.Box(new Rect(0, Screen.height-labsize.y+5, labsize.x+10, labsize.y-5), darwinSays);
	}

}
