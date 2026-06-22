using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MarbleSoundController : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip rollingSound;
    [SerializeField] private AudioClip hitSound;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource rollingAudioSource;
    [SerializeField] private AudioSource hitAudioSource;

    [Header("Rolling Settings")]
    [SerializeField] private float minSpeedToRoll = 0.05f;
    [SerializeField] private float rollingVolume = 1f;
    [SerializeField] private float rollingVolumeMultiplier = 1f;
    [SerializeField] private float rollingFadeSpeed = 20f;
    [SerializeField] private float minRollingPitch = 0.95f;
    [SerializeField] private float maxRollingPitch = 1.1f;
    [SerializeField] private float pitchSpeedNeeded = 8f;

    [Header("Hit Settings")]
    [SerializeField] private float minLandingImpact = 2.5f;
    [SerializeField] private float minHardImpact = 7f;
    [SerializeField] private float maxImpactForce = 12f;
    [SerializeField] private float maxHitVolume = 1f;
    [SerializeField] private float hitCooldown = 0.2f;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;

    private Rigidbody rb;

    private bool isTouchingGround;
    private bool wasTouchingGround;

    private float lastHitTime;
    private float currentRollingVolume;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (rollingAudioSource == null)
        {
            GameObject rollingObject = new GameObject("Rolling Audio Source");
            rollingObject.transform.SetParent(transform);
            rollingObject.transform.localPosition = Vector3.zero;
            rollingAudioSource = rollingObject.AddComponent<AudioSource>();
        }

        if (hitAudioSource == null)
        {
            GameObject hitObject = new GameObject("Hit Audio Source");
            hitObject.transform.SetParent(transform);
            hitObject.transform.localPosition = Vector3.zero;
            hitAudioSource = hitObject.AddComponent<AudioSource>();
        }

        SetupRollingAudio();
        SetupHitAudio();
    }

    private void Start()
    {
        StartRollingLoop();
    }

    private void Update()
    {
        UpdateRollingSound();

        wasTouchingGround = isTouchingGround;
        isTouchingGround = false;
    }

    private void SetupRollingAudio()
    {
        rollingAudioSource.clip = rollingSound;
        rollingAudioSource.loop = true;
        rollingAudioSource.playOnAwake = false;
        rollingAudioSource.volume = 0f;
        rollingAudioSource.pitch = 1f;
        rollingAudioSource.spatialBlend = 0f;
        rollingAudioSource.mute = false;
    }

    private void SetupHitAudio()
    {
        hitAudioSource.loop = false;
        hitAudioSource.playOnAwake = false;
        hitAudioSource.volume = 1f;
        hitAudioSource.pitch = 1f;
        hitAudioSource.spatialBlend = 0f;
        hitAudioSource.mute = false;
    }

    private void StartRollingLoop()
    {
        if (rollingAudioSource == null || rollingSound == null)
        {
            Debug.LogWarning("MarbleSoundController: Rolling sound or rolling AudioSource is missing.");
            return;
        }

        rollingAudioSource.clip = rollingSound;
        rollingAudioSource.loop = true;
        rollingAudioSource.volume = 0f;

        if (!rollingAudioSource.isPlaying)
        {
            rollingAudioSource.Play();
        }
    }

    private void UpdateRollingSound()
    {
        if (rollingAudioSource == null || rollingSound == null)
            return;

        if (!rollingAudioSource.isPlaying)
        {
            rollingAudioSource.Play();
        }

        float speed = rb.linearVelocity.magnitude;

        bool shouldMakeRollingSound = isTouchingGround && speed > minSpeedToRoll;

        float targetVolume = shouldMakeRollingSound
            ? rollingVolume * rollingVolumeMultiplier
            : 0f;

        currentRollingVolume = Mathf.MoveTowards(
            currentRollingVolume,
            targetVolume,
            rollingFadeSpeed * Time.deltaTime
        );

        rollingAudioSource.volume = currentRollingVolume;

        float pitchPercent = Mathf.InverseLerp(minSpeedToRoll, pitchSpeedNeeded, speed);
        rollingAudioSource.pitch = Mathf.Lerp(minRollingPitch, maxRollingPitch, pitchPercent);

        if (showDebugLogs)
        {
            Debug.Log("Rolling | Grounded: " + isTouchingGround + " | Speed: " + speed + " | TargetVolume: " + targetVolume + " | Volume: " + rollingAudioSource.volume);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        isTouchingGround = true;
        TryPlayHitSound(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        isTouchingGround = true;
    }

    private void TryPlayHitSound(Collision collision)
    {
        if (hitAudioSource == null || hitSound == null)
            return;

        if (Time.time < lastHitTime + hitCooldown)
            return;

        float impactForce = collision.relativeVelocity.magnitude;

        bool landedFromAir = !wasTouchingGround && impactForce >= minLandingImpact;
        bool hardImpact = impactForce >= minHardImpact;

        if (!landedFromAir && !hardImpact)
            return;

        float impactPercent = Mathf.InverseLerp(minLandingImpact, maxImpactForce, impactForce);
        float volume = Mathf.Lerp(0.15f, maxHitVolume, impactPercent);

        hitAudioSource.pitch = Random.Range(0.95f, 1.05f);
        hitAudioSource.PlayOneShot(hitSound, volume);

        lastHitTime = Time.time;
    }
}