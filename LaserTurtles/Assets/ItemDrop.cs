using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    public GameObject itemToDrop;
    [SerializeField] float dropRadius = 1.0f;
    [SerializeField] float dropChancePercentage = 50.0f;
    [SerializeField] PlayerCombatSystem playerCombatSystem;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Damager") && playerCombatSystem.isAttacking)
        {
            float randomValue = Random.Range(0f, 100f);
            if (randomValue <= dropChancePercentage)
            {
                DropItem();
            }
        }
    }

    private void DropItem()
    {
        Vector3 dropPosition = transform.position + Random.insideUnitSphere * dropRadius;
        Instantiate(itemToDrop, dropPosition, Quaternion.identity);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, dropRadius);
    }
}