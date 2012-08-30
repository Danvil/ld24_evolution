using UnityEngine;
//using UnityEditor;
using System.Collections.Generic;

public class BlobLineup : MonoBehaviour {

	public GameObject blobPrefab;
	
	public int Count = 20;

	List<GameObject> objs = new List<GameObject>();
	List<float> offset = new List<float>();

	// Use this for initialization
	void Start () {
		float wh = 0.5f * 0.8f * Globals.LevelRect.width;
		float hh = 0.5f * 0.8f * Globals.LevelRect.height;
		float d = Mathf.Sqrt(4.0f*wh*hh / (float)Count);
		float dh = 0.35f * d;
		for(float y=-hh+0.5f*d; y<=hh; y+=d) {
			for(float x=-wh+0.5f*d; x<=wh; x+=d) {
				GameObject blob = (GameObject)Instantiate(blobPrefab);
				blob.GetComponent<BlobGenotype>().CreateRandom();
				blob.transform.position = new Vector3(
					x + Random.Range(-dh,+dh),
					y + Random.Range(-dh,+dh),
					0);
				blob.transform.rotation = Quaternion.AngleAxis(
					Random.Range(0.0f, 360.0f), Vector3.forward);
				objs.Add(blob);
				offset.Add(Random.Range(0.0f,1.0f));
			}
		}
	}

	// Update is called once per frame
	void Update () {
	}
}
