using UnityEngine;
//using UnityEditor;
using System.Collections.Generic;

public class BlobLineup : MonoBehaviour {

	public GameObject blobPrefab;
	
	List<GameObject> objs = new List<GameObject>();
	List<float> offset = new List<float>();

	// Use this for initialization
	void Start () {
		for(int y=-2; y<=2; y++) {
			for(int x=-3; x<=3; x++) {
				GameObject blob = (GameObject)Instantiate(blobPrefab);
				blob.transform.position = new Vector3(
					3*x + Random.Range(-1.0f,+1.0f),
					3*y + Random.Range(-1.0f,+1.0f),
					0);
				blob.transform.rotation = Quaternion.AngleAxis(
					Random.Range(0.0f, 360.0f), Vector3.forward);
				blob.GetComponent<BlobGenotype>().Create();
				objs.Add(blob);
				offset.Add(Random.Range(0.0f,1.0f));
			}
		}
	}

	// Update is called once per frame
	void Update () {
	}
}
