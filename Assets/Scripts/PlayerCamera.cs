using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    //loots of help from chatgpt
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private float fastMoveMultiplier = 2f;

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
        HandleZoom();
        HandleRotation();

        ClampPosition();   // ← important: call last
    }

    void HandleKeyboardMovement()
    {
        float h = Input.GetAxis("Horizontal");  // A/D
        float v = Input.GetAxis("Vertical");    // W/S

        // Get the camera’s forward and right (flattened)
        Vector3 forward = cam.transform.forward;
        Vector3 right = cam.transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        // Combine input with camera directions
        Vector3 direction = (forward * v + right * h).normalized;

        float speed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
            speed *= fastMoveMultiplier;

        transform.Translate(direction * speed * Time.deltaTime, Space.World);

    }
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        float newZoom = cam.orthographicSize - scroll * zoomSpeed;

        cam.orthographicSize = Mathf.Clamp(newZoom, minZoom, maxZoom);
    }

    void HandleRotation()
    {
        float rotate = 0f;

        if (Input.GetKey(KeyCode.Q))
            rotate = -1f;
        if (Input.GetKey(KeyCode.E))
            rotate = 1f;

        transform.Rotate(Vector3.up, rotate * rotateSpeed * Time.deltaTime, Space.World);
    }
    void ClampPosition()
    {
        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);

        transform.position = pos;
    }

}
