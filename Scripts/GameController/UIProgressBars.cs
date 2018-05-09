using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class UIProgressBars : MonoBehaviour {

	// Bottom: status bar
	public Slider statusProgressBar;
	public Text statusText;

	// Top left: radial progress bar
	public Image radialProgressBar;
	public Text pseudo;

	UIController uiController;

	// Use this for initialization
	void Start () {
		uiController = GetComponent<UIController> ();
	}
	
	// Update is called once per frame
	void Update () {}

	// ------------------------- //

	public void Init (string pseudo) {
		this.pseudo.text = pseudo;
	}

	// --------------------- //

	void Anim (Slider slider, bool visible=true, bool glow=false) {
		uiController.Anim (slider.gameObject, visible, glow);
	}

	void Anim (Text text, bool visible=true, bool glow=false) {
		uiController.Anim (text.gameObject, visible, glow);
	}

	void Anim (Image image, bool visible=true, bool glow=false) {
		uiController.Anim (image.gameObject, visible, glow);
	}

	// ------------------------ //

	public void ShowStatus (bool visible=true) {

		Anim (statusProgressBar, visible: visible);
		Anim (statusText, visible: visible);
	}

	public void ShowProgress (bool visible=true) {

		Anim (radialProgressBar, visible: visible);
		Anim (pseudo, visible: visible);
	}

	public void ShowWaitingMessage (int progress) {

		UpdateStatus (progress);
		ShowStatus (true);
		StatusMessage (Texts.waitingOtherPlayers);
	}

	// ------------------------------ //

	public void UpdateRadial (int value, int maxValue) {
		radialProgressBar.fillAmount = (float) value / maxValue;
	}

	public void UpdateStatus (int value, int maxValue=100) {
		statusProgressBar.value = (float) value / maxValue;
	}

	// ------------------------- //

	public void StatusMessage (string msg, Color color=default(Color), bool glow=false) {

		Anim (statusText, visible: true, glow: glow);
		statusText.text = msg;
		if (color == default(Color)) {
			statusText.color = Color.black;
		} else {
			statusText.color = color;
		}
	}

}
