using UnityEngine;

public class RedSphereTarget : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var power = other.GetComponent<PlayerPower>();
        if (power == null) return;

        if (power.HasPowerThreshold)
        {
            VanishEvents.Report("Red Sphere");  // <-- add this
            Destroy(gameObject);
        }
    }
}
