using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopUI : MonoBehaviour {

    // Public Variables
    [Header("UI Elements")]
    [Tooltip("The UI element of the text which displays our money")] public Text moneyText;
    [Tooltip("The text showing the income per minute")] public Text incomePerMinute;

    // Private Variables
    private MoneyManager moneyManager;

    void Start() {
        // Get references to other scripts
        moneyManager = MoneyManager.Instance;

        // Add event listeners
        MoneyManager.onMoneyChanged += OnMoneyChanged;
        Buyable.onVariableChanged += OnBuyableEvent;
        Buyable.onManagerHired += OnBuyableEvent;
        GameManager.onLoadingComplete += OnLoadingComplete;
    }

    // The network is ready; let's go
    void OnLoadingComplete() {
        UpdateIncomePerMinute();
    }

    // Called by MoneyManager whenever the player's balance is changed
    void OnMoneyChanged(double money) {
        moneyText.text = moneyManager.GetFormattedMoney(money, true);
    }

    // Called by Buyable, listening for a change in a variable or when a manager is hired
    void OnBuyableEvent(Buyable b) {
        UpdateIncomePerMinute();
    }

    // Update the UI text to show the new income per minute
    void UpdateIncomePerMinute() {
        incomePerMinute.text = moneyManager.GetFormattedMoney(moneyManager.EstimateMinutelyProfit(), false) + "/min";
    }
}
