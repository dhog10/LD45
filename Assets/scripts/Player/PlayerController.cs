using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.Experimental.Rendering;
// using UnityEngine.PostProcessing;

public class PlayerController : MonoBehaviour
{
    public enum PlayerCameraModes { FIRST_OR_THIRD, FIRST_ONLY, THIRD_ONLY }

    public Vector3 ui3D2DOffset = Vector3.zero;
    public GameObject ui3D2DPrefab;

    [Header("Movement")]
    public float sprintSpeed = 15;
    public float speed = 10;
    public float walkingSpeed = 2;
    public float acceleration = 3;
    public int averageVelocitySamples = 10;
    public bool canJump = true;
    public bool canWalk = false;
    public float jumpForce = 1;
    public float bodyTurnDegrees = 45f;
    public GameObject headObject;
    public GameObject torsoObject;
    public GameObject rightHandObject;
    public GameObject leftHandObject;
    
    [Header("Footsteps")]
    public GameObject leftFootObject;
    public GameObject rightFootObject;
    public AudioMixerGroup footstepMixerGroup;
    public AudioClip[] footstepClips;

    public float stepUpHeight = 0.1f;
    [Header("Camera")]
    public GameObject fpsCameraPosition;
    public float cameraPitch = 0;
    public float cameraYaw = 0;
    public float maxCameraPitch = 80;
    public float thirdPersonCameraPitchOffset = 15;
    [Tooltip("Camera offset while in third person")]
    public Vector3 thirdPersonCameraOffset = Vector3.zero;
    [Tooltip("Camera offset while aiming in third person")]
    public Vector3 thirdPersonAimingOffset = Vector3.zero;
    public float cameraSensitivity = 100;
    public float cameraAdjustSpeed = 10;
    public float cameraDistance = 5;
    public float cameraDistanceAim = 2;
    public bool faceCameraDirection = false;
    public bool thirdPerson = true;
    public PlayerCameraModes cameraMode = PlayerCameraModes.FIRST_OR_THIRD;
    public bool clientControl = false;
    [Header("Weapons")]
    public GameObject weaponObject;
    public Vector3 weaponRotationOffset;
    public Vector3 weaponPositionOffset;
    public Vector3 aimingTwoHandOffset = Vector3.zero;
    public float aimingTorsoYaw = 30f;
    public float aimingTorsoPitch = 15;
    public float aimingTwoHandedTorsoYaw = 45f;
    public float aimingTwoHandTorsoPitch = 30f;
    public float recoilAmount = 5;
    [Header("Emissive")]
    public PlayerControllerEmissiveItem[] emissiveRenderers;
    public Color[] emissiveColors;

    [HideInInspector]
    public bool aimingWeapon = false;
    [HideInInspector]
    public bool walking = false;
    [HideInInspector]
    public bool frozen = false;
    [HideInInspector]
    public Color emissiveColor = Color.white;

    [HideInInspector]
    public NetworkPlayer networkPlayer;
    public Camera playerCamera { get; private set; }

    private float moveX, moveY;
    private CapsuleCollider capsuleCollider;
    public Animator animator { get; private set; }
    private Rigidbody rb;
    private TextMeshPro infoText;
    private Camera mainCamera;
    private Vector3 lastMoveDirection;
    private GameObject visualObject;
    private Vector3 lastVisualDirection;
    private Vector2 currentSpeed;
    private Vector3 currentCameraOffset = Vector3.zero;
    private float currentCameraDistance;
    private float currentThirdPersonCameraPitchOffset = 0;
    private float recoil = 0;
    private float targetRecoil = 0;

    private Quaternion currentBodyRotation;
    private float currentBodyYaw;
    private float currentLookingYaw;
    private Vector3[] velocitySamples;
    private Vector3 averageVelocity;
    private int velocitySampleIndex = 0;

    private AudioSource hitMarkerSource;
    private float armRecoil = 0;
    private float currentArmRecoil = 0;
    private bool recoilUp = false;
    private GameObject ui3D2D;

    private void Awake()
    {
        hitMarkerSource = GetComponents<AudioSource>()[0];
    }

    // Use this for initialization
    void Start () {
        Cursor.visible = false;
        Screen.lockCursor = true;
        lastMoveDirection = Vector3.zero;
        lastVisualDirection = Vector3.zero;
        currentSpeed = Vector2.zero;
        currentBodyRotation = Quaternion.identity;
        currentCameraDistance = cameraDistance;

        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        if (infoText)
        {
            infoText = transform.Find("PlayerInfo").GetComponent<TextMeshPro>();
        }

        playerCamera = transform.Find("Camera").GetComponent<Camera>();

        /*PostProcessingBehaviour behaviour = transform.Find("Camera").GetComponent<PostProcessingBehaviour>();
        if (behaviour)
        {
            PostProcessingBehaviour mainBehaviour = mainCamera.GetComponent<PostProcessingBehaviour>();
            if (mainBehaviour)
            {
                behaviour.profile = mainBehaviour.profile;
            }
        }*/

        visualObject = transform.Find("VisualObject").gameObject;
        var characterTransform = visualObject.transform.Find("Character");
        if (characterTransform)
        {
            animator = characterTransform.GetComponent<Animator>();
        }

        velocitySamples = new Vector3[averageVelocitySamples];
        for(var i = 0; i < averageVelocitySamples; i++)
        {
            velocitySamples[i] = Vector3.zero;
        }

        mpb = new MaterialPropertyBlock();
    }

    private void OnDestroy()
    {

    }

    private float lastTorsoPosition = 0;
    private float torsoOffsetCumulative = 0;
    private MaterialPropertyBlock mpb;
    private float currentCameraHeight = 0f;

    // Update is called once per frame
    void Update () {
        var torsoLocalY = torsoObject.transform.position.y - transform.position.y;

        if(lastTorsoPosition == 0)
        {
            lastTorsoPosition = torsoLocalY;
        }

        var torsoYDifference = torsoLocalY - lastTorsoPosition;
        torsoOffsetCumulative += torsoYDifference;
        lastTorsoPosition = torsoLocalY;

        HandleInput();
        HandleFootsteps();

        if (!playerCamera.gameObject.activeSelf)
        {
            playerCamera.gameObject.SetActive(true);
        }

        if (infoText && infoText.gameObject.activeSelf)
        {
            infoText.gameObject.SetActive(false);
        }

        if (mainCamera && mainCamera.gameObject.activeSelf)
        {
            mainCamera.gameObject.SetActive(false);
        }

        var mouseX = Input.GetAxis("Mouse X");
        var mouseY = Input.GetAxis("Mouse Y");

        cameraPitch = Mathf.Clamp(cameraPitch - (mouseY * cameraSensitivity), -90, maxCameraPitch);
        cameraYaw += mouseX * cameraSensitivity;

        var cameraOffset = GetCameraRotation(GetCameraPitchOffset());
        playerCamera.gameObject.transform.rotation = cameraOffset;

        if (aimingWeapon)
        {
            Debug.DrawLine(playerCamera.transform.position, GetBulletTrace(Vector3.zero).end);
            Debug.DrawLine(GetBulletTrace(Vector3.zero).start, GetBulletTrace(Vector3.zero).end, Color.red);
        }

        if (IsThirdPerson())
        {
            if (aimingWeapon)
            {
                currentThirdPersonCameraPitchOffset -= currentThirdPersonCameraPitchOffset * Time.deltaTime * cameraAdjustSpeed;
                currentCameraOffset += (thirdPersonAimingOffset - currentCameraOffset) * Time.deltaTime * cameraAdjustSpeed;
                // playerCamera.transform.LookAt(GetBulletTrace().end);
                            
            }
            else
            {
                currentThirdPersonCameraPitchOffset += (thirdPersonCameraPitchOffset - currentThirdPersonCameraPitchOffset) * Time.deltaTime * cameraAdjustSpeed;
                currentCameraOffset += (thirdPersonCameraOffset - currentCameraOffset) * Time.deltaTime * cameraAdjustSpeed;
            }

            var offsetPos = Vector3.zero;
            offsetPos += playerCamera.transform.forward * currentCameraOffset.x;
            offsetPos += playerCamera.transform.up * currentCameraOffset.y;
            offsetPos += playerCamera.transform.right * currentCameraOffset.z;

            currentCameraDistance += (((aimingWeapon ? cameraDistanceAim : cameraDistance)) - currentCameraDistance) * Time.deltaTime * cameraAdjustSpeed;
            var newPosition = transform.position - (playerCamera.transform.forward * currentCameraDistance) + offsetPos;
            playerCamera.gameObject.transform.position = newPosition;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, (newPosition - transform.position).normalized, out hit, currentCameraDistance, ~0))
            {
                playerCamera.gameObject.transform.position = hit.point;
            }
        }
        else
        {
            var cameraHeight = fpsCameraPosition.transform.position.y - transform.position.y + 0.15f;

            if(currentCameraHeight == 0f)
            {
                currentCameraHeight = cameraHeight;
            }
            else
            {
                currentCameraHeight = currentCameraHeight + (cameraHeight - currentCameraHeight) * Time.deltaTime * 6f;
            }

            currentCameraOffset = Vector3.zero;
            var direction = fpsCameraPosition.transform.forward;
            direction.y = 0f;
            playerCamera.gameObject.transform.position = transform.position + new Vector3(0f, currentCameraHeight, 0f) + direction * 0.15f;
        }

        // Toggle camera mode
        if (Input.GetKeyDown(KeyCode.F4))
        {
            ToggleFirstPerson();
        }

        // Visual object rotation
        {
            var x = rb.velocity.x;
            var y = rb.velocity.y;

            if (Mathf.Abs(x) > 0.001f || Mathf.Abs(y) > 0.001f || faceCameraDirection || !IsThirdPerson())
            {
                var visualObjectDirection = (faceCameraDirection || !IsThirdPerson()) ? GetMoveDirection() : lastMoveDirection;

                if(!IsMoving() && (faceCameraDirection || !IsThirdPerson()))
                {
                    var degreeDifference = Quaternion.Angle(Quaternion.Euler(0, cameraYaw, 0), currentBodyRotation);
                    if(degreeDifference < bodyTurnDegrees)
                    {
                        // Rotate to previous body rotation
                        visualObjectDirection = currentBodyRotation * Vector3.forward;
                    }
                    else
                    {
                        // Rotate to new rotation, and set current rotation
                        currentBodyRotation = Quaternion.Euler(0, cameraYaw, 0);
                        currentBodyYaw = cameraYaw;
                    }
                }
                else
                {
                    // Rotate to new rotation if moving
                    currentBodyRotation = Quaternion.Euler(0, cameraYaw, 0);
                    currentBodyYaw = cameraYaw;
                }

                if (visualObjectDirection != Vector3.zero)
                {
                    visualObject.transform.rotation = Quaternion.LookRotation(visualObjectDirection);
                }

                lastVisualDirection = visualObjectDirection;
            }

            if (Mathf.Abs(x) > 0.001f || Mathf.Abs(y) > 0.001f)
            {
                lastMoveDirection = rb.velocity;
                lastMoveDirection.y = 0;
                lastMoveDirection.Normalize();
            }

            HandleAnimator();
        }
        
        CalculateAverageVelocity();
    }

    private float lastLeftFootLocalY = 0;
    private float lastRightFootLocalY = 0;
    private float lastLeftFootStep = 0;
    private float lastRightFootStep = 0;
    private float leftFootMoved = 0;
    private float rightFootMoved = 0;
    private bool leftFootPlayed = false;
    private bool rightFootPlayed = false;
    private bool leftFootUp = false;
    private bool rightFootUp = false;

    void HandleFootsteps()
    {
        if (!IsGrounded()) { return; }

        var leftFootLocalY = transform.InverseTransformPoint(leftFootObject.transform.position).y;
        var rightFootLocalY = transform.InverseTransformPoint(rightFootObject.transform.position).y;

        if(Time.time - lastLeftFootStep > 0.1f)
        {
            leftFootMoved += Mathf.Abs(leftFootLocalY - lastLeftFootLocalY);

            if (leftFootLocalY > lastLeftFootLocalY)
            {
                // left foot up
                if (!leftFootPlayed && leftFootMoved > 0.07f)
                {
                    leftFootPlayed = true;
                    PlayFootstepAudio();
                    lastLeftFootStep = Time.time;
                }

                leftFootUp = true;
            }
            else
            {
                // left foot down

                leftFootMoved = 0;
                leftFootPlayed = false;
                leftFootUp = false;
            }
        }

        if(Time.time - lastRightFootStep > 0.1f)
        {
            if (rightFootLocalY > lastRightFootLocalY)
            {
                rightFootMoved += Mathf.Abs(rightFootLocalY - lastRightFootLocalY);

                // right foot up
                if (!rightFootPlayed && rightFootMoved > 0.07f)
                {
                    rightFootPlayed = true;
                    PlayFootstepAudio();
                    lastRightFootStep = Time.time;
                }

                rightFootUp = true;
            }
            else
            {
                // right foot down

                rightFootMoved = 0;
                rightFootPlayed = false;
                rightFootUp = false;
            }
        }

        lastLeftFootLocalY = leftFootLocalY;
        lastRightFootLocalY = rightFootLocalY;
    }

    void PlayFootstepAudio()
    {
        var clip = footstepClips[Random.Range(0, footstepClips.Length - 1)];

        // NetworkAudioManager.instance.PlaySound(clip, transform.position, true, footstepMixerGroup, 1.5f);
    }

    public void HandleInput()
    {
        moveX = Input.GetAxis("Horizontal");
        moveY = Input.GetAxis("Vertical");

        if (!frozen && canJump && Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce);
        }
    }

    private float bodyYaw;

    private void LateUpdate()
    {
        if ((faceCameraDirection || !IsThirdPerson()))
        {
            currentLookingYaw += (cameraYaw - currentLookingYaw) * Time.deltaTime * (aimingWeapon ? 20 : 4);

            bodyYaw += (currentBodyYaw - bodyYaw) * Time.deltaTime * 16;
            var angleDifference = currentLookingYaw - bodyYaw;

            if(!IsMoving())
            {
                // Standard animation

                if (torsoObject)
                {
                    torsoObject.transform.rotation = Quaternion.Euler(0, currentLookingYaw - (angleDifference * 0.5f), 0);
                }

                if (headObject)
                {
                    headObject.transform.localRotation = Quaternion.Euler(cameraPitch % 360, (angleDifference * 0.5f) % 360, 0);
                }
            }
        }
        else
        {
            currentLookingYaw = cameraYaw;
        }
    }

    private void FixedUpdate()
    {        
        HandleVelocity();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsGrounded()) { return; }
        // Step up the player onto a higher surface infront of them

        var normal = collision.contacts[0].normal;
        if(Mathf.Abs(normal.x) < 0.3f && Mathf.Abs(normal.z) < 0.3f) { return; }

        var startPosition = transform.position - new Vector3(0, (capsuleCollider.height * 0.5f), 0) + new Vector3(0, stepUpHeight, 0);
        var offsetPosition = startPosition - normal * capsuleCollider.radius * 1.5f;

        if(!Physics.Linecast(startPosition, offsetPosition))
        {
            RaycastHit hit;

            if (Physics.Raycast(offsetPosition, -Vector3.up, out hit, stepUpHeight))
            {
                if(hit.point.y + (capsuleCollider.height * 0.5f) > transform.position.y)
                {
                    transform.position = new Vector3(transform.position.x, hit.point.y + (capsuleCollider.height * 0.5f), transform.position.z);
                }                
            }
        }
    }

    public void HandleAnimator()
    {
        if(animator == null) { return; }

        if (!IsGrounded())
        {
            animator.SetBool("Falling", true);
            return;
        }
        else
        {
            animator.SetBool("Falling", false);
        }

        var forward = lastVisualDirection;
        var right = Vector3.Cross(forward, Vector3.up);
        var velocity = averageVelocity; // (transform.position - lastPosition) / Time.deltaTime;
        velocity.y = 0; 

        var forwardDot = Vector3.Dot(forward.normalized, velocity.normalized);
        var rightDot = Vector3.Dot(right.normalized, velocity.normalized);

        var currentSpeed = Mathf.Clamp(velocity.magnitude / speed, -1, 1);
        forwardDot *= currentSpeed;// * 1.2f;
        rightDot *= currentSpeed * 1.2f;

        if (IsSprinting())
        {
            forwardDot *= 2;
        }

        animator.SetFloat("VelX", Mathf.Clamp(-rightDot, -1, 1));
        animator.SetFloat("VelY", Mathf.Clamp(forwardDot, -1, 2));

        var characterObject = animator.gameObject;
        if (!aimingWeapon)
        {
            characterObject.transform.localPosition = Vector3.zero;
            characterObject.transform.localRotation = Quaternion.identity;
        }
    }

    public void HandleVelocity()
    {
        currentSpeed.x = moveX; // Mathf.MoveTowards(currentSpeed.x, moveX, Time.fixedDeltaTime * acceleration);
        currentSpeed.y = moveY; // Mathf.MoveTowards(currentSpeed.y, moveY, Time.fixedDeltaTime * acceleration);

        var speedX = currentSpeed.x;
        var speedY = currentSpeed.y;

        var velocity = GetMoveDirection() * speedY;
        velocity += Vector3.Cross(GetMoveDirection(), Vector3.up) * -speedX;
        velocity.Normalize();
        velocity *= GetTargetSpeed();

        var difference = velocity - rb.velocity;
        difference.y = 0;

        rb.AddForce(difference, ForceMode.VelocityChange);
    }

    public void CalculateAverageVelocity()
    {
        velocitySamples[velocitySampleIndex++] = rb.velocity;
        if (velocitySampleIndex >= averageVelocitySamples)
        {
            velocitySampleIndex = 0;
        }
        averageVelocity = Vector3.zero;
        foreach (var sample in velocitySamples)
        {
            averageVelocity += sample;
        }
        averageVelocity /= averageVelocitySamples;
    }

    public bool IsSprinting()
    {
        return Input.GetKey(KeyCode.LeftShift) && !aimingWeapon;
    }

    public float GetTargetSpeed()
    {
        return !aimingWeapon && IsSprinting() ? sprintSpeed : (walking ? walkingSpeed : speed);
    }

    public float GetCameraPitchOffset()
    {
        return recoilAmount * -recoil;
    }

    public void Recoil(float scale = 1)
    {
        var currentRecoil = GetCameraPitchOffset();
        cameraPitch += currentRecoil;
        currentRecoil = 0;
        targetRecoil = scale;
    }

    public void PlayHitmarkerSound()
    {
        if (hitMarkerSource)
        {
            hitMarkerSource.Play();
        }
    }

    public void ToggleFirstPerson()
    {
        thirdPerson = !thirdPerson;

        if (thirdPerson)
        {
            if(cameraMode == PlayerCameraModes.FIRST_ONLY)
            {
                thirdPerson = false;
            }
        }
        else
        {
            if (cameraMode == PlayerCameraModes.THIRD_ONLY)
            {
                thirdPerson = true;
            }
        }
    }

    public bool IsThirdPerson()
    {
        return false;
    }

    public Vector3 GetMoveDirection()
    {
        return Quaternion.Euler(0, cameraYaw, 0) * Vector3.forward;
    }

    public bool IsMoving()
    {
        return Mathf.Abs(rb.velocity.x) > 0.01f || Mathf.Abs(rb.velocity.y) > 0.01f;
    }

    public GameObject GetGroundedTo()
    {
        RaycastHit hit;
        if (Physics.BoxCast(transform.position, new Vector3(0.5f, 0.2f, 0.5f), -Vector3.up, out hit, Quaternion.identity, 1f))
        {
            return hit.transform.gameObject;
        }

        return null;
    }

    public bool IsGrounded()
    {
        return GetGroundedTo() != null;
    }

    public Vector3 GetBulletOrigin()
    {
        return fpsCameraPosition.transform.position;
    }

    public BulletTraceInfo GetBulletTrace(Vector3 offset)
    {
        var direction = GetCameraRotation(GetCameraPitchOffset()) * Vector3.forward;

        if(offset != Vector3.zero)
        {
            var forwardQ = Quaternion.LookRotation(direction);
            forwardQ *= Quaternion.Euler(offset);
            direction = forwardQ * Vector3.forward;
        }

        BulletTraceInfo info;
        info.direction = direction;
        info.hitObject = null;
        info.rigidbody = null;

        if (IsThirdPerson())
        {
            info.start = GetThirdPersonCameraPosition();
        }
        else
        {
            info.start = fpsCameraPosition.transform.position;
        }

        RaycastHit hit;
        if(Physics.Raycast(info.start, direction, out hit, 500, ~0))
        {
            info.end = hit.point;
            info.hitObject = hit.collider.gameObject;
            info.rigidbody = hit.rigidbody;
        }
        else
        {
            info.end = info.start + (direction * 500);
        }

        return info;
    }

    public Vector3 GetThirdPersonCameraPosition()
    {
        Vector3 offset;

        if (aimingWeapon)
        {
            offset = thirdPersonAimingOffset;
        }
        else
        {
            offset = thirdPersonCameraOffset;
        }

        var cameraRotation = GetCameraRotation(GetCameraPitchOffset());

        var offsetPos = Vector3.zero;
        offsetPos += (cameraRotation * Vector3.forward) * offset.x;
        offsetPos += (cameraRotation * Vector3.up) * offset.y;
        offsetPos += (cameraRotation * Vector3.right) * offset.z;

        return transform.position - ((cameraRotation * Vector3.forward) * currentCameraDistance) + offsetPos;
    }

    public Quaternion GetCameraRotation(float pitchOffset = 0)
    {
        var rotQ = Quaternion.Euler(cameraPitch + ((IsThirdPerson()) ? currentThirdPersonCameraPitchOffset : 0) + pitchOffset, cameraYaw, 0);

        return rotQ;
    }
}

public struct BulletTraceInfo
{
    public Vector3 start;
    public Vector3 end;
    public Vector3 direction;
    public GameObject hitObject;
    public Rigidbody rigidbody;
}

[System.Serializable]
public class PlayerControllerEmissiveItem
{
    public Renderer renderer;
    public float intensity;
}