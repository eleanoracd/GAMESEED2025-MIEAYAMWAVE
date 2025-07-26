using UnityEngine;

public interface INPC
{
    void ConvertToCultist(CultistType type);
}

public enum CultistType { Basic, WomanBasic, Adventurer, Aliens}

public class CultistLogic : MonoBehaviour
{
    [System.Serializable]
    public class CultistPrefab
    {
        public CultistType type;
        public GameObject prefab;
    }

    [Header("Settings")]
    [SerializeField] private CultistPrefab[] cultistPrefabs;
    [SerializeField] private CultistType myType;
    [SerializeField] private float followDistance = 1f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float followDelay = 0.3f;

    private Transform leader;
    private bool isLeader = false;
    private Rigidbody2D rb;
    private PlayerController controller;
    private float lastJumpTime;
    private Vector3 targetOffset;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<PlayerController>();
        targetOffset = new Vector3(-followDistance, 0, 0);
    }

    public void SetAsFollower(Transform leaderTransform)
    {
        leader = leaderTransform;
        isLeader = false;
        if (controller != null) controller.enabled = false;
    }

    public void SetAsLeader()
    {
        isLeader = true;
        leader = transform;
        if (controller != null) controller.enabled = true;
    }

    private void Update()
    {
        if (!isLeader && leader != null)
        {
            // Physics-based movement
            Vector3 targetPos = leader.TransformPoint(targetOffset);
            Vector2 direction = (targetPos - transform.position).normalized;
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

            // Delayed jump mirroring
            if (controller != null && leader.TryGetComponent<PlayerController>(out var leaderController))
            {
                if (leaderController.IsJumping && controller.IsGrounded &&
                    Time.time > lastJumpTime + followDelay)
                {
                    controller.Jump();
                    lastJumpTime = Time.time;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        INPC npc = other.GetComponent<INPC>();
        if (npc != null)
        {
            npc.ConvertToCultist(myType);
        }
    }

    public CultistType MyType => myType;
}
