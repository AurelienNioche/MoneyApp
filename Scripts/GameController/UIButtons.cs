using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UIButtons : MonoBehaviour {

	// 
	public Button next;
	public Button previous;

	public Button woodWheat3G;
	public Button woodStone3G;

	public Button stoneWood3G;
	public Button stoneWheat3G;

	public Button woodWheat4G;
	public Button woodStone4G;

	public Button stoneWood4G;
	public Button stoneWheat4G;

	public Button stoneClay;
	public Button woodClay;

	public Button clayWood;
	public Button clayStone;
	public Button clayWheat;


	// ------------------------ //

	Button woodStone;
	Button woodWheat;

	Button stoneWheat;
	Button stoneWood;

	// ------------------ //

	List<Button> wood;
	List<Button> stone;
	List<Button> clay;

	// ------------------- //

	GameController gameController;
	UIController uiController;

	int nGood;
	int goodChosen;

	void Awake () {
		gameController = GetComponent<GameController> ();
		uiController = GetComponent<UIController> ();
	}

	// Use this for initialization
	void Start () {
		AssociatePush ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// -------------------------------------------------------- //

	public void Init(int nGood) {

		this.nGood = nGood;
		Select ();
		MakeList ();
		AssociateChoice ();
	}

	void Select () {

		if (nGood == 3) {
			woodStone = woodStone3G;
			woodWheat = woodWheat3G;

			stoneWheat = stoneWheat3G;
			stoneWood = stoneWood3G;
		} else {
			woodStone = woodStone4G;
			woodWheat = woodWheat4G;

			stoneWheat = stoneWheat4G;
			stoneWood = stoneWood4G;
		}
	}

	void MakeList () {

		// Wood
		wood = new List<Button> () { woodStone, woodWheat };
		if (nGood == 4) {
			wood.Add (woodClay);
		}

		// Stone 
		stone = new List<Button> () { stoneWheat, stoneWood };
		if (nGood == 4) {
			stone.Add (stoneClay);
		}

		// Clay
		clay = new List<Button> () { clayStone, clayWood, clayWheat };

	}

	// ---------------- Get components ----------------------- //

	void AssociatePush () {

		next.onClick.AddListener (Next);
		previous.onClick.AddListener (Previous);
	}

	public void AssociateChoice () {

		stoneWheat.onClick.AddListener (StoneWheat);
		stoneWood.onClick.AddListener (StoneWood);
		stoneClay.onClick.AddListener (StoneClay);

		woodWheat.onClick.AddListener (WoodWheat);
		woodStone.onClick.AddListener (WoodStone);
		woodClay.onClick.AddListener (WoodClay);

		clayStone.onClick.AddListener (ClayStone);
		clayWheat.onClick.AddListener (ClayWheat);
		clayWood.onClick.AddListener (ClayWood);
	}

	// ---------------------------------- //

	void Anim (Button btn, bool visible=true, bool glow=false) {
		uiController.Anim (btn.gameObject, visible, glow);
	}

	// ------------------------------------------ //

	void Next () {

		Debug.Log("[UIButtons] User clicked on button 'Next'.");

		next.interactable = false;
		Anim (next, visible: false);

		gameController.UserReplied ();
	}

	void Previous () {

		Debug.Log("[UIButtons] User clicked on button 'Previous'.");

	}

	void WoodWheat () {

		Debug.Log("[UIButtons] User clicked on button 'WoodWheat'.");

		woodStone.interactable = false;
		woodWheat.interactable = false;
		woodClay.interactable = false;

		Anim (woodWheat, glow: true);
		Anim (woodStone, visible: false);
		Anim (woodClay, visible: false);

		goodChosen = Good.wheat;

		gameController.UserReplied ();
	}

	void WoodStone () {

		Debug.Log("[UIButtons] User clicked on button 'WoodStone'.");

		woodStone.interactable = false;
		woodWheat.interactable = false;
		woodClay.interactable = false;

		Anim (woodStone, glow: true);
		Anim (woodWheat, visible: false);
		Anim (woodClay, visible: false);

		goodChosen = Good.stone;

		gameController.UserReplied ();
	}

	void WoodClay () {

		Debug.Log("[UIButtons] User clicked on button 'WoodClay'.");

		woodStone.interactable = false;
		woodWheat.interactable = false;
		woodClay.interactable = false;

		Anim (woodClay, glow: true);
		Anim (woodWheat, visible: false);
		Anim (woodStone, visible: false);

		goodChosen = Good.clay;

		gameController.UserReplied ();
	}

	void StoneWood () {

		Debug.Log("[UIButtons] User clicked on button 'StoneWood'.");

		stoneWood.interactable = false;
		stoneWheat.interactable = false;
		stoneClay.interactable = false;

		Anim (stoneWood, glow: true);
		Anim (stoneWheat, visible: false);
		Anim (stoneClay, visible: false);

		goodChosen = Good.wood;

		gameController.UserReplied ();
	}

	void StoneWheat () {

		Debug.Log("[UIButtons] User clicked on button 'StoneWheat'.");

		stoneWood.interactable = false;
		stoneWheat.interactable = false;
		stoneClay.interactable = false;

		Anim (stoneWheat, glow: true);
		Anim (stoneWood, visible: false);
		Anim (stoneClay, visible: false);

		goodChosen = Good.wheat;

		gameController.UserReplied ();
	}

	void StoneClay () {

		Debug.Log("[UIButtons] User clicked on button 'StoneClay'.");

		stoneWood.interactable = false;
		stoneWheat.interactable = false;
		stoneClay.interactable = false;

		Anim (stoneClay, glow: true);
		Anim (stoneWood, visible: false);
		Anim (stoneWheat, visible: false);

		goodChosen = Good.clay;

		gameController.UserReplied ();
	}

	void ClayWood () {

		Debug.Log("[UIButtons] User clicked on button 'ClayWood'.");

		clayStone.interactable = false;
		clayWood.interactable = false;
		clayWheat.interactable = false;

		Anim (clayWood, glow: true);
		Anim (clayWheat, visible: false);
		Anim (clayStone, visible: false);

		goodChosen = Good.wood;

		gameController.UserReplied ();
	}

	void ClayStone () {

		Debug.Log("[UIButtons] User clicked on button 'ClayStone'.");

		clayStone.interactable = false;
		clayWood.interactable = false;
		clayWheat.interactable = false;

		Anim (clayStone, glow: true);
		Anim (clayWood, visible: false);
		Anim (clayWheat, visible: false);

		goodChosen = Good.stone;

		gameController.UserReplied ();
	}

	void ClayWheat () {

		Debug.Log("[UIButtons] User clicked on button 'ClayWheat'.");

		clayStone.interactable = false;
		clayWood.interactable = false;
		clayWheat.interactable = false;

		Anim (clayWheat, glow: true);
		Anim (clayWood, visible: false);
		Anim (clayStone, visible: false);

		goodChosen = Good.wheat;

		gameController.UserReplied ();
	}

	void ActionOnButtonList (List<Button> buttonList, bool visible=false, 
		bool glow=false, bool interactable=false) {

		foreach (Button b in buttonList) {
			Anim (b, visible: visible, glow: glow);
			b.interactable = interactable;
		}
	}

	public void ShowNext (bool visible=true, bool glow=false) {
		next.interactable = visible;
		Anim (next, visible: visible, glow: glow);
	}

	public void ShowWood (bool visible=true, bool glow=false) {
		ActionOnButtonList (wood, visible: visible, glow: glow, interactable: visible);
	}

	public void ShowStone (bool visible=true, bool glow=false) {
		ActionOnButtonList (stone, visible: visible, glow: glow, interactable: visible);
	}

	public void ShowClay (bool visible=true, bool glow=false) {
		ActionOnButtonList (clay, visible: visible, glow: glow, interactable: visible);
	}

	public void ShowCorrespondingButton (int goodInHand, int goodDesired) {
		switch (goodInHand) {

		case Good.wood:

			switch (goodDesired) {
			case Good.wheat:
				Anim (woodWheat, visible: true, glow: true);
				break;
			case Good.stone:
				Anim (woodStone, visible: true, glow: true);
				break;
			case Good.clay:
				Anim (woodClay, visible: true, glow: true);
				break;
			default:
				throw new Exception ("[UIButtons] Good " + goodDesired + " doesn't exist or is not a possibility.");
			}
			break;

		case Good.stone:
			switch (goodDesired) {
			case Good.wheat:
				Anim (stoneWheat, visible: true, glow: true);
				break;
			case Good.wood:
				Anim (stoneWood, visible: true, glow: true);
				break;
			case Good.clay:
				Anim (stoneClay, visible: true, glow: true);
				break;
			default:
				throw new Exception ("[UIButtons] Good " + goodDesired + " doesn't exist or is not a possibility.");
			}
			break;

		case Good.clay:
			switch (goodDesired) {
			case Good.wheat:
				Anim (clayWheat, visible: true, glow: true);
				break;
			case Good.wood:
				Anim (clayWood, visible: true, glow: true);
				break;
			case Good.stone:
				Anim (clayStone, visible: true, glow: true);
				break;
			default:
				throw new Exception ("[UIButtons] Good " + goodDesired + " doesn't exist or is not a possibility.");
			}
			break;
		default:
			throw new Exception ("[UIButtons] Good " + goodInHand + " doesn't exist or is not a possibility.");
		}
	}

	public int GetGoodChosen() {return goodChosen;}
}
