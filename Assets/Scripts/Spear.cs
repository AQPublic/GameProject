using Unity.VisualScripting;
using UnityEngine;

public class Spear : MonoBehaviour
{
    [HideInInspector] public SpearManager manager;
    private bool hasHit = false;  // prevents double hits

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return;   // ignore duplicate collision events

        if (collision.collider.CompareTag("Enemy"))
    {
      GameObject obj = collision.collider.gameObject;
    Destroy(obj);
    Debug.Log("ENEMY HIT!");
    }

        // Check if we hit a target
        if (collision.collider.CompareTag("Target"))
        {
            hasHit = true;

Debug.Log("IS TARGET");
            // Tell the target it was hit
            Target target = collision.collider.GetComponent<Target>();
            if (target != null)
            {
                Debug.Log("TARGET HIT!");
                target.Hit();
            }

            // Tell the SpearManager to remove this spear and spawn a new one
            if (manager != null)
            {
                manager.DespawnSpear(this);
            }

            // Destroy the spear after this frame to avoid physics issues
            Destroy(gameObject, 0.01f);
        }
    }
}
