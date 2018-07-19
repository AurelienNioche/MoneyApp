using System.Collections;
using UnityEngine;
using System;
using AssemblyCSharp;


public class Good {
	public const int wood = 0;
	public const int wheat = 1;
	public const int stone = 2;
	public const int clay = 3;
}


public class GameStep {
	public static string training = "training";
	public static string survey = "survey";
	public static string game = "game";
	public static string end = "end";
}


public class GameController : MonoBehaviour {

    public string version;

	UIController uiController;
	UITutorial uiTutorial;
	UIProgressBars uiProgressBars;
	UIButtons uiButtons;
	Survey survey;

	TL state;

	Client client;


	bool choiceMade; 
	bool success;
	bool end;

	int t; 
	int tMax; 
	int goodInHand; 
	int goodDesired; 
	int score;

	// -------------- Inherited from MonoBehavior ---------------------------- //

	void Awake () {

		uiController = GetComponent<UIController> ();
		uiTutorial = GetComponent<UITutorial> ();
		uiProgressBars = GetComponent<UIProgressBars> ();
		uiButtons = GetComponent<UIButtons> ();
		client = GetComponent<Client> ();
		survey = GetComponent<Survey> ();

		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		state = TL.HomeWU;
	}

	void Start () {
		uiController.HomeWU (version);
	}

	void Update () {}

	// ------------------------------ //

	void LogState() {
		Debug.Log ("[GameController] My state is '" + state + "'.");
	}

	IEnumerator ActionWithDelay (Action methodName, float seconds) {

		yield return new WaitForSeconds(seconds);
		methodName ();
	}

	// ----------- From UIManager ---------------- //

	public void UserReplied () {

		Debug.Log ("[GameController] User replied");

		switch (state) {

		case TL.HomeWU:
			uiController.HomeWS ();
			client.Init ();
			state = TL.InitWS;
			break;

		case TL.SurveyWU:

			if (survey.EvaluateUserData ()) {
			    survey.View (false);
                uiController.ShowLogo(visible: true, glow: true);
			    client.Survey (age: survey.GetAge(), sex:survey.GetSex());
			    state = TL.SurveyWS;
			} else {
				uiButtons.ShowNext ();
			}
			break;

		case TL.TutorialWU:

			if (!uiTutorial.IsLastStep ()) {
				uiTutorial.NextStep ();
			
			} else {
				uiTutorial.End ();
				uiController.TrainingBegin ();

				state = TL.TrainingStartWU;
			}
			break;
		
		case TL.TrainingStartWU:

			uiController.HideTrainingMsg ();

			BeginGame (training: true);
			break;

		case TL.TrainingChoiceWU:

			goodDesired = uiButtons.GetGoodChosen ();

			client.TrainingChoice (goodDesired);
			state = TL.TrainingChoiceWS;
			break;

		case TL.TrainingResultWU:
			
			BeginTurn (training:true);
			break;
		
		case TL.TrainingEndWU:

			uiProgressBars.ShowProgress (false);
			uiController.TrainingReady ();
			state = TL.TrainingReadyWU;
			break;

		case TL.TrainingReadyWU:
			
			uiTutorial.ShowText (false);
			uiProgressBars.StatusMessage (Texts.waitingOtherPlayers, glow: true);
			client.TrainingDone ();

			state = TL.TrainingDoneWS;
			break;

		case TL.GameChoiceWU:

			goodDesired = uiButtons.GetGoodChosen ();

			client.Choice (goodDesired);

			state = TL.GameChoiceWS;
			break;
		
		case TL.GameResultWU:

			BeginTurn ();
			break;
		}
	}

	// ----------- From Client ----------- //

	public void ServerReplied () {

		Debug.Log ("[GameController] Received response from server.");

		if (client.GetWait ()) {

			if (state == TL.SurveyWS || state == TL.TrainingDoneWS)  {
				uiController.ShowLogo (glow: true);
				uiProgressBars.StatusMessage (Texts.waitingOtherPlayers, glow: true);
			}

			uiProgressBars.ShowWaitingMessage (client.GetProgress ());

		} else {

			uiProgressBars.ShowStatus (false);
			uiController.ShowLogo(false);

			switch (state) {

			case TL.InitWS:

				// Initialize things
				uiController.Init (client.GetPseudo (), client.GetNGoods ());
					
				if (client.GetStep () == GameStep.training) {
					BeginTutorial ();
				} else if (client.GetStep () == GameStep.survey) {
					BeginSurvey ();
				} else if (client.GetStep () == GameStep.game) {
					BeginGame ();
				} else {
					throw new Exception (String.Format(
                            "[GameController] Step '{0}' was not expected.", 
                            client.GetStep()));
				}

				break;

			case TL.SurveyWS:
				
				BeginTutorial ();
				break;

			case TL.TrainingChoiceWS:

				success = client.GetTrainingSuccess ();
				t = client.GetTrainingT ();
				score = client.GetTrainingScore ();
				end = client.GetTrainingEnd ();

				uiProgressBars.UpdateRadial (t, tMax);
				uiController.ResultView (success, goodInHand, goodDesired);

				state = TL.TrainingResultWU;
				break;
			
			case TL.TrainingDoneWS:

				BeginGame ();
				break;
			
			case TL.GameChoiceWS:

				success = client.GetSuccess ();
				t = client.GetT ();
				score = client.GetScore ();
				end = client.GetEnd ();

				uiProgressBars.UpdateRadial (t, tMax);
				uiController.ResultView (success, goodInHand, goodDesired);

				state = TL.GameResultWU;
				break;
			}
		}
	}

    public void OnDisconnection () {
        uiController.ShowConnected(false);
    }

    public void OnConnection () {
        uiController.ShowConnected ();
    }

    // -------------------------- //

    void BeginTutorial () {
		
		uiController.ShowLogo (false);
		uiProgressBars.ShowStatus (false);
		uiTutorial.Begin ();
		state = TL.TutorialWU;
	}

	void BeginGame (bool training=false) {

		if (training) {
			choiceMade = client.GetTrainingChoiceMade ();
			t = client.GetTrainingT ();
			tMax = client.GetTrainingTMax (); 
			goodInHand = client.GetTrainingGoodInHand (); 
			goodDesired = client.GetTrainingGoodDesired (); 
			score = client.GetTrainingScore ();

			uiController.ShowTitle ();
			uiController.SetTitle (Title.training);
		} else {
			choiceMade = client.GetChoiceMade ();
			t = client.GetT ();
			tMax = client.GetTMax (); 
			goodInHand = client.GetGoodInHand (); 
			goodDesired = client.GetGoodDesired (); 
			score = client.GetScore ();

			uiController.ShowTitle (visible: false);
		}
			
		uiController.SetScore (score);

		uiController.ShowScore ();

		uiProgressBars.ShowProgress ();
		uiProgressBars.UpdateRadial (t, tMax);
		uiProgressBars.ShowStatus (false);

		uiController.ShowLogo (false);
		uiController.ShowCharacter ();

		if (choiceMade) {
			uiController.ChoiceMadeView (goodInHand, goodDesired);
			if (training) {
				client.TrainingChoice (goodDesired);
				state = TL.TrainingChoiceWS;
			} else {
				client.Choice (goodDesired);
				state = TL.GameChoiceWS;
			}
		} else {
			uiController.ChoiceView (goodInHand);
			if (training) {
				state = TL.TrainingChoiceWU;
			} else {
				state = TL.GameChoiceWU;		
			}
		}
	}

	void BeginSurvey () {

		survey.View ();
		state = TL.SurveyWU;
	}

	void UpdateGoodInHand () {

		if (success) {
			if (goodDesired == Good.wheat) {
				goodInHand = Good.wood;
			} else {
				goodInHand = goodDesired;
			}		
		} 
	}

	void BeginTurn (bool training=false) {

		uiController.SetScore (score);
		uiController.HideResults ();

		if (end) {
			uiController.EndView (score, tMax);

			if (training) {
				uiButtons.ShowNext (glow: true);
				state = TL.TrainingEndWU;
			} else {
				state = TL.End;
			}
		} else {
			UpdateGoodInHand ();
			uiController.ChoiceView (goodInHand);
			if (training) {
				state = TL.TrainingChoiceWU;
			} else {
				state = TL.GameChoiceWU;	
			}
		}
	}
}
