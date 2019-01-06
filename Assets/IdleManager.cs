using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleManager : MonoBehaviour {

    [Header("UI Elements")]
    public GameObject gameScreen;
    public GameObject loadingScreen;

    private GameManager gameManager;

    void Awake() {
        gameManager = GameManager.Instance;
    }

    bool isPaused;
    float timeQuit;

    void Start() {
        print("i'm back!");
    }

	void OnApplicationFocus(bool hasFocus) {
        isPaused = !hasFocus;
        if (isPaused)
            OnApplicationQuit();
        else
            Start();
    }

    void OnApplicationPause(bool pauseStatus) {
        isPaused = pauseStatus;
        if (isPaused)
            OnApplicationQuit();
        else
            Start();
    }

    void OnApplicationQuit() {
        print(gameManager.TimeNow());
    }
}
