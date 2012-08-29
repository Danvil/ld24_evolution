using UnityEngine;
using System.Collections;

public class PlayerInteract : MonoBehaviour {

	public AudioClip audioHit;
	public AudioClip audioEatGood;
	public AudioClip audioEatBad;

	public float healthLossRateStill = 0.008f;
	public float healthLossRateMove = 0.010f;
	public float healthLossRateSprint = 0.040f;
	public float healthLossAttack = -0.03f;

	float health = 1.0f;

	public float Health {
		get { return health; }
	}

	public bool IsDead {
		get {
			//return false; // DEV MODE
			return health <= 0;
		}
	}

	void addToHealth(float dh) {
		health += dh;
		if(IsDead && TimeSurvived == 0) {
			Death();
		}
		health = Mathf.Min(1.0f, health);
	}

	void Death() {
		TimeSurvived = Mathf.RoundToInt(MyTime.time - Globals.startTime);
	}
	
	Genes genes;
	
	void Awake() {
		Globals.Player = gameObject;

		genes = new Genes();
		
		genes.isPlant = false;
		genes.size = 1.0f;
		
		genes.swCircle = 0;
		genes.swSquare = 0;
		genes.swStar = 0.5f;
		genes.swRose = 0;
		genes.swRose2 = 0;
		genes.swCardioid = 0.5f;
		
		genes.playerFollowStrength = 0;
		genes.bubbleInterval = 1e9f;
		genes.playerHealthRestoreBase = 0;
		
		genes.species = 0;
		
		//genes.UpdateColor();
		genes.color = Color.blue;
		
		transform.FindChild("Blob").gameObject.GetComponent<BlobShape>().Genes = genes;
	}

	// Use this for initialization
	void Start () {
		GoodEvilRating = 0.0f;
	}

	float deadlyDelay = 0.0f;

	// Update is called once per frame
	void Update () {
		// health decays slowly depending on movement mode
		deadlyDelay += MyTime.deltaTime;
		float healthLossRate = 1.0f;
		if(GetComponent<PlayerMove>().IsSprinting) {
			healthLossRate = healthLossRateSprint;
		}
		else if(GetComponent<PlayerMove>().IsMoving) {
			healthLossRate = healthLossRateMove;
		}
		else {
			healthLossRate = healthLossRateStill;
		}
		addToHealth(-MyTime.deltaTime * healthLossRate);
		// check if attack hits something
		if(GetComponent<PlayerShape>().IsDeadly) {
			if(deadlyDelay > 0.0f) {
				audio.PlayOneShot(audioHit);
				addToHealth(healthLossAttack);
				deadlyDelay = -0.85f*PlayerShape.cAttackLength;
			}
			Vector3 localAttackPoint = GetComponent<PlayerShape>().LocalAttackPoint;
			Vector3 attackPoint = transform.position + transform.rotation * localAttackPoint;
			Vector3 a = transform.position;
			a = new Vector3(a.x, a.y, 0);
			Ray ray = new Ray(a, attackPoint - transform.position);
			float len = (attackPoint - transform.position).magnitude;
			Debug.DrawLine(ray.origin, ray.origin + len * (ray.direction).normalized);
			// check collisions
			foreach(GameObject x in Globals.BlobManager.GetInRange(gameObject, 3.0f)) {
				RaycastHit hit = new RaycastHit();
				if(x.GetComponent<MeshCollider>().Raycast(ray, out hit, len)) {
					handleCollision(hit.collider.gameObject);
				}
			}			
		}
		// update mesh
		if(GoodEvilRating < 0) {
			GoodEvilRating += 0.03f*MyTime.deltaTime;
		}
		else {
			GoodEvilRating += 0.015f*MyTime.deltaTime;
		}
		float quot = 0.5f + MoreMath.Clamp(GoodEvilRating / 100.0f, -0.4f, +0.4f);
		genes.swStar = 1.0f - quot;
		genes.swCardioid = quot;
		// avoid extensive create mesh
		if(Mathf.Abs(lastQuot - quot) > 0.05f) {
			transform.FindChild("Blob").gameObject.GetComponent<BlobShape>().CreateMesh();
			lastQuot = quot;
		}
	}
	
	public float GoodEvilRating {
		get; private set;
	}
	float lastQuot = 0.0f;

	public int TimeSurvived {
		get; private set;
	}

	void handleCollision(GameObject x) {
		var geno = x.GetComponent<BlobGenotype>();
		GoodEvilRating += (geno.IsPlant ? 0.00f : -3.0f);
		audio.PlayOneShot(audioHit);
		geno.Kill(false);
	}

	void eatBubble(Bubble bubble) {
		if(bubble.IsGood) {
			audio.PlayOneShot(audioEatGood);
			GoodEvilRating += 0.10f;
		}
		else {
			audio.PlayOneShot(audioEatBad);
		}
		addToHealth(bubble.Health);
		Globals.BubbleManager.Remove(bubble.gameObject);
		Destroy(bubble.gameObject);
	}

	void OnTriggerEnter(Collider other) {
		Bubble bubble = other.GetComponent<Bubble>();
		if(bubble) {
			eatBubble(bubble);
		}
    }

}
