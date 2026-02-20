using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshRenderer))]
public class Player : MonoBehaviour
{
    [SerializeField] new Camera camera;
    [SerializeField] private float maxHp = 100;
    [SerializeField] private Color startColor;
    [SerializeField] private Color deathColor;

    [Header("Jump settings")]
    [SerializeField] private float jumpHeight;
    [SerializeField] private uint maxJumps = 2;
    [SerializeField] private float gravityMultiplier = 1;

    [Header("Lane Settings")]
    [SerializeField] private RoadData road;
    [SerializeField] private float laneChangeTime = 0.2f;
    [SerializeField] private float continuousLaneChangeDelay = 0.05f;

    public UnityEvent OnRestart;

    private float currentHp;

    private uint jumpsCount = 0;
    private Rigidbody rb;
    private MeshRenderer mr;

    private int currentLane = 1;
    private int moveDirection = 0;
    private float lastMoveTime;

    private float[] lanesX;
    private Tweener laneTween;

    void Start()
    {
        mr = GetComponent<MeshRenderer>();

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        InitializeLanes();

        Restart();
    }

    void Restart()
    {
        mr.material.color = startColor;

        currentLane = road.lanesCount / 2;
        currentHp = maxHp;

        transform.position = new Vector3(
            lanesX[currentLane],
            transform.localScale.y / 2f,
            0
        );

        OnRestart?.Invoke();
    }

    private void InitializeLanes()
    {
        int laneCount = Mathf.Max(1, road.lanesCount);
        lanesX = new float[laneCount];

        float roadWidth = road.Scale.x;
        float laneWidth = roadWidth / laneCount;

        float leftEdge = road.Position.x - roadWidth / 2f;

        for (int i = 0; i < laneCount; i++)
        {
            lanesX[i] = leftEdge + laneWidth * (i + 0.5f);
        }
    }

    void Update()
    {
        camera.transform.position = transform.position + new Vector3(0, 4, -10);

        if (moveDirection != 0 &&
            Time.time - laneChangeTime - lastMoveTime >= continuousLaneChangeDelay)
        {
            ChangeLane(moveDirection);
            lastMoveTime = Time.time;
        }
    }

    void FixedUpdate()
    {
        rb.AddForce(
            Physics.gravity * gravityMultiplier,
            ForceMode.Acceleration
        );
    }

    public void OnJump()
    {
        if (jumpsCount >= maxJumps)
            return;

        float jumpVelocity = Mathf.Sqrt(
            2 * jumpHeight * -Physics.gravity.y * gravityMultiplier
        );

        rb.velocity = new Vector3(
            rb.velocity.x,
            jumpVelocity,
            rb.velocity.z
        );

        jumpsCount++;
    }

    public void OnLeftRight(InputValue input)
    {
        moveDirection = (int)Math.Round(input.Get<float>());

        if (moveDirection != 0)
        {
            ChangeLane(moveDirection);
            lastMoveTime = Time.time;
        }
    }

    private void ChangeLane(int direction)
    {
        int newLane = Mathf.Clamp(
            currentLane + direction,
            0,
            lanesX.Length - 1
        );

        if (newLane == currentLane)
            return;

        currentLane = newLane;

        laneTween?.Kill();

        laneTween = rb
            .DOMoveX(lanesX[currentLane], laneChangeTime)
            .SetEase(Ease.InOutQuad);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            jumpsCount = 0;
        }
    }

    public void TakeDamage(float damage)
    {
        if (currentHp < damage)
        {
            Restart();
        }
        else
        {
            currentHp -= damage;
            mr.material.color = Color.Lerp(startColor, deathColor, (maxHp - currentHp) / maxHp);
        }
    }
}
