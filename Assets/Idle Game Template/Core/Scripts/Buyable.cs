using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buyable : MonoBehaviour {

    // Delegates which can be used by custom scripts to shape the game into your own
    public delegate void OnVariableChanged(Buyable buyable);
    public static OnVariableChanged onVariableChanged;
    public delegate void OnProcessBegin(Buyable buyable);
    public static OnProcessBegin onProcessBegin;
    public delegate void OnProcessUpdate(Buyable buyable, float progress);
    public static OnProcessUpdate onProcessUpdate;
    public delegate void OnProcessFinish(Buyable buyable);
    public static OnProcessFinish onProcessFinish;
    public delegate void OnManagerHired(Buyable buyable);
    public static OnManagerHired onManagerHired;
    public delegate void OnBuyablePurchase(Buyable buyable);
    public static OnBuyablePurchase onBuyablePurchase;

    // Private & Protected Variables
    public BuyableData Data { get; private set; }
    private MoneyManager moneyManager;

    public void Init(BuyableData buyableData) {
        // Set up the data
        Data = buyableData;
        Data.Init();

        // Add references and events
        moneyManager = MoneyManager.Instance;
        BuyableData.onDataLoaded += OnBuyableDataLoaded;
    }

    public void RunProcess() {
        if(Data.Owned > 0 && !Data.ProcessInProgress()) {
            Data.BeginProcess();
            StartCoroutine(Process());

            // Send an event out to say that we've updated some variable (so the UI can be updated)
            if (onProcessBegin != null)
                onProcessBegin(this);
        }
    }

    // Called by the script attached to this buyable when the upgrade button is pressed
    public void UpgradeButtonPressed() {
        if (moneyManager.CanAffordPurchase(Data.Cost)) {
            if(Data.Owned + 1 <= Data.maxAmount) {
                moneyManager.ReduceMoney(Data.Cost);
                Data.Upgrade();

                // Send an event out to say that we've updated some variable (so the UI can be updated)
                if (onVariableChanged != null)
                    onVariableChanged(this);

                // If this is the first upgrade (a purchase), send out an additional delegate
                if (onBuyablePurchase != null)
                    onBuyablePurchase(this);
            }
        }
    }

    // Called by the script attached to this when a manager button is pressed
    public void BuyManagerButtonPressed() {
        if (!Data.HasManager && Data.Owned > 0 && moneyManager.CanAffordPurchase(Data.managerCost)) {
            moneyManager.ReduceMoney(Data.managerCost);
            Data.AddManager();
            RestartProcess();

            // Trigger the event to say that a manager has been hired
            if (onManagerHired != null)
                onManagerHired(this);
        }
    }

    
    // Handles the process UI
    IEnumerator Process() {
        // Run continuously whilst the process is not complete
        while(Data.ProcessInProgress()) {
            float progress = Data.GetProcessCompletion();
            if(onProcessUpdate != null)
                onProcessUpdate(this, progress);

            // Repeat every duration/120f seconds; if a process takes 1 hour there's no need to update UI every frame
            yield return new WaitForSeconds(Data.ProcessTime / 120f);
        }

        // The process is complete
        moneyManager.AddMoney(Data.GetRevenue());
        if(onProcessFinish != null)
            onProcessFinish(this);

        // Restart (if we have a manager)
        RestartProcess();

    }

    // Called when a Buyable's Data is loaded
    void OnBuyableDataLoaded(BuyableData data) {
        if(data == Data) {
            RestartProcess();
        }
    }

    // Restart the coroutine above if we have a manager
    void RestartProcess() {
        if(Data.HasManager) {
            RunProcess();
        }
    }
}
