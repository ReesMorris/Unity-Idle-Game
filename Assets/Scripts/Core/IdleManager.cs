using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleManager : MonoBehaviour {

    public delegate void OnLoaded();
    public static OnLoaded onLoaded;

    [Header("UI Elements")]
    public GameObject gameScreen;
    public GameObject loadingScreen;

    private GameManager gameManager;
    private Idles idles;
    private MoneyManager moneyManager;
    private int idlesLoaded;
    private Utilities utilities;

    void Awake() {
        gameManager = GameManager.Instance;
        utilities = Utilities.Instance;
        idles = Idles.Instance;

        moneyManager = MoneyManager.Instance;
    }

    void Start() {
        LoadData();
    }

    bool isPaused;

    // Load the data when the player returns to the game
    void LoadData() {
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
            double idleEarnings = moneyManager.CalculateMinutelyProfit() / (60f / secondsGone);
            moneyManager.AddMoney(idleEarnings);

            // Tell other scripts we've loaded, incase they want to do anything with the new data
            if (onLoaded != null)
                onLoaded();

            // Todo: move this 
            print("Welcome back! You were gone for " + utilities.SecondsToHHMMSS(secondsGone, false));
            print("You earned " + moneyManager.GetFormattedMoney(idleEarnings, false) + " whilst you were away");
        }
    }

    void OnApplicationQuit() {
        PlayerPrefs.SetString("TimeQuit", gameManager.TimeNow().ToString());
    }
}
