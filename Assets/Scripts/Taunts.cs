using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Taunts : MonoBehaviour {

	class Entry {
		public Transform target;
		public string text;
		public float showtime;
	}

	List<Entry> entries = new List<Entry>();

	float delay = Random.Range(3.0f, 5.0f);
	
	void Awake() {
		strFriend = new string[] {
			"I love you",
			"Hug me",
		};
		
		strFoe = new string[] {
			"You suck",
			"I hate you",
			"You are ugly",
		};
		
		strBomb = new string[] {
			"You will die.",
			"BOOOM",
			"(muhaha)"
		};
		
		strYummy = new string[] {
			"Eat me",
			"I'm hungry",
			"Fresh meat",
			"Must eat"
		};
		
		strFleeing = new string[] {
			"Help!",
			"Bastard!",
			//"Become vegetarian!",
			"I kill you!",
			"Go away!",
		};
		
		strRuler = new string[] {
			"We rule the world",
			"We will survive",
			"We are mighty",
			"The world is ours",
			"Bow before me",
			"We rule supreme",
		};
		
		strCreator = new string[] {
			"The creator is strong.",
			"Ommm",
			"I saw spaghetti",
			"HE does not throw dice",
		};
		
		strPlayerGood = new string[] {
			"Good boy",
			"No meat hu?",
			"Greenie!",
		};
		
		strPlayerEvil = new string[] {
			"Don't kill us!",
			"We will have revenge",
			"WAR"
		};
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		delay -= MyTime.deltaTime;
		if(delay < 0.0f) {
			delay += Random.Range(3.0f, 5.0f);
			taunt();
		}
		foreach(Entry x in entries.ToArray()) {
			x.showtime -= MyTime.deltaTime;
		}
	}
	
	bool HasEntry(GameObject obj) {
		foreach(var x in entries) {
			if(x.target == obj.transform) {
				return true;
			}
		}
		return false;
	}

	IEnumerable<BlobGenotype> blobs {
		get {
			var v = Globals.BlobManager.BlobGenotypes;
			v.Shuffle();
			return from x in v where !x.IsPlant && !HasEntry(x.gameObject) select x;
		}
	}

	BlobGenotype findFriend() {
		return (from x in blobs where x.PlayerFollowStrength >= +0.8f select x).FirstOrDefault();
	}

	BlobGenotype findFoe() {
		return (from x in blobs where x.PlayerFollowStrength <= -0.8f select x).FirstOrDefault();
	}

	BlobGenotype findFleeing() {
		return (from x in blobs
			where x.PlayerFollowStrength <= -0.8f && (Globals.Player.transform.position - x.transform.position).magnitude < 3.0f
			select x).FirstOrDefault();
	}

	BlobGenotype findBomb() {
		return (from x in blobs
			where Globals.BubbleManager.BubbleCountSigned(x.PlayerHealthRestoreTotal) <= -4
			select x).FirstOrDefault();
	}

	BlobGenotype findYummy() {
		return (from x in blobs
			where Globals.BubbleManager.BubbleCountSigned(x.PlayerHealthRestoreTotal) >= +4
			select x).FirstOrDefault();
	}
	
	BlobGenotype findRuler() {
		int id = Globals.Darwin.FindDominatingSpecies();
		if(Globals.Darwin.GetSpeciesCount(id) < 5) {
			return null;
		}
		return (from x in blobs	where x.genes.species == id	select x).FirstOrDefault();
	}
	
	string[] strFriend;
	
	string[] strFoe;
	
	string[] strBomb;
	
	string[] strYummy;
	
	string[] strFleeing;
	
	string[] strRuler;
	
	string[] strCreator;
	
	string[] strPlayerGood;
	string[] strPlayerEvil;
	
	public string pick(string[] str) {
		return str[Random.Range(0, str.Length)];
	}
	
	void taunt() {
		for(int i=0; i<10; i++) {
			BlobGenotype x = null;
			string text = "";
			// try to find a taunt
			int s = Random.Range(0, 8);
			switch(s) {
			case 0:
				x = findFriend();
				text = pick(strFriend);
				break;
			case 1:
				x = findFoe();
				text = pick(strFoe);
				break;
			case 2:
				x = findBomb();
				text = pick(strBomb);
				break;
			case 3:
				x = findYummy();
				text = pick(strYummy);
				break;
			case 4:
				x = findFleeing();
				text = pick(strFleeing);
				break;
			case 5:
				if(Random.value < 0.25) {
					x = findRuler();
					text = pick(strRuler);
				}
				break;
			case 6:
				if(Random.value < 0.10) {
					x = blobs.First();
					text = pick(strCreator);
				}
				break;
			case 7:
				if(Random.value < 0.15) {
					float r = Globals.Player.GetComponent<PlayerInteract>().GoodEvilRating;
					if(r < -15.0f) {
						x = blobs.First();
						text = pick(strPlayerEvil);
					}
					if(r > 15.0f) {
						x = blobs.First();
						text = pick(strPlayerGood);
					}
				}
				break;
			}
			if(x != null && text != null) {
				// create taunt
				Entry entry = new Entry();
				entry.showtime = 4.0f;
				entry.target = x.transform;
				entry.text = text;
				entries.Add(entry);
				return;
			}
		}
	}

	void OnGUI()
	{
		foreach(Entry x in entries.ToArray()) {
			if(x.showtime <= 0.0f || !x.target) {
				entries.Remove(x);
			}
		}
		foreach(Entry x in entries) {
			Vector2 labsize = GUI.skin.GetStyle("label").CalcSize(new GUIContent(x.text));
			float w = labsize.x;
			float h = labsize.y;

			Vector3 pos;
			pos = Globals.MainCamera.WorldToScreenPoint(x.target.position);
			pos.y = Screen.height - pos.y - 20;

			GUI.Label(new Rect(pos.x - w/2, pos.y - h/2, w, h), x.text);
		}
	}


}
