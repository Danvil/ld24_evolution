using UnityEngine;
using System.Collections.Generic;

public class DecalManager : MonoBehaviour {

	int decalId = 1;

	List<GameObject> decals = new List<GameObject>();

	public GameObject decalPrefab;

	public void Create(Vector3 pos, float radius, Color color, float wobbelRadius=0.0f, float height=0.0f) {
		GameObject x = (GameObject)Instantiate(decalPrefab);
		decals.Add(x);
		Decal decal = x.GetComponent<Decal>();
		decal.lifetime = 12.0f;
		decal.Color = new Color(color.r, color.g, color.b, 0.0f);
		decal.wobbelRadius = wobbelRadius;
		x.transform.position = new Vector3(pos.x, pos.y, -height);
		x.transform.parent = this.transform;
		x.transform.localScale = radius * Vector3.one;
		x.name = "decal_" + System.String.Format("{0:D5}", decalId++);
	}

	void Awake() {
		Globals.DecalManager = this;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		foreach(GameObject x in decals.ToArray()) {
			Decal decal = x.GetComponent<Decal>();
			if(decal.IsDead) {
				decals.Remove(x);
				Destroy(x);
			}
		}
	}
}
