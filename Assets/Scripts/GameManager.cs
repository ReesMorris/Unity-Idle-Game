using UnityEngine;
using System;


public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    [Header("Idle Settings")]
    [Tooltip("The percentage of the total earnings that will be earned when re-opening the game")] public float idleEarnings = 80f;

    [Header("Time")]
    [Tooltip("If true, the time will be pulled from the URL resource defined below. The resource must be a blank page with just a timestamp. This is recommended to prevent the user from modifying their system time\n\nIf disabled, the timestamp will be based on the system time of the device")] public bool pullTimeFromUrl;
    [Tooltip("The URL to the resource which will provide the current timestamp (timezone does not matter)")] public string timeUrl;

    private int timestamp;

    void Awake() {
        Instance = this;
    }

    void Start() {
        if(!pullTimeFromUrl) {
            DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            timestamp = (int)(DateTime.UtcNow - epochStart).TotalSeconds;

            // Allows us to store the Timestamp in an integer; kind of messy but will do for years
            if(timestamp.ToString().Length > 5)
            timestamp /= 100000;
            print(timestamp);
        }
    }

    // Return the current timestamp
    public float TimeNow() {
        return timestamp + Time.time;
    }

    // Return a timestamp of a future time
    public float FutureTime(float secondsAhead) {
        return TimeNow() + secondsAhead;
    }
}