using UnityEngine;

public class MapTouchController : MonoBehaviour
{
    public Camera mapCamera;

    [Header("ズーム（高さ移動）設定")]
    public float zoomSpeed = 0.01f; // 3D空間の移動なので少し小さめの値から調整
    public float minHeight = 2f;    // これ以上は床に近づけない（ズームイン限界）
    public float maxHeight = 15f;   // これ以上は高く上がれない（ズームアウト限界）

    [Header("回転設定")]
    public float rotateSpeed = 1.0f;

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

        // ==========================================
        // 2. ROG Ally 実機用（2本指でのタッチ操作）
        // ==========================================
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // --- ズーム（前進・後退）処理 ---
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
            
            // ピンチイン（狭める）でマイナス、ピンチアウト（広げる）でプラス
            float deltaMagnitudeDiff = touchDeltaMag - prevTouchDeltaMag; 

            Vector3 pos = mapCamera.transform.position;
            // 広げると前進（ズームイン）、狭めると後退（ズームアウト）
            pos += mapCamera.transform.forward * deltaMagnitudeDiff * zoomSpeed;
            
            // 高さ制限
            pos.y = Mathf.Clamp(pos.y, minHeight, maxHeight);
            mapCamera.transform.position = pos;

            // --- 回転処理（2本指をひねる動き） ---
            Vector2 prevDir = touchOnePrevPos - touchZeroPrevPos;
            Vector2 currDir = touchOne.position - touchZero.position;
            
            float angle = Vector2.SignedAngle(prevDir, currDir);
            
            // カメラのZ軸（見下ろしている視界の回転）を回す
            mapCamera.transform.Rotate(0, 0, angle * rotateSpeed, Space.Self);
        }
    }
}