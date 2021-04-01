
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

namespace NineDots {

	// TODO: The set of segments might be better implemented as a single
	// gameobject/mesh, rather than several
	public class GameManager : MonoBehaviour {

		public GameObject dotPrefab;
		public bool facePositiveZ;
		public float zOffset;
		public Color segmentColor;
		public Color pendingSegmentColor;

		private string TAG_RAYTARGET = "RayTarget";
		private float UNITY_PLANE_UNITS = 10f;

		private OVRInputModule oim = null;
		private GameObject gamePlane = null;
		private float zValue;
		private Dictionary<GameObject, Dot> dots = new Dictionary<GameObject, Dot>();
		private List<LineSegment> segments = new List<LineSegment>();
		private LineSegment pendingSegment = null;
		private float lineThickness = 1f;
        private MeshRenderer myDotRenderer;

		void Awake() {
			GameObject eventSystem = GameObject.Find("/UIHelpers/EventSystem");
			oim = eventSystem.GetComponent<OVRInputModule>();
			gamePlane = GameObject.Find("/GameBoard/GamePlane");
			zValue = gamePlane.transform.localPosition.z + zOffset;
		}

		void Start() {
		}

		void Update() {
			if (pendingSegment == null)
				return;
			RaycastHit[] hits = Physics.RaycastAll(oim.rayTransform.position, oim.rayTransform.forward);
			foreach (RaycastHit h in hits) {
				if (IsRelevantTarget(h.collider.gameObject)) {
					Debug.Log("_______________ CHANGING PENDINGSEGMENT");
					Vector2 hit = new Vector2(h.point.x, h.point.y);
					pendingSegment.SetVertices(pendingSegment.p0, hit);
					pendingSegment.SetActive(true);
					return;
				}
			}
			pendingSegment.SetActive(false);
		}

		public void SetGamePlaneMaterial(Material material) {
			gamePlane.GetComponent<Renderer>().material = material;
		}

		private bool IsRelevantTarget(GameObject gobj) {
			if (gobj.CompareTag(TAG_RAYTARGET))
				return true;
			return false;
		}

		public void SetLineThickness(float thickness) {
			lineThickness = thickness;
		}

		public void HandleObjectClick(RaycastHit[] hits) {
			foreach (RaycastHit hit in hits) {
				if (IsRelevantTarget(hit.collider.gameObject)) {
					AddVertex(new Vector2(hit.point.x, hit.point.y));
					return;
				}
			}
		}

		public void DotEntered(GameObject dobj, Collider collider) {
			Dot dot = dots[dobj];
			GameObject cobj = collider.gameObject;
			foreach (LineSegment ls in segments) {
				if (ls.gobj == cobj) {
					dot.AddCollisionObject(ls.gobj);
					return;
				}
			}
		}

		public void DotExited(GameObject dobj, Collider collider) {
			Dot dot = dots[dobj];
			GameObject cobj = collider.gameObject;
			foreach (LineSegment ls in segments) {
				if (ls.gobj == cobj) {
					dot.RemoveCollisionObject(ls.gobj);
					return;
				}
			}
		}

		public int GetNumSegments() {
			return segments.Count;
		}

		public int GetNumDotCollisions() {
			int n = 0;
			foreach (Dot dot in dots.Values)
				if (dot.InCollision())
					++n;
			return n;
		}

		public void ClearDots() {
			foreach (Dot dot in dots.Values)
				dot.Destroy();
		}

		public void ShowDots(float spaceDotRatio, float borderDotsRatio) {
			ClearDots();
			dots = MakeDots(spaceDotRatio, borderDotsRatio);
		}

		public void ClearSegments() {
			foreach (LineSegment seg in segments)
				seg.Destroy();
			segments.Clear();
			foreach (Dot dot in dots.Values)
				dot.ClearCollisionObjects();
			if (pendingSegment != null) {
				pendingSegment.Destroy();
				pendingSegment = null;
			}
		}

		private void AddVertex(Vector2 point) {
			if (pendingSegment == null) {
				pendingSegment = new LineSegment(transform, point, point, zValue, pendingSegmentColor, facePositiveZ, lineThickness, null);
			}
			else {
				segments.Add(new LineSegment(transform, pendingSegment.p0, point, zValue, segmentColor, facePositiveZ, lineThickness, TAG_RAYTARGET));
				pendingSegment.SetVertices(point, point);
			}
		}

        //Used to start coroutine that scales the dots
        public void modifyMyDots()
        {
            //get each key for the dots from the dictonary
            foreach (GameObject myDot in dots.Keys)
            {
                //Start Coroutine to scale dots
                StartCoroutine(Scale(myDot, 1.0f));
            }
        }

        //Takes gameobject (such as dots), and a time (as a float) for how long to take
        public IEnumerator Scale(GameObject myObject, float howLong)
        {
            float t = 0.0f;
            //fixed scale factor
            Vector3 myScaleFactor = new Vector3(0.002f, 0.00f, 0.002f);
            while (t < howLong)
            {
                //add time taken for previous frame
                t += Time.deltaTime;
                //increase scale of object
                myObject.transform.localScale += myScaleFactor;
                yield return null;
            }
        }

		private Dictionary<GameObject, Dot> MakeDots(float spaceDotRatio, float borderDotsRatio) {
			Dictionary<GameObject, Dot> dotdict = new Dictionary<GameObject, Dot>();
			float planeWidth = UNITY_PLANE_UNITS * gamePlane.transform.localScale.x;
			float planeHeight = UNITY_PLANE_UNITS * gamePlane.transform.localScale.y;
			float planeSize = Math.Min(planeWidth, planeHeight);
			float dotAreaSize = planeSize / (borderDotsRatio + 1f);
			float dotScale = (1f / ((4f * spaceDotRatio) + 3f)) * dotAreaSize;
			float dotOffset = 2f * dotAreaSize * (1f / (4f + 3f));
			for (int j=-1; j<=1; ++j) {
				for (int i=-1; i<=1; ++i) {
					Vector3 pos = new Vector3(
						gamePlane.transform.position.x + (((float)i) * dotOffset),
						gamePlane.transform.position.y + (((float)j) * dotOffset),
						zValue
					);
					GameObject dobj = Instantiate(dotPrefab, pos, Quaternion.identity, transform);
					dobj.tag = TAG_RAYTARGET;
					dobj.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
					dobj.transform.localScale = new Vector3(dotScale, dotScale*0.01f, dotScale);
					dotdict[dobj] = new Dot(dobj);
				}
			}
			return dotdict;
		}

	}

}
