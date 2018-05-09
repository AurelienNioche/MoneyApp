using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;
using UnityEngine.Networking;
using WebSocketSharp;


class Demand {

	public static string init = "init";
	public static string survey = "survey";
	public static string choice = "choice";
	public static string trainingDone = "training_done";
	public static string trainingChoice = "training_choice";
}

[System.Serializable]
class RequestInit {
	public string demand = Demand.init;
	public string deviceId = "";
}

[System.Serializable]
class ResponseInit {

	public int userId = -1;
	public string pseudo = "";

	public bool wait = false;
	public int progress =  -1;

	public string step = null;

	public int trainingT = -1;
	public int trainingTMax = -1;
	public int trainingGoodInHand = -1;
	public int trainingGoodDesired = -1;
	public bool trainingChoiceMade = false;
	public int trainingScore = -1;
	 
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
class RequestSurvey {
	public string demand = Demand.survey;
	public int userId = -1;
	public int age = -1;
	public string sex = "";
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
class RequestTrainingDone {
	public string demand = Demand.trainingDone;
	public int userId = -1;
}

[System.Serializable]
class ResponseTrainingDone {

	public bool wait = false;
	public int progress =  -1;

	public static ResponseTrainingDone CreateFromJson(string jsonString) {
		return JsonUtility.FromJson<ResponseTrainingDone> (jsonString);
	}
}

[System.Serializable]
class RequestTrainingChoice {
	public string demand = Demand.trainingChoice;
	public int userId = -1;
	public int t = -1;
	public int good = -1;
}

[System.Serializable]
class ResponseTrainingChoice {

	public bool wait = false;
	public int progress =  -1;

	public bool trainingSuccess = false;
	public int trainingT = -1;
	public int trainingScore = -1;
	public bool trainingEnd = false;

	public static ResponseTrainingChoice CreateFromJson(string jsonString) {
		return JsonUtility.FromJson<ResponseTrainingChoice> (jsonString);
	}
}

[System.Serializable]
class RequestChoice {
	public string demand = Demand.choice;
	public int userId = -1;
	public int t = -1;
	public int good = -1;
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

	public string url = "http://127.0.0.1:8000/";

	string deviceId;

	GameController gameController;

	WWWForm form;
	WWWForm formInMemory;

	string demand;

	// ------------------------------------------------------------ //

	int userId = -1;
	string pseudo = "";

	bool wait = false;
	int progress =  -1;

	string step = null;

	int trainingT = -1;
	int trainingTMax = -1;
	int trainingGoodInHand = -1;
	int trainingGoodDesired = -1;
	int trainingScore = -1;
	bool trainingChoiceMade = false;
	bool trainingSuccess = false;
	bool trainingEnd = false;

	int t = -1;
	int tMax = -1;
	int score = -1;
	int goodInHand = -1;
	int goodDesired = -1;
	bool choiceMade = false;
	bool success = false;
	bool end = false;

	int nGood = -1;

	WebSocket w;
	string error;
	bool connected;

	bool occupied;

	List <string> responses;

	// --------------- Overloaded Unity's functions -------------------------- //

	void Awake () {

		deviceId = UnityEngine.SystemInfo.deviceUniqueIdentifier;
		gameController = GetComponent<GameController> ();
	}

	// Use this for initialization

	void Start () {
		
		gameController = GetComponent<GameController> ();
		StartCoroutine(StartWebSocket ());
		responses = new List<string> ();
	}

	IEnumerator StartWebSocket () {

		Debug.Log ("[Client] Connecting...");
		w = new WebSocket(url);
		w.OnMessage += (sender, e) => OnMessage (e);
		w.OnOpen += (sender, e) => OnOpen();
		w.OnError += (sender, e) => OnError(e);
		w.ConnectAsync();
		while (!connected && error == null)
			yield return 0;
	}

	void OnOpen() {
		connected = true;
		Debug.Log ("Connected!");
	}

	void OnError (WebSocketSharp.ErrorEventArgs e) {

		connected = false;
		error = e.Message;
		Debug.Log (String.Format("[Client] Using url '{0}', I got error '{1}'.", new object [] {url, error}));
	}

	// Update is called once per frame
	void Update () {

		if (!occupied) {
			occupied = true;
			if (error != null) {
				error = null;
				StartCoroutine (ReConnect ());
			} else if (responses.Count > 0) {
				HandleServerResponse (responses [0]);
				responses.RemoveAt (0);
			}
			occupied = false;
		} 
	}

	public IEnumerator ReConnect() {
		yield return new WaitForSeconds (1);
		StartCoroutine (StartWebSocket ());
	}

	public IEnumerator Send (string msg) {

		while (!connected) {
			Debug.Log ("[Client] Not connected... I will try to send in a short while.");
			yield return new WaitForSeconds (1);
		}
		Debug.Log("[Client] Sending msg: " + msg);
		w.Send(msg);
	}
		
	void OnMessage (WebSocketSharp.MessageEventArgs e) {

		string msg = e.Data;
		Debug.Log ("[Client] Received msg: " + msg);
		responses.Add (msg);
		//gameController.OnServerResponse (msg);
	}
		
	// ------------------------------------------------------------------------------------- //

	public void Init () {

		demand = Demand.init;

		RequestInit req = new RequestInit ();
		req.deviceId = deviceId;

		Debug.Log(String.Format(
			"[RequestInit] deviceId: {0}",
			new object [] {req.deviceId}
		));

		string json = JsonUtility.ToJson (req);
		StartCoroutine (Send (json));
	}

	public void Survey (int age, string sex) {

		demand = Demand.survey;
		RequestSurvey req = new RequestSurvey ();
		req.userId = userId;
		req.age = age;
		req.sex = sex;

		Debug.Log(String.Format(
			"[RequestSurvey] userId: {0}, age: {1}, sex: {2}",
			new object [] {req.userId, req.age, req.sex}
		));

		string json = JsonUtility.ToJson (req);
		StartCoroutine (Send (json));
	}

	public void TrainingDone () {

		demand = Demand.trainingDone;

		RequestTrainingDone req = new RequestTrainingDone ();
		req.userId = userId;

		Debug.Log(String.Format(
			"[RequestTrainingDone] userId: {0}",
			new object [] {req.userId}
		));

		string json = JsonUtility.ToJson (req);
		StartCoroutine (Send (json));
	} 

	public void Choice (int value) {

		demand = Demand.choice;

		RequestChoice req = new RequestChoice ();
		req.userId = userId;
		req.good = value;
		req.t = t;

		Debug.Log(String.Format(
			"[RequestChoice] userId: {0}, good: {1}, t: {2}",
			new object [] {req.userId, req.good, req.t}
		));

		string json = JsonUtility.ToJson (req);
		StartCoroutine (Send (json));
	}

	public void TrainingChoice (int value) {

		demand = Demand.trainingChoice;

		RequestTrainingChoice req = new RequestTrainingChoice ();
		req.userId = userId;
		req.good = value;
		req.t = trainingT;

		Debug.Log(String.Format(
			"[RequestTrainingChoice] userId: {0}, good: {1}, t: {2}",
			new object [] {req.userId, req.good, req.t}
		));

		string json = JsonUtility.ToJson (req);
		StartCoroutine (Send (json));
	}

	// ------------------ General methods for communicating with the server --------- //

	void HandleServerResponse (string response) {

		if (demand == Demand.init) {

			ResponseInit ri = ResponseInit.CreateFromJson (response);

			wait = ri.wait;
			progress = ri.progress;

			userId = ri.userId;
			pseudo = ri.pseudo;

			step = ri.step;

			trainingT = ri.trainingT;
			trainingTMax = ri.trainingTMax;
			trainingGoodInHand = ri.trainingGoodInHand;
			trainingGoodDesired = ri.trainingGoodDesired;
			trainingChoiceMade = ri.trainingChoiceMade;
			trainingScore = ri.trainingScore;

			t = ri.t;
			tMax = ri.tMax;
			goodInHand = ri.goodInHand;
			goodDesired = ri.goodDesired;
			choiceMade = ri.choiceMade;
			score = ri.score;

			nGood = ri.nGood;

			Debug.Log (String.Format(
				"[ResponseInit] wait: {0}, progress: {1}, userId: {2}, pseudo: {3}, step: {4}, \n" +
				"trainingT: {5}, trainingTMax: {6}, trainingGoodInHand: {7}, trainingGoodDesired: {8}, trainingChoiceMade: {9}, trainingScore: {10}, \n" +
				"t: {11}, tMax: {12}, goodInHand: {13}, goodDesired: {14}, choiceMade: {15}, score: {16}, nGood: {17}.", 
				new object [] {wait, progress, userId, pseudo, step, trainingT, trainingTMax, trainingGoodInHand, trainingGoodDesired, trainingChoiceMade, trainingScore,
				t, tMax, goodInHand, goodDesired, choiceMade, score, nGood}
			));
		
		} else if (demand == Demand.survey) {

			ResponseSurvey rs = ResponseSurvey.CreateFromJson (response);
			wait = rs.wait;
			progress = rs.progress;

			Debug.Log(String.Format(
				"[ResponseSurvey] wait: {0}, progress: {1}", new object [] {wait, progress}	
			));
		
		} else if (demand == Demand.choice) { 

			ResponseChoice rc = ResponseChoice.CreateFromJson (response);

			if (t != rc.t) {
				Debug.LogWarning(String.Format("Got a server response for different t (content='{0}').", response));
				return;
			}
				
			wait = rc.wait;
			progress = rc.progress;

			if (!wait) {
				
				success = rc.success;
				score = rc.score;
				end = rc.end;

				t = rc.t += 1;
			}

			Debug.Log (String.Format (
				"[ResponseChoice] wait: {0}, progress: {1}, success: {2}, t: {3}, score: {4}, end: {5}.", 
				new object [] { wait, progress, success, t, score, end }	
			));

		} else if (demand == Demand.trainingChoice) { 
			
			ResponseTrainingChoice rtc = ResponseTrainingChoice.CreateFromJson (response);

			if (rtc.trainingT != trainingT) {
				Debug.LogWarning(String.Format("Got a server response for different t (content='{0}').", response));
				return;
			}

			wait = rtc.wait;
			progress = rtc.progress;

			if (!wait) {
				
				trainingSuccess = rtc.trainingSuccess;
				trainingScore = rtc.trainingScore;
				trainingEnd = rtc.trainingEnd;

				trainingT = rtc.trainingT + 1;
			}

			Debug.Log (String.Format (
				"[ResponseTrainingChoice] wait: {0}, progress: {1}, trainingSuccess: {2}, trainingT: {3}, trainingScore: {4}, trainingEnd: {5}.", 
				new object [] { wait, progress, trainingSuccess, trainingT, trainingScore, trainingEnd }	
			));

		} else if (demand == Demand.trainingDone) {
			
			ResponseTrainingDone rtd = ResponseTrainingDone.CreateFromJson (response);
			wait = rtd.wait;
			progress = rtd.progress;

			Debug.Log (String.Format (
				"[ResponseTrainingDone] wait: {0}, progress: {1}.", new object [] { wait, progress }	
			));
		
		} else {
			Debug.LogWarning(String.Format("Not expected server response (content='{0}').", response));
			return;
		}

		if (!wait) {
			demand = null;
		} 
			
		gameController.ServerReplied ();
	}

	// ----- 'classic' getters -------- //

	public bool GetWait () {return wait;}

	public int GetProgress () {return progress;}

	public string GetStep () {return step;}

	public int GetScore () {return score;}

	public bool GetSuccess () {return success;}

	public bool GetChoiceMade () {return choiceMade;}

	public int GetGoodInHand () {return goodInHand;}

	public int GetGoodDesired () {return goodDesired;}

	public bool GetEnd () {return end;}

	public string GetPseudo () {return pseudo;}

	public int GetT () {return t;}

	public int GetTMax () {return tMax;}

	public int GetTrainingScore () {return trainingScore;}

	public bool GetTrainingSuccess () {return trainingSuccess;}

	public bool GetTrainingChoiceMade () {return trainingChoiceMade;}

	public bool GetTrainingEnd () {return trainingEnd;}

	public int GetTrainingGoodInHand () {return trainingGoodInHand;}

	public int GetTrainingGoodDesired () {return trainingGoodDesired;}

	public int GetTrainingT () {return trainingT;}

	public int GetTrainingTMax () {return trainingTMax;}

	public int GetNGoods () {return nGood;}
}
