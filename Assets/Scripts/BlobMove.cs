using UnityEngine;
using System.Collections;

public class BlobMove : MonoBehaviour {

	const float cSpeedMaxBoost = 1.2f;
	const float cGoalReachedTolerance = 0.1f;
	const float cAvoidRadius = 5.0f;
	const float cAvoidStrength = 0.5f;
	const float cRotationMixStrength = 0.5f;

	float playerFollowStrength {
		get {
			return GetComponent<BlobGenotype>().genes.playerFollowStrength;
		}
	}

	Vector3 goal;
	float goalWaitTime;
	float goalFollowTime;

	void setNewGoal() {
		goal = new Vector3(
			Random.Range(Globals.LevelRect.xMin+1,Globals.LevelRect.xMax-1),
			Random.Range(Globals.LevelRect.yMin+1,Globals.LevelRect.yMax-1),
			0.0f);
		goalWaitTime = Random.Range(1.0f, 3.0f);
		goalFollowTime = 5.0f;
	}

	bool isGoalReached() {
		return (transform.position - goal).magnitude < cGoalReachedTolerance;
	}

	// Use this for initialization
	void Start () {
		setNewGoal();
	}

	float slerpAngle(float x, float y, float p) {
		float d = y - x;
		if(d > Mathf.PI) {
			d = 2.0f * Mathf.PI - d;
		}
		return x + p * d;
	}

	Vector3 computePlayerFollow() {
		const float cMinRadius = 1.0f;
		Vector3 d = Globals.Player.transform.position - transform.position;
		float m = d.magnitude;
		if(m == 0) {
			return Vector3.zero; // FIXME
		}
		if(playerFollowStrength >= 0) {
			if(m < cMinRadius) {
				return Vector3.zero;
			}
			else {
				return playerFollowStrength * d.normalized;
			}
		}
		else {
			m = 0.5f + 0.4f*Mathf.Max(0.0f, m - 3.0f);
			return playerFollowStrength / (m*m) * d.normalized;
		}
	}

	Vector3 computeGoalFollow() {
		return GetComponent<BlobGenotype>().Speed * (goal - transform.position).normalized;
	}

	float playerDist;
	float playerInfluence;

	// move towards goal and avoid/follow player
	Vector3 computeFollow() {
		playerDist = (Globals.Player.transform.position - transform.position).magnitude;
		playerInfluence = 1.0f / (1.0f + 0.1f*Mathf.Max(0.0f,playerDist-1.0f));
		if(Globals.Player.GetComponent<PlayerInteract>().IsDead) {
			playerInfluence = 0.0f;
		}
		return playerInfluence * computePlayerFollow() + (1.0f - playerInfluence) * computeGoalFollow();
	}

	float avoidFalloff(float d, float d_min) {
//		a = Mathf.Max(0.0f, a + 1.0f);
//		return 1.0f / (0.01f + a*a);
		float z = Mathf.Max(d/d_min, 0.4f);
		return 1.0f / (z*z);
	}

	// avoid other
	Vector3 computeAvoidOther() {
		Vector3 force = Vector3.zero;
		float size_this = GetComponent<BlobGenotype>().Size;
		foreach(GameObject x in Globals.BlobManager.GetInRange(gameObject, cAvoidRadius)) {
			Vector3 delta = x.transform.position - transform.position;
			float d_min = size_this + x.GetComponent<BlobGenotype>().Size;
			force -= avoidFalloff(delta.magnitude, d_min) * delta.normalized;
		}
		return cAvoidStrength * force;
	}

	// avoid level
	Vector3 computeAvoidLevel() {
		float size_this = GetComponent<BlobGenotype>().Size;
		Vector3 force = Vector3.zero;
		float x = this.transform.position.x;
		float y = this.transform.position.y;
		force += avoidFalloff(x - Globals.LevelRect.xMin, size_this) * new Vector3(1,0,0);
		force += avoidFalloff(Globals.LevelRect.xMax - x, size_this) * new Vector3(-1,0,0);
		force += avoidFalloff(y - Globals.LevelRect.yMin, size_this) * new Vector3(0,1,0);
		force += avoidFalloff(Globals.LevelRect.yMax - y, size_this) * new Vector3(0,-1,0);
/*		Debug.DrawLine(new Vector3(Globals.LevelRect.xMin, Globals.LevelRect.yMin, -1.0f),
					   new Vector3(Globals.LevelRect.xMax, Globals.LevelRect.yMin, -1.0f));
		Debug.DrawLine(new Vector3(Globals.LevelRect.xMin, Globals.LevelRect.yMax, -1.0f),
					   new Vector3(Globals.LevelRect.xMax, Globals.LevelRect.yMax, -1.0f));
		Debug.DrawLine(new Vector3(Globals.LevelRect.xMin, Globals.LevelRect.yMin, -1.0f),
					   new Vector3(Globals.LevelRect.xMin, Globals.LevelRect.yMax, -1.0f));
		Debug.DrawLine(new Vector3(Globals.LevelRect.xMax, Globals.LevelRect.yMin, -1.0f),
					   new Vector3(Globals.LevelRect.xMax, Globals.LevelRect.yMax, -1.0f));
*/		return force;
	}
	
	// Update is called once per frame
	void Update () {
		goalFollowTime -= MyTime.deltaTime;
		if(goalFollowTime < 0.0f) {
			setNewGoal();
		}
		else if(isGoalReached()) {
			goalWaitTime -= MyTime.deltaTime;
			if(goalWaitTime < 0.0f) {
				setNewGoal();
			}
		}
		else {
			Vector3 moveFollow = computeFollow();
			Vector3 moveAvoid = computeAvoidOther();
			Vector3 moveLevel = computeAvoidLevel();
			Vector3 move = moveFollow + moveAvoid + moveLevel;
			Debug.DrawRay(this.transform.position, moveFollow, Color.red);
			Debug.DrawRay(this.transform.position, moveAvoid, Color.green);
			Debug.DrawRay(this.transform.position, moveLevel, Color.blue);
			// some randomness
			move += 0.05f * MoreMath.RandomInsideUnitCircle3;
			// limit max velocity
			float mag = move.magnitude;
			float speedMax = cSpeedMaxBoost * GetComponent<BlobGenotype>().Speed;
			if(mag > speedMax) {
				move *= speedMax / mag;
			}
			// compute new position
			transform.position += MyTime.deltaTime * move;
			transform.position = new Vector3(transform.position.x, transform.position.y, 0);
			// compute new rotation
			float angle_old = MoreMath.VectorAngle(transform.localRotation * Vector3.right);
			float angle_new = MoreMath.VectorAngle(move.normalized);
			float angle_final = slerpAngle(angle_old, angle_new, cRotationMixStrength * MyTime.deltaTime);
			transform.localRotation = MoreMath.RotAngle(angle_final);
		}
	}
}
