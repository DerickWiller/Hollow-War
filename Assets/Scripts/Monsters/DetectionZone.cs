using UnityEngine;

public class DetectionZone : MonoBehaviour
{
    private EnemyAI parentAI;

    void Awake()
    {
        parentAI = GetComponentInParent<EnemyAI>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            parentAI.OnPlayerEnterDetectionZone();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            parentAI.OnPlayerExitDetectionZone();
        }
    }
}