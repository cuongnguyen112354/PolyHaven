using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshPro))]
public class FloatingTextWorld : MonoBehaviour
{
    private float moveUpSpeed = 1f;
    private float lifetime = .5f;
    
    private TextMeshPro textMesh;

    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    void Start()
    {
        // Nhìn về camera
        transform.forward = Camera.main.transform.forward;
    }

    void Update()
    {
        // Di chuyển lên
        transform.position += Vector3.up * moveUpSpeed * Time.deltaTime;

        // Giảm alpha
        Color c = textMesh.color;
        c.a -= Time.deltaTime / lifetime;
        textMesh.color = c;

        // Hết thời gian thì xóa
        if (c.a <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void SetText(string text)
    {
        textMesh.text = text;
    }
}