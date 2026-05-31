using UnityEngine;

public class DeliveryPoint : MonoBehaviour
{
    public ItemType itemType;

    public void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Player player = other.GetComponent<Player>();
        if (player == null) return;

        GameManager.instance.CheckDelivery(player,this);

        Debug.Log("ƒS[ƒ‹“’B");
    }
}
