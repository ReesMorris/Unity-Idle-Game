using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour {

    // Public Variables
    [Header("UI Elements")]
    [Tooltip("The UI element of the text which displays our money")] public Text moneyText;

    [Header("Config")]
    [Tooltip("The currency icon used throughout the game")] public string currencyIcon;
    [Tooltip("The minimum length of a number's digits before the first name (ie. Thousands) is displayed. Must be divisible by 3")] public int firstNameLength = 3;
    [Tooltip("The number of decimal places to display")] public int decimalPlaces = 3;
    [Tooltip("If enabled, the user's balance will never drop below zero")] public bool allowNegativeBalance;

    // Private Variables
    float money;

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
    bool CanAffordPurchase(float cost) {

        // Returns whether the player's balance is higher than the cost
        return money > cost;
    }

    // Called when the balance of the user has been changed
    void OnBalanceChanged() {

        // Update the UI text to show our new balance
        SetDisplayMoneyText();
    }

    // Change the text of the money to show how much we have
    void SetDisplayMoneyText() {

        // Grab a string version of the amount of money we have, so that we can read the number of digits it has
        string stringMoney = money.ToString("F0");
        string displayMoney = stringMoney;

        // Calculate how many digits come before the decmial place, which will be at most 3 digits
        int digitsBeforeDecimal = displayMoney.Length % 3;
        if (digitsBeforeDecimal == 0) digitsBeforeDecimal = 3;

        // Do we want to display decimal places?
        if (decimalPlaces > 0) {

            // Only show a decimal place if there is a word for the number amount; otherwise it looks weird
            if (displayMoney.Length > firstNameLength) {

                // Insert a decimal place into the string based on the amount of digits before the decimal, calculated above
                displayMoney = displayMoney.Insert(digitsBeforeDecimal, ".");
            }

            // Check to see if there are enough digits in the number to actually display a decimal place
            if (displayMoney.Length - digitsBeforeDecimal > decimalPlaces) {

                // Reduce the string to be a length equal to the number of places it has before the decimal, as well as the decimal places (plus the decimal itself)
                displayMoney = displayMoney.Substring(0, digitsBeforeDecimal + decimalPlaces + 1);
            }
        }

        // Update the UI text to show the new amount of money we have
        moneyText.text = displayMoney + " " + GetNumberName(stringMoney.Length);
    }

    // Will return the name of the number the integer is connected to
    string GetNumberName(int numberLength) {

        // If we don't want to show a name for this number, just return an empty string
        if (numberLength < firstNameLength) return "";

        // The number should have a name, let's return it
        else {

            // The string of potential names (to be moved to a .txt file in future)
            string[] names = { "Hundred", "Thousand", "Million", "Billion", "Trillion", "Quadrillion", "Quintillion", "Sextillion", "Septillion", "Octillion" };

            // Grab the index value based on the length of the number, as well as the offset we start from
            int index = Mathf.CeilToInt((float)numberLength / 3f) - Mathf.CeilToInt((float)firstNameLength / 3f);

            // If the name exists in the array (the number is small enough) then return the number
            if (names.Length > index) return names[index];

            // If the number doesn't exist in the array for whatever reason, return a generic phrase
            else return "a lot";
        }
    }

    // Returns the money value formatted with the currency character, amount, and the name of the number (if applicable)
    public string GetFormattedMoney(string stringMoney) {

        // Add the currency icon
        stringMoney.Insert(0, currencyIcon);

        // Add the number name
        stringMoney += " " + GetNumberName(stringMoney.Length);

        return stringMoney;
    }
}