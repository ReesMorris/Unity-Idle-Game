﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BuyableData {

    public delegate void OnDataLoaded(BuyableData data);
    public static OnDataLoaded onDataLoaded;

    public enum ProductionAlgorithm { Standard, Multiplicative, Additive, Custom };
    public enum ActionToTakeOnMilestone { Standard, Mixed, Custom, None };

    // Public Variables
    [Header("General")]
    [Tooltip("The name of the idle")] public string name;
    [Tooltip("A unique identifier. Do not change after being set or progress will be lost")] public string identifier;
    [Tooltip("The cost to purchase the first idle")] public double baseCost;
    [Tooltip("The rate of growth after every purchase, typically between 1.05 and 1.3")] public float growthRate;
    [Tooltip("The time (in seconds) the first idle will take to be completed")] public float baseTime;
    [Tooltip("The amount received from every production on level 1")] public float baseProfit;
    [Tooltip("The base multiplier for this buyable")] public double baseMultiplier = 1;
    [Tooltip("The cost to purchase a manager. Managers will automatically click for you once a process ends (and earn offline revenue!)")] public double managerCost;
    [Tooltip("The maximum amount that can be purchased. It is best to ensure that this value is less than 10^308 as any cost larger than this value will be set to this automatically")] public int maxAmount = 400;
    [Tooltip("The milestones used by the UpgradeActions function for either standard or custom actions")] public int[] upgradeMilestones = { 25, 50, 100, 200, 300, 400 };

    [Header("Calculations")]
    [Tooltip("The algorithm used to determine the cost of the next upgrade.\n\nStandard is the most popular algorithm for idle games and is\nbaseCost * (growthRate)^owned\n\nMultiplicative will only multiply by growthRate and is\nbaseCost * growthRate * owned\n\nAdditive will add all values together and is\nbaseCost + growthRate + owned\n\nCustom allows you to determine your own algorithm by modifying this script")] public ProductionAlgorithm productionAlgorithm;
    [Tooltip("The action to complete when an upgrade is done.\n\nNone will do nothing\n\nStandard will half the time to produce every time the amount owned equals an upgradeMilestone\n\nMixed will alternate between halfing the time to produce and doubling the profit each time (starting from half time, double profit, half time, ...)\n\nCustom allows you to determine your own algorithm by modifying this script")] public ActionToTakeOnMilestone actionToTakeOnMilestone;

    [Header("UI")]
    [Tooltip("Customise the idle by adding your own sprites")] public Sprite[] sprites;

    // Private & Protected Variables
    public float ProcessTime { get; protected set; }
    public double Profit { get; protected set; }
    public double Cost { get; protected set; }
    public int Owned { get; protected set; }
    public int NextMilestoneIndex { get; protected set; }
    public bool HasManager { get; protected set; }
    public double ProcessCompleteTime { get; protected set; }
    public double ProcessStartTime { get; protected set; }
    public double MinutelyProfit { get; protected set; }
    public double IdleEarnings { get; protected set; }
    public double Multiplier { get; protected set; }
    private GameManager gameManager;

    public void Init() {
        gameManager = GameManager.Instance;
        ProcessTime = baseTime;
        Cost = baseCost;
        Profit = baseProfit;
        Multiplier = baseMultiplier;
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

        // Failsafe to prevent cost being more than the size of a double
        if (Cost > double.MaxValue)
            Cost = double.MaxValue;
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
        switch(actionToTakeOnMilestone) {
            case ActionToTakeOnMilestone.Standard:
                while (ReachedMilestone()) {
                    ProcessTime /= 2f;
                    ProcessCompleteTime -= ProcessTime;
                }
                break;
            case ActionToTakeOnMilestone.Mixed:
                while (ReachedMilestone())
                    if (NextMilestoneIndex % 2 == 1) {
                        ProcessTime /= 2f;
                        ProcessCompleteTime -= ProcessTime;
                    } else {
                        Profit *= 2;
                    }
                break;
            case ActionToTakeOnMilestone.Custom:
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

    // Called by other functions to upgrade this; does not check the price but will prevent an upgrade exceeding maxAmount
    public void Upgrade() {
        if(Owned + 1 <= maxAmount) {
            Owned++;
            UpgradeActions();
            SetUpgradeCost();
            SetMinutelyProfit();
            SaveData();
        }
    }

    // Returns true if a process is currently in progress
    public bool ProcessInProgress() {
        return gameManager.TimeNow() < ProcessCompleteTime;
    }

    // Called by other functions to set a manager
    public void AddManager() {
        HasManager = true;
        SetMinutelyProfit();
        SaveData();
    }

    // Called to get the amount earned from a single process
    public double GetRevenue() {
        return Profit * Owned * Multiplier * gameManager.globalRevenueMultiplier;
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
        double currentTime = gameManager.TimeNow();
        if (currentTime > ProcessCompleteTime)
            return 0f;
        else
            return float.Parse(((currentTime - ProcessStartTime) / (ProcessCompleteTime - ProcessStartTime)).ToString());
    }

    // Return the number of seconds before a process is completed
    public double SecondsToProcessCompletion() {
        double time = ProcessCompleteTime - gameManager.TimeNow();
        if (time <= 0)
            return 0f;
        return time;
    }

    // Update storage data
    public void SaveData() {
        PlayerPrefs.SetInt("Idle" + identifier + "Owned", Owned);
        PlayerPrefs.SetInt("Idle" + identifier + "HasManager", HasManager ? 1 : 0);
        PlayerPrefs.SetString("Idle" + identifier + "TimeRemaining", SecondsToProcessCompletion().ToString());
    }

    // Called by IdleManager when the game is loaded if the player has played before
    public void LoadData(int timeGone) {
        Owned = PlayerPrefs.GetInt("Idle" + identifier + "Owned");
        HasManager = PlayerPrefs.GetInt("Idle" + identifier + "HasManager") == 1;
        UpgradeActions();
        SetUpgradeCost();
        SetMinutelyProfit();

        // Calculate the earnings whilst idle
        IdleEarnings = 0;
        if(HasManager) {
            IdleEarnings = Mathf.Floor(timeGone / ProcessTime) * GetRevenue();
        }

        // Calculate the progress the timer should be at when returning
        double timeRemaining = -1;
        double.TryParse(PlayerPrefs.GetString("Idle" + identifier + "TimeRemaining"), out timeRemaining);

        if(timeGone < timeRemaining) {
            ProcessStartTime = gameManager.PastTime(ProcessTime - timeRemaining);
            ProcessCompleteTime = gameManager.FutureTime(timeRemaining - timeGone);
        } else {
            if(HasManager) {
                if(timeRemaining != -1) {
                    double counter = timeGone - timeRemaining;
                    while(counter > 0)
                        counter -= ProcessTime;

                    ProcessStartTime = gameManager.PastTime(ProcessTime + counter);
                    ProcessCompleteTime = gameManager.FutureTime(-counter);
                    SaveData();
                }
            }
        }

        SaveData();

        // Tell other scripts that we've loaded the save data
        if (onDataLoaded != null)
            onDataLoaded(this);
    }

    // Wipes and resets everything back to the start
    public void ResetData() {
        Owned = 0;
        HasManager = false;
        PlayerPrefs.SetInt("Idle" + identifier + "Owned", 0);
        PlayerPrefs.SetInt("Idle" + identifier + "HasManager", 0);
        PlayerPrefs.SetString("Idle" + identifier + "TimeRemaining", "");
    }
}