using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour {

    public delegate void ResetAllData();
    public static ResetAllData resetAllData;
    public delegate void OnLoadingComplete();
    public static OnLoadingComplete onLoadingComplete;
    public delegate void OnLoadingPercentageChange(float percentage);
    public static OnLoadingPercentageChange onLoadingPercentageChange;

    public static GameManager Instance;
    public enum TimeFetchType { GET, POST };

    [Header("General")]
    [Tooltip("The multiplier that all revenue sources will be multiplied by; great for prestige")] public float globalRevenueMultiplier = 1f;

    [Header("Idle Settings")]
    [Tooltip("The percentage of the total earnings that will be earned when re-opening the game")] public float idleEarnings = 80f;
    [Tooltip("The maximum amount of idle hours that will be counted when the player returns")] public int idleHours = 1;

    [Header("Time")]
    [Tooltip("If true, the time will be pulled from the URL resource defined below. The resource must be a blank page with just a timestamp. This is recommended to prevent the user from modifying their system time\n\nIf disabled, the timestamp will be based on the system time of the device")] public bool pullTimeFromUrl;
    [Tooltip("The URL to the resource which will provide the current timestamp (timezone does not matter)")] public string timeUrl;

    private double timestamp;
    private int totalProcesses;
    private int processesComplete;

    void Awake() {
        Instance = this;
    }

    // This is the first thing done in the game and will handle when other scripts do what
    // Only once every aspect inside here has been satisfied then we will continue (it'll be a loading screen until then)
    // Todo: clean this up
    void Start() {
        totalProcesses = 4;
        switch(processesComplete) {
            case 0:
                FetchTimestamp(); break;
            case 1:
                Idles.Instance.Setup(); break;
            case 2:
                MoneyManager.Instance.Setup(); break;
            case 3:
                IdleManager.Instance.Setup(); break;
        }
    }

    // Called by functions inside LoadGame() to say that they have begun working on things
    public void ProcessBegin() {
        // Todo: cleanup with above
    }

    // Called by functions inside LoadGame() once completed to say they are done
    public void ProcessComplete() {
        processesComplete++;

        // Send out an event
        if (onLoadingPercentageChange != null)
            onLoadingPercentageChange(((float)processesComplete / (float)totalProcesses) * 100f);

        // Is everything loaded?
        if(processesComplete == totalProcesses) {
            processesComplete = 0;
            StartCoroutine(BeginGame());
        } else {
            Start();
        }
    }

    // Give all scripts enough time to load (and add references to the callback here)
    IEnumerator BeginGame() {
        yield return new WaitForSeconds(0.1f);
        if (onLoadingComplete != null)
            onLoadingComplete();
    }

    // Get the timestamp at the current time
    void FetchTimestamp() {
        ProcessBegin();

        // Are we pulling from the user's PC or a webpage?
        if (!pullTimeFromUrl) {
            DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            timestamp = (double)(DateTime.UtcNow - epochStart).TotalSeconds;
            ProcessComplete();

        } else {
            StartCoroutine(GetTime());
        }
    }

    IEnumerator GetTime() {
        UnityWebRequest www = UnityWebRequest.Get(timeUrl);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        }
        else {
            double.TryParse(www.downloadHandler.text, out timestamp);
            if(timestamp != 0)
                ProcessComplete();
        }
    }

    // Return the current timestamp
    public double TimeNow() {
        return timestamp + Time.time;
    }

    // Return a timestamp of a future time
    public double FutureTime(double secondsAhead) {
        return TimeNow() + secondsAhead;
    }

    // Call this function to reset all of a player's progress
    public void Reset() {
        if (resetAllData != null)
            resetAllData();
    }
}