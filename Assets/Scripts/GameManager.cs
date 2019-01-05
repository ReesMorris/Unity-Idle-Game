using UnityEngine;
using System;


public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    [Header("Time")]
    [Header("If true, the time will be pulled from the URL resource defined below. The resource must be a blank page with just a timestamp. This is recommended to prevent the user from modifying their system time\n\nIf disabled, the timestamp will be based on the system time of the device")]
    public bool pullTimeFromUrl;
    [Header("The URL to the resource which will provide the current timestamp (timezone does not matter)")] public string timeUrl;

    private float timestamp;

    void Awake() {
        Instance = this;
    }

    void Start() {
        if(!pullTimeFromUrl) {
            DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            int currentEpochTime = (int)(DateTime.UtcNow - epochStart).TotalSeconds;
        }
    }

    // Return the current timestamp
    public float TimeNow() {
        return timestamp + Time.time;
    }

    public float FutureTime(float secondsAhead) {
        return TimeNow() + secondsAhead;
    }
}