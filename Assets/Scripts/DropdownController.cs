using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DropdownController : MonoBehaviour
{
    public Pathfinder pathfinder; // Reference to the Pathfinder script

    private TMP_Dropdown dropdown; // Reference to the Dropdown component

    void Start()
    {
        GameObject dropdownGO = GameObject.Find("AlgorithmSelectionDropdown");
        if (dropdownGO != null)
        {
            dropdown = dropdownGO.GetComponent<TMP_Dropdown>(); // Get the Dropdown component attached to this GameObject
            dropdown.onValueChanged.AddListener(OnDropdownValueChanged); // Add listener for value change
        } else
        {
            Debug.Log("Error");
        }
    }

    void OnDropdownValueChanged(int value)
    {
        pathfinder.SetModeDropdown(value); // Call SetMode in Pathfinder script with the selected value
    }
}

