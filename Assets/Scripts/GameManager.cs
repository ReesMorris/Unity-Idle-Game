using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    [Tooltip("The base delay between purchases")] public float baseBuyCooldown;

    void Start() {
        Instance = this;
    }
}