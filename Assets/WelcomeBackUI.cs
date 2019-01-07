using UnityEngine;
using UnityEngine.UI;

public class WelcomeBackUI : MonoBehaviour {

    // Public Variables
    public GameObject awayContainer;
    public Text awayText;
    public Text earningsText;

    // Private Variables
    private Utilities utilities;
    private MoneyManager moneyManager;

    void Start() {
        IdleManager.onAwayProfitsCalculated += DisplayUI;

        utilities = Utilities.Instance;
        moneyManager = MoneyManager.Instance;
    }

    // Show the UI once we receive the event that our away profits have been calculated
    void DisplayUI(int secondsGone, double earnings) {

        // Only show the UI if we earned something
        if(earnings > 0) {
            awayContainer.SetActive(true);
            awayText.text = "You've been gone for\n" + utilities.SecondsToHHMMSS(secondsGone, false);
            earningsText.text = moneyManager.GetFormattedMoney(earnings, false);
        }
    }

    // Called when the continue button is clicked
    public void CloseUI() {
        awayContainer.SetActive(false);
    }

}
