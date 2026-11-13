using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; // UIの誤タップ防止用

public class MapPinController : MonoBehaviour
{
    public Camera mapCamera;
    
    [Header("ピンとUIの紐づけ")]
    public GameObject pinMarker;
    public TMP_Text coordinateText;
    
    [Header("ピン刺しモード切り替えボタン")]
    public Button pinModeButton;
    public Image buttonBackground;
    public Color activeColor = new Color(1f, 0.5f, 0.5f); // モードON時の色（薄い赤など）
    public Color inactiveColor = Color.white;             // モードOFF時の色

    private bool isPinModeActive = false;

    void Start()
    {
        // 初期状態のリセット
        if (pinMarker != null) pinMarker.SetActive(false);
        if (coordinateText != null) coordinateText.text = "None";
        
        // ボタンに切り替えイベントを登録
        if (pinModeButton != null)
        {
            pinModeButton.onClick.AddListener(TogglePinMode);
            UpdateButtonVisual();
        }
    }

public void TogglePinMode()
    {
        isPinModeActive = !isPinModeActive;
        UpdateButtonVisual();

        // モードをOFFにした時はピンと座標テキストをリセットする
        if (!isPinModeActive)
        {
            if (pinMarker != null) pinMarker.SetActive(false);
            // 修正箇所："Null" ではなく、英語の初期テキストを代入
            if (coordinateText != null) coordinateText.text = "None";
        }
    }

    private void UpdateButtonVisual()
    {
        if (buttonBackground != null) buttonBackground.color = isPinModeActive ? activeColor : inactiveColor;
        
        TMP_Text btnText = pinModeButton.GetComponentInChildren<TMP_Text>();
        if (btnText != null) btnText.text = isPinModeActive ? "ON" : "OFF";
    }

void Update()
{
    if (!isPinModeActive || mapCamera == null || pinMarker == null) return;

    bool isTapped = false;
    Vector3 inputPos = Vector3.zero;
    int fingerId = -1; // -1はマウスを表す

    // 入力の検知
    if (Input.GetMouseButtonDown(0))
    {
        isTapped = true;
        inputPos = Input.mousePosition;
        fingerId = -1; 
    }
    else if (Input.touchCount == 1)
    {
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
            isTapped = true;
            inputPos = touch.position;
            fingerId = touch.fingerId;
        }
    }

    // タップされていないフレームはここで終了
    if (!isTapped) return; 

    // 【安全装置】UIの上をタップしたかどうかの判定
    if (fingerId == -1)
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("【ブロック】透明なパネルなどのUIをクリックしたためキャンセルしました。");
            return;
        }
    }
    else
    {
        if (EventSystem.current.IsPointerOverGameObject(fingerId))
        {
            Debug.Log("【ブロック】透明なパネルなどのUIをタッチしたためキャンセルしました。");
            return;
        }
    }

    Debug.Log("【通過】UIブロックを抜けました！Rayを飛ばします。");
    DropPin(inputPos);
}

private void DropPin(Vector3 screenPos)
    {
        // カメラから画面のタップ位置を通る光線を作成
        Ray ray = mapCamera.ScreenPointToRay(screenPos);
        RaycastHit hit;

        // 【修正点】距離を無限大（Mathf.Infinity）にし、全レイヤーを対象にする
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, Physics.AllLayers))
        {
            // 1. ピンを表示して、当たった座標に移動させる
            pinMarker.SetActive(true);
            pinMarker.transform.position = hit.point;

            // 2. 座標をUIに表示する
            if (coordinateText != null)
            {
                coordinateText.text = $"Coord: X: {hit.point.x:F2} \nY: {hit.point.z:F2}";
            }
        }
        else
        {
            // 【テスト用】Rayが何にも当たらなかった場合の確認用ログ
            Debug.Log("Rayは飛びましたが、何にも当たりませんでした（Colliderがないか、UIに遮られています）");
        }
    }
}