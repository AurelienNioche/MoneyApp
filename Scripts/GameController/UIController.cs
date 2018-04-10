using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


class Bool {
	public static string visible = "Visible";
	public static string glow = "Glow";
}

class Good {
	public static int wood = 0;
	public static int wheat = 1;
	public static int stone = 2;
}


public class UIController : MonoBehaviour {

	// Buttons
	public Button buttonNext;
	public Button buttonPrevious;

	public Button buttonWoodWheat;
	public Button buttonWoodStone;
	public Button buttonStoneWood;
	public Button buttonStoneWheat;

	// Bottom: status bar
	public Slider statusProgressBar;
	public Text statusText;

	// Top left: radial progress bar
	public Image radialProgressBar;
	public Text pseudo;

	// Top right: Score
	public Text score;
	public Text scoreToAdd;
	public Text scoreFinal;

	public Text title;

	public Image logo;
	public Image ScoreLogo;
	public Image pictureSuccess;
	public Image pictureLost;
	public Image woodInHand;
	public Image stoneInHand;
	public Image woodInBox;
	public Image stoneInBox;
	public Image woodDesired;
	public Image stoneDesired;
	public Image wheatDesired;
	public Image character;
	public Image arrow;

	//  --------- Tutorial ----------- //

	public Image illustrationThreeGoods;
	public Image illustrationSpecialization;
	public Image illustrationMarkets;
	public Image illustrationYou;
	public Image illustrationStrategies;

	public Text textThreeGoods;
	public Text textSpecialization;
	public Text textMarkets;
	public Text textYou;
	public Text textStrategyDirect;
	public Text textStrategyIndirect;
	public Text textTraining;
	public Text textReady;

	// -------- Survey --------- //

	public GameObject survey;
	public InputField age;
	public Toggle male;
	public Toggle female;
	public Toggle consent;

	// -------------------- //

	GameController gameController;

	int goodChosen;

	// -------------- Inherited from MonoBehavior ---------------------------- //

	void Awake () {
		
		gameController = GetComponent<GameController> ();
		AssociatePushButtons ();
	}

	void Start () {}

	void Update () {}

	// ---------------- Get components ----------------------- //

	// ---------------- Get components ----------------------- //

	void AssociatePushButtons () {

		buttonNext.onClick.AddListener (ButtonNext);
		buttonPrevious.onClick.AddListener (ButtonPrevious);

		buttonStoneWheat.onClick.AddListener (ButtonStoneWheat);
		buttonStoneWood.onClick.AddListener (ButtonStoneWood);
		buttonWoodWheat.onClick.AddListener (ButtonWoodWheat);
		buttonWoodStone.onClick.AddListener (ButtonWoodStone);
	}

	// ------------------------------------------ //

	void ButtonNext () {

		Debug.Log("UIController: User clicked on button 'Next'.");

		buttonNext.interactable = false;
		Anim (buttonNext, visible: false);

		gameController.UserReplied ();
	}

	void ButtonPrevious () {
		
		Debug.Log("UIController: User clicked on button 'Previous'.");
	
	}

	void ButtonWoodWheat () {

		buttonWoodStone.interactable = false;
		buttonWoodWheat.interactable = false;

		Anim (buttonWoodWheat, glow: true);
		Anim (buttonWoodStone, visible: false);

		goodChosen = Good.wheat;

		gameController.UserReplied ();
	}

	void ButtonWoodStone () {

		buttonWoodStone.interactable = false;
		buttonWoodWheat.interactable = false;

		Anim (buttonWoodStone, glow: true);
		Anim (buttonWoodWheat, visible: false);

		goodChosen = Good.stone;

		gameController.UserReplied ();
	}

	void ButtonStoneWood () {

		buttonStoneWood.interactable = false;
		buttonStoneWheat.interactable = false;

		Anim (buttonStoneWood, glow: true);
		Anim (buttonStoneWheat, visible: false);

		goodChosen = Good.wood;

		gameController.UserReplied ();
	}

	void ButtonStoneWheat () {

		buttonStoneWood.interactable = false;
		buttonStoneWheat.interactable = false;

		Anim (buttonStoneWheat, glow: true);
		Anim (buttonStoneWood, visible: false);

		goodChosen = Good.wheat;

		gameController.UserReplied ();
	}

	// --------------- Communication with gameController ---------- //

	public void ShowNextButton (bool visible=true, bool glow=false) {
		buttonNext.interactable = true;
		Anim (buttonNext, visible: visible, glow: glow);
	}

	public void ShowWoodButtons (bool value=true) {

		foreach (Button b in new Button[] {buttonWoodStone, buttonWoodWheat}) {
			Anim (b, visible: value, glow:true);
			b.interactable = true;
		}
	}

	public void ShowStoneButtons (bool value=true) {
		foreach (Button b in new Button[] {buttonStoneWood, buttonStoneWheat}) {
			Anim (b, visible: value, glow: true);
			b.interactable = true;
		}
	}

	// ----------------- //

	void Anim (GameObject go, bool visible=true, bool glow=false) {
		Animator anim = go.GetComponent<Animator> ();
		anim.SetBool (Bool.visible, visible);
		anim.SetBool(Bool.glow, glow);
	}

	void Anim (Text txt, bool visible=true, bool glow=false) {
		Anim (txt.gameObject, visible, glow);
	}

	void Anim (Image img, bool visible=true, bool glow=false) {
		Anim (img.gameObject, visible, glow);
	}

	void Anim (Button btn, bool visible=true, bool glow=false) {
		Anim (btn.gameObject, visible, glow);
	}

	void Anim (Slider slider, bool visible=true, bool glow=false) {
		Anim (slider.gameObject, visible, glow);
	}


	// -------------------------- //

	public void UpdateRadialProgressBar (int value, int maxValue) {
		radialProgressBar.fillAmount = (float) value / maxValue;
	}

	public void UpdateStatusProgressBar (int progress) {
		statusProgressBar.value = progress / 100f;
	}
		
	// ------------------------- //

	void StatusMessage (string msg, Color color=default(Color), bool glow=false) {

		Anim (statusText, visible: true, glow: glow);
		statusText.text = msg;
		if (color == default(Color)) {
			statusText.color = Color.black;
		} else {
			statusText.color = color;
		}
	}

	// -------------------- //

	public bool EvaluateUserData () {

		if (age.text.Length == 0) {

			StatusMessage ("Vous devez indiquer votre age!", color:Color.red, glow: true);
			return false;
		}	

		if (!male.isOn && !female.isOn) {
			
			StatusMessage ("Vous devez indiquer votre sexe!", color: Color.red, glow: true);
			return false;
		}

		if (!consent.isOn) {
			
			StatusMessage ("Vous devez donner votre consentement!", color: Color.red, glow: true);
			return false;
		}
			
		return true;
	}

	// ------------------- // 

	public int GetGoodChosen () {
		return goodChosen;
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

	// --------------------- //

	public void SetPseudo (string value) {
		pseudo.text = value;
		textYou.text = String.Format (textYou.text, value);
	}
		
	// ----------------------- //

	void ShowStatusBar(bool visible=true) {
		Anim (statusProgressBar, visible: visible);
		Anim (statusText, visible: visible);
	}


	public void ShowHome (bool visible=true) {
		
		Anim (logo, visible: visible);
		Anim (title, visible: visible);
	}

	public void ShowCharacter (bool visible=true) {
		Anim (character, visible:visible);
	}

	public void ShowScore (bool visible=true) {
		Anim (ScoreLogo, visible: visible);
		Anim (score, visible: visible);
	}

	public void ShowProgress (bool visible=true) {

		Anim (radialProgressBar, visible: visible);
		Anim (pseudo, visible: visible);

	}

	public void ShowTitle (bool visible=true) {
		Anim (title, visible: false);
	}


	public void HideLogoAndStatusBar () {
		ShowStatusBar (false);
		Anim (logo, visible: false);
	}

	public void HideResults () {

		Anim (buttonNext, visible: false);

		Anim (pictureSuccess, visible: false);
		Anim (pictureLost, visible: false);

		Anim (arrow, visible: false);

		Anim (woodInHand, visible: false);
		Anim (stoneInHand, visible: false);

		Anim (woodDesired, visible: false);
		Anim (wheatDesired, visible: false);
		Anim (stoneDesired, visible: false);

		Anim (scoreToAdd, visible: false);
	}

	// --------------------- //

	public void SetScore (int n) {
		score.text = n.ToString ();
	}

	// -------------------- //

	public void HomeWU () {

		ShowHome ();
		Anim (buttonNext, visible: true, glow: true);
	}

	public void HomeWS () {

		Anim (logo, glow: true);
	}

	public void SurveyWU () {

		ShowStatusBar (false);
		Anim (logo, visible: false);
		ShowNextButton ();
		Anim (survey, visible: true);
		title.text = "Prélude";
	}

	public void SurveyWS () {

		Anim (survey, visible: false);
		Anim (logo, visible: true, glow: true);
	}

	public void TutoThreeGoods (bool visible=true) {

		title.text = "Prélude";

		Anim (illustrationThreeGoods, visible: visible);
		Anim (textThreeGoods, visible: visible);
	}

	public void TutoSpec (bool visible=true) {

		Anim (illustrationSpecialization, visible: visible);
		Anim (textSpecialization, visible: visible);
	}

	public void TutoMarkets (bool visible=true) {

		Anim (illustrationMarkets, visible: visible);
		Anim (textMarkets, visible: visible);
	}

	public void TutoYou (bool visible=true) {

		Anim (illustrationYou, visible: visible);
		Anim (textYou, visible: visible);
	}

	public void TutoStrategyDirect (bool visible=true) {

		Anim (illustrationStrategies, visible: visible);
		Anim (textStrategyDirect, visible: visible);
	}

	public void TutoStrategyIndirect (bool visible=true) {

		Anim (illustrationStrategies, visible: visible);
		Anim (textStrategyIndirect, visible: visible);
	}
		
	public void TutoTraining (bool visible=true) {

		Anim (illustrationStrategies, visible: visible);
		Anim (textTraining, visible: visible);
	}

	public void TutoReady (bool visible=true) {

		Anim (woodInBox, visible: false);
		Anim (stoneInBox, visible: false);
		Anim (character, visible: false);
		Anim (scoreFinal, visible: false);
		Anim (wheatDesired, visible: false);
		title.text = "Ganomics";
		Anim (title, visible: true);

		Anim (logo, visible: visible);
		Anim (textReady, visible: visible);
	}

	// ----------------------------------- //
		
	public void StatusProgressBar (int progress, string msg) {

		ShowStatusBar (true);
		UpdateStatusProgressBar (progress);
		StatusMessage (msg);
	}

	// ----------------------------- //

	public void ChoiceViewWU (int goodInHand) {

		HideResults ();

		if (goodInHand == Good.wood) {
			Anim (woodInBox, visible: true);
			ShowWoodButtons ();
		} else {
			Anim (stoneInBox, visible: true);
			ShowStoneButtons ();
		}
	}

	public void ResultView (bool success, int goodInHand, int goodDesired) {

		ShowStoneButtons (false);
		ShowWoodButtons (false);

		if (success) {

			Anim (pictureSuccess, visible: true);
			Anim (arrow, visible: true);

			// Depending of good in hand
			if (goodInHand == Good.wood) {
				Anim (woodInBox, visible: false);
				Anim (woodInHand, visible: true);
			} else {
				Anim (stoneInBox, visible: false);
				Anim (stoneInHand, visible: true);
			}

			// Depending of the desired good
			if (goodDesired == Good.wheat) {
				Anim (scoreToAdd, visible: true, glow: true);
				Anim (wheatDesired, visible: true);
			} else if (goodDesired == Good.stone) {
				Anim (stoneDesired, visible: true);
			} else {
				Anim (woodDesired, visible: true);
			}

		// if not a success...
		} else {
			Anim (pictureLost, visible: true);
		}

		ShowNextButton (visible: true, glow: true);

	}

	public void ChoiceMadeViewWS (int goodInHand, int goodDesired) {

		if (goodInHand == Good.wood) {
			Anim (woodInBox, visible: true);
			if (goodDesired == Good.wheat) {
				Anim (buttonWoodWheat, visible: true, glow: true);
			} else {
				Anim (buttonWoodStone, visible: true, glow: true);
			}
		} else {
			Anim (stoneInHand, visible: false);
			ShowStoneButtons ();
		}
	}

	public void EndView (int scoreValue, int tMax) {

		UpdateRadialProgressBar (tMax, tMax);
		ShowScore (false);
		scoreFinal.text = scoreValue.ToString ();
		Anim (scoreFinal, visible: true, glow:true);
		Anim (wheatDesired, visible: true);
		title.text = "End";
		Anim (title, visible: true);
	}

}



