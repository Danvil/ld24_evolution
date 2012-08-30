using UnityEngine;
using System.Collections;

public class BlobShowReel : MonoBehaviour {

	public GameObject blobPrefab;

	GameObject blob;

	// Use this for initialization
	void Start () {
		blob = (GameObject)Instantiate(blobPrefab);
		blob.transform.position = Vector3.zero;
		Destroy(blob.GetComponent<BlobMove>()); // hold still damnit
		blob.GetComponent<BlobShape>().isUpdateMesh = true;
		blob.GetComponent<BlobGenotype>().genes.reproductionIntervalBase = 1e9f;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		Genes genes = blob.GetComponent<BlobGenotype>().genes;

		GUILayout.Label("Shape: Circle Weight");
		genes.swCircle = GUILayout.HorizontalSlider(genes.swCircle, 0, 1);

		GUILayout.Label("Shape: Square Weight");
		genes.swSquare = GUILayout.HorizontalSlider(genes.swSquare, 0, 1);

		GUILayout.Label("Shape: Star Weight");
		genes.swStar = GUILayout.HorizontalSlider(genes.swStar, 0, 1);
		GUILayout.Label("Shape: Star Count");
		genes.swStarCount = Mathf.RoundToInt(GUILayout.HorizontalSlider((float)genes.swStarCount, 1, 12));
		GUILayout.Label("Shape: Star Power");
		genes.swStarPower = GUILayout.HorizontalSlider(genes.swStarPower, 1, 6);
		GUILayout.Label("Shape: Star Min Radius");
		genes.swStarRadiusMin = GUILayout.HorizontalSlider(genes.swStarRadiusMin, 0, 1);

		GUILayout.Label("Shape: Rose Weight");
		genes.swRose = GUILayout.HorizontalSlider(genes.swRose, 0, 1);
		GUILayout.Label("Shape: Rose Petal Count");
		genes.swRosePetals = Mathf.RoundToInt(GUILayout.HorizontalSlider((float)genes.swRosePetals, 1, 12));

		GUILayout.Label("Shape: Rose (Abs) Weight");
		genes.swRose2 = GUILayout.HorizontalSlider(genes.swRose2, 0, 1);
		GUILayout.Label("Shape: Rose (Abs) Petal Count");
		genes.swRoseAbsPetals = Mathf.RoundToInt(GUILayout.HorizontalSlider((float)genes.swRoseAbsPetals, 1, 12));

		GUILayout.Label("Shape: Cardioid Weight");
		genes.swCardioid = GUILayout.HorizontalSlider(genes.swCardioid, 0, 1);
	}
}
