
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;


namespace RelatedPair {

	public class Main : MonoBehaviour {

		enum FsmState {
			START,
			PROMPT_PUZZLE,
			PROMPT_CORRECT,
			PROMPT_INCORRECT
		}

		public string puzzlePrompt;
		public string correctPrompt;
		public string incorrectPrompt;
		public Text promptText;
		public Button noButton;
		public Button yesButton;
		public Button resetButton;
		public Button okButton;
		public Button exitButton;
		public GameObject interactiveContainerPrefab;
		public GameObject leftSpawn;
		public GameObject rightSpawn;

		private static System.Random rand = new System.Random();
		private Puzzle[] puzzles;

		// game state
		private FsmState fsmState = FsmState.START;
		private int puzzleIdx = -1;
		private bool spawnLR;
		private GameObject object1Instance = null;
		private GameObject object2Instance = null;

		void Start() {
			puzzles = gameObject.GetComponents<Puzzle>();
			ResetPuzzleObjects();
			UpdateState(FsmState.PROMPT_PUZZLE);
		}

		public void HandleNoButtonClick() {
			if (fsmState != FsmState.PROMPT_PUZZLE)
				return;
			if (puzzles[puzzleIdx].match)
				UpdateState(FsmState.PROMPT_INCORRECT);
			else
				UpdateState(FsmState.PROMPT_CORRECT);
		}

		public void HandleYesButtonClick() {
			if (fsmState != FsmState.PROMPT_PUZZLE)
				return;
			if (puzzles[puzzleIdx].match)
				UpdateState(FsmState.PROMPT_CORRECT);
			else
				UpdateState(FsmState.PROMPT_INCORRECT);
		}

		public void HandleResetButtonClick() {
			ResetPuzzleObjects();
			UpdateState(FsmState.PROMPT_PUZZLE);
		}

		public void HandleOkButtonClick() {
			if (fsmState == FsmState.PROMPT_CORRECT) {
				ResetPuzzleObjects();
				UpdateState(FsmState.PROMPT_PUZZLE);
			}
			else if (fsmState == FsmState.PROMPT_INCORRECT) {
				UpdateState(FsmState.PROMPT_PUZZLE);
			}
			else {
				return;
			}
		}

		public void HandleExitButtonClick() {
			Application.Quit();
		}

		private void ResetPuzzleObjects() {
			DeterminePuzzle();
			InstantiatePuzzleObjects();
		}

		private void UpdateState(FsmState state) {
			switch(state) {
				case FsmState.PROMPT_PUZZLE:
					promptText.text = puzzlePrompt;
					fsmState = FsmState.PROMPT_PUZZLE;
					UpdateButtons();
					break;
				case FsmState.PROMPT_CORRECT:
					promptText.text = correctPrompt;
					fsmState = FsmState.PROMPT_CORRECT;
					UpdateButtons();
					break;
				case FsmState.PROMPT_INCORRECT:
					promptText.text = incorrectPrompt;
					fsmState = FsmState.PROMPT_INCORRECT;
					UpdateButtons();
					break;
			}
		}

		private void UpdateButtons() {
			switch(fsmState) {
				case FsmState.PROMPT_PUZZLE:
					noButton.gameObject.SetActive(true);
					yesButton.gameObject.SetActive(true);
					resetButton.gameObject.SetActive(true);
					okButton.gameObject.SetActive(false);
					exitButton.gameObject.SetActive(true);
					break;
				case FsmState.PROMPT_CORRECT:
				case FsmState.PROMPT_INCORRECT:
					noButton.gameObject.SetActive(false);
					yesButton.gameObject.SetActive(false);
					resetButton.gameObject.SetActive(false);
					okButton.gameObject.SetActive(true);
					exitButton.gameObject.SetActive(false);
					break;
			}
		}

		private void DeterminePuzzle() {
			puzzleIdx = rand.Next(puzzles.Length);
			spawnLR = (rand.Next(2) == 0);
		}

		private void InstantiatePuzzleObjects() {
			GameObject object1Spawn = spawnLR ? leftSpawn : rightSpawn;
			GameObject object2Spawn = spawnLR ? rightSpawn : leftSpawn;
			if (object1Instance != null)
				Destroy(object1Instance);
			object1Instance = InstantiatePrefab(
				puzzles[puzzleIdx].object1Prefab,
				object1Spawn,
				true
			);
			if (object2Instance != null)
				Destroy(object2Instance);
			object2Instance = InstantiatePrefab(
				puzzles[puzzleIdx].object2Prefab,
				object2Spawn,
				true
			);
		}

		private GameObject InstantiatePrefab(GameObject prefab, GameObject spawn, bool interactive) {
			if (interactive) {
				GameObject widget = Instantiate(prefab, spawn.transform);
				widget.transform.localPosition = Vector3.zero;
				GameObject container = Instantiate(interactiveContainerPrefab, spawn.transform);
				container.transform.localPosition = Vector3.zero;
				container.transform.localRotation = widget.transform.localRotation;
				container.transform.localScale = widget.transform.localScale;
				BoxCollider bc = container.GetComponent<BoxCollider>();
				MeshFilter mf = widget.GetComponent<MeshFilter>();
				bc.size = mf.mesh.bounds.size;
				bc.center = mf.mesh.bounds.center;
				widget.transform.parent = container.transform;
				return container;
			}
			else {
				GameObject widget = Instantiate(prefab, spawn.transform);
				return widget;
			}
		}

	}

}
