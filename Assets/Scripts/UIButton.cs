using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour {

    public Color colour;
    public Color disabledColour;

    private Button button;
    private Image image;
    public bool Pressed { get; protected set; }
	
	void Start () {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
	}

    public void SetState(bool enabled) {
        if (enabled)
            Enable();
        else
            Disable();
    }
	
    public void Enable() {
        button.interactable = true;
        image.color = colour;
    }

    public void Disable() {
        button.interactable = false;
        image.color = disabledColour;
    }

    public void OnClickHeld() {
        if (button.interactable) {
            Pressed = true;
            transform.localScale = new Vector3(.95f, .95f, .95f);
        } else {
            OnClickReleased();
        }
    }

    public void OnClickReleased() {
        Pressed = false;
        transform.localScale = Vector3.one;

    }
}
