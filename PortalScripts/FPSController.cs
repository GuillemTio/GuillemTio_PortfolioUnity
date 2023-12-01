using System;
using UnityEngine;
public class FPSController : MonoBehaviour
{
    float m_Yaw;
    float m_Pitch;
    float m_VerticalSpeed;
    public Transform m_PitchController;

    public float m_YawSpeed;

    public float m_PitchSpeed;
    public bool m_YawInverted;
    public bool m_PitchInverted;
    public float m_MinPitch;
    public float m_MaxPitch;
    public float m_Speed;
    public float m_SprintSpeed;
    public float m_JumpSpeed;

    int m_Life = 100;

    public float m_PortalOffset = 0.8f;
    Vector3 m_MovementDirection;
    [Range(0.0f, 1.0f)]
    public float m_DotTraversePortal = 0.42f;

    Vector3 m_StartPosition;
    Quaternion m_StartRotation;

    [Header("Animation")]

    public Animation m_WeaponAnimation;
    public AnimationClip m_ShootAnimationClip;
    public AnimationClip m_IdleAnimationClip;

    public Camera m_Camera;

    CharacterController m_CharacterController;

    [Header("Shoot")]

    public float m_MaxShootDistance;
    public LayerMask m_LayerMask;

    [Header("Portals")]

    public Portal m_BluePortal;
    public Portal m_OrangePortal;
    float m_PortalSize = 1;
    public float m_MaxPortalSize = 1;
    public float m_MinPortalSize = 0.1f;
    private GameObject m_PortalPlaceholder;
    public GameObject m_PortalPlaceholderPrefab;


    [Header("Input")]

    public KeyCode m_LeftKeyCode = KeyCode.A;
    public KeyCode m_RightKeyCode = KeyCode.D;
    public KeyCode m_UpKeyCode = KeyCode.W;
    public KeyCode m_DownKeyCode = KeyCode.S;
    public KeyCode m_JumpKeyCode = KeyCode.Space;
    public KeyCode m_SprintKeyCode = KeyCode.LeftShift;
    public KeyCode m_InteractKeyCode = KeyCode.E;
    public int m_BluePortalShootMouseButton = 0;
    public int m_OrangePortalShootMouseButton = 1;

    [Header("DebugInput")]

    bool m_AngleLocked = false;
    bool m_AimLocked = true;
    public KeyCode m_DebugLockAngleKeyCode = KeyCode.I;
    public KeyCode m_DebugLockKeyCode = KeyCode.O;

    [Header("AttachObjects")]

    bool m_AttachedObject = false;
    public Transform m_AttachingPosition;
    Rigidbody m_ObjectAttached;
    Quaternion m_AttachingObjectStartRotation;
    public float m_AttachingObjectSpeed = 2f;
    public float m_MaxDistanceToInteractObject = 25f;
    public LayerMask m_InteractObjectLayerMask;
    public float m_ShootAttachedObjectForce = 20f;

    private void Awake()
    {

        m_CharacterController = GetComponent<CharacterController>();
        if (GameController.GetGameController().m_Player == null)
        {
            GameController.GetGameController().m_Player = this;
            GameObject.DontDestroyOnLoad(gameObject);
            m_StartPosition = transform.position;
            m_StartRotation = transform.rotation;

            m_Yaw = transform.rotation.eulerAngles.y;
        }
        else
        {
            GameController.GetGameController().m_Player.SetStartPosition(transform);
            GameObject.Destroy(this.gameObject);
        }

    }


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        m_PortalPlaceholder = GameObject.Instantiate(m_PortalPlaceholderPrefab);
        m_PortalPlaceholder.SetActive(false);
        SetIdleWeaponAnimation();
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(m_DebugLockAngleKeyCode))
            m_AngleLocked = !m_AngleLocked;
        if (Input.GetKeyDown(m_DebugLockKeyCode))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
            m_AimLocked = Cursor.lockState == CursorLockMode.Locked;
        }
#endif


        float l_HorizontalMovement = Input.GetAxis("Mouse X");
        float l_VerticalMovement = Input.GetAxis("Mouse Y");

        if (m_AngleLocked)
        {
            l_HorizontalMovement = 0f;
            l_VerticalMovement = 0f;
        }



        float l_Speed = m_Speed;

        if (Input.GetKeyDown(m_JumpKeyCode) && m_VerticalSpeed == 0)
        {
            m_VerticalSpeed = m_JumpSpeed;
        }

        if (Input.GetKey(m_SprintKeyCode))
        {
            l_Speed = m_SprintSpeed;
        }
        /*
        float l_YawInverted = 1f;
        float l_PitchInverted = 1f;

        if (m_YawInverted)
        {
            l_YawInverted = -1.0f;
        }
        if (m_PitchInverted)
        {
            l_PitchInverted = -1.0f;
        }*/

        float l_YawInverted = m_YawInverted ? -1f : 1f;
        float l_PitchInverted = m_PitchInverted ? -1f : 1f;

        m_Yaw = m_Yaw + m_YawSpeed * l_HorizontalMovement * Time.deltaTime * l_YawInverted;
        m_Pitch = m_Pitch + m_PitchSpeed * l_VerticalMovement * Time.deltaTime * l_PitchInverted;
        m_Pitch = Mathf.Clamp(m_Pitch, m_MinPitch, m_MaxPitch);

        transform.rotation = Quaternion.Euler(0.0f, m_Yaw, 0.0f);
        m_PitchController.localRotation = Quaternion.Euler(m_Pitch, 0.0f, 0.0f);

        float l_YawInRadians = m_Yaw * Mathf.Deg2Rad;
        float l_Yaw90InRadians = (m_Yaw + 90) * Mathf.Deg2Rad;

        Vector3 l_Forward = new Vector3(Mathf.Sin(l_YawInRadians), 0, Mathf.Cos(l_YawInRadians));
        Vector3 l_Right = new Vector3(Mathf.Sin(l_Yaw90InRadians), 0, Mathf.Cos(l_Yaw90InRadians));

        Vector3 l_Movement = Vector3.zero;

        if (Input.GetKey(m_LeftKeyCode))
        {
            l_Movement = -l_Right;
        }
        else if (Input.GetKey(m_RightKeyCode))
        {
            l_Movement = l_Right;
        }

        if (Input.GetKey(m_UpKeyCode))
        {
            l_Movement += l_Forward;
        }
        else if (Input.GetKey(m_DownKeyCode))
        {
            l_Movement -= l_Forward;
        }

        l_Movement.Normalize();
        m_MovementDirection = l_Movement;

        l_Movement *= l_Speed * Time.deltaTime;

        m_VerticalSpeed = m_VerticalSpeed + Physics.gravity.y * Time.deltaTime;
        l_Movement.y = m_VerticalSpeed * Time.deltaTime;

        CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Movement);
        if ((l_CollisionFlags & CollisionFlags.CollidedBelow) != 0)
            m_VerticalSpeed = 0f;
        if ((l_CollisionFlags & CollisionFlags.CollidedBelow) != 0 && m_VerticalSpeed > 0f)
            m_VerticalSpeed = 0f;

        //m_CharacterController.Move(l_Movement);

        if (CanShoot())
        {
            if (Input.GetMouseButton(m_BluePortalShootMouseButton))
                UpdatePortalPosition(m_BluePortal);
            if (Input.GetMouseButton(m_OrangePortalShootMouseButton))
                UpdatePortalPosition(m_OrangePortal);

            if (Input.GetMouseButtonUp(m_BluePortalShootMouseButton))
                Shoot(m_BluePortal);
            if (Input.GetMouseButtonUp(m_OrangePortalShootMouseButton))
                Shoot(m_OrangePortal);
        }
        if (m_ObjectAttached != null)
        {
            UpdateAttachedObject();
            if (Input.GetKeyDown(m_InteractKeyCode) && m_ObjectAttached)
            {
                ReleaseAttachedObject(0.0f);
            }
            if (Input.GetMouseButtonUp(m_BluePortalShootMouseButton) && m_ObjectAttached)
            {
                ReleaseAttachedObject(m_ShootAttachedObjectForce);
            }
        }
        else if (m_ObjectAttached == null && Input.GetKeyDown(m_InteractKeyCode))
            InteractObject();


        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            m_PortalSize += 0.1f;
            if (m_PortalSize >= m_MaxPortalSize)
                m_PortalSize = m_MaxPortalSize;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            m_PortalSize -= 0.1f;
            if (m_PortalSize <= m_MinPortalSize)
                m_PortalSize = m_MinPortalSize;
        }
    }

    private void ReleaseAttachedObject(float Force)
    {
        m_ObjectAttached.transform.SetParent(null);
        m_ObjectAttached.isKinematic = false;
        m_ObjectAttached.velocity = Force * m_Camera.transform.forward;
        if (m_ObjectAttached.GetComponent<CompanionCube>())
            m_ObjectAttached.GetComponent<CompanionCube>().SetAttached(false);
        else if (m_ObjectAttached.GetComponent<RefractionCube>())
            m_ObjectAttached.GetComponent<RefractionCube>().SetAttached(false);
        else if (m_ObjectAttached.GetComponent<Turret>())
            m_ObjectAttached.GetComponent<Turret>().SetAttached(false);
        m_ObjectAttached = null;
        m_AttachedObject = false;
    }

    public void ResetAttachedObject()
    {
        m_ObjectAttached = null;
        m_AttachedObject = false;
    }

    private void InteractObject()
    {
        Ray l_Ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(l_Ray, out RaycastHit l_RaycastHit, m_MaxDistanceToInteractObject, m_InteractObjectLayerMask.value))
        {
            if (l_RaycastHit.collider.tag == "CubeSpawnerButton")
            {
                l_RaycastHit.transform.gameObject.GetComponent<SpawnerCompanionCube>().Spawn();
            }
            if (l_RaycastHit.collider.tag == "Turret")
            {
                m_ObjectAttached = l_RaycastHit.rigidbody;
                m_ObjectAttached.isKinematic = true;
                m_ObjectAttached.GetComponent<Turret>().SetAttached(true);
                m_AttachedObject = true;
                m_AttachingObjectStartRotation = l_RaycastHit.collider.transform.rotation;
            }
            if (l_RaycastHit.collider.tag == "CompanionCube")
            {
                m_ObjectAttached = l_RaycastHit.rigidbody;
                m_ObjectAttached.isKinematic = true;
                m_ObjectAttached.GetComponent<CompanionCube>().SetAttached(true);
                m_AttachedObject = true;
                m_AttachingObjectStartRotation = l_RaycastHit.collider.transform.rotation;
            }
            if (l_RaycastHit.collider.tag == "RefractionCube")
            {
                m_ObjectAttached = l_RaycastHit.rigidbody;
                m_ObjectAttached.isKinematic = true;
                m_ObjectAttached.GetComponent<RefractionCube>().SetAttached(true);
                m_AttachedObject = true;
                m_AttachingObjectStartRotation = l_RaycastHit.collider.transform.rotation;
            }
        }
    }

    void UpdateAttachedObject()
    {
        Vector3 l_EulerAngles = m_AttachingPosition.rotation.eulerAngles;
        if (!m_AttachedObject)
        {
            Vector3 l_Direction = m_AttachingPosition.transform.position - m_ObjectAttached.transform.position;
            float l_Distance = l_Direction.magnitude;
            float l_Movement = m_AttachingObjectSpeed * Time.deltaTime;
            if (l_Movement >= l_Distance)
            {
                m_AttachedObject = true;
                m_ObjectAttached.MovePosition(m_AttachingPosition.position);
                m_ObjectAttached.MoveRotation(Quaternion.Euler(0.0f, l_EulerAngles.y, l_EulerAngles.z));
                m_ObjectAttached.transform.SetParent(m_AttachingPosition);
            }
            else
            {
                l_Direction /= l_Distance;
                m_ObjectAttached.MovePosition(m_ObjectAttached.transform.position + l_Direction * l_Movement);
                m_ObjectAttached.MoveRotation(Quaternion.Lerp(m_AttachingObjectStartRotation, Quaternion.Euler(0.0f, l_EulerAngles.y, l_EulerAngles.z), 1.0f - Mathf.Min(l_Distance / 1.5f, 1.0f)));
            }
        }
        else
        {
            m_ObjectAttached.MoveRotation(Quaternion.Euler(0.0f, l_EulerAngles.y, l_EulerAngles.z));
            m_ObjectAttached.MovePosition(m_AttachingPosition.position);
        }
    }


    bool CanShoot()
    {
        return m_AttachedObject == false;
    }

    private void Shoot(Portal _Portal)
    {
        m_PortalPlaceholder.SetActive(false);

        SetShootWeaponAnimation();
        Ray l_Ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit l_RaycastHit;
        if (Physics.Raycast(l_Ray, out l_RaycastHit, m_MaxShootDistance, m_LayerMask.value))
        {

            if (_Portal.IsValidPosition(m_Camera.transform.position, l_RaycastHit.point, l_RaycastHit.normal, m_LayerMask, m_PortalSize))
            {
                _Portal.transform.localScale = new Vector3(m_PortalSize, m_PortalSize, m_PortalSize);
                _Portal.gameObject.SetActive(true);
            }
            else
            {
                _Portal.m_MirrorPortal.m_LineRenderer.gameObject.SetActive(false);
                _Portal.gameObject.SetActive(false);
            }

        }
    }

    void SetIdleWeaponAnimation()
    {
        //m_WeaponAnimation.CrossFade(m_IdleAnimationClip.name);
    }

    void SetShootWeaponAnimation()
    {
        //m_WeaponAnimation.CrossFade(m_ShootAnimationClip.name,0.1f);
        //m_WeaponAnimation.CrossFadeQueued(m_IdleAnimationClip.name,0.1f);
    }

    public void RestartLevel()
    {
        m_CharacterController.enabled = false;
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
        m_Yaw = transform.rotation.eulerAngles.y;
        m_Pitch = 0.0f;
        m_BluePortal.gameObject.SetActive(false);
        m_OrangePortal.gameObject.SetActive(false);
        m_CharacterController.enabled = true;
    }
    void SetStartPosition(Transform startTransform)
    {
        m_StartPosition = startTransform.position;
        m_StartRotation = startTransform.rotation;
        m_CharacterController.enabled = false;
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
        m_Yaw = transform.rotation.eulerAngles.y;
        m_Pitch = 0.0f;
        m_CharacterController.enabled = true;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            //Item l_Item = other.GetComponent<Item>();
            //if (l_Item.CanPick()) l_Item.Pick();
        }
        else if (other.tag == "DeadZone")
        {
            Kill();
        }
        else if (other.tag == "Portal")
        {
            Portal l_Portal = other.gameObject.GetComponent<Portal>();
            if (CanTeleport(l_Portal))
                Teleport(l_Portal);


        }
    }

    void Kill()
    {
        m_Life = 0;
        //it should start after a while
        GameController.GetGameController().RestartLevel();
    }
    bool CanTeleport(Portal _Portal)
    {
        float l_DotAngle = Vector3.Dot(_Portal.m_OtherPortal.forward, m_MovementDirection);
        return (l_DotAngle > m_DotTraversePortal) && (m_BluePortal.isActiveAndEnabled && m_OrangePortal.isActiveAndEnabled);
    }
    void Teleport(Portal _Portal)
    {
        Vector3 l_LocalPosition = _Portal.m_OtherPortal.InverseTransformPoint(transform.position);
        Vector3 l_WorldPosition = _Portal.m_MirrorPortal.transform.TransformPoint(l_LocalPosition);
        Vector3 l_LocalMovement = _Portal.m_OtherPortal.InverseTransformDirection(m_MovementDirection);
        Vector3 l_WorldMovement = _Portal.m_MirrorPortal.transform.TransformDirection(l_LocalMovement);
        m_CharacterController.enabled = false;
        transform.position = l_WorldPosition + l_WorldMovement * m_PortalOffset;
        if (transform.position.y < 0)
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }

        Vector3 l_Forward = transform.forward;
        l_Forward.y = 0f;
        l_Forward.Normalize();
        Vector3 l_LocalForward = _Portal.m_OtherPortal.InverseTransformDirection(l_Forward);
        Vector3 l_WorldForward = _Portal.m_MirrorPortal.transform.TransformDirection(l_LocalForward);

        m_Yaw = Mathf.Atan2(l_WorldForward.x, l_WorldForward.z) * Mathf.Rad2Deg;

        transform.localScale = _Portal.m_MirrorPortal.transform.localScale;

        m_CharacterController.enabled = true;
    }

    void UpdatePortalPosition(Portal _Portal)
    {
        m_PortalPlaceholder.transform.localScale = new Vector3(m_PortalSize, m_PortalSize, m_PortalSize);
        m_PortalPlaceholder.SetActive(true);

        Ray l_Ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit l_RaycastHit;
        if (Physics.Raycast(l_Ray, out l_RaycastHit, m_MaxShootDistance, m_LayerMask.value))
        {
            //Debug.Log(_Portal.IsValidPosition(m_Camera.transform.position, l_RaycastHit.point, l_RaycastHit.normal, m_LayerMask));
            if (_Portal.IsValidPlaceholderPosition(m_Camera.transform.position, l_RaycastHit.point, l_RaycastHit.normal, m_LayerMask, m_PortalSize))
            {
                m_PortalPlaceholder.transform.position = l_RaycastHit.point;
                m_PortalPlaceholder.transform.rotation = Quaternion.LookRotation(l_RaycastHit.normal);
            }
            else
                m_PortalPlaceholder.SetActive(false);

        }
        else
            m_PortalPlaceholder.SetActive(false);

    }
}
