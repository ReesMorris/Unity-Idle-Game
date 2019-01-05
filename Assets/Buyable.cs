using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Buyable : MonoBehaviour {

    public string name;
    public Button buttonUI;
    public Text costUI;
    public float cost;
    public float upgradeCostIncrement;

    int owned = 0;
    MoneyManager moneyManager;

	void Start () {
        moneyManager = MoneyManager.Instance;
        buttonUI.onClick.AddListener(OnButtonClick);

        UpdateUI();

    }

    void OnButtonClick() {

        if (moneyManager.CanAffordPurchase(cost))
            OnPurchase();
    }

    void OnPurchase() {
        owned++;

        cost *= upgradeCostIncrement;

        UpdateUI();
    }

    void UpdateUI() {
        costUI.text = moneyManager.GetFormattedMoney(cost);
    }
}
