using System.Collections;
using System;
using UnityEngine;
using WebSocketSharp;

class Demand {

	public static string init = "init";
	public static string survey = "survey";
	public static string choice = "choice";
	public static string trainingDone = "training_done";
	public static string trainingChoice = "training_choice";
    public static string ping = "ping";
}

[Serializable]
class RequestPing {
    
    public string demand = "ping";
    public string deviceId = "";
}

[Serializable]
class ResponseShared {
    public bool wait = false;
    public string demand = "";
    public int progress = -1;
    public int t = -1;

    public static ResponseShared CreateFromJson(string jsonString)
    {
        return JsonUtility.FromJson<ResponseShared>(jsonString);
    }
}


[Serializable]
class RequestInit {
	public string demand = Demand.init;
	public string deviceId = "";
}

[Serializable]
class ResponseInit {

	public int userId = -1;
	public string pseudo = "";

	public string step = "";

	public int trainingT = -1;
	public int trainingTMax = -1;
	public int trainingGoodInHand = -1;
	public int trainingGoodDesired = -1;
    public int trainingScore = -1;
    public bool trainingChoiceMade = false;

	 
	public int t = -1;
	public int tMax = -1;
	public int goodInHand = -1;
	public int goodDesired = -1;
	public int score = -1;
    public bool choiceMade = false;

	public int nGood = -1;

	public static ResponseInit CreateFromJson(string jsonString) {
		return JsonUtility.FromJson<ResponseInit> (jsonString);
	}
}

[Serializable]
class RequestSurvey {
	public string demand = Demand.survey;
	public int userId = -1;
	public int age = -1;
	public string sex = "";
}

[Serializable]
class RequestTrainingDone {

	public string demand = Demand.trainingDone;
	public int userId = -1;
}

[Serializable]
class RequestTrainingChoice {
    
	public string demand = Demand.trainingChoice;
	public int userId = -1;
	public int t = -1;
	public int good = -1;
}

[Serializable]
class ResponseTrainingChoice {
	public bool trainingSuccess = false;
    public bool trainingEnd = false;
	public int trainingScore = -1;

	public static ResponseTrainingChoice CreateFromJson(string jsonString) {
		return JsonUtility.FromJson<ResponseTrainingChoice> (jsonString);
	}
}

[Serializable]
class RequestChoice {
	public string demand = Demand.choice;
	public int userId = -1;
	public int t = -1;
	public int good = -1;
}

[Serializable]
class ResponseChoice {
	public bool success = false;
	public bool end = false;
    public int score = -1;

	public static ResponseChoice CreateFromJson(string jsonString) {
		return JsonUtility.FromJson<ResponseChoice> (jsonString);
	}
}

public class Client : MonoBehaviour {

	public string url = "<url>";
    public int reconnectTime = 1000;
    public int timeOut = 30000;
    public int delayRequest = 2000;
    public int delayRequestNoResponse = 10000;
    //  public int delayPing = 1000;

    // ----------------------------------------------------------- //

	string deviceId;

	GameController gameController;

	string demand;

    // ------------------------------------------------------------ //

	int userId = -1;
	string pseudo = "";

	bool wait;
	int progress =  -1;

	string step;

	int trainingT = -1;
	int trainingTMax = -1;
	int trainingGoodInHand = -1;
	int trainingGoodDesired = -1;
	int trainingScore = -1;
	bool trainingChoiceMade;
	bool trainingSuccess;
	bool trainingEnd;

	int t = -1;
	int tMax = -1;
	int score = -1;
	int goodInHand = -1;
	int goodDesired = -1;
	bool choiceMade;
	bool success;
	bool end;

	int nGood = -1;

    // -------------------------------------------------------------- //

	WebSocket w;

	string error;
    string currentJSONRequest = "";

    bool connected;
    bool justDisconnect;
    bool justConnect;
    bool receivedResponse;
    bool receivedPing;
    bool readyToSend = true;

    bool responseTreated;

    float timeLastReconnection = 0;

    // ------------------------------------------------------ //

	void Start () 
    {
        deviceId = SystemInfo.deviceUniqueIdentifier;
        gameController = GetComponent<GameController>();

        StartCoroutine (StartWebSocket ());
	}

	IEnumerator StartWebSocket () 
    {
        Debug.Log("[Client] Connecting...");

        error = null;

        float timeConnection = Time.time;

        w = new WebSocket(url)
        {
            WaitTime = TimeSpan.FromSeconds (timeOut / 1000.0f)
        };
        w.OnMessage += (sender, e) => OnMessage(e);
        w.OnOpen += (sender, e) => OnOpen();
        w.OnError += (sender, e) => OnError(e);
        w.OnClose += (sender, e) => OnClose(e);
        w.ConnectAsync();
        while (!connected && error == null)
        {
            yield return 0;
        }
	}

    void OnApplicationQuit ()
    {
        StartCoroutine (Disconnect());
    }

    void OnOpen () 
    {
        connected = true;
        justConnect = true;
		Debug.Log ("[Client] Connected!");
	}

	void OnError (ErrorEventArgs e) 
    {
		error = e.Message;
		Debug.Log (String.Format(
            "[Client] Using url '{0}', I got error '{1}'.", 
            new object [] {url, error}));
	}

    void OnClose (CloseEventArgs e) 
    {
        connected = false;
        justDisconnect = true;
        Debug.Log(String.Format(
            "[Client] WebSocket has been closed with reason: '{0}' (code {1}).", 
            new object[] {e.Reason, e.Code }));
    }

    void OnMessage (MessageEventArgs e)
    {
        string msg = e.Data;
        Debug.Log("[Client] Received msg: " + msg);
        if (msg == "pong")
        {
            Debug.Log("[Client] I received a pong");
            receivedPing = true;
        }
        else 
        {
            Debug.Log("[Client] I received a response");
            HandleResponse (msg);
            receivedResponse = true;
        }
    }

    IEnumerator Disconnect () 
    {
        if (w == null)
        {
            Debug.Log("[Client] No websocket to disconnect.");
            yield return 1;
        }
        else
        {
            Debug.Log("[Client] Disconnect.");
            w.CloseAsync();
            while (connected)
            {
                yield return 0;
            }
        }
    }

    IEnumerator ReConnect ()
    {
        yield return new WaitWhile(
            () => Time.time - timeLastReconnection < reconnectTime / 1000.0f
        );

        if (!connected)
        {
            timeLastReconnection = Time.time;
            StartCoroutine (StartWebSocket ());
        }
    }

    IEnumerator SendRequest () 
    {
        bool sending = false;
        try
        {
            if (!connected || w == null)
            {
                Debug.Log("[Client] I'm not connected so I couldn't send a message.");
            }
            else
            {
                if (currentJSONRequest == "")
                {
                    Debug.Log("[Client] I send a ping");
                    RequestPing req = new RequestPing
                    {
                        deviceId = deviceId
                    };
                    w.Send(JsonUtility.ToJson(req));
                }
                else
                {
                    Debug.Log("[Client] I send a json request");
                    w.Send(currentJSONRequest);
                }
                sending = true;
            }
        } 
        catch (Exception)
        {
            Debug.Log("[Client ] I got an exception during sending.");
        }
        if (sending) {
            float sendingTime = Time.time; 
            receivedPing = false;
            receivedResponse = false;
            yield return new WaitUntil(
                () => Time.time - sendingTime > delayRequestNoResponse / 1000.0f
                || receivedResponse || receivedPing);
        } 
        yield return new WaitForSeconds(delayRequest / 1000.0f);
        readyToSend = true;
    }

	void Update () {

        if (justConnect)
        {
            justConnect = false;
            gameController.OnConnection();
        }
        else if (justDisconnect)
        {
            justDisconnect = false;
            Debug.Log("[Client] I've been disconnected!");

            // Try to reconnect
            Debug.Log("[Client] I will try to reconnect...");
            StartCoroutine (ReConnect ());
            gameController.OnDisconnection();
        }
        else if (responseTreated) 
        {
            responseTreated = false;
            gameController.ServerReplied ();
        } 
        else if (readyToSend) 
        {
            readyToSend = false;
            StartCoroutine (SendRequest());
        }
	}
		
	// ------------------------------------------------------------------------------------- //

	public void Init () {

		demand = Demand.init;

        RequestInit req = new RequestInit {
            deviceId = deviceId
        };

        Debug.Log(String.Format(
            "[Client] [RequestInit] deviceId: {0}",
			new object [] {req.deviceId}
		));

		currentJSONRequest = JsonUtility.ToJson (req);
	}

	public void Survey (int age, string sex) {

		demand = Demand.survey;
        RequestSurvey req = new RequestSurvey {
            userId = userId,
            age = age,
            sex = sex
        };

        Debug.Log(String.Format (
            "[Client] [RequestSurvey] userId: {0}, age: {1}, sex: {2}",
			new object [] {req.userId, req.age, req.sex}
		));

		currentJSONRequest = JsonUtility.ToJson (req);
	}

	public void TrainingDone () {

		demand = Demand.trainingDone;

        RequestTrainingDone req = new RequestTrainingDone
        {
            userId = userId
        };

        Debug.Log(String.Format(
            "[Client] [RequestTrainingDone] userId: {0}",
			new object [] {req.userId}
		));

		currentJSONRequest = JsonUtility.ToJson (req);
	} 

	public void Choice (int value) {

		demand = Demand.choice;

        RequestChoice req = new RequestChoice
        {
            userId = userId,
            good = value,
            t = t
        };

        Debug.Log(String.Format(
            "[Client] [RequestChoice] userId: {0}, good: {1}, t: {2}",
			new object [] {req.userId, req.good, req.t}
		));

		currentJSONRequest = JsonUtility.ToJson (req);
	}

	public void TrainingChoice (int value) {

		demand = Demand.trainingChoice;

        RequestTrainingChoice req = new RequestTrainingChoice
        {
            userId = userId,
            good = value,
            t = trainingT
        };

        Debug.Log(String.Format(
            "[Client] [RequestTrainingChoice] userId: {0}, good: {1}, t: {2}",
			new object [] {req.userId, req.good, req.t}
		));

		currentJSONRequest = JsonUtility.ToJson (req);
	}

	// ------------------ General methods for communicating with the server --------- //

	void HandleResponse (string response) {

        ResponseShared rs = ResponseShared.CreateFromJson(response);

        if ((rs.demand != demand) ||
            ((rs.demand == Demand.choice && t != rs.t) ||
            (rs.demand == Demand.trainingChoice && trainingT != rs.t))
           ) {

            Debug.LogWarning(String.Format("[Client] Not expected server response (content='{0}', expected response = ').", response));
            return; 
        }

        wait = rs.wait;
        progress = rs.progress;

        Debug.Log(String.Format(
            "[Client] Received response for demand: '{0}' with wait: {1}, " +
            "progress: {2}.", new object[] {rs.demand, wait, progress }
            ));

        if (demand == Demand.init) {

			ResponseInit ri = ResponseInit.CreateFromJson (response);

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
                "[Client] [ResponseInit] wait: {0}, progress: {1}, userId: {2}, pseudo: {3}, step: {4}, \n" +
				"trainingT: {5}, trainingTMax: {6}, trainingGoodInHand: {7}, trainingGoodDesired: {8}, trainingChoiceMade: {9}, trainingScore: {10}, \n" +
				"t: {11}, tMax: {12}, goodInHand: {13}, goodDesired: {14}, choiceMade: {15}, score: {16}, nGood: {17}.", 
				new object [] {wait, progress, userId, pseudo, step, trainingT, trainingTMax, trainingGoodInHand, trainingGoodDesired, trainingChoiceMade, trainingScore,
				t, tMax, goodInHand, goodDesired, choiceMade, score, nGood}
			));
		
		} else if (demand == Demand.survey) {

            wait = rs.wait;
			progress = rs.progress;

			Debug.Log(String.Format(
				"[ResponseSurvey] wait: {0}, progress: {1}", new object [] {wait, progress}	
			));
		
        } else if (demand == Demand.choice && !wait) { 

			ResponseChoice rc = ResponseChoice.CreateFromJson (response);
				
			success = rc.success;
			score = rc.score;
			end = rc.end;

            t += 1;

			Debug.Log (String.Format (
                "[Client] [ResponseChoice] wait: {0}, progress: {1}, success: {2}, t: {3}, score: {4}, end: {5}.", 
				new object [] { wait, progress, success, t, score, end }	
			));

        } else if (demand == Demand.trainingChoice && !wait) { 
			
			ResponseTrainingChoice rtc = ResponseTrainingChoice.CreateFromJson (response);
				
			trainingSuccess = rtc.trainingSuccess;
			trainingScore = rtc.trainingScore;
			trainingEnd = rtc.trainingEnd;

            trainingT += 1;

			Debug.Log (String.Format (
                "[Client] [ResponseTrainingChoice] wait: {0}, progress: {1}, trainingSuccess: {2}, trainingT: {3}, trainingScore: {4}, trainingEnd: {5}.", 
				new object [] { wait, progress, trainingSuccess, trainingT, trainingScore, trainingEnd }	
			));

		}

        if (!wait)
        {
            demand = null;
            currentJSONRequest = "";
        }

        responseTreated = true;
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


    //static string GetTimestamp()
    //{
    //    return System.DateTime.UtcNow.ToString("HH:mm:ss.fff: ");
    //}
}
