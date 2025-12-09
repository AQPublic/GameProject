using UnityEngine;

public class PickupBlueCube : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var power = other.GetComponent<PlayerPower>();
        if (power == null) return;

        power.AddPower(1);
        VanishEvents.Report("Blue Cube");   // <-- add this
        Destroy(gameObject);                // blue cube disappears
    }
}
