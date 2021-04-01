
using UnityEngine;
using UnityEngine.EventSystems;


namespace NineDots {

	public class GameBoardEventTrigger : EventTrigger {

		private GameManager gameManager = null;
		private OVRInputModule oim = null;

		void Awake() {
			GameObject gb = GameObject.Find("/GameBoard");
			gameManager = gb.GetComponent<GameManager>();
		}

		void Start() {
			GameObject eventSystem = GameObject.Find("/UIHelpers/EventSystem");
			oim = eventSystem.GetComponent<OVRInputModule>();
		}

		public override void OnPointerClick(PointerEventData data) {
			RaycastHit[] hits = Physics.RaycastAll(oim.rayTransform.position, oim.rayTransform.forward);
			gameManager.HandleObjectClick(hits);
		}

	}

}
