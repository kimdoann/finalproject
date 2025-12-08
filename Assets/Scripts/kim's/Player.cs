using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int itemsCollected = 0;
    public Inventory inventory;
    public float speed = 5f;

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(h, 0f, v);
        transform.Translate(move * speed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Collectible")) return;

        string pickedName = other.name;
        int pickedValue = 0;

        Collectible c = other.GetComponent<Collectible>();
        if (c != null && !string.IsNullOrEmpty(c.itemName))
            pickedName = c.itemName;

        CollectItem(pickedName, pickedValue);
        Destroy(other.gameObject);
    }

    public void CollectItem(string itemName, int value = 0)
    {
        itemsCollected++;
        Debug.Log($"Player collected: {itemName}. Total items: {itemsCollected}");

        if (inventory != null)
            inventory.AddItem(new Item { itemName = itemName, value = value });
    }
}