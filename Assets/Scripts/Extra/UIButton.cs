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
    private MoneyManager moneyManager;
	
	void Awake () {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        moneyManager = MoneyManager.Instance;
        MoneyManager.onMoneyChanged += OnMoneyChanged;
    }

    void Start() {
        SetUI();
    }

    // Called when the money value is changed
    void OnMoneyChanged(double money) {
        SetUI();
    }

    void SetUI() {
        if(buyable != null) {
            double cost = buyable.Data.Cost;
            if (buttonType == ButtonTypes.ManagerBuy)
                cost = buyable.Data.managerCost;
            if (moneyManager.Money > cost)
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
