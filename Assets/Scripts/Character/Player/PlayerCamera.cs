using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;
    public Camera cameraObject;
    public PlayerManager player;
    [SerializeField] Transform cameraPivotTransform;

    // To tweak camera performance
    [Header("Camera Settings")]
    private float cameraSmoothSpeed = 1; // The bigger, the slower the camera will reach its position during movement
    [SerializeField] float leftAndRightRotationSpeed = 200;
    [SerializeField] float upAndDownRotationSpeed = 200;
    [SerializeField] float minimumPivot = -30; // Lowest point to look down
    [SerializeField] float maximumPivot = 60;  // Highest point to look up
    //[SerializeField] float cameraCollisionOffset = 0.2f;
    [SerializeField] LayerMask collideWithLayers;
    [SerializeField] float cameraCollisionRadius = 0.2f; 

    [Header("Camera Values")]
    private Vector3 cameraVelocity;
    private Vector3 cameraObjectPosition; //used for camera collision
    [SerializeField] float leftAndRightLookAngle;
    [SerializeField] float upAndDownLookAngle;
    private float cameraZPosition; //Value used for camera collision
    private float targetCameraZPosition; //for collision

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        cameraZPosition = cameraObject.transform.localPosition.z;
        cameraObjectPosition = cameraObject.transform.localPosition;
    }

    public void HandleAllCameraActions()
    {
        if (player != null)
        {
            HandleFollowTarget();
            HandleRotations();
            HandleCollisions();
        }
    }

    private void HandleFollowTarget()
    {
        Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, player.transform.position, ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);
        transform.position = targetCameraPosition;
    }

    private void HandleRotations()
    {
        // Rotate left and right based on horizontal movement on right joystick
        leftAndRightLookAngle += (PlayerInputManager.instance.cameraHorizontalInput * leftAndRightRotationSpeed) * Time.deltaTime;
        // Rotate up and down based on vertical movement
        upAndDownLookAngle -= (PlayerInputManager.instance.cameraVerticalInput * upAndDownRotationSpeed) * Time.deltaTime;
        // Clamp the up and down angle between a min and max value
        upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

        // Rotate pivot left and right
        Vector3 cameraRotation = Vector3.zero;
        Quaternion targetRotation;

        cameraRotation.y = leftAndRightLookAngle;
        targetRotation = Quaternion.Euler(cameraRotation);
        transform.rotation = targetRotation;

        // Rotate pivot up and down
        cameraRotation = Vector3.zero;
        cameraRotation.x = upAndDownLookAngle;
        targetRotation = Quaternion.Euler(cameraRotation);
        cameraPivotTransform.localRotation = targetRotation;
    }

    private void HandleCollisions()
    {
        targetCameraZPosition = cameraZPosition;

        RaycastHit hit;
        //direction for coliision check
        Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
        direction.Normalize();

    //we check if there is aan object in front of desired position
        if (Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetCameraZPosition), collideWithLayers))
        {
            //if there is, we get our distance for it
            float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
            //we then equate our target z position to the following
            targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
        }

        //if our target position is less than our colision radius, we substract our collision radius (making it snap back)
        if (Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius)
        {
            targetCameraZPosition = -cameraCollisionRadius;
        }
        //apply final position using lerp 0.2
        cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition, 0.2f);
        cameraObject.transform.localPosition = cameraObjectPosition;
    }
}
