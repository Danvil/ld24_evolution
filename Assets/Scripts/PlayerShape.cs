using UnityEngine;
using System.Collections;

public class PlayerShape : MonoBehaviour {

	public Color color = Color.red;

	const int cVertexCount = 6;

	const float cTPrepare = 0.4f;
	const float cTGo = 0.1f;
	const float cTRelease = 0.2f;
	public const float cAttackLength = cTPrepare + cTGo + cTRelease;

	public bool isAttacking = false;
	public float attackTime = 0.0f;

	Mesh generateMesh(float scl, int smooth, float midh, bool genColor) {
		Mesh mesh = new Mesh();
		// vertices
		float y = 0.0f;
		if(isAttacking) {	
			float t = attackTime % cAttackLength;
			if(t < cTPrepare) {
				// prepare phase
				float q = t / cTPrepare;
				y = -q*q;
			}
			else if(t < cTPrepare + cTGo) {
				// go phase
				float q = (t - cTPrepare) / cTGo;
				y = 2.0f * q - 1.0f;
			}
			else {
				// release phase
				float q = (t - cTPrepare - cTGo) / cTRelease;
				y = 1.0f - q;
			}
		}
		else {
			y = 0.0f;
		}
		float a = 1.0f - 0.5f*y;
		float b = 1.0f + 0.5f*y;
		attackPoint = scl * new Vector3(+1*b,0,0); // HACK
		Vector3[] vertices = new Vector3[cVertexCount] {
			scl * new Vector3(-0.5f,0,midh/scl),
			scl * new Vector3(-1,+0.5f,0),
			scl * new Vector3(0,+0.25f*a,0),
			attackPoint,
			scl * new Vector3(0,-0.25f*a,0),
			scl * new Vector3(-1,-0.5f,0)
		};
		if(smooth > 0) {
			Vector3[] verticesRaw = new Vector3[cVertexCount];
			for(int i=0; i<cVertexCount; i++) {
				verticesRaw[i] = vertices[i];
			}			
			for(int i=0; i<cVertexCount; i++) {
				float total = 0;
				float wTotal = 0.0f;
				for(int j=-smooth; j<=+smooth; j++) {
					float w = 1.0f / (1.0f + Mathf.Abs((float)j));
					wTotal += w;
					// int k = (i+j) % cVertexCount ???
					int k = i + j;
					while(k < 0) k += cVertexCount;
					while(k >= cVertexCount) k -= cVertexCount;
					total += w * verticesRaw[k].magnitude;
				}
				vertices[i] = total / wTotal * verticesRaw[i].normalized;
			}
		}
		mesh.vertices = vertices;
		// uv
		Vector2[] uv = new Vector2[cVertexCount];
		for(int i=0; i<cVertexCount; i++) {
			float phi = 2.0f * Mathf.PI * (float)i / (float)cVertexCount;
			uv[i] = new Vector2(Mathf.Cos(phi), Mathf.Sin(phi));
		}
		mesh.uv = uv;
		// vertex colors
		if(genColor) {
			Color[] colors = new Color[cVertexCount];
			for(int i=0; i<cVertexCount; i++) {
				colors[i] = color;
			}
			mesh.colors = colors;
		}
		// indices
		int[] indices = new int[3*cVertexCount];
		for(int i=0; i<cVertexCount; i++) {
			indices[3*i] = 0;
			indices[3*i + 1] = i;
			indices[3*i + 2] = (i+1) % cVertexCount;
		}
		mesh.triangles = indices;
		mesh.RecalculateNormals();
		return mesh;
	}

	Mesh generateDiffuseMesh() {
		return generateMesh(1.0f, 0, -0.5f, true);
	}

	Mesh generateShadowMesh() {
		return generateMesh(1.5f, 2, 0.0f, false);
	}

	void createMesh() {
		Mesh mesh = generateDiffuseMesh();
		// update shadow mesh
		// HACK update shadow mesh first, or attackPoint will be wrong!
		var shadow = this.transform.Find("DropShadow");
		if(shadow) {
			var shadowMeshFilter = shadow.gameObject.GetComponent<MeshFilter>();
			if(shadowMeshFilter)
				shadowMeshFilter.mesh = generateShadowMesh();
		}
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
	}

	void updateMesh() {
		createMesh();
	}

	Vector3 attackPoint = Vector3.zero;

	public void Attack() {
		if(!isAttacking) {
			isAttacking = true;
			attackTime = 0.0f;
		}
	}

	public bool IsAttacking {
		get {
			return isAttacking;
		}
	}

	public bool IsDeadly {
		get {
			return isAttacking && cTPrepare <= attackTime && attackTime <= cTPrepare + cTGo;
		}
	}

	public bool CanMove {
		get {
			return !isAttacking || attackTime > cTPrepare;
		}
	}

	public Vector3 LocalAttackPoint {
		get {
			return attackPoint;
		}
	}

	// Use this for initialization
	void Start () {
		createMesh();
	}
	
	// Update is called once per frame
	void Update () {
		if(isAttacking) {
			attackTime += MyTime.deltaTime;
			if(attackTime > cAttackLength) {
				isAttacking = false;
			}
		}
		updateMesh();
	}
}
