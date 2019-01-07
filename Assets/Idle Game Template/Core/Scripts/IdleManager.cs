using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleManager : MonoBehaviour {

    public static IdleManager Instance;

    public delegate void OnLoaded();
    public static OnLoaded onLoaded;
    public delegate void OnAwayProfitsCalculated(int secondsGone, double earnings);
    public static OnAwayProfitsCalculated onAwayProfitsCalculated;

    private GameManager gameManager;
    private Idles idles;
    private MoneyManager moneyManager;
    private Utilities utilities;

    void Awake() {
        Instance = this;
    }

    void OnSetup() {
        // Add references to other scripts
        gameManager = GameManager.Instance;
        utilities = Utilities.Instance;
        idles = Idles.Instance;
        moneyManager = MoneyManager.Instance;
        gameManager = GameManager.Instance;
    }

    // Set up the money (called during init phase of GameManager)
    public void Setup() {
        OnSetup();
        LoadData();
    }

    // Load the data when the player returns to the game
    void LoadData() {
        gameManager.ProcessBegin();
        string timeQuit = PlayerPrefs.GetString("TimeQuit");

        // Do we have data about the last time the user played?
        if(timeQuit != "") {

            double difference = gameManager.TimeNow() - double.Parse(PlayerPrefs.GetString("TimeQuit"));
            int secondsGone = (int)Mathf.Floor((float)difference);

            // Tell the buyables to check their playerprefs to load data
            foreach(BuyableData data in idles.idles) {
                data.LoadData(secondsGone);
            }

            // We only want the earnings made for 'x' hours whilst the player was away; let's see which is the smaller of the two
            int maxSeconds = gameManager.idleHours * 3600;
            secondsGone = Mathf.Min(secondsGone, maxSeconds);

            // Calculate the amount earned offline and give it to the player
            double idleEarnings = moneyManager.ActualIdleProfit();
            moneyManager.AddMoney(idleEarnings);

            // Tell other scripts we've loaded, incase they want to do anything with the new data
            if (onLoaded != null)
                onLoaded();

            // Tell other scripts (typically a UI-handler) that we've calculated some away profits
            if (onAwayProfitsCalculated != null)
                onAwayProfitsCalculated(secondsGone, idleEarnings);
        }
        gameManager.ProcessComplete();
    }

    // Store the time we're quitting, if we are in the game successfully
    void OnApplicationQuit() {

        // Save all idle data (we have to do it here since BuyableData does not derive from MonoBehaviour)
        foreach(BuyableData data in idles.idles) {
            data.SaveData();
        }

        if(gameManager != null)
            PlayerPrefs.SetString("TimeQuit", gameManager.TimeNow().ToString());
    }
}
