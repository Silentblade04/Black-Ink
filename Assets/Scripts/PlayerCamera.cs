using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    //loots of help from chatgpt
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private float fastMoveMultiplier = 2f;
    [SerializeField] private float edgeScrollSpeed = 10f;
    [SerializeField] private float edgeThickness = 20f;

    [Header("Zoom")]
    [SerializeField] private Camera cam;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minZoom = 10f;
    [SerializeField] private float maxZoom = 40f;

    [Header("Rotation")]
    [SerializeField] private float rotateSpeed = 100f;

    [Header("Map Bounds")]
    [SerializeField] private float minX = -50f;
    [SerializeField] private float maxX = 50f;
    [SerializeField] private float minZ = -50f;
    [SerializeField] private float maxZ = 50f;
    private void Start()
    {
        if (cam == null)
            cam = Camera.main;
    }
    void Update()
    {
        HandleKeyboardMovement();
        HandleEdgeScrolling();
        HandleZoom();
        HandleRotation();

        ClampPosition();   // ← important: call last
    }

    void HandleKeyboardMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(h, 0, v);

        float speed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
            speed *= fastMoveMultiplier;

        transform.Translate(dir * speed * Time.deltaTime, Space.World);
    }

    void HandleEdgeScrolling()
    {
        Vector3 move = Vector3.zero;
        Vector3 mousePos = Input.mousePosition;

        if (mousePos.x < edgeThickness)
            move.x -= edgeScrollSpeed;
        if (mousePos.x > Screen.width - edgeThickness)
            move.x += edgeScrollSpeed;
        if (mousePos.y < edgeThickness)
            move.z -= edgeScrollSpeed;
        if (mousePos.y > Screen.height - edgeThickness)
            move.z += edgeScrollSpeed;

        transform.Translate(move * Time.deltaTime, Space.World);
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        float newZoom = cam.orthographicSize - scroll * zoomSpeed;

        cam.orthographicSize = Mathf.Clamp(newZoom, minZoom, maxZoom);
    }

    void HandleRotation()
    {
        if (Input.GetMouseButton(2)) // middle mouse button
        {
            float rotX = Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, rotX, Space.World);
        }
    }
    void ClampPosition()
    {
        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);

        transform.position = pos;
    }

}
