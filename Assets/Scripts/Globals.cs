using UnityEngine;
using System.Collections;

public class Globals : MonoBehaviour {

	public static float LevelWidth = 24.0f+1.0f;
	public static float LevelHeight = 18.0f+1.0f;

	public static Rect LevelRect = new Rect(-LevelWidth*0.5f, -LevelHeight*0.5f, LevelWidth, LevelHeight);

	public static GameObject Player;

	public static BlobManager BlobManager;

	public static BubbleManager BubbleManager;

	public static DecalManager DecalManager;

	public static Healthbar Healthbar;
	
	public static Darwin Darwin;

	public static Kongregate Kongregate;

	public static Camera MainCamera;

	public static float startTime;

	void Awake() {
		MainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
