using System;
using UnityEngine;

public class PlayerPower : MonoBehaviour
{
    [Tooltip("How many blue cubes must be collected to clear red spheres.")]
    public int powerNeeded = 3;

    public int Power { get; private set; } = 0;

    public event Action<int> OnPowerChanged;

    public bool HasPowerThreshold => Power >= powerNeeded;

    public void AddPower(int amount = 1)
    {
        Power += Mathf.Max(0, amount);
        OnPowerChanged?.Invoke(Power);
        // Optional: Debug
        Debug.Log($"Power collected: {Power}/{powerNeeded}");
    }
}
