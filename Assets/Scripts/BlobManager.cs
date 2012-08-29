using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BlobManager : MonoBehaviour {

	const int cMaxBlobs = 24*5;

	int blobId = 1;

	List<GameObject> blobs = new List<GameObject>();

	public void Add(GameObject blob) {
		blobs.Add(blob);
		blob.name = "blob_" + System.String.Format("{0:D5}", blobId++);
		blob.transform.parent = this.gameObject.transform;
	}

	public void Remove(GameObject blob) {
		blobs.Remove(blob);
	}

	public BlobGenotype[] BlobGenotypes {
		get {
			List<BlobGenotype> v = new List<BlobGenotype>();
			foreach(var x in blobs) {
				v.Add(x.GetComponent<BlobGenotype>());
			}
			return v.ToArray();
		}
	}

	public bool IsFull {
		get {
			return blobs.Count >= cMaxBlobs;
		}
	}

	public IEnumerable<GameObject> GetInRange(Vector3 pos, float r) {
		foreach(GameObject x in blobs.ToArray()) {
			float d = (pos - x.transform.position).magnitude;
			if(d < r) {
				yield return x;
			}
		}
	}

	public IEnumerable<GameObject> GetInRange(GameObject source, float r) {
		foreach(GameObject x in blobs.ToArray()) {
			float d = (source.transform.position - x.transform.position).magnitude;
			if(x != source && d < r) {
				yield return x;
			}
		}
	}

	public bool TryFindFreeSpace(Vector3 center, float r, float freeR, out Vector3 result) {
		for(int i=0; i<3; i++) {
			Vector3 x = center + r * MoreMath.RandomInsideUnitCircle3;
			// is it in the level?
			if(!Globals.LevelRect.Contains(x)) {
				continue;
			}
			// is another blob in the way?
			if(GetInRange(x, freeR).Any()) {
				continue;
			}
			// point ist ok
			result = x;
			return true;
		}
		result = Vector3.zero;
		return false;
	}

	void Awake() {
		Globals.BlobManager = this;
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}
}
