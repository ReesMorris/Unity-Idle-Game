using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour {

    public Color colour;
    public Color disabledColour;
    [Tooltip("Not required, but will enable/disable depending on player income if not null")] public Buyable buyable;
    public enum ButtonTypes { Normal, IdleUpgrade, ManagerBuy }
    public ButtonTypes buttonType;

    private Button button;
    private Image image;
    public bool Pressed { get; protected set; }
	
	void Awake () {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        MoneyManager.onMoneyChanged += OnMoneyChanged;
    }

    // Called when the money value is changed
    void OnMoneyChanged(double money) {

        // A button related to the game?
        if(buyable != null) {

            // We don't care about WHAT we're buying here; just how much it costs
            double cost = buyable.Data.Cost;
            if(buttonType == ButtonTypes.ManagerBuy)
                cost = buyable.Data.managerCost;

            if (money > cost)
                Enable();
            else
                Disable();
        }
    }
	
    /* Handle Enable & Disabling */

    void Enable() {
        button.interactable = true;
        image.color = colour;
    }
    public void Disable() {
        button.interactable = false;
        image.color = disabledColour;
    }

}
