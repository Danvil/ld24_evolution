using UnityEngine;
using System.Collections;

public static class MoreMesh
{

	public static Mesh ExtrudeMesh(Mesh mesh) {
		int n = mesh.vertices.Length;
		Mesh result = new Mesh();
		Vector3[] vertices = new Vector3[2*n];
		for(int i=0; i<n; i++) {
			Vector3 v = mesh.vertices[i];
			vertices[2*i] = v;
			v.z = 1.0f;
			vertices[2*i + 1] = v;
		}
		result.vertices = vertices;
		Color[] colors = new Color[2*n];
		for(int i=0; i<n; i++) {
			Color c = mesh.colors[i];
			colors[2*i] = c;
			colors[2*i + 1] = c;
		}
		result.colors = colors;
		
		int[] indices = new int[2*n];// + 2*(n-1)];
		for(int i=0; i<n; i++) {
			indices[3*i] = mesh.triangles[3*i];
			indices[3*i+1] = mesh.triangles[3*i+1];
			indices[3*i+2] = mesh.triangles[3*i+2];
			indices[3*n+3*i] = n + mesh.triangles[3*i];
			indices[3*n+3*i+1] = n + mesh.triangles[3*i+1];
			indices[3*n+3*i+2] = n + mesh.triangles[3*i+2];
		}
		result.triangles = indices;
		return result;
	}

}
