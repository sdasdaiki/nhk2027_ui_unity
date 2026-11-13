using UnityEngine;

public class MapNode : MonoBehaviour
{
    public Transform mapCamera; // 動かすカメラ
    
    [Header("赤と青の目的地")]
    public Transform redTarget;  // 赤用のStartArea等
    public Transform blueTarget; // 青用のStartArea等
    
    [Header("チーム判定用")]
    public TeamSwitchController teamController; // 今どちらのチームか知るため

    // ボタンが押された時に呼ばれるメソッド
public void MoveToTarget()
    {
        if (teamController == null || mapCamera == null) return;

        Transform target = teamController.isRedTeam ? redTarget : blueTarget;
        
        if (target != null)
        {
            // 【変更】localPosition から position に戻す
            mapCamera.position = target.position;
        }
    }
}