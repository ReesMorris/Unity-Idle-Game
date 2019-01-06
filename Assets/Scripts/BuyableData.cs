using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuyableData {

    // Public Variables
    [Tooltip("The name of the idle")] public string name;
    [Tooltip("The amount of money the player needs for this to appear (if DisplayWhenFundsAvailable is selected in GameManager/Idles)")] public double displayPrice;
    [Tooltip("The cost to purchase the first idle")] public double baseCost;
    [Tooltip("The rate of growth after every purchase, typically between 1.05 and 1.3")] public float growthRate;
    public enum ProductionAlgorithm { Standard, Multiplicative, Additive, Custom };
    [Tooltip("The algorithm used to determine the cost of the next upgrade.\n\nStandard is the most popular algorithm for idle games and is\nbaseCost * (growthRate)^owned\n\nMultiplicative will only multiply by growthRate and is\nbaseCost * growthRate * owned\n\nAdditive will add all values together and is\nbaseCost + growthRate + owned\n\nCustom allows you to determine your own algorithm by modifying this script")]public ProductionAlgorithm productionAlgorithm;
    [Tooltip("The time (in seconds) the first idle will take to be completed")] public float baseTime;
    public enum ActionToTakeOnUpgrade { Standard, Custom, None }
    [Tooltip("The milestones used by the UpgradeActions function for either standard or custom actions")] public int[] upgradeMilestones = {25, 50, 100, 200, 300, 400};
    [Tooltip("The action to complete when an upgrade is done.\n\nNone will do nothing\n\nStandard will half the time to produce every time the amount owned equals 25, 50, 100, 200, 300, and 400\n\nCustom allows you to determine your own algorithm by modifying this script")]  public ActionToTakeOnUpgrade actionToTakeOnUpgrade;
    [Tooltip("The amount received from every production on level 1")] public float initialRevenue;
    [Tooltip("The cost to purchase a manager. Managers will automatically click for you once a process ends (and earn offline revenue!)")] public double managerCost;

    // Private & Protected Variables
    public float ProcessTime { get; protected set; }
    public double Cost { get; protected set; }
    public int Owned { get; protected set; }
    public int NextMilestoneIndex { get; protected set; }
    public bool HasManager { get; protected set; }
    public float ProcessCompleteTime { get; protected set; }
    public float ProcessStartTime { get; protected set; }
    public double MinutelyProfit { get; protected set; }
    private float multiplier = 1;
    private GameManager gameManager;

    public void Init() {
        gameManager = GameManager.Instance;
        ProcessTime = baseTime;
        Cost = baseCost;
    }

    // Called to fetch the next buy cost
    void SetUpgradeCost() {
        switch(productionAlgorithm) {
            case ProductionAlgorithm.Multiplicative:
                Cost = baseCost * growthRate * Owned; break;
            case ProductionAlgorithm.Additive:
                Cost = baseCost + growthRate + Owned; break;
            case ProductionAlgorithm.Custom:
                // If you wish to use a custom algorithm, drop it here.
                // Make sure to break after the statement is complete.
            default:
                Cost = baseCost * Mathf.Pow(growthRate, Owned); break;
        }
    }

    // Set the minutely profit based on current upgrades
    public void SetMinutelyProfit() {
        if (!HasManager)
            MinutelyProfit = 0f;
        else
            MinutelyProfit = (60f / ProcessTime) * GetRevenue();
    }

    // Called when the item is upgraded
    void UpgradeActions() {
        switch(actionToTakeOnUpgrade) {
            case ActionToTakeOnUpgrade.Standard:
                if (ReachedMilestone())
                    ProcessTime /= 2f;
                break;
            case ActionToTakeOnUpgrade.Custom:
                // If you wish to use a custom algorithm, drop it here.
                // Make sure to break after the statement is complete.
                break;
        }
    }

    // Returns true if the number of items owned is a milestone
    bool ReachedMilestone() {
        if(NextMilestoneIndex != -1) {
            if(Owned >= upgradeMilestones[NextMilestoneIndex]) {
                NextMilestoneIndex++;
                if (NextMilestoneIndex >= upgradeMilestones.Length)
                    NextMilestoneIndex = -1;
                return true;
            }
        }
        return false;
    }

    public float NextMilestonePercentage() {
        if (NextMilestoneIndex == -1) return 100;

        float lastMilestoneAmount = 0;
        if (NextMilestoneIndex > 0)
            lastMilestoneAmount = upgradeMilestones[NextMilestoneIndex - 1];
        return (float)(Owned - lastMilestoneAmount) / (float)(upgradeMilestones[NextMilestoneIndex] - lastMilestoneAmount);

    }

    // Called by other functions to upgrade this; does not check the price
    public void Upgrade() {
        Owned++;
        UpgradeActions();
        SetUpgradeCost();
        SetMinutelyProfit();
    }

    // Returns true if a process is currently in progress
    public bool ProcessInProgress() {
        return gameManager.TimeNow() < ProcessCompleteTime;
    }

    // Called by other functions to set a manager
    public void AddManager() {
        HasManager = true;
        SetMinutelyProfit();
    }

    // Called to get the amount earned from a single process
    public double GetRevenue() {
        return initialRevenue * Owned * multiplier;
    }

    // Begin a process
    public void BeginProcess() {
        if(!ProcessInProgress()) {
            ProcessStartTime = gameManager.TimeNow();
            ProcessCompleteTime = gameManager.FutureTime(ProcessTime);
        }
    }

    // Return the progress of a process as a percentage
    public float GetProcessCompletion() {
        float currentTime = gameManager.TimeNow(); // to be replaced
        if (currentTime > ProcessCompleteTime)
            return 0f;
        else
            return (currentTime - ProcessStartTime) / (ProcessCompleteTime - ProcessStartTime);
    }

    // Return the number of seconds before a process is completed
    public float SecondsToProcessCompletion() {
        return Mathf.Max(0f, ProcessCompleteTime - gameManager.TimeNow());
    }
}