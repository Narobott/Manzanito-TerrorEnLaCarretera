// DraggableObject.cs
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class DraggableObject : MonoBehaviour
{
    [Header("Throw Settings")]
    [SerializeField] private float throwMultiplier = 12f;
    [SerializeField] private float maxThrowSpeed = 25f;
    [SerializeField] private float spinMultiplier = 18f;
    [SerializeField] private float maxAngularSpeed = 720f;

    [Header("Drag Feel")]
    [SerializeField] private float followLerp = 0.25f;
    [SerializeField] private float liftScale = 1.08f;
    [SerializeField] private float liftScaleSpeed = 12f;

    [Header("Velocity Sampling")]
    [SerializeField] private int velocityBufferSize = 8;
    [SerializeField] private float velocityMaxAge = 0.12f;

    // -- Components ----------
    private Rigidbody2D rb;
    private Collider2D col;
    private Camera cam;

    // -- Drag State ----------
    private bool isDragging;
    private Vector2 dragOffset;
    private Vector3 targetScale;
    private Vector3 baseScale;

    // -- Movement ------------
    private float moveSpeed;

    // -- Velocity Buffer -----
    private struct Sample { public Vector2 worldPos; public float time; }
    private readonly Queue<Sample> velBuffer = new Queue<Sample>();

    // Called by the spawner after Instantiate
    public void Init(float speed)
    {
        moveSpeed = speed;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        cam = Camera.main;
        baseScale = transform.localScale;
        targetScale = baseScale;

        var input = MenuInput.instance;
        input.pressStarted.AddListener(OnPressStarted);
        input.pressMoved.AddListener(OnPressMoved);
        input.pressEnded.AddListener(OnPressEnded);

        rb.isKinematic = true;
    }

    private void Update()
    {
        // Animate lift scale
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.deltaTime * liftScaleSpeed
        );

        // Move left automatically when not being dragged or thrown
        if (!isDragging && rb.isKinematic)
        {
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
        }
    }

    // -- Trigger Destroy -----

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Destroy"))
            Destroy(gameObject);
    }

    // -- Input Handlers ------

    private void OnPressStarted(Vector2 screenPos)
    {
        Vector2 worldPos = ScreenToWorld(screenPos);
        if (!col.OverlapPoint(worldPos)) return;

        isDragging = true;
        dragOffset = (Vector2)transform.position - worldPos;
        targetScale = baseScale * liftScale;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.isKinematic = true;

        velBuffer.Clear();
        RecordSample(worldPos);
    }

    private void OnPressMoved(Vector2 screenPos)
    {
        if (!isDragging) return;
        Vector2 worldPos = ScreenToWorld(screenPos);
        RecordSample(worldPos);

        Vector2 target = worldPos + dragOffset;
        transform.position = Vector2.Lerp(transform.position, target, 1f - followLerp);
    }

    private void OnPressEnded(Vector2 screenPos)
    {
        if (!isDragging) return;
        RecordSample(ScreenToWorld(screenPos));
        Throw(ComputeReleaseVelocity());
        StopDragging();
    }

    // -- Core Logic ----------

    private void Throw(Vector2 velocity)
    {
        rb.isKinematic = false;
        rb.linearVelocity = velocity;

        float speed = velocity.magnitude;
        float targetSpin = Mathf.Sign(velocity.x) * Mathf.Min(speed * spinMultiplier, maxAngularSpeed);
        rb.angularVelocity = targetSpin;
    }

    private void StopDragging()
    {
        isDragging = false;
        targetScale = baseScale;
        rb.isKinematic = false;
    }

    // -- Velocity Sampling ---

    private void RecordSample(Vector2 worldPos)
    {
        velBuffer.Enqueue(new Sample { worldPos = worldPos, time = Time.time });
        while (velBuffer.Count > velocityBufferSize)
            velBuffer.Dequeue();
    }

    private Vector2 ComputeReleaseVelocity()
    {
        float cutoff = Time.time - velocityMaxAge;
        while (velBuffer.Count > 1 && velBuffer.Peek().time < cutoff)
            velBuffer.Dequeue();

        var samples = new List<Sample>(velBuffer);
        if (samples.Count < 2) return Vector2.zero;

        Vector2 velocity = Vector2.zero;
        float totalWeight = 0f;

        for (int i = 1; i < samples.Count; i++)
        {
            float dt = samples[i].time - samples[i - 1].time;
            if (dt <= 0f) continue;

            float weight = (float)i / samples.Count;
            Vector2 v = (samples[i].worldPos - samples[i - 1].worldPos) / dt;
            velocity += v * weight;
            totalWeight += weight;
        }

        if (totalWeight <= 0f) return Vector2.zero;

        velocity /= totalWeight;
        velocity *= throwMultiplier;
        velocity = Vector2.ClampMagnitude(velocity, maxThrowSpeed);
        return velocity;
    }

    // -- Helpers -------------

    private Vector2 ScreenToWorld(Vector2 screenPos)
        => cam.ScreenToWorldPoint(screenPos);

    private void OnDestroy()
    {
        if (MenuInput.instance == null) return;
        var input = MenuInput.instance;
        input.pressStarted.RemoveListener(OnPressStarted);
        input.pressMoved.RemoveListener(OnPressMoved);
        input.pressEnded.RemoveListener(OnPressEnded);
    }
}