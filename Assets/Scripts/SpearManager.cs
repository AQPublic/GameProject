using UnityEngine;
using UnityEngine.InputSystem;

public class SpearManager : MonoBehaviour
{
    [Header("Spear Settings")]
    public GameObject spearPrefab;       // Your spear prefab
    public Transform spawnPoint;         // Where spears spawn
    public float respawnDelay = 0.2f;    // Slight delay to avoid physics overlap

    private GameObject currentSpear;

    void Start()
    {
        SpawnSpear();
    }

    void Update()
  {
    
    if (Input.GetButtonDown("Jump"))
    {
        Debug.Log("spawn spear")
;        SpawnSpear();
    }
  }

    public void SpawnSpear()
    {
        // Prevent multiple spears
        if (currentSpear != null) return;

        // Instantiate spear
        currentSpear = Instantiate(spearPrefab, spawnPoint.position, spawnPoint.rotation);

        // Give the spear a reference to this manager
        Spear spearScript = currentSpear.GetComponent<Spear>();
        if (spearScript == null) return;
        spearScript.manager = this;
    }

    public void DespawnSpear(Spear spear)
    {
        // Only despawn if this is the active spear
        if (currentSpear == spear.gameObject)
        {
            Destroy(currentSpear);
            currentSpear = null;

            // Respawn a new spear after a tiny delay
            Invoke(nameof(SpawnSpear), respawnDelay);
        }
    }
}
