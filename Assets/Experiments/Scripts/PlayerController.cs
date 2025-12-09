using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float MoveSpeed = 6f;
    public float TurnSpeed = 150f;   // yaw rotation with Left/Right
    public float Gravity = 20f;

    private CharacterController _cc;
    private float _verticalVelocity;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // Turn with left/right arrows (or A/D)
        float turn = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up, turn * TurnSpeed * Time.deltaTime);

        // Move forward/back with up/down arrows (or W/S)
        float forward = Input.GetAxis("Vertical");
        Vector3 move = transform.forward * (forward * MoveSpeed);

        // Gravity
        if (_cc.isGrounded)
        {
            _verticalVelocity = -1f;
        }
        else
        {
            _verticalVelocity -= Gravity * Time.deltaTime;
        }
        move.y = _verticalVelocity;

        _cc.Move(move * Time.deltaTime);
    }
}
