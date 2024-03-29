using UnityEngine;
using System.Collections;

public class BlobGenotype : MonoBehaviour {

	public AudioClip audioReproduce;
	public AudioClip audioDeathNatural;
	public AudioClip audioDeathChop;
	public AudioClip audioDeathCry;

	const float cLifespawnBase = 100.0f;

	public float Lifespawn {
		get {
			return cLifespawnBase
				* (1.0f + 2.0f*(genes.size - 0.5f)); // big lifes longer
		}
	}

	float age = 0.0f;

	public Genes genes;

	public bool IsPlant {
		get { return genes.isPlant; }
	}

	public float Size {
		get { return genes.size; }
	}

	public float PlayerFollowStrength {
		get { return genes.playerFollowStrength; }
	}

	public float BubbleFrequency {
		get {
			return 1.0f / genes.bubbleInterval
				* (1.0f - 1.0f*(genes.size - 0.5f)) // big produces more
				* (IsPlant ? 1.0f : 0.5f); // animals produce much less
		}
	}

	public float PlayerHealthRestoreTotal {
		get {
			return genes.playerHealthRestoreBase
				* (IsPlant ? 0.70f : 1.0f) // plants give less
				* 2.0f * genes.size; // bigger gives more
		}
	}

	public float ReprodutionFrequency {
		get {
			return 1.0f / genes.reproductionIntervalBase
				* (1.0f + 1.0f*(genes.size - 0.5f)) // big needs more time
				* (IsPlant ? 1.2f : 0.8f); // animals need more time
		}
	}

	public float Speed {
		get {
			return 2.0f - 1.5f*(genes.size - 0.5f);
		}
	}

	public float WobbelFrequency = 1.0f / 10.0f;

	public Color HealthRestoreColor {
		get {
			float dh = PlayerHealthRestoreTotal;
			float q = 0.5f * (1.0f + MoreMath.Clamp(dh, -0.2f, +0.2f)/0.2f);
			return new Color(1.0f - q, q, 0.0f);
		}
	}
	
	public void CreateRandom() {
		Genes the_genes = new Genes();
		the_genes.Randomize();
		Create(the_genes);
	}
	
	public void Mutate(Genes other) {
		Genes the_genes = other.Clone();
		the_genes.Mutate();
		Create(the_genes);
	}

	void Create(Genes genes) {
		this.genes = genes;
		GetComponent<BlobShape>().Genes = genes;
		GetComponent<BlobShape>().middleHeight = (IsPlant ? 1.0f : 0.5f);
		if(IsPlant) {
			Destroy(GetComponent<BlobMove>()); // plants to not move
		}
		if(Globals.Darwin) {
			Globals.Darwin.Register(genes);
		}
	}

	public void Kill(bool isNatural=true) {
		// create bubbles
		float healthReleaseFactor = (isNatural && !IsPlant ? 0.5f : 1.0f);
		Globals.BubbleManager.Create(this.transform.position, genes.size, healthReleaseFactor*PlayerHealthRestoreTotal);
		// play sound (use player audio because blob is destroyed ...)
		if(isNatural) {
			Globals.Player.audio.PlayOneShot(audioDeathNatural);
		}
		else {
			if(IsPlant) {
				Globals.Player.audio.PlayOneShot(audioDeathChop);
			}
			else {
				Globals.Player.audio.PlayOneShot(audioDeathCry);
			}
		}
		// Darwin
		Globals.Darwin.RegisterDeath(genes);
		// destroy blob
		Globals.BlobManager.Remove(this.gameObject);
		Destroy(this.gameObject);
	}

	bool isReproducing() {
		if(Globals.BlobManager.IsFull) {
			return false;
		}
		return MoreMath.CheckOccurence(ReprodutionFrequency);
	}

	void Reproduce() {
		Vector3 pos = this.transform.position;
		// try to find a place
		if(!Globals.BlobManager.TryFindFreeSpace(pos, 1.3f + 0.5f*genes.size, genes.size, out pos)) {
			// can not place
			return;
		}
		// create child
		GameObject x = (GameObject)Instantiate(gameObject); // clone
		x.transform.position = pos;
		x.transform.rotation = Quaternion.AngleAxis(Random.Range(0.0f, 2.0f*Mathf.PI), Vector3.forward);
		var genotype = x.GetComponent<BlobGenotype>();
		genotype.Mutate(genes);
		// play sound
		audio.PlayOneShot(audioReproduce);
		// show ring if new species
		if(genotype.genes.isFounder) {
			Globals.DecalManager.CreateNewSpecies(genotype.transform, 1.5f*genes.size, 0.0f);
		}
		else {
			Globals.DecalManager.CreateNewIndividum(genotype.transform, 1.5f*genes.size, 0.0f);
		}
	}

	// Use this for initialization
	void Start () {
		Globals.BlobManager.Add(this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		age += MyTime.deltaTime;
		if(age > Lifespawn) {
			// dies ...
			Kill();
			return;
		}
		if(isReproducing()) {
			Reproduce();
		}
		if(MoreMath.CheckOccurence(BubbleFrequency)) {
			Globals.BubbleManager.CreateBubble(this.transform.position, genes.size, PlayerHealthRestoreTotal > 0);
		}
		if(IsPlant && MoreMath.CheckOccurence(WobbelFrequency)) {
			Globals.DecalManager.CreateEnv(this.transform, 2.0f*genes.size, 
				HealthRestoreColor, 2.0f*genes.size,
				genes.size);
		}
	}
}
