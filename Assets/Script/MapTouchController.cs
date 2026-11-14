using UnityEngine;

public class MapTouchController : MonoBehaviour
{
    public Camera mapCamera;

    [Header("ズーム（マウスホイール）設定")]
    public float minHeight = 2f;    // これ以上は床に近づけない（ズームイン限界）
    public float maxHeight = 15f;   // これ以上は高く上がれない（ズームアウト限界）

    void Update()
    {
        if (mapCamera == null || !mapCamera.gameObject.activeInHierarchy) return;

        // ==========================================
        // 1. エディタでのテスト用（マウスホイールでズーム）
        // ==========================================
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            // カメラを前（下）後（上）に移動させる
            Vector3 pos = mapCamera.transform.position;
            // カメラの向いている方向（transform.forward）に向かって前進・後退
            pos += mapCamera.transform.forward * scroll * 5f; 
            
            // 高さを制限内に収める（地面にめり込まないように）
            pos.y = Mathf.Clamp(pos.y, minHeight, maxHeight);
            mapCamera.transform.position = pos;
        }

    }
}