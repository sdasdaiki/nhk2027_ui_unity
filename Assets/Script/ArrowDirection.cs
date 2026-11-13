using UnityEngine;

public class ArrowDirection : MonoBehaviour
{
    [Header("この矢印が指し示す移動先のPoint")]
    public Transform targetPoint;
    
    [Header("マップを映しているカメラ（MapCamera）")]
    public Camera mapCamera;

    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (targetPoint == null || mapCamera == null) return;

        // 1. カメラから目標地点へのベクトル（高さYは無視して平面で考える）
        Vector3 dirToTarget = targetPoint.position - mapCamera.transform.position;
        dirToTarget.y = 0; 

        // すでに目標地点の真上にいる場合は回転処理をスキップ
        if (dirToTarget.sqrMagnitude < 0.1f) return;

        // 2. 目標への方向を、カメラの現在の向きを基準にした方向に変換
        // （例：カメラが東を向いていてターゲットが北なら、カメラから見て「左」と計算される）
        Vector3 localDir = mapCamera.transform.InverseTransformDirection(dirToTarget);

        // 3. X（左右）とZ（奥手前）から、UI上で回転させる角度を割り出す
        float angle = Mathf.Atan2(localDir.x, localDir.z) * Mathf.Rad2Deg;

        // 4. ボタン画像（RectTransform）のZ軸回転に適用
        // （UnityのUI回転は反時計回りがプラスなのでマイナスをつける）
        rectTransform.localEulerAngles = new Vector3(0, 0, -angle);
    }
}