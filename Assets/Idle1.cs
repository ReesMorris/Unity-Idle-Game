using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This script is one example of how you can make the game your own
// You're recommended to use this as a template for your own work, but it's up to you.

[RequireComponent(typeof(Buyable))]
public class Idle1 : MonoBehaviour {

    // Public Variables
    [Header("UI Elements")]
    public Button upgradeButton;
    public Button manualTapButton;
    public Button managerButton;
    public Text upgradeCostText;
    public Text profitText;
    public Text managerCostText;
    public Text amountOwnedText;
    public Text timerText;
    public Image progressFill;
    public Image ownedFill;

    // Private Variables
    Buyable buyable;
    Utilities utilities;
    MoneyManager moneyManager;

    void Awake() {
        buyable = GetComponent<Buyable>();
        utilities = Utilities.Instance;
        moneyManager = MoneyManager.Instance;

        // Events to tell the Buyable script (attached) that we're interacting
        upgradeButton.onClick.AddListener(buyable.UpgradeButtonPressed);
        manualTapButton.onClick.AddListener(buyable.RunProcess);
        managerButton.onClick.AddListener(buyable.BuyManagerButtonPressed);

        // Event listeners from the Buyable script
        Buyable.onVariableChanged += OnVariableChanged;
        Buyable.onProcessBegin += OnProcessBegin;
        Buyable.onProcessUpdate += OnProcessUpdate;
        Buyable.onProcessFinish += OnProcessFinish;
        Buyable.onManagerHired += OnManagerHired;
    }

    void Start() {
        UpdateUI();
        StartCoroutine(UpdateTimer());
    }

    // Update the UI display with content from the Buyable Data
    void UpdateUI() {
        upgradeCostText.text = moneyManager.GetFormattedMoney(buyable.Data.Cost, false);
        profitText.text = moneyManager.GetFormattedMoney(buyable.Data.GetRevenue(), false);
        amountOwnedText.text = buyable.Data.Owned.ToString();
        ownedFill.fillAmount = buyable.Data.NextMilestonePercentage();
        managerCostText.text = moneyManager.GetFormattedMoney(buyable.Data.managerCost, false);
    }

    IEnumerator UpdateTimer() {
        while(true) {
            print(buyable.Data.MinutelyProfit);
            if(buyable.Data.ProcessInProgress()) {
                timerText.text = utilities.SecondsToHHMMSS(buyable.Data.SecondsToProcessCompletion());
            } else {
                timerText.text = utilities.SecondsToHHMMSS(buyable.Data.ProcessTime);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    /* Event listeners from the Buyable delegate */

    // Called when a variable is changed
    void OnVariableChanged(Buyable b) {
        if (b == buyable) {
            UpdateUI();
        }
    }

    // Called when the process begins
    void OnProcessBegin(Buyable b) {
        if (b == buyable) {

        }
    }

    // Called when the process percentage is changed
    void OnProcessUpdate(Buyable b, float progress) {
        if (b == buyable) {
            if (buyable.Data.ProcessTime > 0.25f)
                progressFill.fillAmount = progress;
            else
                progressFill.fillAmount = 1000f;
        }
    }

    // Called when the process end
    void OnProcessFinish(Buyable b) {
        if (b == buyable) {
            progressFill.fillAmount = 0f;
        }
    }

    // Called after a manager is hired
    void OnManagerHired(Buyable b) {
        if (b == buyable) {
            managerButton.gameObject.SetActive(false);
        }
    }
}
