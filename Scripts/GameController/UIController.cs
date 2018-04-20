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


public class UIController : MonoBehaviour {

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
	public Image clayInHand;

	public Image woodInBox;
	public Image stoneInBox;
	public Image clayInBox;

	public Image woodDesired;
	public Image stoneDesired;
	public Image wheatDesired;
	public Image clayDesired;

	public Image character;
	public Image arrow;

	// -------------------- //

	UIButtons uiButtons;
	UITutorial uiTutorial;
	UIProgressBars uiProgressBars;

	// -------------- Inherited from MonoBehavior ---------------------------- //

	void Awake () {
		
		uiButtons = GetComponent<UIButtons> ();
		uiTutorial = GetComponent<UITutorial> ();
		uiProgressBars = GetComponent<UIProgressBars> ();
	}

	void Start () {}

	void Update () {}

	// ----------------- //

	public void Anim (GameObject go, bool visible=true, bool glow=false) {
		Animator anim = go.GetComponent<Animator> ();
		anim.SetBool (Bool.visible, visible);
		anim.SetBool(Bool.glow, glow);
	}

	void Anim (Button btn, bool visible=true, bool glow=false) {
		Anim (btn.gameObject, visible, glow);
	}

	void Anim (Slider slider, bool visible=true, bool glow=false) {
		Anim (slider.gameObject, visible, glow);
	}

	void Anim (Text text, bool visible=true, bool glow=false) {
		Anim (text.gameObject, visible, glow);
	}

	void Anim (Image image, bool visible=true, bool glow=false) {
		Anim (image.gameObject, visible, glow);
	}

	// --------------- Communication with gameController ---------- //

	public void Init (string pseudo, int nGoods) {

		Debug.Log ("nGoods: " + nGoods);

		uiProgressBars.Init (pseudo);
		uiButtons.Init (nGoods);
		uiTutorial.Init (nGoods, pseudo);
	}

	// ------------------- // 

	public int GetGoodChosen () {
		return uiButtons.GetGoodChosen();
	}

	// ---------------------- //

	public void SetTitle (string value) {
		title.text = value;
	}
		
	public void SetScore (int n) {
		score.text = n.ToString ();
	}

	// ------- //

	public void ShowCharacter (bool visible=true) {
		Anim (character, visible:visible);
	}

	public void ShowScore (bool visible=true) {
		Anim (ScoreLogo, visible: visible);
		Anim (score, visible: visible);
	}

	public void ShowTitle (bool visible=true) {
		Anim (title, visible: visible);
	}

	public void ShowLogo (bool visible=true, bool glow=false) {
		Anim (logo, visible: visible, glow: glow);
	}

	// ------- HOME VIEW --------- //

	public void HomeWU () {

		ShowLogo ();
		ShowTitle ();
		SetTitle (Title.title);
		uiButtons.ShowNext (glow: true);
	}

	public void HomeWS () {

		ShowLogo (glow: true);
	}

	// -------- TRAINING VIEW ----- //

	public void TrainingBegin () {

		Anim (logo, visible: true);
		uiButtons.ShowNext (glow: true);
		uiTutorial.SetText (Texts.training);
	}

	public void HideTrainingMsg () {

		ShowLogo (visible: false);
		uiTutorial.ShowText (false);
	}

	public void TrainingReady () {

		Anim (woodInBox, false);
		Anim (stoneInBox, false);
		Anim (clayInBox, false);

		Anim (character, false);
		Anim (scoreFinal, false);

		Anim (wheatDesired, false);

		title.text = Title.title;
		Anim (title);

		Anim (logo);
		uiTutorial.SetText(Texts.ready);
		uiTutorial.ShowText ();
		uiButtons.ShowNext (glow: true);
	}

	// ---------------------------------- //

	public void ChoiceView (int goodInHand) {

		HideResults ();

		switch (goodInHand) {

		case Good.wood:
			Anim (woodInBox);
			uiButtons.ShowWood (glow: true);
			break;
		
		case Good.stone:
			Anim (stoneInBox);
			uiButtons.ShowStone (glow: true);
			break;
		
		case Good.clay:
			Anim (clayInBox);
			uiButtons.ShowClay (glow: true);
			break;
		
		default:
			throw new Exception ("Good " + goodInHand + " doesn't exist");
		}

	}

	public void ResultView (bool success, int goodInHand, int goodDesired) {

		uiButtons.ShowStone (false);
		uiButtons.ShowWood (false);
		uiButtons.ShowClay (false);

		if (success) {

			Anim (pictureSuccess);
			Anim (arrow);

			// Depending of good in hand
			switch (goodInHand) {

			case Good.wood:
				Anim (woodInBox, false);
				Anim (woodInHand);
				break;
			
			case Good.stone:
				Anim (stoneInBox, false);
				Anim (stoneInHand);
				break;
			
			case Good.clay:
				Anim (clayInBox, false);
				Anim (clayInHand);
				break;

			default:
				throw new Exception ("Good " + goodInHand + " doesn't exist");
			}

			// Depending of the desired good
			switch (goodDesired) {
			case Good.wheat:
				Anim (scoreToAdd, glow: true);
				Anim (wheatDesired);
				break;
			
			case Good.wood:
				Anim (woodDesired);
				break;

			case Good.stone:
				Anim (stoneDesired);
				break;

			case Good.clay:
				Anim (clayDesired);
				break;

			default:
				throw new Exception ("Good " + goodDesired + " doesn't exist");
			}

		// if not a success...
		} else {
			Anim (pictureLost);
		}

		uiButtons.ShowNext (glow: true);
	}

	public void HideResults () {

		uiButtons.ShowNext (false);

		foreach (Image img in new Image[] {
			pictureSuccess, pictureLost,
			arrow,
			woodInHand, stoneInHand, clayInHand,
			woodDesired, wheatDesired, stoneDesired, clayDesired
		}) {
			Anim (img, false);
		}

		Anim (scoreToAdd, false);
	}

	public void ChoiceMadeView (int goodInHand, int goodDesired) {

		uiButtons.ShowCorrespondingButton (goodInHand: goodInHand, goodDesired: goodDesired);

		switch (goodInHand) {

		case Good.wood:
			Anim (woodInBox);
			break;

		case Good.stone:
			Anim (stoneInBox);
			break;

		case Good.clay:
			Anim (clayInHand);
			break;
		default:
			throw new Exception ("Good " + goodDesired + " doesn't exist");
		}
	}

	public void EndView (int scoreValue, int tMax) {

		uiProgressBars.UpdateRadial (tMax, tMax);
		ShowScore (false);
		scoreFinal.text = scoreValue.ToString ();
		Anim (scoreFinal, glow: true);
		Anim (wheatDesired);
		title.text = Title.end;
		Anim (title);
	}

}
