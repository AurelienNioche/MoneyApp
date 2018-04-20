using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;
using UnityEngine.Networking;


class Demand {

	public static string init = "init";
	public static string survey = "survey";
	public static string choice = "choice";
	public static string trainingDone = "tutorial_done";
	public static string trainingChoice = "tutorial_choice";
}

class Key {

	public static string demand = "demand";
	public static string deviceId = "device_id";
	public static string userId = "user_id";
	public static string age = "age";
	public static string sex = "sex";
	public static string good = "good";
	public static string t = "t";
}

[System.Serializable]
class ResponseInit {

	public int userId = -1;
	public string pseudo = "";

	public bool wait = false;
	public int progress =  -1;

	public string step = null;

	public int tutoT = -1;
	public int tutoTMax = -1;
	public int tutoGoodInHand = -1;
	public int tutoGoodDesired = -1;
	public bool tutoChoiceMade = false;
	public int tutoScore = -1;
	 
	public int t = -1;
	public int tMax = -1;
	public int goodInHand = -1;
	public int goodDesired = -1;
	public bool choiceMade = false;
	public int score = -1;

	public int nGood = -1;

	public static ResponseInit CreateFromJson(string jsonString) {
		return JsonUtility.FromJson<ResponseInit> (jsonString);
	}
}

[System.Serializable]
class ResponseSurvey {

	public bool wait = false;
	public int progress =  -1;

	public static ResponseSurvey CreateFromJson(string jsonString) {
		return JsonUtility.FromJson<ResponseSurvey> (jsonString);
	}
}


[System.Serializable]
class ResponseTutorialDone {

	public bool wait = false;
	public int progress =  -1;

	public static ResponseTutorialDone CreateFromJson(string jsonString) {
		return JsonUtility.FromJson<ResponseTutorialDone> (jsonString);
	}
}

[System.Serializable]
class ResponseTutorialChoice {

	public bool wait = false;
	public int progress =  -1;

	public bool tutoSuccess = false;
	public int tutoT = -1;
	public int tutoScore = -1;
	public bool tutoEnd = false;

	public static ResponseTutorialChoice CreateFromJson(string jsonString) {
		return JsonUtility.FromJson<ResponseTutorialChoice> (jsonString);
	}
}


[System.Serializable]
class ResponseChoice {

	public bool wait = false;
	public int progress = -1;

	public bool success = false;
	public int t = -1;
	public int score = -1;
	public bool end = false;

	public static ResponseChoice CreateFromJson(string jsonString) {
		return JsonUtility.FromJson<ResponseChoice> (jsonString);
	}
}


public class Client : MonoBehaviour {

	// ---------- For communication with the server -------------------------- //

	public string url = "http://127.0.0.1:8000/client_request/";
	public float timeBeforeRetryingDemand = 1f; 

	bool gotError;
	bool gotResponse;

	string deviceId;

	bool occupied;

	GameController gameController;

	WWWForm form;
	WWWForm formInMemory;

	TLClient state;

	string response;
	string demand;

	// ------------------------------------------------------------ //

	int userId = -1;
	string pseudo = "";

	bool wait = false;
	int progress =  -1;

	string step = null;

	int tutoT = -1;
	int tutoTMax = -1;
	int tutoGoodInHand = -1;
	int tutoGoodDesired = -1;
	int tutoScore = -1;
	bool tutoChoiceMade = false;
	bool tutoSuccess = false;
	bool tutoEnd = false;

	int t = -1;
	int tMax = -1;
	int score = -1;
	int goodInHand = -1;
	int goodDesired = -1;
	bool choiceMade = false;
	bool success = false;
	bool end = false;

	int nGood = -1;

	// --------------- Overloaded Unity's functions -------------------------- //

	void Awake () {

		// response = new Dictionary<string, string> ();
		
		deviceId = UnityEngine.SystemInfo.deviceUniqueIdentifier;

		state = TLClient.WaitingRequest;
		occupied = false;

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

		demand = Demand.init;

		form = new WWWForm();
		form.AddField (Key.demand, Demand.init);
		form.AddField (Key.deviceId, deviceId);
		Request ();
	}

	public void Survey (int age, string sex) {

		demand = Demand.survey;

		form = new WWWForm();
		form.AddField (Key.userId, userId);
		form.AddField (Key.demand, Demand.survey);
		form.AddField (Key.age, age);
		form.AddField (Key.sex, sex);
		Request ();
	}

	public void TrainingDone () {

		demand = Demand.trainingDone;

		form = new WWWForm();
		form.AddField (Key.userId, userId);
		form.AddField (Key.demand, Demand.trainingDone);
		Request ();
	} 

	public void Choice (int value) {

		demand = Demand.choice;

		form = new WWWForm();
		form.AddField (Key.userId, userId);
		form.AddField (Key.demand, Demand.choice);
		form.AddField (Key.t, t);
		form.AddField (Key.good, value);
		Request ();
	}

	public void TrainingChoice (int value) {

		demand = Demand.trainingChoice;

		form = new WWWForm();
		form.AddField (Key.userId, userId);
		form.AddField (Key.demand, Demand.trainingChoice);
		form.AddField (Key.t, tutoT);
		form.AddField (Key.good, value);
		Request ();
	}

	// ------------------ General methods for communicating with the server --------- //

	void HandleServerResponse () {

		gotResponse = false;

		if (demand == Demand.init) {

			ResponseInit ri = ResponseInit.CreateFromJson (response);
			userId = ri.userId;
			pseudo = ri.pseudo;

			wait = ri.wait;
			progress = ri.progress;

			step = ri.step;

			tutoT = ri.tutoT;
			tutoTMax = ri.tutoTMax;
			tutoGoodInHand = ri.tutoGoodInHand;
			tutoGoodDesired = ri.tutoGoodDesired;
			tutoChoiceMade = ri.tutoChoiceMade;
			tutoScore = ri.tutoScore;

			t = ri.t;
			tMax = ri.tMax;
			goodInHand = ri.goodInHand;
			goodDesired = ri.goodDesired;
			choiceMade = ri.choiceMade;
			score = ri.score;

			nGood = ri.nGood;

			if (!wait) {
				Debug.Log (
					String.Format("userId: {0}, pseudo: {1}, step: {2}, \n" +
						"tutoT: {3}, tutoTMax: {4}, tutoGoodInHand: {5}, tutoGoodDesired: {6}, tutoChoiceMade: {7}, tutoScore: {8}, \n" +
						"t: {9}, tMax: {10}, goodInHand: {11}, goodDesired: {12}, choiceMade: {13}, score: {14}, nGood: {15}", 
						new object [] {userId, pseudo, step, tutoT, tutoTMax, tutoGoodInHand, tutoGoodDesired, tutoChoiceMade, tutoScore,
						t, tMax, goodInHand, goodDesired, choiceMade, score, nGood})
					);
			}
		
		} else if (demand == Demand.survey) {

			ResponseSurvey rs = ResponseSurvey.CreateFromJson (response);
			wait = rs.wait;
			progress = rs.progress;
		
		} else if (demand == Demand.choice) { 

			ResponseChoice rc = ResponseChoice.CreateFromJson (response);
			wait = rc.wait;
			progress = rc.progress;

			success = rc.success;
			t = rc.t;
			end = rc.end;
			score = rc.score;

		} else if (demand == Demand.trainingChoice) { 
			
			ResponseTutorialChoice rtc = ResponseTutorialChoice.CreateFromJson (response);
			wait = rtc.wait;
			progress = rtc.progress;

			tutoSuccess = rtc.tutoSuccess;
			tutoT = rtc.tutoT;
			tutoScore = rtc.tutoScore;
			tutoEnd = rtc.tutoEnd;

		} else if (demand == Demand.trainingDone) {
			
			ResponseTutorialDone rtd = ResponseTutorialDone.CreateFromJson (response);
			wait = rtd.wait;
			progress = rtd.progress;
		} else {
			throw new Exception ("Not expected demand: " + demand);
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

	public string GetStep () {
		return step;
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

	public int GetNGoods () {
		return nGood;
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

		gotError = false;

		yield return www.SendWebRequest();

		if (www.isNetworkError || www.isHttpError) {

			gotError = true;
			Debug.Log ("Client: I got an error: '" + www.error + "'.");

		} else {

			try { 
				response = www.downloadHandler.text;
				Debug.Log("Client: Create JSON");
			} catch (Exception) {
				gotError = true;
			}
		}

		gotResponse = true;
	}

	bool HasResponse () {

		// Return if client recieves a response.
		if (gotResponse) {

			Debug.Log ("Client: Got response from server.");

			if (gotError == false) {
				return true;

			} else {
				gotResponse = false;
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
