using UnityEngine;

public class MapNode : MonoBehaviour
{
    [Header("この場所から移動できる「隣のポイント」")]
    public MapNode[] connectedNodes;
}