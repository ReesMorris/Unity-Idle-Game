using UnityEngine;
using System;


public class GameManager : MonoBehaviour {

    public delegate void ResetAllData();
    public static ResetAllData resetAllData;

    public static GameManager Instance;

    [Header("Idle Settings")]
    [Tooltip("The percentage of the total earnings that will be earned when re-opening the game")] public float idleEarnings = 80f;
    [Tooltip("The maximum amount of idle hours that will be counted when the player returns")] public int idleHours = 1;

    [Header("Time")]
    [Tooltip("If true, the time will be pulled from the URL resource defined below. The resource must be a blank page with just a timestamp. This is recommended to prevent the user from modifying their system time\n\nIf disabled, the timestamp will be based on the system time of the device")] public bool pullTimeFromUrl;
    [Tooltip("The URL to the resource which will provide the current timestamp (timezone does not matter)")] public string timeUrl;

    private double timestamp;

    void Awake() {
        Instance = this;
        
        if(!pullTimeFromUrl) {
            DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            timestamp = (double)(DateTime.UtcNow - epochStart).TotalSeconds;
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

    void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            if(resetAllData != null)
                resetAllData();
        }
    }
}