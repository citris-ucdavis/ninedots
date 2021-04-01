
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

namespace NineDots {

	public class Main : MonoBehaviour {

		enum FsmState {
			START,
			STAGE1,
			STAGE1_INCORRECT,
            STAGE2,
			STAGE2_INCORRECT,
			STAGE3,
			STAGE3_SOLUTION
		}

		public Text topPromptText;
		public Text bottomPromptText;
		public Button restartButton;
		public Button clearButton;
		public Button okButton;
		public Button submitButton;
		public Button exitButton;
        public GameObject speechBubble;
		public Material gameBoardBlankMaterial;
		public Material gameBoardOneLineMaterial;

        private static int STAGE1_MAX_SEGS = 4;
		private static int STAGE2_MAX_SEGS = 3;
		private static readonly string STAGE1_PROMPT_A =
			"<B>Can a series of four or fewer contiguous line segments be drawn " +
			"such that they pass through all nine dots on the gameboard above?</B>";
		private static readonly string STAGE12_PROMPT_B =
			"<B>Click the trigger on the gameboard to begin drawing.\nPress the " +
			"\"Submit\" button once you are done,\nor press the \"Clear\" button to " +
			"clear the canvas and begin again.</B>";
		private static readonly string STAGE12_DOTSOK_SEGSBAD_PROMPT =
			"Sorry, you covered all nine dots but you used too many line segments " +
			"to do so.";
		private static readonly string STAGE12_DOTSBAD_SEGSOK_PROMPT =
			"Sorry, you did not cover all nine dots.";
		private static readonly string STAGE12_DOTSBAD_SEGSBAD_PROMPT =
			"Sorry, you did not cover all nine dots. You also used too many line " +
			"segments.";
		private static readonly string STAGE12_INCORRECT_PROMPT_B =
			"Press \"OK\" to try again!";
		private static readonly string STAGE2_PROMPT_A =
			"<B>Correct! Now, can you do it with <I>only three</I>?\nTry to create " +
			"a series of three line segments\nthat pass through all nine dots.</B>";
		private static readonly string STAGE3_PROMPT_A =
			"<B>Correct! How about a <I>single</I> line? Is this possible?</B>";
		private static readonly string STAGE3_PROMPT_B =
			"<B>Think about how you might do this, then press the \"OK\" button " +
			"to see one possible solution.</B>";
		private static readonly string STAGE3_SOLUTION_PROMPT_A =
			"<B>It can be done! Simply fold the gameboard and embed it in a higher dimensional space" +
            " to line up all nine dots. Now one line can pass through all the dots.</B>";
		private static readonly string STAGE3_SOLUTION_PROMPT_B =
			"<B>Think outside the box!\n:-)</B>";
        private FsmState fsmState = FsmState.START;
        //Other classes used
        private GameManager gameManager = null;
        private BotMoveController myBotController = null;

		void Awake() {
			// TODO: checks
			GameObject gb = GameObject.Find("/GameBoard");
			gameManager = gb.GetComponent<GameManager>();
			GameObject ovrcr = GameObject.Find("/LocalAvatarWithGrab/OVRCameraRig");
			ovrcr.AddComponent<OVRPhysicsRaycaster>();
            //Sean added for robot
            GameObject robot = GameObject.Find("/ybot@Clapping");
            myBotController = robot.GetComponent<BotMoveController>();
        }

		void Start() {
            gameManager.SetLineThickness(0.025f);
			UpdateState(FsmState.STAGE1);
            //get the speech bubble and make it invisible 
            speechBubble = GameObject.Find("SpeechBubble");
            speechBubble.SetActive(false);
        }

        void Update() {

        }

		public void HandleRestartButtonClick() {
            speechBubble.SetActive(false);
            UpdateState(FsmState.STAGE1);
        }

		public void HandleOkButtonClick() {
			switch(fsmState) {
				case FsmState.STAGE1_INCORRECT:
					UpdateState(FsmState.STAGE1);
					break;
				case FsmState.STAGE2_INCORRECT:
					UpdateState(FsmState.STAGE2);
					break;
				case FsmState.STAGE3:
					UpdateState(FsmState.STAGE3_SOLUTION);
					break;
			}
		}

		public void HandleSubmitButtonClick() {
            switch (fsmState)
            {
                case FsmState.STAGE1:
                    if (Validate(STAGE1_MAX_SEGS) == null)
                    {
                        UpdateState(FsmState.STAGE2);
                        //Sean make robot clap since you were right
                        myBotController.MakeClap();
                    }
                    else
                        UpdateState(FsmState.STAGE1_INCORRECT);
                    break;
                case FsmState.STAGE2:
                    if (Validate(STAGE2_MAX_SEGS) == null)
                    { 
                        UpdateState(FsmState.STAGE3);
                        //Sean make robot cheer since you were right
                        myBotController.MakeCheer();
                    }
					else
						UpdateState(FsmState.STAGE2_INCORRECT);
					break;
			}
		}

        public void HandleClearButtonClick() {
			gameManager.ClearSegments();
		}

		public void HandleExitButtonClick() {
			Application.Quit();
		}

        public void GrowDots()
        {
            //make speech bubble visible
            speechBubble.SetActive(true);
            //animate the change in dot scale
            gameManager.modifyMyDots();
        }

        private void UpdateState(FsmState newState) {
			switch(newState) {
				case FsmState.STAGE1:
					gameManager.SetGamePlaneMaterial(gameBoardBlankMaterial);
					gameManager.ClearSegments();
					gameManager.ShowDots(10f, 0.3f);
					topPromptText.text = STAGE1_PROMPT_A;
					bottomPromptText.text = STAGE12_PROMPT_B;
					restartButton.gameObject.SetActive(true);
					clearButton.gameObject.SetActive(true);
					okButton.gameObject.SetActive(false);
					submitButton.gameObject.SetActive(true);
					exitButton.gameObject.SetActive(true);
					fsmState = FsmState.STAGE1;
					break;
				case FsmState.STAGE1_INCORRECT:
					topPromptText.text = Validate(STAGE1_MAX_SEGS);
					bottomPromptText.text = STAGE12_INCORRECT_PROMPT_B;
					restartButton.gameObject.SetActive(true);
					clearButton.gameObject.SetActive(false);
					okButton.gameObject.SetActive(true);
					submitButton.gameObject.SetActive(false);
					exitButton.gameObject.SetActive(true);
					fsmState = FsmState.STAGE1_INCORRECT;
					break;
                case FsmState.STAGE2:
					gameManager.SetGamePlaneMaterial(gameBoardBlankMaterial);
					gameManager.ClearSegments();
                    gameManager.ShowDots(10f, 0.3f); //start same size as stage one
                    Invoke("GrowDots",15); //Use invoke to make "grow dots" happen after 15 seconds
                    topPromptText.text = STAGE2_PROMPT_A;
					bottomPromptText.text = STAGE12_PROMPT_B;
					restartButton.gameObject.SetActive(true);
					clearButton.gameObject.SetActive(true);
					okButton.gameObject.SetActive(false);
					submitButton.gameObject.SetActive(true);
					exitButton.gameObject.SetActive(true);
					fsmState = FsmState.STAGE2;
					break;
				case FsmState.STAGE2_INCORRECT:
					topPromptText.text = Validate(STAGE2_MAX_SEGS);
					bottomPromptText.text = STAGE12_INCORRECT_PROMPT_B;
					restartButton.gameObject.SetActive(true);
					clearButton.gameObject.SetActive(false);
					okButton.gameObject.SetActive(true);
					submitButton.gameObject.SetActive(false);
					exitButton.gameObject.SetActive(true);
					fsmState = FsmState.STAGE2_INCORRECT;
					break;
				case FsmState.STAGE3:
					gameManager.SetGamePlaneMaterial(gameBoardBlankMaterial);
					gameManager.ClearSegments();
					gameManager.ClearDots();
                    speechBubble.SetActive(false);
                    topPromptText.text = STAGE3_PROMPT_A;
					bottomPromptText.text = STAGE3_PROMPT_B;
					restartButton.gameObject.SetActive(true);
					clearButton.gameObject.SetActive(false);
					okButton.gameObject.SetActive(true);
					submitButton.gameObject.SetActive(false);
					exitButton.gameObject.SetActive(true);
					fsmState = FsmState.STAGE3;
					break;
				case FsmState.STAGE3_SOLUTION:
					gameManager.SetGamePlaneMaterial(gameBoardOneLineMaterial);
					topPromptText.text = STAGE3_SOLUTION_PROMPT_A;
					bottomPromptText.text = STAGE3_SOLUTION_PROMPT_B;
					restartButton.gameObject.SetActive(true);
					clearButton.gameObject.SetActive(false);
					okButton.gameObject.SetActive(false);
					submitButton.gameObject.SetActive(false);
					exitButton.gameObject.SetActive(true);
					fsmState = FsmState.STAGE3_SOLUTION;
					break;
				default:
					break;
			}
		}

		// returns null if valid, else an appropriate error message
		private string Validate(int maxSegs) {
			bool dotsOk = (gameManager.GetNumDotCollisions() >= 9);
			bool segsOk = (gameManager.GetNumSegments() <= maxSegs);
			if ((dotsOk) && (segsOk))
				return null;
			if ((dotsOk) && (!segsOk))
				return STAGE12_DOTSOK_SEGSBAD_PROMPT;
			if ((!dotsOk) && (segsOk))
				return STAGE12_DOTSBAD_SEGSOK_PROMPT;
			else
				return STAGE12_DOTSBAD_SEGSBAD_PROMPT;
		}

	}

}
