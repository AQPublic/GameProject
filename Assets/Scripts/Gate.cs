using UnityEngine;

public class Gate : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float openAngle = 90f;      // how far to rotate
    public float speed = 2f;           // how fast the gate opens

    private bool opening = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        // Save the default rotation
        closedRotation = transform.localRotation;

        // The gate swings around its Y axis by +90 degrees
        openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);
    }

    void Update()
    {
        if (opening)
        {
            transform.localRotation = Quaternion.Lerp(
                transform.localRotation,
                openRotation,
                Time.deltaTime * speed
            );
        }
    }

    public void OpenGate()
    {
        opening = true;
    }
}
