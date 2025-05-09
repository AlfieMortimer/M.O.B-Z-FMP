using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    public LayerMask player;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.layer);

        if (other.gameObject.layer == player)
        {
            other.gameObject.GetComponent<PlayerHealth>().LoseHealthRPC(gameObject.GetComponentInParent<BasicEnemyStats>().damage);
            Debug.Log("Damage?");
        }
    }
}
