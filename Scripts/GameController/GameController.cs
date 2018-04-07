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

	bool occupied = false;

	Client client;

	// -------------- Inherited from MonoBehavior ---------------------------- //

	void Awake () {

		uiController = GetComponent<UIController> ();
		client = GetComponent<Client> ();
	}

	void Start () {

		Screen.sleepTimeout = SleepTimeout.NeverSleep;

		state = TL.HomeWaitingUser;

		uiController.ShowNextButton ();
	}

	void Update () {

		if (!occupied) {

			occupied = true;
			ManageState ();
			occupied = false;
		}
	}

	// ------------------- Between user and player --------------------------- //

	void ManageState () {

		switch (state) {

		case TL.HomeWaitingUser:
			break;

		case TL.HomeUserNext:
			client.Init ();
			state = TL.InitWaitReply;
			break;

		case TL.InitWaitReply:
			if (client.IsState (TLClient.GotReply)) {
				Debug.Log ("Got response");
				state = TL.Dead;
			}
			break;
		case TL.Dead:

			break;
		
		default:
			throw new System.Exception ("GameController: Bad state.");
		}
	}

	// ----------- From UIManager ---------------- //

	public void UserNext () {
		Debug.Log ("GameController: User clicked on Next");
		if (state == TL.HomeWaitingUser) {
			state = TL.HomeUserNext;
		}
	}

	// ------------------------------ //

	void LogState() {
		Debug.Log ("GameController: My state is '" + state + "'.");
	}

	IEnumerator ActionWithDelay (Action methodName, float seconds) {

		yield return new WaitForSeconds(seconds);
		methodName ();
	}
}
