using UnityEngine;

public class Genes
{
	public const float cMutationStrengthLow = 0.25f;
	public const float cMutationStrengthHigh = 2.00f;
	public const float cHighMutationProbability = 0.05f;

	public const float cRestoreMin = -0.20f;
	public const float cRestoreMax = +0.20f;
	public const float cSizeMin = 0.40f;
	public const float cSizeMax = 0.90f;
	public const float cBubbleIntervalMin = 10.0f;
	public const float cBubbleIntervalMax = 200.0f;

	public bool isPlant = true;

	public float size = 0.5f;

	public float swCircle = 0.0f;
	public float swSquare = 1.0f;
	public float swStar = 0.0f;
	public int swStarCount = 6;
	public float swStarPower = 2.0f;
	public float swStarRadiusMin = 0.33f;
	public float swRose = 0.0f;
	public int swRosePetals = 2;
	public float swRose2 = 0.0f;
	public int swRoseAbsPetals = 2;
	public float swCardioid = 0.0f;

	public float playerFollowStrength = 0.0f;

	public float bubbleInterval = 100.0f;

	public float playerHealthRestoreBase = 0.0f;

	public float reproductionIntervalBase = 80.0f;
	
	public Color color = Color.blue;

	public int species = 0;

	public bool isFounder = true;
	
	public Genes Clone() {
		Genes g = new Genes();
		g.isPlant = this.isPlant;
		g.size = this.size;
		g.swCircle = this.swCircle;
		g.swSquare = this.swSquare;
		g.swStar = this.swStar;
		g.swStarCount = this.swStarCount;
		g.swStarPower = this.swStarPower;
		g.swStarRadiusMin = this.swStarRadiusMin;
		g.swRose = this.swRose;
		g.swRosePetals = this.swRosePetals;
		g.swRose2 = this.swRose2;
		g.swRoseAbsPetals = this.swRoseAbsPetals;
		g.swCardioid = this.swCardioid;
		g.playerFollowStrength = this.playerFollowStrength;
		g.bubbleInterval = this.bubbleInterval;
		g.playerHealthRestoreBase = this.playerHealthRestoreBase;
		g.reproductionIntervalBase = this.reproductionIntervalBase;
		g.color = this.color;
		g.species = this.species;
		g.isFounder = this.isFounder;
		return g;
	}
	
	public void Randomize() {
		isPlant = Random.Range(0, 2) == 1;

		size = Random.Range(cSizeMin, cSizeMax);

		playerHealthRestoreBase = Random.Range(cRestoreMin, cRestoreMax);

		if(isPlant) {
			swCircle = Random.value;
			swSquare = 0;
			swStar = Random.value;
			swStarCount = Random.Range(2,6);
			swStarPower = Random.Range(1.0f, 3.0f);
			swStarRadiusMin = Random.Range(0.1f, 0.67f);
			swRose = 0;
			swRosePetals = 2;
			swRose2 = Random.value;
			swRoseAbsPetals = Random.Range(1,6);
			swCardioid = 0;

			playerFollowStrength = 0.0f;

			bubbleInterval = Random.Range(cBubbleIntervalMin, cBubbleIntervalMax);
		}
		else {
			swCircle = 0;
			swSquare = Random.value;
			swStar = Random.value;
			swStarCount = Random.Range(2,8);
			swStarPower = Random.Range(2.0f, 6.0f);
			swStarRadiusMin = Random.Range(0.1f, 0.67f);
			swRose = Random.value;
			swRosePetals = Random.Range(1,6);
			swRose2 = 0;
			swRoseAbsPetals = 2;
			swCardioid = Random.value;

			playerFollowStrength = Random.Range(-1.0f, +1.0f);

			bubbleInterval = 10000000000000000.0f; // ...
		}

		UpdateColor();
	}
	
	static float mutateVar(float x, float min, float max, float a, float b) {
		return MoreMath.Clamp(x + Random.Range(a, b), min, max);
	}

	static float mutateVar(float x, float min, float max, float a) {
		return mutateVar(x, min, max, -a, +a);
	}

	static float mutateVar(float x, float a) {
		return mutateVar(x, 0.0f, 1.0f, -a, +a);
	}

	/** Add +1/-1 with probability of p */
	static int mutateIntOne(int x, int min, int max, float p) {
		if(Random.value < p) {
			int y = x;
			if(Random.Range(0,2) == 0)
				y --;
			else
				y ++;
			return MoreMath.Clamp(min, max, y);
		}
		else
			return x;
	}

	public void Mutate() {
		// decide if we have a low or a high mutation
		float mutationStrength = (Random.value < cHighMutationProbability)
				? cMutationStrengthHigh : cMutationStrengthLow;

		size = mutateVar(size, cSizeMin, cSizeMax, 0.1f*mutationStrength);

//			swCircle = mutateVar(swCircle, 0.1f*mutationStrength);
//			swSquare = mutateVar(swSquare, 0.1f*mutationStrength);
//			swStar = mutateVar(swStar, 0.1f*mutationStrength);
//			swRose = mutateVar(swRose, 0.1f*mutationStrength);
//			swCardioid = mutateVar(swCardioid, 0.1f*mutationStrength);
		if(isPlant) {
			swCircle = mutateVar(swCircle, 0.1f*mutationStrength);
			swSquare = 0;
			swStar = mutateVar(swStar, 0.1f*mutationStrength);
			swStarCount = mutateIntOne(swStarCount, 2, 6, 0.1f*mutationStrength);
			swStarPower = mutateVar(swStar, 1.0f, 3.0f, 0.2f*mutationStrength);
			swStarRadiusMin = mutateVar(swStar, 0.1f, 0.67f, 0.1f*mutationStrength);
			swRose = 0;
			swRosePetals = 2;
			swRose2 = mutateVar(swRose2, 0.1f*mutationStrength);
			swRoseAbsPetals = mutateIntOne(swStarCount, 1, 6, 0.1f*mutationStrength);
			swCardioid = 0;
		}
		else {
			swCircle = 0;
			swSquare = mutateVar(swSquare, 0.1f*mutationStrength);
			swStar = mutateVar(swStar, 0.1f*mutationStrength);
			swStarCount = mutateIntOne(swStarCount, 2, 8, 0.1f*mutationStrength);
			swStarPower = mutateVar(swStar, 2.0f, 6.0f, 0.2f*mutationStrength);
			swStarRadiusMin = mutateVar(swStar, 0.1f, 0.67f, 0.1f*mutationStrength);
			swRose = mutateVar(swRose, 0.1f*mutationStrength);
			swRosePetals = mutateIntOne(swStarCount, 1, 6, 0.1f*mutationStrength);
			swRose2 = 0;
			swRoseAbsPetals = 2;
			swCardioid = mutateVar(swCardioid, 0.1f*mutationStrength);
		}

		playerFollowStrength = mutateVar(playerFollowStrength, -1.0f, 1.0f, 0.1f*mutationStrength);

		playerHealthRestoreBase = mutateVar(playerHealthRestoreBase, cRestoreMin, cRestoreMax, 0.1f*mutationStrength);

		if(isPlant)
			bubbleInterval = mutateVar(bubbleInterval, cBubbleIntervalMin, cBubbleIntervalMax, 1.0f*mutationStrength);

		UpdateColor();
	}
	
	static float D(float x, float y, float scl=1.0f) {
		float q = x - y;
		return scl*q*q;
	}
	
	static float D(int x, int y, float scl=1.0f) {
		float q = (float)(x - y);
		return scl*q*q;
	}
	
	public float Difference(Genes other) {
		if(isPlant != other.isPlant) {
			return 1.0f;
		}
		float d =
			D(size, other.size, 1.0f)
			+ D(swCircle, other.swCircle, 1.0f)
			+ D(swSquare, other.swSquare, 1.0f)
			+ D(swStar, other.swStar, 1.0f)
			+ D(swStarCount, other.swStarCount, 1.0f)
			+ D(swStarPower, other.swStarPower, 1.0f)
			+ D(swStarRadiusMin, other.swStarRadiusMin, 1.0f)
			+ D(swRose, other.swRose, 1.0f)
			+ D(swRosePetals, other.swRosePetals, 1.0f)
			+ D(swRose2, other.swRose2, 1.0f)
			+ D(swRoseAbsPetals, other.swRoseAbsPetals, 1.0f)
			+ D(swCardioid, other.swCardioid, 1.0f)
			+ D(playerFollowStrength, other.playerFollowStrength, 3.0f)
			+ D(bubbleInterval, other.bubbleInterval, 3.0f)
			+ D(playerHealthRestoreBase, other.playerHealthRestoreBase, 3.0f);
		
		return Mathf.Sqrt(d / 10.0f);
	}
	
	public void UpdateColor() {
		float rnd = 0.05f;
		float q = MoreMath.Clamp((playerHealthRestoreBase - cRestoreMin) / (cRestoreMax - cRestoreMin));
		if(isPlant) {
//			color = ColorHSVtoRGB.FromHSL(
//				100.0f*(0.10f + (1.20f - 2.0f*rnd)*q + 2.0f*rnd*Random.value)/360.0f,
//				0.40f + (0.40f - 2.0f*rnd)*q + 2.0f*rnd*Random.value,
//				0.20f + (0.30f - 2.0f*rnd)*q + 2.0f*rnd*Random.value
//			);
			swStar = 1.0f - q*q; // gives more spikeiness to poisonous plants
			swCircle = 1.0f - swStar; // gives more roundness to healthy plants
			color = ColorHSVtoRGB.FromHSL(
				100.0f*(0.60f + (0.70f - 2.0f*rnd)*q + 2.0f*rnd*Random.value)/360.0f,
				0.40f + (0.50f - 2.0f*rnd)*q + 2.0f*rnd*Random.value,
				0.20f + (0.30f - 2.0f*rnd)*q + 2.0f*rnd*Random.value
			);
		}
		else {
			color = Color.Lerp(
				new Color(255.0f/255.0f, 212.0f/255.0f, 153.0f/255.0f),	// good
				new Color(255.0f/255.0f, 0.0f/255.0f, 161.0f/255.0f), // bad
				q*q - 2.0f*q + 1.0f
			);
		}

//		// color = health restore
//		color = MoreMath.IndicatorColor(playerHealthRestoreBase / 0.1f);
		// color = avoid player
//		color = MoreMath.IndicatorColorPosNeg(playerFollowStrength);
	}

}
