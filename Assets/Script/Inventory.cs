using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Item
{
    public string itemName;
    public int value;
}

public class Inventory : MonoBehaviour
{
    public List<Item> items = new List<Item>();


    public UnityEvent onInventoryChanged;

    void Start()
    {
        onInventoryChanged?.Invoke();
    }

    public void AddItem(Item item)
    {
        items.Add(item);
        onInventoryChanged?.Invoke();
    }
}
