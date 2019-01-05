using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour {

    // Define the instance so we can use it easily from other scripts
    public static MoneyManager Instance;

    // Public Variables
    [Header("UI Elements")]
    [Tooltip("The UI element of the text which displays our money")] public Text moneyText;

    [Header("Config")]
    [Tooltip("The amount of money the player starts the game with")] public float startingMoney;
    [Tooltip("The currency icon used throughout the game")] public string currencyIcon;
    [Tooltip("The minimum length of a number's digits before the first name (ie. Thousands) is displayed. Must be divisible by 3")] public int firstNameLength = 3;
    [Tooltip("The number of decimal places to display")] public int decimalPlaces = 3;
    [Tooltip("If enabled, the user's balance will never drop below zero")] public bool allowNegativeBalance;

    // Private Variables
    float money;

    // Called when the game begins
    void Start() {
        // Set this to be an instance of itself, so we can call this script from others
        Instance = this;

        // Add the starting funds
        AddMoney(startingMoney);
    }



    // Can be called by other scripts to add money to the user's balance
    public void AddMoney(float amount) {

        // Increase the amount of money we have
        money += amount;

        // Call a function to say that our balance has changed
        OnBalanceChanged();
    }



    // Reduce the amount of money the player has. This function will not check to see if the user has enough money
    public void ReduceMoney(float amount) {

        // Reduce the amount of money we have
        money -= amount;

        // If we cannot go into negative, let's just make sure we haven't
        if (!allowNegativeBalance) {

            // Set the player's money to be the higher of two values: 0 or the money itself. We can never be negative in this way.
            money = Mathf.Max(0, money);
        }

        // Call a function to say that our balance has changed
        OnBalanceChanged();
    }



    // Returns true or false depending on whether or not the player has enough money
    public bool CanAffordPurchase(float cost) {

        // Returns whether the player's balance is higher than the cost
        return money > cost;
    }



    // Called when the balance of the user has been changed
    void OnBalanceChanged() {

        moneyText.text = GetStringMoney(money);
    }



    // Returns the balance as a string
    public string GetStringMoney(float money) {

        // Convert the money value to a string with zero decimal places
        return money.ToString("F0");
    }



    // Returns the money value formatted with the currency character, decimal place, amount, and the name of the number (if applicable)
    public string GetFormattedMoney(float money) {

        // Convert the money to a string
        string stringMoney = GetStringMoney(money);

        /*
        // Calculate how many digits come before the decmial place, which will be at most 3 digits
        int digitsBeforeDecimal = stringMoney.Length % 3;
        if (digitsBeforeDecimal == 0) digitsBeforeDecimal = 3;
        */

        /*
        // Do we want to display decimal places?
        if (decimalPlaces > 0) {

            // Only show a decimal place if there is a word for the number amount; otherwise it looks weird
            if (stringMoney.Length > firstNameLength) {

                // Insert a decimal place into the string based on the amount of digits before the decimal, calculated above
                stringMoney = stringMoney.Insert(digitsBeforeDecimal, ".");
            }

            // Check to see if there are enough digits in the number to actually display a decimal place
            if (stringMoney.Length - digitsBeforeDecimal > decimalPlaces) {

                // Reduce the string to be a length equal to the number of places it has before the decimal, as well as the decimal places (plus the decimal itself)
                stringMoney = stringMoney.Substring(0, digitsBeforeDecimal + decimalPlaces + 1);
            }
        }
        */

        /*
        // Add the currency icon
        stringMoney.Insert(0, currencyIcon);
        */

        // Add the number name
            //stringMoney += " " + GetNumberName(stringMoney.Length);

        return stringMoney;
    }
}