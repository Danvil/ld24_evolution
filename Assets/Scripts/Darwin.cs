using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Darwin : MonoBehaviour {
	
	const float cSpeciesOffset = 0.2f;
	
	int speciesId = 1;

	class Entry {
		public int id;
		public Genes genes;
		public int count;
		public int maxCount;
	}
	
	List<Entry> species = new List<Entry>();
	
	public void Register(Genes genes) {
		foreach(Entry x in species) {
			float d = genes.Difference(x.genes);
			if(d < cSpeciesOffset) {
				// found it!
				x.count ++;
				x.maxCount ++;
				genes.species = x.id;
				return;
			}
		}
		// hurray!
		Entry y = new Entry();
		y.id = speciesId++;
		y.genes = genes.Clone(); // can we have a clone plz?
		y.count = 1;
		genes.species = y.id;
		species.Add(y);
	}
	
	Entry find(int id) {
		foreach(var x in species) {
			if(x.id == id)
				return x;
		}
		return null;
	}
	
	public void RegisterDeath(Genes genes) {
		find(genes.species).count --;
	}
	
	public int NumberOfSpecies {
		get {
			return species.Count;
		}
	}
	
	public int NumberOfSpeciesAlive {
		get {
			int count = 0;
			foreach(var x in species) {
				if(x.count > 0)
					count++;
			}
			return count;
		}
	}
	
	public int NumberOfSpeciesAliveYummy {
		get {
			int count = 0;
			foreach(var x in species) {
				if(x.count > 0 && x.genes.playerHealthRestoreBase > 0)
					count++;
			}
			return count;
		}
	}
	
	public int NumberOfSpeciesAlivePoisonous {
		get {
			int count = 0;
			foreach(var x in species) {
				if(x.count > 0 && x.genes.playerHealthRestoreBase < 0)
					count++;
			}
			return count;
		}
	}
	
	public int GetSpeciesCount(int id)
	{
		Entry x = find(id);
		if(x != null) {
			return x.count;
		}
		return 0;
	}

	public int FindDominatingSpecies() {
		if(species.Count == 0) {
			return 0;
		}
		int cnt = species.Max(x => x.count);
		// HACK
		foreach(var x in species) {
			if(x.count == cnt)
				return x.id;
		}
		return 0;
	}
	
	void Awake() {
		Globals.Darwin = this;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
