using UnityEngine;

[CreateAssetMenu(menuName = "Item/ItemData")]

public class ItemData : ScriptableObject
{
    [Header("ID")]
    public int itemID;

    [Header("アイテム名")]
    public string itemName;

    [Header("重量")]
    public float weight;

    [Header("運搬モデル")]
    public GameObject carryPrefab;
}
