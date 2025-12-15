using UnityEngine;

public class PlaceableObject : MonoBehaviour
{
    [SerializeField] private int distanceToPlace = 50;
    [SerializeField] private float gridSize = 0.1f;

    private MeshRenderer meshRenderer;
    private int layerMask;
    private bool isValid = true;

    public void Init()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        layerMask = ~LayerMask.GetMask("Ghost", "Player");

        // Đảm bảo MeshCollider là trigger
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (meshCollider != null)
            meshCollider.isTrigger = true;

        SetValid();
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        if (!GameManager.Instance.CompareGameState("Playing"))
            return;

        UpdateGhostPosition();

        if (Input.GetMouseButtonDown(0) && meshRenderer.enabled && isValid)
            PlaceBlock();
    }

    void UpdateGhostPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, distanceToPlace, layerMask))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                Vector3 pos = hit.point;
                pos.x = Mathf.Round(pos.x / gridSize) * gridSize;
                pos.z = Mathf.Round(pos.z / gridSize) * gridSize;
                transform.position = pos;

                if (Input.GetKeyDown(KeyCode.R))
                    transform.Rotate(0, 45f, 0, Space.World);

                // Kiểm tra va chạm
                CheckOverlap();

                meshRenderer.enabled = true;
            }
            else
            {
                meshRenderer.enabled = false;
                SetInvalid();
            }
        }
        else
        {
            meshRenderer.enabled = false;
            SetInvalid();
        }
    }

    void CheckOverlap()
    {
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        Vector3 center = transform.position;
        Vector3 halfExtents = meshCollider.bounds.extents;

        Collider[] colliders = Physics.OverlapBox(center, halfExtents, transform.rotation, layerMask);
        isValid = true;
        foreach (Collider collider in colliders)
        {
            if (!collider.CompareTag("Ground"))
            {
                isValid = false;
                break;
            }
        }

        if (isValid)
            SetValid();
        else
            SetInvalid();
    }

    void PlaceBlock()
    {
        HotBar.Instance.RemoveItem(gameObject.name);
        GameObject obj = ConstructionManager.Instance.AddPlacedObject(gameObject.name, transform.position, transform.rotation);
        
        if (obj.TryGetComponent<Chest>(out var chest))
        {
            chest.storageCode = ChestManager.Instance.GenerateChestCode();
            StorageCodeMap.AddCode(chest.storageCode, chest);
        }

        Destroy(gameObject);
    }

    private void SetValid()
    {
        isValid = true;
        meshRenderer.material = ConstructionManager.Instance.validMat;
    }

    private void SetInvalid()
    {
        isValid = false;
        meshRenderer.material = ConstructionManager.Instance.invalidMat;
    }
}