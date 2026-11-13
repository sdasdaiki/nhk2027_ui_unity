using UnityEngine;

public class MapModeController : MonoBehaviour
{
    public Camera mapCamera;

    [Header("カメラが移動するポイントのリスト")]
    public Transform[] targetPoints;

    [Header("カメラの移動速度")]
    public float moveSpeed = 10f;

    private Vector3 currentTargetPosition;
    private bool isMoving = false;

    private void OnEnable()
    {
        if (mapCamera != null) mapCamera.gameObject.SetActive(true);
        
        // パネルを開いた時は、0番目のポイントへ瞬時に移動しておく
        if (targetPoints.Length > 0 && mapCamera != null)
        {
            SetTargetPosition(targetPoints[0].position);
            mapCamera.transform.position = currentTargetPosition;
        }
    }

    private void OnDisable()
    {
        if (mapCamera != null) mapCamera.gameObject.SetActive(false);
        isMoving = false;
    }

    void Update()
    {
        // 目標地点に向かって滑らかにカメラを移動させる
        if (isMoving && mapCamera != null)
        {
            mapCamera.transform.position = Vector3.Lerp(mapCamera.transform.position, currentTargetPosition, Time.deltaTime * moveSpeed);

            // ほぼ到着したらピタッと止める
            if (Vector3.Distance(mapCamera.transform.position, currentTargetPosition) < 0.01f)
            {
                mapCamera.transform.position = currentTargetPosition;
                isMoving = false;
            }
        }
    }

    // UIボタンから直接呼ばれるメソッド（ボタンごとに番号を渡す）
    public void MoveToPoint(int pointIndex)
    {
        if (pointIndex >= 0 && pointIndex < targetPoints.Length)
        {
            SetTargetPosition(targetPoints[pointIndex].position);
        }
    }

    private void SetTargetPosition(Vector3 targetPos)
    {
        if (mapCamera != null)
        {
            // カメラの高さ(Y)は維持したまま、XとZ（平面）だけをターゲットに合わせる
            currentTargetPosition = new Vector3(targetPos.x, mapCamera.transform.position.y, targetPos.z);
            isMoving = true;
        }
    }
}