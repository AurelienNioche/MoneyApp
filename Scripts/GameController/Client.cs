using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;
using UnityEngine.Networking;


public class Demand {

	public static string init = "init";
	public static string survey = "survey";
	public static string tutorial = "tutorial";
	public static string choice = "choice";
}

class Key {

	public static string demand = "demand";
	public static string deviceId = "device_id";
	public static string userId = "user_id";
	public static string choice = "choice";
	public static string wait = "wait";
	public static string state = "state";
	public static string choiceMade = "choice_made";
	public static string age = "age";
	public static string sex = "sex";
	public static string score = "score";
	public static string goodInHand = "good";
	public static string goodDesired = "desired_good";
	public static string t = "t";
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

	JSONObject response;

	WWWForm form;
	WWWForm formInMemory;

	// -------------- For player ------------------------ //

	TLClient state;
	string deviceId;
	int t;

	bool occupied;

	bool wait;
	int progress;
	string currentStep;
	int score;
	bool choiceMade;
	int goodInHand;
	int goodDesired;
	int userId;
	string pseudo;
	bool success;
	bool end;

	bool skipTutorial;
	bool skipSurvey;

	// --------------- Overloaded Unity's functions -------------------------- //

	void Awake () {}

	// Use this for initialization
	void Start () {

		deviceId = UnityEngine.SystemInfo.deviceUniqueIdentifier;

		state = TLClient.WaitingRequest;
		occupied = false;

		currentStep = GameStep.tutorial;
	}

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
			throw new Exception ("Bad state '" + state + "'");
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
		form.AddField (Key.deviceId, deviceId);
		form.AddField (Key.demand, Demand.survey);
		form.AddField (Key.age, age);
		form.AddField (Key.sex, sex);
		Request ();
	}

	public void Tutorial () {

		form = new WWWForm();
		form.AddField (Key.deviceId, deviceId);
		form.AddField (Key.demand, Demand.tutorial);
		Request ();
	} 

	public void Choice (int desiredGood) {

		form = new WWWForm();
		form.AddField (Key.deviceId, deviceId);
		form.AddField (Key.demand, Demand.choice);
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
			skipSurvey = response.GetField (Key.skipSurvey).b;
			skipTutorial = response.GetField (Key.skipTutorial).b;
			pseudo = response.GetField (Key.pseudo).str;
		}
	}

	void ReplySurvey () {

		wait = response.GetField (Key.wait).b;
		if (wait) {
			progress = (int) response.GetField (Key.progress).n;
		}
	}

	void ReplyTutorial () {

		wait = response.GetField (Key.wait).b;
		if (wait) {
			progress = (int) response.GetField (Key.progress).n;
		} else {
			success = response.GetField (Key.success).b;
			end = response.GetField (Key.end).b;
		}
	}

	void ReplyChoice () {

		wait = response.GetField (Key.wait).b;
		if (wait) {
			progress = (int) response.GetField (Key.progress).n;
		}
	}

	// ------------------ General methods for communicating with the server --------- //

	void HandleServerResponse () {

		string what = response.GetField (Key.demand).str;
		if (what == Demand.init) {
			ReplyInit ();
		} else if (what == Demand.survey) {
			ReplySurvey ();
		} else if (what == Demand.tutorial) {
			ReplyTutorial ();
		} else if (what == Demand.choice) {
			ReplyChoice();
		}
		Debug.Log ("Get here");
		state = TLClient.GotReply;
	}

	void LogState () {
		Debug.Log ("ClientLfp: My state is '" + state + "'.");
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

	public int GetScore () {
		return score;
	}

	public string GetCurrentStep () {
		return currentStep;
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

	public bool GetSkipTutorial () {
		return skipTutorial;
	}

	public bool GetSkipSurvey () {
		return skipSurvey;
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

	public void RetryDemand() {
		StartCoroutine (CoroutineRetryDemand ());
	}

	IEnumerator CoroutineRetryDemand () {
		yield return new WaitForSeconds (timeBeforeRetryingDemand);
		Request ();
	}
}
