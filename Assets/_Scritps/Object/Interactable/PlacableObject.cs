using UnityEngine;

public class PlacableObject : MonoBehaviour
{
    [SerializeField] private int distanceToPlace = 50;
    
    [Header("----------GameObjects will activate after Place----------")]
    [SerializeField] private GameObject[] gameObjects;
    [Header("----------MonoBehaviour will activate after Place----------")]
    [SerializeField] private MonoBehaviour[] scripts;

    private MeshRenderer meshRenderer;
    private Material sourceMat;
    private int layerMask;
    private bool isValid = true;
    private float gridSize = 0.1f;

    public void Init()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        sourceMat = meshRenderer.material;
        gameObject.layer = LayerMask.NameToLayer("Ghost");
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
        meshRenderer.enabled = true;
        meshRenderer.material = sourceMat;
        GetComponent<MeshCollider>().isTrigger = false;
        GetComponent<MeshCollider>().convex = false;
        gameObject.layer = LayerMask.NameToLayer("Default");

        foreach (GameObject go in gameObjects)
            go.SetActive(true);

        foreach (MonoBehaviour script in scripts)
            script.enabled = true;

        Destroy(GetComponent<Rigidbody>());
        Destroy(this);
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