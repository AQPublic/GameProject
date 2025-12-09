using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class purgatory : MonoBehaviour
{   [Header("Duplication Settings")]
    public float spawnInterval = 2.5f;
    public Vector2 horizontalOffsetRange = new Vector2(-10f, 10f);
    public Vector2 verticalOffsetRange = new Vector2(0f, 15f);

    [Header("Fling Settings")]
    public float flingCheckInterval = 4f;  // how often we *check* for a fling
    [Range(0f, 1f)]
    public float flingChance = 0.5f;      // 1% chance each check
    public Vector2 flingAccelerationRange = new Vector2(50f, 100f); // random strength
    public float flingDuration = 0.5f;    // how long to apply fling acceleration

    private void Start()
    {
        // Repeated duplication
        InvokeRepeating(nameof(SpawnRandomEnemyCopy), spawnInterval, spawnInterval);

        // Repeated fling attempts
        InvokeRepeating(nameof(TryFlingRandomEnemy), flingCheckInterval, flingCheckInterval);
    }

    void SpawnRandomEnemyCopy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0) return;

        GameObject selectedEnemy = enemies[Random.Range(0, enemies.Length)];

        // Random offset
        float offsetX = Random.Range(horizontalOffsetRange.x, horizontalOffsetRange.y);
        float offsetZ = Random.Range(horizontalOffsetRange.x, horizontalOffsetRange.y);
        float offsetY = Random.Range(verticalOffsetRange.x, verticalOffsetRange.y);

        Vector3 spawnPos = selectedEnemy.transform.position + new Vector3(offsetX, offsetY, offsetZ);

        Instantiate(selectedEnemy, spawnPos, selectedEnemy.transform.rotation);
    }

    void TryFlingRandomEnemy()
    {
        if (Random.value > flingChance)
            return;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0) return;

        GameObject selectedEnemy = enemies[Random.Range(0, enemies.Length)];

        Rigidbody rb = selectedEnemy.GetComponent<Rigidbody>();
        if (rb == null || rb.isKinematic) return;

        // Random direction, biased upward
        Vector3 randomDir = Random.onUnitSphere;
        if (randomDir.y < 0f) randomDir.y = -randomDir.y;
        //randomDir.Normalize();

        float accelMagnitude = Random.Range(flingAccelerationRange.x, flingAccelerationRange.y);
        Vector3 accel = randomDir * accelMagnitude;

        rb.WakeUp(); // make sure physics is active

        StartCoroutine(ApplyAccelerationOverTime(rb, accel, flingDuration));
    }

    IEnumerator ApplyAccelerationOverTime(Rigidbody rb, Vector3 acceleration, float duration)
    {
        float elapsed = 0f;

        // Apply acceleration each physics step
        while (elapsed < duration && rb != null)
        {
            rb.AddForce(acceleration, ForceMode.Acceleration);
            rb.AddForce(new Vector3(0f, 20f + acceleration.y * 100f, 0f), ForceMode.Acceleration); // extra upward boost
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
}
