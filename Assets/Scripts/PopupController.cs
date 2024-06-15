using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class PopupController : MonoBehaviour
{
    public GameObject popupPanel;
    public TMP_Text userDistanceText;
    public TMP_Text algorithmDistanceText;

    // Method to show the popup window
    public void ShowPopup(float userDistance, float algorithmDistance)
    {
        if (userDistanceText != null)
        {
            userDistanceText.text = "User Distance: " + userDistance.ToString("F2");
            Debug.Log(userDistanceText.text);
        }

        if (algorithmDistanceText != null)
        {
            algorithmDistanceText.text = "Algorithm Distance: " + algorithmDistance.ToString("F2");
        }

        popupPanel.SetActive(true);
        // Optionally, play open animation here if using Animator or Animation
    }

    // Method to hide the popup window
    public void HidePopup()
    {
        popupPanel.SetActive(false);
        // Optionally, play close animation here if using Animator or Animation
    }

    public void OnRetryButton()
    {
        HidePopup();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }

}