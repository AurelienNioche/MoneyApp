using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UITutorial : MonoBehaviour {

	public Image character;

	public Image woodInBox;
	public Image stoneInBox;
	public Image clayInBox;
	public Image wheatInBox;

	public Image woodDesired;
	public Image stoneDesired;
	public Image wheatDesired;
	public Image clayDesired;

	public Image frame;
	public Image bubble;
	public Image redCross;

	public GameObject goodsPresentation3G;
	public GameObject goodsPresentation4G;

	public GameObject strategies3G;
	public GameObject strategies4G;

	public GameObject secondCharacter;
	public GameObject win;

	public Text mainText;
	public Text plusOne;

	int step;

	int nGoods;

	UnityAction [] explanations;

	UIController uiController;
	UIButtons uiButtons;

	// Use this for initialization
	void Start () {
		uiController = GetComponent<UIController> ();
		uiButtons = GetComponent<UIButtons> ();

		step = -1;
	}
	
	// Update is called once per frame
	void Update () {}

	// ------------------------------------ //

	public void Init (int nGoods, string pseudo) {
		this.nGoods = nGoods;

		Texts.you = String.Format (Texts.you, pseudo);
		Texts.goods = String.Format (Texts.goods, nGoods);

		explanations = new UnityAction[] {
			Goods, Specialization, SpecWood, SpecWheat, SpecStone, SpecClay,
			NoCoincidenceP1, NoCoincidenceP2, Exchange, You, 
			DirectStrategy, IndirectStrategy, Production
		};
	}

	// ------------------------------------ //

	void Anim (GameObject go, bool visible=true, bool glow=false) {
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

	void Anim (Image img, bool visible=true, bool glow=false) {
		Anim (img.gameObject, visible, glow);
	}

	// ------------------------- //

	public void SetText (string value) {
		mainText.text = value;
	}

	public void ShowText (bool visible=true) {
		Anim (mainText, visible: visible);
	}

	// ---------  Steps  -------------- //

	public void Begin () {

		uiController.SetTitle (Title.tutorial);
		Anim (frame);
		ShowText();

		step = 0;
		explanations [step] ();

		uiButtons.ShowNext (glow: true);
	}

	public void Goods () {

		switch (nGoods) {
		case 3:
			Anim (goodsPresentation3G);
			break;

		case 4:
			Anim (goodsPresentation4G);
			break;

		default:
			throw new Exception (nGoods + " is not a number of goods that can be handled.");
		}

		SetText(Texts.goods);
	}

	public void Specialization () {

		Anim (goodsPresentation3G, false);
		Anim (goodsPresentation4G, false);

		Anim (character);
		Anim (bubble);

		Anim (woodInBox);
		Anim (wheatDesired);

		SetText (Texts.specialization);
	}

	public void SpecWood () {

		SetText (Texts.specWood);
	}

	public void SpecWheat () {

		Anim (woodInBox, false);
		Anim (wheatDesired, false);

		Anim (wheatInBox);
		Anim (stoneDesired);

		SetText (Texts.specWheat);
	}

	public void SpecStone () {

		Anim (wheatInBox, false);
		Anim (stoneDesired, false);

		Anim (stoneInBox);

		switch (nGoods) {
		case 3:
			Anim (woodDesired);
			SetText (Texts.specStone3G);
			step += 1; // Skip part 'SpecClay'
			break;
		
		case 4:
			Anim (clayDesired);
			SetText (Texts.specStone4G);
			break;
		
		default:
			throw new Exception (nGoods + " is not a number of goods that can be handled.");
		}
	}

	public void SpecClay () {

		Anim (stoneInBox, false);
		Anim (clayDesired, false);

		Anim (clayInBox);
		Anim (woodDesired);

		SetText (Texts.specClay);
	}

	public void NoCoincidenceP1 () {

		Anim (stoneInBox, false);
		Anim (clayInBox, false);
		Anim (woodDesired, false);

		Anim (woodInBox);
		Anim (wheatDesired);

		SetText (Texts.noCoincidenceP1);
	}

	public void NoCoincidenceP2 () {

		Anim (woodInBox, false);
		Anim (wheatDesired, false);

		Anim (redCross);
		Anim (wheatInBox);
		Anim (woodDesired);

		SetText (Texts.noCoincidenceP2);
	}

	public void Exchange () {

		Anim (redCross, false);
		Anim (wheatInBox, false);
		Anim (woodDesired, false);
		Anim (bubble, false);

		Anim (woodInBox);
		Anim (secondCharacter);

		SetText(Texts.exchange);
	}

	public void You () {

		Anim (secondCharacter, false);

		Anim (bubble);
		Anim (wheatDesired);

		SetText (Texts.you);
	}

	public void DirectStrategy () {

		Anim (woodInBox, false);
		Anim (character, false);
		Anim (bubble, false);
		Anim (wheatDesired, false);

		switch (nGoods) {
		case 3:
			Anim (strategies3G);
			break;
		case 4:
			Anim (strategies4G);
			break;
		default:
			throw new Exception (nGoods + " is not a number of goods that can be handled.");
		}

		SetText (Texts.directStrategy);
	}

	public void IndirectStrategy () {

		switch (nGoods) {
		case 3:
			SetText (Texts.indirectStrategy3G);
			Anim (strategies3G);
			break;
		case 4:
			SetText (Texts.indirectStrategy4G);
			Anim (strategies4G);
			break;
		default:
			throw new Exception (nGoods + " is not a number of goods that can be handled.");
		}
	}

	public void Production () {

		Anim (strategies3G, false);
		Anim (strategies4G, false);

		Anim (character);
		Anim (win);
		Anim (plusOne, glow: true);
		Anim (woodInBox, glow: true);
		SetText (Texts.production);
	}

	public void End () {
		
		Anim (win, false);
		Anim (plusOne, false);
		Anim (woodInBox, false);
		Anim (character, false);
		Anim (frame, false);
		SetText (Texts.production);
	}

	public void NextStep() {

		step += 1;
		uiButtons.ShowNext (glow: true);
		explanations [step] ();
	}

	public bool IsLastStep () {
		return (step + 1) == explanations.Length;
	}
}
