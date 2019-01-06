using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities : MonoBehaviour {

    public static Utilities Instance;

    void Awake() {
        Instance = this;
    }

    public string SecondsToHHMMSS(double totalSeconds, bool alwaysShowHours) {
        double hours = totalSeconds / 3600;
        double minutes = (totalSeconds % 3600) / 60;
        double seconds = totalSeconds % 60;

        if(hours > 0 || alwaysShowHours)
            return string.Format("{0}:{1}:{2}", hours.ToString("00"), minutes.ToString("00"), seconds.ToString("00"));
        else
            return string.Format("{0}:{1}", minutes.ToString("00"), seconds.ToString("00"));
    }
}
