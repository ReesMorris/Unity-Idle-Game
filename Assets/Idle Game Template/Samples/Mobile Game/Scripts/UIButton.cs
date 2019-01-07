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

    void Start() {
        button = GetComponent<Button>();
        image = GetComponent<Image>();

        // Get references to other scripts
        moneyManager = MoneyManager.Instance;

        // Add event listeners
        MoneyManager.onMoneyChanged += OnMoneyChanged;
        GameManager.onLoadingComplete += OnLoadingComplete;
    }

    // The network is ready; let's go
    void OnLoadingComplete() {
        SetUI();
    }

    // Called when the money value is changed
    void OnMoneyChanged(double money) {
        SetUI();
    }

    void SetUI() {
        if(buyable != null) {
            if (buttonType == ButtonTypes.ManagerBuy && buyable.Data.Owned == 0) {
                Disable();
            } else {
                double cost = buyable.Data.Cost;
                if (buttonType == ButtonTypes.ManagerBuy) {
                    cost = buyable.Data.managerCost;
                }
                if (moneyManager.Money > cost)
                    Enable();
                else
                    Disable();
            }
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
