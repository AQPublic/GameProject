using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Target Settings")]
    public int totalTargets = 3;     // How many targets must be hit
    private int targetsHit = 0;

    [Header("Gate")]
    public Gate gate;                // assign your gate in inspector

    private void Awake()
    {
        // Setup singleton instance
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TargetHit()
    {
        targetsHit++;

        Debug.Log("Target Hit! (" + targetsHit + "/" + totalTargets + ")");

        // When all targets are hit → open the gate
        if (targetsHit >= totalTargets)
        {
            if (gate != null)
                gate.OpenGate();
        }
    }
}
