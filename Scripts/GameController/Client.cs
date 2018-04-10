using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;
using UnityEngine.Networking;


public class Demand {

	public static string init = "init";
	public static string survey = "survey";
	public static string choice = "choice";
	public static string tutorialDone = "tutorial_done";
	public static string tutorialChoice = "tutorial_choice";
}

class Key {

	public static string demand = "demand";
	public static string deviceId = "device_id";
	public static string userId = "user_id";
	public static string wait = "wait";
	public static string state = "state";
	public static string age = "age";
	public static string sex = "sex";
	public static string score = "score";
	public static string choiceMade = "choice_made";
	public static string goodInHand = "good";
	public static string goodDesired = "desired_good";
	public static string t = "t";
	public static string tMax = "t_max";
	public static string tutoT = "tuto_t";
	public static string tutoTMax = "tuto_t_max";
	public static string tutoGoodInHand = "tuto_good";
	public static string tutoGoodDesired = "tuto_desired_good";
	public static string tutoChoiceMade = "tuto_choice_made";
	public static string progress = "progress";
	public static string end = "end";
	public static string pseudo = "pseudo";
	public static string success = "success";
	public static string skipTutorial = "skip_tutorial";
	public static string skipSurvey = "skip_survey";
}


public class Client : MonoBehaviour {

	// ---------- For communication with the server -------------------------- //

	public string url = "http://127.0.0.1:8000/client_request/";
	public float timeBeforeRetryingDemand = 1f; 

	bool serverError;
	bool serverResponse;

	GameController gameController;
	JSONObject response;

	WWWForm form;
	WWWForm formInMemory;

	// -------------- For player ------------------------ //

	TLClient state;

	string deviceId;
	int userId;
	string pseudo;

	bool occupied;

	bool wait;
	int progress;

	string currentStep;

	int tutoT;
	int tutoTMax;
	int tutoGoodInHand;
	int tutoGoodDesired;
	int tutoScore;
	bool tutoChoiceMade;
	bool tutoSuccess;
	bool tutoEnd;

	int t;
	int tMax;
	int score;
	int goodInHand;
	int goodDesired;
	bool choiceMade;
	bool success;
	bool end;

	bool skipTutorial;
	bool skipSurvey;

	// --------------- Overloaded Unity's functions -------------------------- //

	void Awake () {
		
		deviceId = UnityEngine.SystemInfo.deviceUniqueIdentifier;

		state = TLClient.WaitingRequest;
		occupied = false;

		currentStep = GameStep.tutorial;

		gameController = GetComponent<GameController> ();
	}

	// Use this for initialization
	void Start () {}

	// Update is called once per frame
	void Update () {
		if (!occupied) {

			occupied = true;
			ManageState ();
			occupied = false;
		}
	}

	void ManageState () {

		switch (state) {

		case TLClient.WaitingReply:

			if (HasResponse ()) {
				HandleServerResponse ();
			}
			break;

		case TLClient.WaitingRequest:
		case TLClient.GotReply:
			break;

		default:
			throw new Exception ("Client: Bad state '" + state + "'");
		}
	}

	// ------------------------------------------------------------------------------------- //

	public void Init () {

		form = new WWWForm();
		form.AddField (Key.demand, Demand.init);
		form.AddField (Key.deviceId, deviceId);
		Request ();
	}

	public void Survey (int age, string sex) {

		form = new WWWForm();
		form.AddField (Key.userId, userId);
		form.AddField (Key.demand, Demand.survey);
		form.AddField (Key.age, age);
		form.AddField (Key.sex, sex);
		Request ();
	}

	public void TutorialDone () {

		form = new WWWForm();
		form.AddField (Key.userId, userId);
		form.AddField (Key.demand, Demand.tutorialDone);
		Request ();
	} 

	public void Choice (int value) {

		form = new WWWForm();
		form.AddField (Key.userId, userId);
		form.AddField (Key.demand, Demand.choice);
		form.AddField (Key.goodDesired, value);
		Request ();
	}

	public void TutorialChoice (int value) {

		form = new WWWForm();
		form.AddField (Key.userId, userId);
		form.AddField (Key.demand, Demand.tutorialChoice);
		form.AddField (Key.goodDesired, value);
		Request ();
	}

	// ------------------------------------------------------------------------------------ //

	void ReplyInit () {
		wait = response.GetField (Key.wait).b;
		if (wait) {
			progress = (int) response.GetField (Key.progress).n;
		} else {
			currentStep = response.GetField (Key.state).str;
			choiceMade = response.GetField (Key.choiceMade).b;
			score = (int) response.GetField (Key.score).n;
			goodInHand = (int) response.GetField (Key.goodInHand).n;
			goodDesired = (int) response.GetField (Key.goodDesired).n;
			t = (int) response.GetField (Key.t).n;
			tMax = (int) response.GetField (Key.tMax).n;
			skipSurvey = response.GetField (Key.skipSurvey).b;
			skipTutorial = response.GetField (Key.skipTutorial).b;
			pseudo = response.GetField (Key.pseudo).str;
			userId = (int) response.GetField (Key.userId).n;
			tutoGoodInHand = (int) response.GetField (Key.tutoGoodInHand).n;
			tutoGoodDesired = (int) response.GetField (Key.tutoGoodDesired).n;
			tutoT = (int) response.GetField (Key.tutoT).n;
			tutoTMax = (int) response.GetField (Key.tutoTMax).n;
			tutoChoiceMade = response.GetField (Key.tutoChoiceMade).b;

			Debug.Log ("Pseudo:" + pseudo);
		}
	}

	void ReplySurvey () {

		wait = response.GetField (Key.wait).b;
		if (wait) {
			progress = (int) response.GetField (Key.progress).n;
		}
	}

	void ReplyTutorialChoice () {

		wait = response.GetField (Key.wait).b;
		if (wait) {
			progress = (int) response.GetField (Key.progress).n;
		} else {
			tutoSuccess = response.GetField (Key.success).b;
			tutoEnd = response.GetField (Key.end).b;
			tutoT = (int) response.GetField (Key.t).n;
			tutoScore = (int) response.GetField (Key.score).n;
			Debug.Log (tutoEnd);
		}
	}

	void ReplyTutorialDone () {

		wait = response.GetField (Key.wait).b;
		if (wait) {
			progress = (int) response.GetField (Key.progress).n;
		}
	}

	void ReplyChoice () {

		wait = response.GetField (Key.wait).b;
		if (wait) {
			progress = (int) response.GetField (Key.progress).n;
		} else {
			success = response.GetField (Key.success).b;
			end = response.GetField (Key.end).b;
			t = (int) response.GetField (Key.t).n;
			score = (int) response.GetField (Key.score).n;
		}
	}

	// ------------------ General methods for communicating with the server --------- //

	void HandleServerResponse () {

		serverResponse = false;

		string what = response.GetField (Key.demand).str;
		if (what == Demand.init) {
			ReplyInit ();
		} else if (what == Demand.survey) {
			ReplySurvey ();
		} else if (what == Demand.tutorialDone) {
			ReplyTutorialDone ();
		} else if (what == Demand.choice) {
			ReplyChoice ();
		} else if (what == Demand.tutorialChoice) {
			ReplyTutorialChoice ();
		} else {
			throw new Exception ("Client: Not expected case");
		}
		state = TLClient.GotReply;
		gameController.ServerReplied ();
	}

	void LogState () {
		Debug.Log ("Client: My state is '" + state + "'.");
	}

	// ------- State ------------- // 

	public bool IsState (TLClient value) {
		return state == value;
	}

	// ----- 'classic' getters -------- //

	public bool GetWait () {
		return wait;
	}

	public int GetProgress () {
		return progress;
	}

	public string GetCurrentStep () {
		return currentStep;
	}

	public int GetScore () {
		return score;
	}

	public bool GetSuccess () {
		return success;
	}

	public bool GetChoiceMade () {
		return choiceMade;
	}

	public int GetGoodInHand () {
		return goodInHand;
	}

	public int GetGoodDesired () {
		return goodDesired;
	}

	public bool GetEnd() {
		return end;
	}

	public string GetPseudo () {
		return pseudo;
	}

	public int GetT() {
		return t;
	}

	public int GetTMax() {
		return tMax;
	}

	public bool GetSkipTutorial () {
		return skipTutorial;
	}

	public bool GetSkipSurvey () {
		return skipSurvey;
	}

	public int GetTutoScore () {
		return tutoScore;
	}

	public bool GetTutoSuccess () {
		return tutoSuccess;
	}

	public bool GetTutoChoiceMade () {
		return tutoChoiceMade;
	}

	public bool GetTutoEnd () {
		return tutoEnd;
	}

	public int GetTutoGoodInHand () {
		return tutoGoodInHand;
	}

	public int GetTutoGoodDesired () {
		return tutoGoodDesired;
	}

	public int GetTutoT () {
		return tutoT;
	}

	public int GetTutoTMax () {
		return tutoTMax;
	}
		
	// ----------------------- Communication ----------------- //

	void Request () {
		state = TLClient.WaitingReply;
		StartCoroutine (CoroutineSendForm ());
	}

	IEnumerator CoroutineSendForm () {

		UnityWebRequest www = UnityWebRequest.Post(url, form);
		www.chunkedTransfer = false;
		// www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

		serverError = false;

		yield return www.SendWebRequest();

		if (www.isNetworkError || www.isHttpError) {

			serverError = true;
			Debug.Log ("Client: I got an error: '" + www.error + "'.");

		} else {

			try { 
				response = new JSONObject(www.downloadHandler.text);
				Debug.Log("Client: Create JSON");
			} catch (Exception) {
				serverError = true;
			}
		}

		serverResponse = true;
	}

	bool HasResponse () {

		// Return if client recieves a response.
		if (serverResponse) {

			Debug.Log ("Client: Got response from server.");

			if (serverError == false) {
				return true;

			} else {
				serverResponse = false;
				RetryDemand ();
				return false;
			}
		} else {
			return false;
		}	
	}

	public void RetryDemand () {
		StartCoroutine (CoroutineRetryDemand ());
	}

	IEnumerator CoroutineRetryDemand () {
		yield return new WaitForSeconds (timeBeforeRetryingDemand);
		Request ();
	}
}
