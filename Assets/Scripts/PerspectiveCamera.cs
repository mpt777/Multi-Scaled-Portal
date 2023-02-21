using UnityEngine;

public class PerspectiveCamera : MonoBehaviour
{
    [SerializeField] private Transform display;
    [SerializeField] private float width = 1f;
    private Transform _camera;
    private Vector3 _initialForward;
    private Camera _ownCamera;

    private void Start()
    {
        _camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
     
        _ownCamera = GetComponent<Camera>();
        _initialForward = transform.forward;
    }

    private void Update()
    {
       UpdateFieldOfView();
       //UpdateViewDirection();
    }

    private void UpdateFieldOfView()
    {
        Debug.Log("FOV");
        float distance = Vector3.Distance(display.position, _camera.position);
        float fieldOfView = 2 * Mathf.Atan2(width / 2f, distance) * Mathf.Rad2Deg;

        fieldOfView = Mathf.Clamp(fieldOfView, 30, 180);
        _ownCamera.fieldOfView = fieldOfView;
    }

    private void UpdateViewDirection()
    {
        Vector3 axisOfRotation = Vector3.Cross(display.position - _camera.position, -display.forward).normalized;

        float angleBetween = Vector3.Angle(_camera.position - display.position, -display.forward);

        transform.forward = Quaternion.AngleAxis(angleBetween, axisOfRotation) * _initialForward;
    }
}