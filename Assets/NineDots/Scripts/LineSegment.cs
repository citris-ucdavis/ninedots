
using System.Collections.Generic;
using System;
using UnityEngine;


namespace NineDots {

	public class LineSegment {

		public GameObject gobj;
		public Vector2 p0;
		public Vector2 p1;
		private float z;
		private bool facePositiveZ;
		private float thickness;

		public LineSegment(Transform parent, Vector2 p0, Vector2 p1, float z, Color color, bool facePositiveZ, float thickness, string tag) {
			this.p0 = p0;
			this.p1 = p1;
			this.z = z;
			this.facePositiveZ = facePositiveZ;
			this.thickness = thickness;
			gobj = new GameObject("LineSegment");
			if (tag != null)
				gobj.tag = tag;
			gobj.transform.parent = parent;
			gobj.AddComponent<MeshFilter>();
			gobj.AddComponent<MeshRenderer>();
			gobj.AddComponent<MeshCollider>();
			gobj.GetComponent<MeshCollider>().isTrigger = false;
			gobj.AddComponent<Rigidbody>();
			gobj.GetComponent<Rigidbody>().useGravity = false;
			gobj.GetComponent<Rigidbody>().isKinematic = true;
			gobj.GetComponent<MeshFilter>().mesh = CreateMesh();
			gobj.GetComponent<Renderer>().material.color = color;
			Shader shader = Shader.Find("Diffuse");
			gobj.GetComponent<Renderer>().material.shader = shader;
		}

		public void Destroy() {
			UnityEngine.Object.Destroy(gobj);
		}

		public void SetActive(bool active) {
			if (active != gobj.activeSelf)
				gobj.SetActive(active);
		}

		public void SetVertices(Vector2 p0, Vector2 p1) {
			this.p0 = p0;
			this.p1 = p1;
			AdjustMesh(gobj.GetComponent<MeshFilter>().mesh, p0, p1);
		}

		private Mesh CreateMesh() {
			Mesh mesh = new Mesh();
			AdjustMesh(mesh, p0, p1);
			Vector3 meshNorm = facePositiveZ ? Vector3.forward : Vector3.back;
			mesh.normals = new Vector3[4] {meshNorm, meshNorm, meshNorm, meshNorm};
			Vector2[] uvs = new Vector2[4];
			uvs[0] = new Vector2(0, 0);
			uvs[1] = new Vector2(0, 1);
			uvs[2] = new Vector2(1, 0);
			uvs[3] = new Vector2(1, 1);
			mesh.uv = uvs;
			mesh.triangles = facePositiveZ ? (new int[6]{0,1,2,1,3,2}) : new int[6]{0,2,1,2,3,1};
			gobj.GetComponent<MeshCollider>().sharedMesh = mesh;
			return mesh;
		}

		private void AdjustMesh(Mesh mesh, Vector2 p0, Vector2 p1) {
			Vector2 line = p1 - p0;
			Vector2 norm = new Vector2(-line.y, line.x).normalized;
			Vector2 a = p0 - (norm * thickness / 2f);
			Vector2 b = p1 - (norm * thickness / 2f);
			Vector2 c = p0 + (norm * thickness / 2f);
			Vector2 d = p1 + (norm * thickness / 2f);
			mesh.vertices = new Vector3[4] {
				new Vector3(a.x, a.y, z),
				new Vector3(b.x, b.y, z),
				new Vector3(c.x, c.y, z),
				new Vector3(d.x, d.y, z)
			};
			mesh.RecalculateBounds();
			gobj.GetComponent<MeshCollider>().sharedMesh = mesh;
		}

	}

}
