using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefractionCube : MonoBehaviour
{
    public LineRenderer m_LineRenderer;
    bool m_Reflected = false;
    public float m_MaxLaserDistance;
    public LayerMask m_LayerMask;
    bool m_IsAttached = false;

    [Range(0f, 1f)]
    public float m_DotTraversePortal = 0.42f;
    Rigidbody m_Rigidbody;
    public float m_PortalOffset = 0.8f;

    Vector3 m_StartPosition;
    Quaternion m_StartRotation;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
    }
    void Start()
    {
        m_LineRenderer.gameObject.SetActive(false);
        GameController.GetGameController().AddRefractionCube(this);
    }

    private void Update()
    {
        if (!m_Reflected)
        {
            m_LineRenderer.gameObject.SetActive(false);
        }
    }
    public void LateUpdate()
    {
        m_Reflected = false;
    }

    public void Reflect()
    {
        if (m_Reflected)
            return;
        m_Reflected = true;
        Ray l_Ray = new Ray(m_LineRenderer.transform.position, m_LineRenderer.transform.forward);
        float l_MaxDistance = m_MaxLaserDistance;
        RaycastHit l_RaycastHit;
        if(Physics.Raycast(l_Ray,out l_RaycastHit, m_MaxLaserDistance, m_LayerMask.value))
        {
            l_MaxDistance = Vector3.Distance(m_LineRenderer.transform.position, l_RaycastHit.point);
            if (l_RaycastHit.collider.tag == "RefractionCube")
                l_RaycastHit.collider.GetComponent<RefractionCube>().Reflect();

            else if (l_RaycastHit.collider.tag == "Portal" && l_RaycastHit.collider.gameObject.GetComponent<Portal>().m_MirrorPortal.isActiveAndEnabled)
                l_RaycastHit.collider.GetComponent<Portal>().Reflect(l_RaycastHit.point, m_LineRenderer.transform.rotation);

            else if (l_RaycastHit.collider.tag == "Player")
                GameController.GetGameController().RestartLevel();

            else if (l_RaycastHit.collider.tag == "Turret")
                l_RaycastHit.collider.gameObject.SetActive(false);

            else if (l_RaycastHit.collider.tag == "LaserReceiver")
            l_RaycastHit.collider.gameObject.GetComponent<LaserReceiver>().SetRayReceiver();
        }
        m_LineRenderer.SetPosition(1, new Vector3(0, 0, l_MaxDistance));
        m_LineRenderer.gameObject.SetActive(true);

    }
    public void SetAttached(bool Attached)
    {
        m_IsAttached = Attached;
    }

    bool CanTeleport(Portal _Portal)
    {
        float l_DotAngle = Vector3.Dot(_Portal.m_OtherPortal.forward, m_Rigidbody.velocity);
        return l_DotAngle > m_DotTraversePortal;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Portal")
        {
            Portal l_Portal = other.gameObject.GetComponent<Portal>();
            if (CanTeleport(l_Portal))
                Teleport(l_Portal);
        }
    }

    private void Teleport(Portal _Portal)
    {
        Vector3 l_LocalPosition = _Portal.m_OtherPortal.InverseTransformPoint(transform.position);
        Vector3 l_WorldPosition = _Portal.m_MirrorPortal.transform.TransformPoint(l_LocalPosition);
        Vector3 l_LocalDirection = _Portal.m_OtherPortal.InverseTransformPoint(transform.forward);
        Vector3 l_WorldDirection = _Portal.m_MirrorPortal.transform.TransformPoint(l_LocalDirection);
        Vector3 l_LocalVelocity = _Portal.m_OtherPortal.InverseTransformDirection(m_Rigidbody.velocity);
        Vector3 l_WorldVelocity = _Portal.m_MirrorPortal.transform.TransformDirection(l_LocalVelocity);
        Vector3 l_ForwardVelocity = l_WorldVelocity;
        l_ForwardVelocity.Normalize();

        m_Rigidbody.isKinematic = true;
        transform.position = l_WorldPosition + l_ForwardVelocity * m_PortalOffset;
        transform.forward = l_WorldDirection;
        m_Rigidbody.isKinematic = false;
        m_Rigidbody.velocity = l_WorldVelocity;
    }

    public void RestartLevel()
    {
        gameObject.SetActive(true);
        m_Rigidbody.isKinematic = true;
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
        m_Rigidbody.isKinematic = false;
    }
}
