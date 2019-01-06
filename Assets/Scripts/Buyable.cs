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

    // Private & Protected Variables
    public BuyableData Data { get; private set; }
    private MoneyManager moneyManager;
    private GameManager gameManager;

    public void Init(BuyableData buyableData) {
        Data = buyableData;
        Data.Init();
    }

    private void Awake() {
        gameManager = GameManager.Instance;
        moneyManager = MoneyManager.Instance;
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
            moneyManager.ReduceMoney(Data.Cost);
            Data.Upgrade();

            // Send an event out to say that we've updated some variable (so the UI can be updated)
            if (onVariableChanged != null)
                onVariableChanged(this);
        }
    }

    // Called by the script attached to this when a manager button is pressed
    public void BuyManagerButtonPressed() {
        if (!Data.HasManager && Data.Owned > 0 && moneyManager.CanAffordPurchase(Data.managerCost)) {
            Data.AddManager();
            RestartProcess();
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

    // Restart the coroutine above if we have a manager
    void RestartProcess() {
        if(Data.HasManager) {
            RunProcess();
        }
    }
}
