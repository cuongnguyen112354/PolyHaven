using UnityEngine;

public class TopDownCameraDrag : MonoBehaviour
{
    [Header("Camera Movement")]
    public float dragSpeed = 2f;              // Tốc độ kéo nhanh/chậm
    public float edgeScrollSpeed = 20f;       // Tốc độ khi di chuột ra mép (tùy chọn)
    public bool edgeScroll = false;           // Bật/tắt di chuyển khi ra mép màn hình

    [Header("Zoom")]
    public float zoomSpeed = 10f;
    public float minZoom = 5f;
    public float maxZoom = 50f;

    private Vector3 dragOrigin;
    private bool isDragging = false;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        HandleMouseDrag();
        HandleEdgeScroll();
        HandleZoom();
    }

    void HandleMouseDrag()
    {
        // Bắt đầu kéo bằng chuột trái
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            isDragging = true;
            return;
        }

        // Nếu không giữ chuột trái → thoát
        if (!Input.GetMouseButton(0))
        {
            isDragging = false;
            return;
        }

        if (!isDragging) return;

        Vector3 pos = Input.mousePosition;
        Vector3 delta = pos - dragOrigin;               // Delta pixel trên màn hình
        delta *= dragSpeed * 0.01f;                      // Điều chỉnh tốc độ

        // === PHẦN QUAN TRỌNG NHẤT ===
        // Chuyển delta pixel thành di chuyển trên mặt phẳng XZ theo hướng camera
        Vector3 move = new Vector3(delta.x, 0, delta.y);
        Vector3 worldMove = Camera.main.transform.TransformDirection(move);
        worldMove.y = 0;                                 // Không cho bay lên/xuống khi kéo

        transform.position -= worldMove;                 // Dấu trừ vì kéo sang trái → camera phải đi sang trái

        // Cập nhật origin cho frame tiếp theo (rất quan trọng để mượt)
        dragOrigin = pos;
    }

    void HandleEdgeScroll()
    {
        if (!edgeScroll | isDragging) return;

        float edgeSize = 20f; // pixel từ mép
        Vector3 move = Vector3.zero;

        if (Input.mousePosition.x <= edgeSize)          move.x -= 1;
        if (Input.mousePosition.x >= Screen.width - edgeSize) move.x += 1;
        if (Input.mousePosition.y <= edgeSize)          move.z -= 1;
        if (Input.mousePosition.y >= Screen.height - edgeSize) move.z += 1;

        transform.Translate(move * edgeScrollSpeed * Time.deltaTime, Space.World);
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            Vector3 pos = transform.position;
            pos.y -= scroll * zoomSpeed * 100f * Time.deltaTime;
            pos.y = Mathf.Clamp(pos.y, minZoom, maxZoom);
            transform.position = pos;
        }
    }
}