using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamSwitchController : MonoBehaviour
{
    [Header("マップ移動")]
    public MapModeController mapModeController;

    [Header("UI設定")]
    public Button switchButton;
    public Image buttonBackground;
    public TMP_Text buttonText;

    public bool isRedTeam = true;

    void Start()
    {
        if (switchButton != null)
        {
            switchButton.onClick.AddListener(ToggleTeam);
        }

        if (mapModeController == null)
        {
            Debug.LogError("TeamSwitchController: Map Mode Controller is not assigned.", this);
        }
        else
        {
            mapModeController.SetTeam(isRedTeam);
        }

        UpdateButtonVisual();
    }

    void ToggleTeam()
    {
        isRedTeam = !isRedTeam;

        if (mapModeController == null)
        {
            Debug.LogError("TeamSwitchController: Map Mode Controller is not assigned.", this);
        }
        else
        {
            mapModeController.SetTeam(isRedTeam);
        }

        UpdateButtonVisual();
    }

    private void UpdateButtonVisual()
    {
        if (buttonText != null)
        {
            buttonText.text = isRedTeam ? "Team: RED" : "Team: BLUE";
        }

        if (buttonBackground != null)
        {
            buttonBackground.color = isRedTeam ? new Color(1f, 0.5f, 0.5f) : new Color(0.5f, 0.5f, 1f);
        }
    }
}