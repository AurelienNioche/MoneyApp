using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using AssemblyCSharp;


public class GameStep {
	public static string tutorial = "tutorial";
	public static string survey = "survey";
	public static string game = "game";
	public static string end = "end";
}


public class GameController : MonoBehaviour {

	UIController uiController;
	TL state;

	Client client;


	bool choiceMade; 
	bool success;
	int t; 
	int tMax; 
	int goodInHand; 
	int goodDesired; 
	int score;

	// -------------- Inherited from MonoBehavior ---------------------------- //

	void Awake () {

		uiController = GetComponent<UIController> ();
		client = GetComponent<Client> ();

		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		state = TL.HomeWU;
	}

	void Start () {

		uiController.HomeWU ();
	}

	void Update () {}

	// ------------------------------ //

	void LogState() {
		Debug.Log ("GameController: My state is '" + state + "'.");
	}

	IEnumerator ActionWithDelay (Action methodName, float seconds) {

		yield return new WaitForSeconds(seconds);
		methodName ();
	}

	// ----------- From UIManager ---------------- //

	public void UserReplied () {

		Debug.Log ("GameController: User clicked on Next");

		switch (state) {

		case TL.HomeWU:
			uiController.HomeWS ();
			client.Init ();
			state = TL.InitWS;
			break;

		case TL.SurveyWU:

			if (uiController.EvaluateUserData ()) {
				uiController.SurveyWS ();
				client.Survey (age: uiController.GetAge(), sex:uiController.GetSex());
				state = TL.SurveyWS;
			} else {
				uiController.ShowNextButton ();
			}

			break;

		case TL.TutoThreeGoodsWU:

			uiController.ShowNextButton (glow: true);
			uiController.TutoThreeGoods (false);
			uiController.TutoSpec ();
			state = TL.TutoSpecWU;
			break;

		case TL.TutoSpecWU:

			uiController.ShowNextButton (glow: true);
			uiController.TutoSpec (false);
			uiController.TutoMarkets ();
			state = TL.TutoMarketsWU;
			break;
		
		case TL.TutoMarketsWU:
			uiController.ShowNextButton (glow: true);
			uiController.TutoMarkets (false);
			uiController.TutoYou ();
			state = TL.TutoYouWU;
			break;
		
		case TL.TutoYouWU:

			uiController.ShowNextButton (glow: true);
			uiController.TutoYou (false);
			uiController.TutoStrategyDirect ();
			state = TL.TutoDirStrWU;
			break;

		case TL.TutoDirStrWU:

			uiController.ShowNextButton (glow: true);
			uiController.TutoStrategyDirect (false);
			uiController.TutoStrategyIndirect ();
			state = TL.TutoIndStrWU;
			break;
		
		case TL.TutoIndStrWU:

			uiController.ShowNextButton (glow: true);
			uiController.TutoStrategyIndirect (false);
			uiController.TutoTraining ();
			state = TL.TutoTrainingWU;
			break;
		
		case TL.TutoTrainingWU:

			uiController.ShowNextButton (glow: true);
			uiController.TutoTraining (false);
			BeginGame (training: true);
			break;

		case TL.TutoChoiceWU:

			goodDesired = uiController.GetGoodChosen ();

			client.TutorialChoice (goodDesired);
			state = TL.TutoChoiceWS;
			break;

		case TL.TutoResultWU:
			
			uiController.SetScore (client.GetTutoScore ());
			uiController.HideResults ();

			if (client.GetTutoEnd ()) {
				uiController.EndView (client.GetTutoScore (), client.GetTutoTMax());
				uiController.ShowNextButton (glow: true);
				state = TL.TutoEndWU;
			} else {
				uiController.ChoiceViewWU (client.GetTutoGoodInHand());
				state = TL.GameChoiceWU;	
			}
			break;
		
		case TL.TutoEndWU:

			uiController.TutoReady ();
			state = TL.TutoReadyWU;
			break;

		case TL.TutoReadyWU:

			uiController.TutoReady (false);
			uiController.HomeWS ();
			uiController.ShowNextButton (visible: true, glow: true);
			client.TutorialDone ();
			state = TL.TutoDoneWS;
			break;

		case TL.GameChoiceWU:

			goodDesired = uiController.GetGoodChosen ();

			client.Choice (goodDesired);

			state = TL.GameChoiceWS;
			break;
		
		case TL.GameResultWU:

			uiController.SetScore (score);
			uiController.HideResults ();

			if (client.GetEnd ()) {
				uiController.EndView (score, tMax);
				state = TL.End;
			} else {
				UpdateGoodInHand ();
				uiController.ChoiceViewWU (goodInHand);
				state = TL.GameChoiceWU;	
			}
			break;
		}
	}

	// ----------- From Client ----------- //

	public void ServerReplied () {

		Debug.Log ("GameController: Received response from server.");

		if (client.GetWait ()) {

			uiController.StatusProgressBar (
				progress: client.GetProgress (), 
				msg: "En attente des autres joueurs");
			client.RetryDemand ();

		} else {

			switch (state) {

			case TL.InitWS:

				// Initialize things
				uiController.SetPseudo (client.GetPseudo ());
					
				if (client.GetCurrentStep () == GameStep.tutorial) {
					BeginTutorial ();
				} else if (client.GetCurrentStep () == GameStep.survey) {
					BeginSurvey ();
				} else if (client.GetCurrentStep () == GameStep.game) {
					BeginGame ();
				} else {
					throw new Exception ();
				}

				break;

			case TL.SurveyWS:
				
				if (client.GetSkipTutorial ()) {
					BeginGame ();
				} else {
					BeginTutorial ();
				}
				break;

			case TL.TutoChoiceWS:

				success = client.GetTutoSuccess ();
				t = client.GetTutoT ();
				score = client.GetTutoScore ();

				uiController.UpdateRadialProgressBar (t, tMax);
				uiController.ResultView (success, goodInHand, goodDesired);

				state = TL.TutoResultWU;
				break;
			
			case TL.TutoDoneWS:

				BeginGame ();
				break;
			
			case TL.GameChoiceWS:

				success = client.GetSuccess ();
				t = client.GetT ();

				uiController.UpdateRadialProgressBar (t, tMax);
				uiController.ResultView (success, goodInHand, goodDesired);

				state = TL.GameResultWU;
				break;
			}

		}
			
	}

	// -------------------------- //

	void BeginTutorial () {
		
		uiController.ShowNextButton (glow:true);
		uiController.HideLogoAndStatusBar ();
		uiController.TutoThreeGoods ();
		state = TL.TutoThreeGoodsWU;
	}

	void BeginGame (bool training=false) {

		if (training) {
			choiceMade = client.GetTutoChoiceMade ();
			t = client.GetTutoT ();
			tMax = client.GetTutoTMax (); 
			goodInHand = client.GetTutoGoodInHand (); 
			goodDesired = client.GetTutoGoodDesired (); 
			score = client.GetTutoScore ();
		
		} else {
			choiceMade = client.GetChoiceMade ();
			t = client.GetT ();
			tMax = client.GetTMax (); 
			goodInHand = client.GetGoodInHand (); 
			goodDesired = client.GetGoodDesired (); 
			score = client.GetTutoScore ();
		}
			
		uiController.SetScore (score);
		uiController.ShowTitle (visible: false);

		uiController.ShowScore ();
		uiController.ShowProgress ();

		uiController.UpdateRadialProgressBar (t, tMax);

		uiController.HideLogoAndStatusBar ();
		uiController.ShowCharacter ();

		if (choiceMade) {
			uiController.ChoiceMadeViewWS (goodInHand, goodDesired);
			if (training) {
				client.TutorialChoice (goodDesired);
				state = TL.TutoChoiceWS;
			} else {
				client.Choice (goodDesired);
				state = TL.GameChoiceWS;
			}
		} else {
			uiController.ChoiceViewWU (goodInHand);
			if (training) {
				state = TL.TutoChoiceWU;
			} else {
				state = TL.GameChoiceWU;		
			}
		}
	}

	void BeginSurvey () {

		uiController.SurveyWU ();
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

}
