using UnityEngine;
using System.Collections.Generic;

public class BubbleManager : MonoBehaviour {

	int bubbleId = 1;

	List<GameObject> bubbles = new List<GameObject>();

	public GameObject bubbleHealthPrefab;

	public GameObject bubblePoisonPrefab;

	public int BubbleCountSigned(float dh) {
		bool isCreateHealtyh = dh > 0;
		int num = Mathf.RoundToInt(Mathf.Abs(dh) / Bubble.cHealthPerBubble);
		return isCreateHealtyh ? num : -num;
	}

	public void Create(Vector3 position, float radius, float dh) {
		bool isCreateHealtyh = dh > 0;
		dh = Mathf.Abs(dh);
		int num = Mathf.RoundToInt(dh / Bubble.cHealthPerBubble);
		if(num == 0) {
			// create live and death bubble
			CreateBubble(position, radius, true);
			CreateBubble(position, radius, false);
		}
		else {
			while(num > 0) {
				CreateBubble(position, radius, isCreateHealtyh);
				num --;
			}
		}
	}

	public void CreateBubble(Vector3 position, float radius, bool isHealty) {
		GameObject x;
		if(isHealty)
			x = (GameObject)Instantiate(bubbleHealthPrefab);
		else
			x = (GameObject)Instantiate(bubblePoisonPrefab);
		Bubble bubble = x.GetComponent<Bubble>();
		if(isHealty)
			bubble.SetHealthBubble();
		else
			bubble.SetPoisonBubble();
		x.transform.position = position + radius * MoreMath.RandomInsideUnitCircle3;
		bubble.velocity = 2.0f*MoreMath.RandomInsideUnitCircle3;
		add(x);
	}

	void add(GameObject bubble) {
		bubbles.Add(bubble);
		bubble.name = "bubble_" + System.String.Format("{0:D5}", bubbleId++);
		bubble.transform.parent = this.gameObject.transform;
	}

	public void Remove(GameObject bubble) {
		bubbles.Remove(bubble);
	}

	public IEnumerable<GameObject> GetInRange(Vector3 pos, float r) {
		foreach(GameObject x in bubbles.ToArray()) {
			float d = (pos - x.transform.position).magnitude;
			if(d < r) {
				yield return x;
			}
		}
	}

	public IEnumerable<GameObject> GetInRange(GameObject source, float r) {
		foreach(GameObject x in bubbles.ToArray()) {
			float d = (source.transform.position - x.transform.position).magnitude;
			if(x != source && d < r) {
				yield return x;
			}
		}
	}

	public bool TryFindFreeSpace(Vector3 center, float r, float freeR, out Vector3 result) {
		for(int i=0; i<3; i++) {
			Vector2 u = Random.insideUnitCircle;
			Vector3 x = center + r * new Vector3(u.x, u.y, 0.0f);
			bool failed = false;
			foreach(GameObject obj in GetInRange(x, freeR)) {
				failed = true;
				break;
			}
			if(!failed) {
				result = x;
				return true;
			}
		}
		result = Vector3.zero;
		return false;
	}

	void Awake() {
		Globals.BubbleManager = this;
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}
}
