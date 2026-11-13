using UnityEngine;
using UnityEngine.Serialization;

public class MapModeController : MonoBehaviour
{
    public Camera mapCamera;

    [Header("赤チームのカメラ移動先")]
    [FormerlySerializedAs("targetPoints")]
    public Transform[] redTargetPoints;

    [Header("青チームのカメラ移動先")]
    public Transform[] blueTargetPoints;

    [Header("カメラの移動速度")]
    public float moveSpeed = 10f;

    private Vector3 currentTargetPosition;
    private bool isMoving = false;
    private bool isRedTeam = true;
    private int selectedPointIndex;

    private void OnEnable()
    {
        if (mapCamera != null) mapCamera.gameObject.SetActive(true);

        Transform[] points = GetCurrentTargetPoints();
        if (mapCamera != null && points != null && points.Length > 0 && points[0] != null)
        {
            selectedPointIndex = 0;
            SetTargetPosition(points[0].position);
            mapCamera.transform.position = currentTargetPosition;
            isMoving = false;
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

    public void SetTeam(bool red)
    {
        isRedTeam = red;
        MoveToPoint(selectedPointIndex);
    }

    // UIボタンから直接呼ばれるメソッド（ボタンごとに番号を渡す）
    public void MoveToPoint(int pointIndex)
    {
        Transform[] points = GetCurrentTargetPoints();
        if (mapCamera == null)
        {
            Debug.LogError("MapModeController: Map Camera is not assigned.", this);
            return;
        }

        if (points == null || pointIndex < 0 || pointIndex >= points.Length || points[pointIndex] == null)
        {
            Debug.LogError($"MapModeController: No {(isRedTeam ? "red" : "blue")} team target is assigned at index {pointIndex}.", this);
            return;
        }

        selectedPointIndex = pointIndex;
        SetTargetPosition(points[pointIndex].position);
    }

    private Transform[] GetCurrentTargetPoints()
    {
        return isRedTeam ? redTargetPoints : blueTargetPoints;
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