using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Search;

public class Inventory : MonoBehaviour
{
    [Header("所持アイテム")]
    [SerializeField] private List<ItemData> items = new List<ItemData>();

    /// <summary>
    /// アイテム追加
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(ItemData item)
    {
        items.Add(item);
        Debug.Log(item.itemName + "を取得");
    }

    /// <summary>
    /// アイテム削除
    /// </summary>
    /// <param name="item"></param>
    public void RemoveItem(ItemData item)
    {
        items.Remove(item);
    }

    /// <summary>
    /// 所持確認
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool HasItem(ItemData item)
    {
        return items.Contains(item);
    }

    /// <summary>
    /// アイテム一覧取得
    /// </summary>
    /// <returns></returns>
    public  List<ItemData> GetItems()
    {
        return items;
    }
}
