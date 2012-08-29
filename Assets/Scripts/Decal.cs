using UnityEngine;
using System.Collections;

public class Decal : MonoBehaviour {

	public float lifetime;
	float timeleft;

	public float wobbelRadius = 0.0f;

	Vector3 wobbelCenter;
	Vector3 wobbelTarget;

	public Color Color {
		get {
			return renderer.material.color;
		}
		set {
			renderer.material.color = value;
			renderer.material.SetColor("_TintColor", value);
		}
	}

	public bool IsDead {
		get { return timeleft <= 0.0f; }
	}

	// Use this for initialization
	void Start () {
		wobbelCenter = this.transform.position;
		wobbelTarget = wobbelCenter;
		timeleft = lifetime;
	}
	
	float alphaFromTimeleft() {
		return 0.25f * MoreMath.Parabel(timeleft, lifetime*0.5f, 1.0f, 0.0f, 0.0f);
	}

	// Update is called once per frame
	void Update () {
		timeleft -= MyTime.deltaTime;
		Color col = this.Color;
		col.a = alphaFromTimeleft();
		this.Color = col;
		// wobbeling
		if(wobbelRadius > 0.0f) {
			Vector3 d = wobbelTarget - this.transform.position;
			d.z = 0.0f;
			if(d.magnitude <= 0.1f) {
				wobbelTarget = wobbelCenter + wobbelRadius * MoreMath.RandomInsideUnitCircle3;
			}
			else {
				this.transform.position += MyTime.deltaTime * d.normalized * 0.1f;
			}
		}
	}
}
