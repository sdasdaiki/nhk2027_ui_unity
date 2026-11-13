using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamSwitchController : MonoBehaviour
{
    [Header("カメラと視点の移動先")]
    public Transform mapCamera;     // 動かす対象（MapPinCamera）
    public Transform redTeamView;   // 赤チームの視点（Empty）
    public Transform blueTeamView;  // 青チームの視点（Empty）

    [Header("UI設定")]
    public Button switchButton;
    public Image buttonBackground;
    public TMP_Text buttonText;

    private bool isRedTeam = true;

    void Start()
    {
        if (switchButton != null)
        {
            switchButton.onClick.AddListener(ToggleTeam);
        }
        UpdateCameraView();
    }

    void ToggleTeam()
    {
        isRedTeam = !isRedTeam;
        UpdateCameraView();
    }

void UpdateCameraView()
    {
        Transform targetView = isRedTeam ? redTeamView : blueTeamView;
        
        // 位置はそのまま代入
        mapCamera.position = targetView.position;
        
        // 【修正点】rotation（Quaternion）ではなく、eulerAngles（XYZの角度）を代入する
        mapCamera.eulerAngles = targetView.eulerAngles;

        // ボタンの見た目を変える
        if (buttonText != null)
        {
            buttonText.text = isRedTeam ? "Team: RED" : "Team: BLUE";
            if (buttonBackground != null)
            {
                buttonBackground.color = isRedTeam ? new Color(1f, 0.5f, 0.5f) : new Color(0.5f, 0.5f, 1f);
            }
        }
    }
}