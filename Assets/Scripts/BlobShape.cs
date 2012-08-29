using UnityEngine;
using System.Collections;
using System.Linq;

public class BlobShape : MonoBehaviour {

	const int N = 24; // LD24!

	public bool isUpdateMesh = false;

	public float middleHeight = 0.5f;

	static float R_circle(float x) {
		return 1.0f;
	}

	static float R_square(float x) {
//		while(x < 0) x += 2.0f * Mathf.PI;
		const float w = 0.5f * Mathf.PI;
		x = x % w;
		if(x >= 0.5f * w) {
			x = w - x;
		}
		return 1.0f / Mathf.Cos(x);
	}

	static float R_star(float x) {
		const float w = 2.0f * Mathf.PI / 6.0f;
		x = (x % w) / w;
		const float R1 = 0.33f;
		const float R2 = 1.00f;
		float q = 1.0f - 2.0f * Mathf.Abs(x - 0.5f);
		q = q * q;
		return (1.0f - q) * R1 + q * R2;
	}

	static float R_rose(float x) {
		return Mathf.Cos((float)2.0f * x);
	}

	static float R_rose2(float x) {
		return Mathf.Abs(Mathf.Cos((float)2.0f * x));
	}

	static float R_cardioid(float x) {
		return 1.0f + Mathf.Cos(x + Mathf.PI);
	}

	public Genes Genes { get; set; }

	float R_total(float x) {
		float[] radii = new float[] {
			R_circle(x), R_square(x), R_star(x), R_rose(x), R_rose2(x), R_cardioid(x)
		};
		float[] weights = new float[] {
			 Genes.swCircle,  Genes.swSquare,  Genes.swStar,  Genes.swRose,  Genes.swRose2,  Genes.swCardioid
		};
		float weightTotal = weights.Sum();
		for(int i=0; i<weights.Length; i++) {
			float q = weights[i] / weightTotal;
			weights[i] = q*q;
		}
		weightTotal = weights.Sum();
		for(int i=0; i<weights.Length; i++) {
			weights[i] /= weightTotal;
		}
		float result = 0.0f;
		for(int i=0; i<weights.Length; i++) {
			result += weights[i] * radii[i];
		}
		return result;
	}

	Mesh generateMesh(float scl, float rOffset, int smooth, float midh, bool genColor) {
		Mesh mesh = new Mesh();
		// vertices
		Vector3[] vertices = new Vector3[N+1];
		Color[] colors = new Color[N+1];
		const float phiScl = 2.0f * Mathf.PI / (float)N;
		for(int i=0; i<N; i++) {
			float r = 0.0f;
			float wTotal = 0.0f;
			for(int j=-smooth; j<=+smooth; j++) {
				float phi = (float)(i+j) * phiScl;
				float w = 1.0f / (1.0f + Mathf.Abs((float)j));
				wTotal += w;
				r += w * R_total(phi);
			}
			r /= wTotal;
			float phi0 = ((float)i) * phiScl;
			vertices[i] = (scl * r + rOffset) * new Vector3(Mathf.Cos(phi0), Mathf.Sin(phi0), 0);
			// make color a bit darker for high second derivative in radius
			float r0 = R_total(phi0 - phiScl);
			float r1 = R_total(phi0 + phiScl);
			float deriv2 = (r0 + r1) - 2.0f*r;
			float q = 0.2f + 0.8f * MoreMath.Clamp(-deriv2/0.5f, -0.2f, +0.2f);
			colors[i] = q * Genes.color;
		};
		vertices[N] = new Vector3(0,0,midh);
		mesh.vertices = vertices;
		colors[N] = Genes.color;
		if(genColor) {
			mesh.colors = colors;
		}
		// indices
		int[] indices = new int[3*N];
		for(int i=0; i<N; i++) {
			indices[3*i] = N;
			indices[3*i + 1] = (i+1) % N;
			indices[3*i + 2] = i;
		}
		mesh.triangles = indices;
		// uv
		Vector2[] uv = new Vector2[N+1];
		for(int i=0; i<N; i++) {
			float phi = phiScl * (float)i;
			uv[i] = new Vector2(Mathf.Cos(phi), Mathf.Sin(phi));
		}
		mesh.uv = uv;
		mesh.RecalculateNormals();
		return mesh;
	}

	Mesh generateDiffuseMesh() {
		return generateMesh(1.0f, 0.0f, 0, -middleHeight, true);
	}

	Mesh generateShadowMesh() {
		return generateMesh(1.0f, 0.25f, 2, 0.0f, false);
	}

	public void CreateMesh() {
		Mesh mesh = generateDiffuseMesh();
		// update mesh filter
		var meshFilter = GetComponent<MeshFilter>();
		if(meshFilter) {
			meshFilter.mesh = mesh;
		}
		// update mesh collider
		var meshCollider = GetComponent<MeshCollider>();
		if(meshCollider) {
			meshCollider.sharedMesh = mesh;
		}
		// update shadow mesh
		var shadow = this.transform.Find("DropShadow");
		if(shadow) {
			var shadowMeshFilter = shadow.gameObject.GetComponent<MeshFilter>();
			if(shadowMeshFilter)
				shadowMeshFilter.mesh = generateShadowMesh();
		}
	}

//	void UpdateMesh() {
//		Mesh mesh = GetComponent<MeshFilter>().mesh;
//		// vertices
//		Vector3[] vertices = mesh.vertices;
//		for(int i=0; i<N; i++) {
//			float phi = 2.0f * Mathf.PI * (float)i / (float)N;
//			float r = R_total(phi);
//			vertices[i] = r * new Vector3(Mathf.Cos(phi),Mathf.Sin(phi),0);
//		};
//		vertices[N] = new Vector3(0,0,0);
//		// vertex colors
//		Color32[] colors32 = mesh.colors32;
//		for(int i=0; i<N+1; i++) {
//			colors32[i] = new Color32(69,141,225,255);
//		}
//		mesh.colors32 = colors32;
//		mesh.RecalculateNormals();
//	}

	// Use this for initialization
	void Start () {
		CreateMesh();
	}
	
	// Update is called once per frame
	void Update () {
		if(isUpdateMesh)
			CreateMesh();
		this.transform.localScale = Genes.size * Vector3.one;
	}
}
