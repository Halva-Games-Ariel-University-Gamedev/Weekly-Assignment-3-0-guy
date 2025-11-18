using UnityEngine;
using System.Collections;

/**
 * This component spawns the given laser-prefab whenever the player clicks a given key.
 * It also updates the "scoreText" field of the new laser.
 */
public class LaserShooter : ClickSpawner
{
    [SerializeField]
    [Tooltip("How many points to add to the shooter, if the laser hits its target")]
    int pointsToAdd = 1;

    [SerializeField]
    [Tooltip("Total shots available (no reload)")]
    int totalShots = 10;

    [SerializeField]
    [Tooltip("Cooldown time between shots in seconds")]
    float shotCooldown = 0.5f;

    [SerializeField]
    [Tooltip("Recoil amount (distance to move back)")]
    float recoilAmount = 0.1f;

    [SerializeField]
    [Tooltip("Duration of the recoil effect in seconds")]
    float recoilDuration = 0.1f;

    // A reference to the field that holds the score that has to be updated when the laser hits its target.
    private NumberField scoreField;

    private int remainingShots;
    private float lastShotTime;

    private void Start()
    {
        scoreField = GetComponentInChildren<NumberField>();
        if (!scoreField)
            Debug.LogError($"No child of {gameObject.name} has a NumberField component!");
        remainingShots = totalShots; // Start with 10 shots
        lastShotTime = -shotCooldown; // Allow first shot immediately
    }

    private void AddScore()
    {
        scoreField.AddNumber(pointsToAdd);
    }

    protected override GameObject spawnObject()
    {
        // Enforce cooldown
        if (Time.time - lastShotTime < shotCooldown)
        {
            return null; // Prevent spawning
        }

        // Enforce ammo check
        if (remainingShots <= 0)
        {
            return null; // Prevent spawning if out of shots
        }

        lastShotTime = Time.time;
        remainingShots--;

        GameObject newObject = base.spawnObject();
        if (newObject == null) return null;

        DestroyOnTrigger2D newObjectDestroyer = newObject.GetComponent<DestroyOnTrigger2D>();
        if (newObjectDestroyer)
            newObjectDestroyer.onHit += AddScore;

        // Apply recoil effect
        StartCoroutine(ApplyRecoil());

        return newObject;
    }

    private IEnumerator ApplyRecoil()
    {
        Vector3 originalPos = transform.localPosition;
        Vector3 recoilOffset = -transform.right * recoilAmount;

        //(recoil)
        transform.localPosition += recoilOffset;
        yield return new WaitForSeconds(recoilDuration / 2f);


        transform.localPosition = originalPos;
    }
}