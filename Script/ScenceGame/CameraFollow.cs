using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("Follow Mode")]
    public bool smoothFollow = true;   // ✅ bật/tắt smooth
    public float smoothSpeed = 5f;     // tốc độ mượt 

    [Header("Bounds")]
    public bool useBounds = true;      // bật/tắt giới hạn theo map
    public Tilemap boundsTilemap;      // ưu tiên dùng Tilemap này (nếu không set sẽ auto tìm)
    public Collider2D boundsCollider;  // có thể dùng collider để làm bounds thay cho Tilemap

    [Header("Map Bounds (auto from Tilemap)")]
    private float halfHeight;
    private float halfWidth;
    private Vector2 minBounds;
    private Vector2 maxBounds;
    private bool hasValidBounds;
    private bool clampX;
    private bool clampY;

    void Start()
    {
        Camera cam = GetComponent<Camera>();
        halfHeight = cam.orthographicSize;
        halfWidth = halfHeight * cam.aspect;

        // Nếu quên gán target, thử tìm theo Tag "Player"
        if (target == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                target = playerObject.transform;
            }
        }

        // Lấy bounds từ Tilemap hoặc Collider (nếu có)
        hasValidBounds = false;
        if (useBounds)
        {
            if (boundsTilemap == null)
            {
                boundsTilemap = FindObjectOfType<Tilemap>();
            }

            if (boundsTilemap != null)
            {
                BoundsInt cellBounds = boundsTilemap.cellBounds;
                Vector3 min = boundsTilemap.CellToWorld(cellBounds.min);
                Vector3 max = boundsTilemap.CellToWorld(cellBounds.max);
                minBounds = min;
                maxBounds = max;
                hasValidBounds = true;
            }
            else if (boundsCollider != null)
            {
                Bounds worldBounds = boundsCollider.bounds;
                minBounds = new Vector2(worldBounds.min.x, worldBounds.min.y);
                maxBounds = new Vector2(worldBounds.max.x, worldBounds.max.y);
                hasValidBounds = true;
            }

            if (hasValidBounds)
            {
                float worldWidth = maxBounds.x - minBounds.x;
                float worldHeight = maxBounds.y - minBounds.y;

                // Nếu map nhỏ hơn khung hình theo một chiều thì không clamp chiều đó
                clampX = worldWidth > halfWidth * 2f + 0.01f;
                clampY = worldHeight > halfHeight * 2f + 0.01f;

                Debug.Log($"📏 Bounds: min={minBounds}, max={maxBounds}, clampX={clampX}, clampY={clampY}");
            }
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        // Follow mượt hay bám sát
        Vector3 finalPosition;
        if (smoothFollow)
        {
            finalPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        }
        else
        {
            finalPosition = desiredPosition;
        }

        if (useBounds && hasValidBounds)
        {
            float clampedX = clampX ? Mathf.Clamp(finalPosition.x, minBounds.x + halfWidth, maxBounds.x - halfWidth) : desiredPosition.x;
            float clampedY = clampY ? Mathf.Clamp(finalPosition.y, minBounds.y + halfHeight, maxBounds.y - halfHeight) : desiredPosition.y;
            transform.position = new Vector3(clampedX, clampedY, finalPosition.z);
        }
        else
        {
            transform.position = finalPosition;
        }
    }
}
