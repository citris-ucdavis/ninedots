
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;


namespace NineDots {

	public class DotScript : MonoBehaviour {

		private GameManager gameManager = null;
		//private HashSet<GameObject> collisionObjects = new HashSet<GameObject>();

		void Awake() {
			GameObject gmgr = GameObject.Find("/GameBoard");
			gameManager = gmgr.GetComponent<GameManager>();
		}

		private void OnTriggerEnter(Collider other) {
			gameManager.DotEntered(gameObject, other);
		}

		private void OnTriggerExit(Collider other) {
			gameManager.DotExited(gameObject, other);
		}

	}

}
