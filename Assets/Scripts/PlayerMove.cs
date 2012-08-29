using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour {

	const float cPlayerSpeed = 1.80f;
	const float cPlayerSprintBoost = 2.00f;

	const float cRotationSpeed = 3.0f;
	const float cMinDist = 0.5f;
	const float cFadeoffStartDist = 1.3f;

	public bool IsMoving {
		get;
		private set;
	}

	public bool IsSprinting {
		get;
		private set;
	}

	// Use this for initialization
	void Start () {
	}

	Vector3 getMouseTarget() {
		// Generate a plane that intersects the transform's position with an upwards normal.
		Plane playerPlane = new Plane(Vector3.forward, this.transform.position);
		// Generate a ray from the cursor position
		Ray ray = Globals.MainCamera.ScreenPointToRay(Input.mousePosition);
		float hitdist = 0.0f;
		// If the ray is parallel to the plane, Raycast will return false.
		if (playerPlane.Raycast(ray, out hitdist)) {
			// Get the point along the ray that hits the calculated distance.
			return ray.GetPoint(hitdist);
		}
		else {
			return Vector3.zero;
		}
	}

	// Update is called once per frame
	void Update () {
		if(GetComponent<PlayerInteract>().IsDead) {
			return;
		}

		Vector3 mouseTarget = getMouseTarget();
		Quaternion targetRotation = MoreMath.RotHeading(mouseTarget - this.transform.position);
		// Smoothly rotate towards the target point.
		transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, cRotationSpeed * MyTime.deltaTime);

//		float x = cPlayerSpeed * Input.GetAxis("Vertical") * MyTime.deltaTime;
//		float y = - 0.5f * cPlayerSpeed * Input.GetAxis("Horizontal") * MyTime.deltaTime;

		float x_base = cPlayerSpeed * MyTime.deltaTime;
		float x = x_base;
		float distToMouse = (this.transform.position - mouseTarget).magnitude;
		if(distToMouse < cFadeoffStartDist) {
			x *= MoreMath.Parabel(distToMouse, cFadeoffStartDist, 1.0f, 0.0f, -1.0f);
		}
		float y = 0.0f;

		// check shift for sprint
		IsSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
		float boost = (IsSprinting && x > 0 ? cPlayerSprintBoost : 1.0f);
		
		Vector3 d = new Vector3(boost*x, y, 0.0f);
		if(GetComponent<PlayerShape>().CanMove) {
			IsMoving = d.magnitude > 0.2f*x_base;
			if(IsMoving) {
				transform.Translate(d);
			}
		}
		if(Input.GetButton("Fire1") || Input.GetButton("Jump")) {
			GetComponent<PlayerShape>().Attack();
		}
		// test for collision
	}
}
