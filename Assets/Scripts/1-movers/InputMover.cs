using UnityEngine;
using UnityEngine.InputSystem;

/**
 * This component moves its object when the player clicks the arrow keys.
 */
public class InputMover : MonoBehaviour
{
    [Tooltip("Speed of movement, in meters per second")]
    [SerializeField] float speed = 10f;

    [Tooltip("Dash speed (faster than regular movement)")]
    [SerializeField] float dashSpeed = 30f;

    [Tooltip("Duration of the dash in seconds")]
    [SerializeField] float dashDuration = 0.2f;

    [Tooltip("Cooldown between dashes in seconds")]
    [SerializeField] float dashCooldown = 1f;

    [SerializeField]
    InputAction move = new InputAction(
        type: InputActionType.Value, expectedControlType: nameof(Vector2));

    [SerializeField]
    InputAction dashAction = new InputAction(
        type: InputActionType.Button);

    private bool isDashing = false;
    private Vector2 dashDirection;
    private float lastDashTime;

    private void Start()
    {
        lastDashTime = -dashCooldown;
    }

    void OnEnable()
    {
        move.Enable();
        dashAction.Enable();
    }

    void OnDisable()
    {
        move.Disable();
        dashAction.Disable();
    }

    void Update()
    {
        Vector2 input = move.ReadValue<Vector2>();

        Vector3 movement;
        if (isDashing)
        {
            movement = new Vector3(dashDirection.x, dashDirection.y, 0) * dashSpeed * Time.deltaTime;
        }
        else
        {
            movement = new Vector3(input.x, input.y, 0) * speed * Time.deltaTime;
        }

        transform.position += movement;

        // Check for dash input
        if (dashAction.triggered && !isDashing && Time.time >= lastDashTime + dashCooldown && input.magnitude > 0.1f)
        {
            dashDirection = input.normalized;
            isDashing = true;
            lastDashTime = Time.time;
            StartCoroutine(EndDash());
        }
    }

    private System.Collections.IEnumerator EndDash()
    {
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
    }
}