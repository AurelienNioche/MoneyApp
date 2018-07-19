using UnityEngine;
using UnityEngine.UI;


public class Survey : MonoBehaviour {

	public InputField age;
	public Toggle male;
	public Toggle female;
	public Toggle consent;

	GameObject survey;

	UIController uiController;
	UIProgressBars uiProgressBars;
	UIButtons uiButtons;

	// Use this for initialization
	void Start () {
		uiController = GetComponent<UIController> ();
		uiProgressBars = GetComponent<UIProgressBars> ();
		uiButtons = GetComponent<UIButtons> ();
		survey = age.transform.parent.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public int GetAge () {
		return int.Parse (age.text);
	}

	public string GetSex () {
		if (male.isOn) {
			return "male";
		} else {
			return "female";
		}
	}

	public bool EvaluateUserData () {

		if (age.text.Length == 0) {

			uiProgressBars.StatusMessage (ErrorMsg.age, color:Color.red, glow: true);
			return false;
		}	

		if (!male.isOn && !female.isOn) {

			uiProgressBars.StatusMessage (ErrorMsg.sex, color: Color.red, glow: true);
			return false;
		}

		if (!consent.isOn) {

			uiProgressBars.StatusMessage (ErrorMsg.consent, color: Color.red, glow: true);
			return false;
		}

		return true;
	}

	public void View (bool visible=true) {
		uiButtons.ShowNext (visible: visible);
		uiController.Anim (survey, visible: visible);
		if (visible) {
			uiController.SetTitle(Title.survey);
		}
	}

}
