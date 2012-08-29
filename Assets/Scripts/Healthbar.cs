using UnityEngine;
using System.Collections;

public class Healthbar : MonoBehaviour {

	void setPercentage(float x) {
		x = Mathf.Max(0, Mathf.Min(1, x));
		Transform life = this.transform.Find("Life");
		Transform death = this.transform.Find("Death");
		life.localScale = new Vector3(x, 1, 1);
		death.localScale = new Vector3(1 - x, 1, 1);
		life.localPosition = new Vector3(-0.5f + 0.5f*x, 0, 0);
		death.localPosition = new Vector3(0.5f*x, 0, 0);
	}

	void Awake() {
		Globals.Healthbar = this;
	}

	// Use this for initialization
	void Start () {
	}

	float popupTime = 0.0f;
	string popupText = "";
	Color popupColor;
	Vector3 popupPosition;

	public void ShowHealthPopup(GameObject obj, float dh) {
		popupTime = 1.5f;
		popupText = Mathf.RoundToInt(100.0f*dh).ToString("+#;-#;0");
		float q = 0.5f + 0.5f * MoreMath.Clamp(dh/0.2f, -1.0f, +1.0f);
		popupColor = new Color(1.0f - q, q, 0.0f);
		popupPosition = Globals.MainCamera.WorldToScreenPoint(obj.transform.position);
		popupPosition.y = Screen.height - popupPosition.y - 20;
		popupPosition.x = popupPosition.x + 20;
	}

	// Update is called once per frame
	void Update () {
		var player = Globals.Player.GetComponent<PlayerInteract>();
		float health = player.Health;
		setPercentage(health);
		// increase volume of hearbeat sound
		if(!player.IsDead && health < 0.5f) {
			float x = 1.0f - (health/0.5f);
			audio.volume = x*x;
		}
		else {
			audio.volume = 0.0f;
		}
	}

	void OnGUI()
	{
		if(popupTime > 0) {
			popupTime -= MyTime.deltaTime;
			int w = 50;
			int h = 30;
			GUI.Label(new Rect(popupPosition.x - w/2, popupPosition.y - h/2, w, h), popupText);
		}
	}

}
