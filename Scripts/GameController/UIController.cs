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

	public Button buttonNext;
	public Button buttonPrevious;

	public Button buttonWoodWheat;
	public Button buttonWoodStone;
	public Button buttonStoneWood;
	public Button buttonStoneWheat;

	public Slider statusProgressBar;
	public Image radialProgressBar;

	public Text score;
	public Text scoreToAdd;
	public Text statusText;
	public Text pseudo;
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

	GameController gameController;

	// -------------- Inherited from MonoBehavior ---------------------------- //

	void Awake () {
		
		gameController = GetComponent<GameController> ();
	}

	void Start () {
		AssociatePushButtons ();
	}

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

		Debug.Log("UIController (Main): User clicked on button 'Next'.");

		gameController.UserNext ();
		buttonNext.interactable = false;
		Anim (buttonNext.gameObject, visible: false);
	}

	void ButtonPrevious () {
		
		Debug.Log("UIController (Main): User clicked on button 'Previous'.");
	
	}

	void ButtonWoodWheat () {

		buttonWoodStone.interactable = false;
		buttonWoodWheat.interactable = false;

		Anim (buttonWoodWheat.gameObject, glow: true);
		Anim (buttonWoodStone.gameObject, visible: false);
	}

	void ButtonWoodStone () {

		buttonWoodStone.interactable = false;
		buttonWoodWheat.interactable = false;

		Anim (buttonWoodStone.gameObject, glow: true);
		Anim (buttonWoodWheat.gameObject, visible: false);
	
	}

	void ButtonStoneWood () {

		buttonStoneWood.interactable = false;
		buttonStoneWheat.interactable = false;

		Anim (buttonStoneWood.gameObject, glow: true);
		Anim (buttonStoneWheat.gameObject, visible: false);
		
	}

	void ButtonStoneWheat () {

		buttonStoneWood.interactable = false;
		buttonStoneWheat.interactable = false;

		Anim (buttonStoneWheat.gameObject, glow: true);
		Anim (buttonStoneWood.gameObject, visible: false);
		
	}

	// --------------- Communication with gameController ---------- //

	public void ShowNextButton (bool value = true) {
		Anim (buttonNext.gameObject, visible: value);
	}

	public void ShowWoodButtons (bool value = true) {

		foreach (Button b in new Button[] {buttonWoodStone, buttonWoodWheat}) {
			Anim (b.gameObject, visible: value);
			b.interactable = true;
		}
	}

	public void ShowStoneButtons (bool value = true) {
		foreach (Button b in new Button[] {buttonStoneWood, buttonStoneWheat}) {
			Anim (b.gameObject, visible: value);
			b.interactable = true;
		}
	}

	// ----------------- //

	void Anim (GameObject go, bool visible = true, bool glow = false) {
		Animator anim = go.GetComponent<Animator> ();
		anim.SetBool (Bool.visible, visible);
		anim.SetBool(Bool.glow, glow);
	}

	void UpdateRadialProgressBar (int progress) {
		radialProgressBar.fillAmount = progress / 100;
	}

	void UpdateStatusProgressBar (int progress) {
		statusProgressBar.value = progress / 100;
	}


}



