using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    [Tooltip("The base delay between purchases to prevent buying lots at once")] public float baseBuyCooldown = 0.2f;
    [Tooltip("Increment the profit multiplier every time ownership passes this amount")] public int ownershipMultiplier = 50;

    void Start() {
        Instance = this;
    }
}