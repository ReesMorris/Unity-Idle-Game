using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour {

    // Public Variables
    [Header("UI Elements")]
    public Text loadingText;

    // Get the events of the GameManager
    void Awake() {
        GameManager.onLoadingPercentageChange += UpdateLoader;
        GameManager.onLoadingComplete += OnLoadingComplete;
    }

    // When the GameManager has processed loading something, let's update the value on-screen
    void UpdateLoader(float percentage) {
        loadingText.text = string.Format("Loading ({0}%)", percentage);
    }

    // When the loading is complete
    void OnLoadingComplete() {
        gameObject.SetActive(false);
    }
}
