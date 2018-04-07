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

	public void UserNext () {

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
			uiController.TutoSpec ();
			state = TL.TutoSpecWU;
			break;

		case TL.GameChoiceWU:
			client.Choice (uiController.GetGoodDesired ());
			state = TL.GameChoiceWS;
			break;
		}
	}

	// ----------- From Client ----------- //

	public void ServerReplied () {

		Debug.Log ("GameController: Received response from server.");

		if (client.GetWait ()) {

			uiController.StatusProgressBar (progress: client.GetProgress (), 
				msg: "En attente des autres joueurs");
			client.RetryDemand ();

		} else {

			switch (state) {

			case TL.InitWS:
					
				if (client.GetCurrentStep () == GameStep.tutorial) {
					uiController.TutoThreeGoods ();
					state = TL.TutoThreeGoodsWU;
					
				} else if (client.GetCurrentStep () == GameStep.survey) {
					uiController.SurveyWU ();
					state = TL.SurveyWU;
					
				} else if (client.GetCurrentStep () == GameStep.game) {
					state = TL.GameChoiceWU;
				} else {
					throw new Exception ();
				}

				break;

			case TL.SurveyWS:
				if (client.GetSkipTutorial ()) {
					state = TL.GameChoiceWU;
				} else {
					uiController.TutoThreeGoods ();
					state = TL.TutoThreeGoodsWU;
				}
				break;
			
			}

		}
			
	}

}
