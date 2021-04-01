
using System.Collections.Generic;
using System;
using UnityEngine;


namespace NineDots {

	public class Dot {

		public GameObject gobj;
		private HashSet<GameObject> collisionObjects = new HashSet<GameObject>();

		public Dot(GameObject gobj) {
			this.gobj = gobj;
		}

		public void Destroy() {
			UnityEngine.Object.Destroy(gobj);
		}

		public void AddCollisionObject(GameObject cobj) {
			collisionObjects.Add(cobj);
		}

		public void RemoveCollisionObject(GameObject cobj) {
			collisionObjects.Remove(cobj);
		}

		public void ClearCollisionObjects() {
			collisionObjects.Clear();
		}

		public bool InCollision() {
			return (collisionObjects.Count > 0);
		}

	}

}
