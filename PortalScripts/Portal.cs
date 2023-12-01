using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Camera m_Camera;
    public Portal m_MirrorPortal;
    public Transform m_OtherPortal;
    FPSController m_FPSController;
    public float m_OffsetNearPlane = 0.1f;
    public List<Transform> m_ValidPoints;
    public float m_MinDistanceToValidPoint;
    public float m_MaxDistanceToValidPoint;
    public float m_ValidPointOffset = 0.1f;
    public float m_MinValidDotAngle = 0.95f;

    public LineRenderer m_LineRenderer;
    bool m_Reflected = false;
    public float m_MaxLaserDistance;
    public LayerMask m_LayerMask;

    public GameObject m_Window;

    void Start()
    {
        m_FPSController = GameController.GetGameController().m_Player;
        m_LineRenderer.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!m_MirrorPortal.m_Reflected)
        {
            m_MirrorPortal.m_LineRenderer.gameObject.SetActive(false);
        }
        if (!m_MirrorPortal.isActiveAndEnabled)        
            m_Window.SetActive(false);
        
        else
            m_Window.SetActive(true);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 l_LocalPosition = m_OtherPortal.InverseTransformPoint(m_FPSController.m_Camera.transform.position);
        Vector3 l_WorldPosition = m_MirrorPortal.transform.TransformPoint(l_LocalPosition);
        m_MirrorPortal.m_Camera.transform.position = l_WorldPosition;
        Vector3 l_LocalDirection = m_OtherPortal.InverseTransformDirection(m_FPSController.m_Camera.transform.forward);
        Vector3 l_WorldDirection = m_MirrorPortal.transform.TransformDirection(l_LocalDirection);
        m_MirrorPortal.m_Camera.transform.forward = l_WorldDirection;
        float l_Distance = Vector3.Distance(l_WorldPosition, m_MirrorPortal.transform.position) + m_OffsetNearPlane;
        m_MirrorPortal.m_Camera.nearClipPlane = l_Distance;

        m_MirrorPortal.m_Reflected = false;
    }

    public bool IsValidPosition(Vector3 ShootPosition, Vector3 Position, Vector3 Normal, LayerMask _LayerMask, float PortalScale)
    {
        Vector3 l_OldPortalSize = transform.localScale;
        transform.localScale = new Vector3(PortalScale, PortalScale, PortalScale);
        transform.position = Position;
        transform.rotation = Quaternion.LookRotation(Normal);
        bool l_IsValid = true;
        for (int i = 0; i < m_ValidPoints.Count; i++)
        {
            Vector3 l_Direction = m_ValidPoints[i].position - ShootPosition;
            float l_Distance = l_Direction.magnitude;
            l_Direction.Normalize();
            Ray l_Ray = new Ray(ShootPosition, l_Direction);
            RaycastHit l_RaycastHit;
            if (Physics.Raycast(l_Ray, out l_RaycastHit, l_Distance + m_ValidPointOffset, _LayerMask.value))
            {
                if (l_RaycastHit.collider.tag == "Drawable")
                {
                    float l_DistanceToHit = Vector3.Distance(m_ValidPoints[i].position, l_RaycastHit.point);
                    if (l_DistanceToHit >= m_MinDistanceToValidPoint && l_DistanceToHit <= m_MaxDistanceToValidPoint)
                    {
                        float l_DotAngle = Vector3.Dot(Normal, l_RaycastHit.normal);
                        if (l_DotAngle < m_MinValidDotAngle)
                        {
                            l_IsValid = false;
                        }
                    }
                    else
                    {
                        l_IsValid = false;
                    }
                }
                else
                {
                    l_IsValid = false;
                }
            }
            else
            {
                l_IsValid = false;
            }

        }

        transform.localScale = l_OldPortalSize;
        return l_IsValid;
    }

    internal bool IsValidPlaceholderPosition(Vector3 ShootPosition, Vector3 Position, Vector3 Normal, LayerMask _LayerMask, float PortalScale)
    {
        Vector3 l_OldPortalSize = transform.localScale;
        Vector3 l_OldPosition = transform.position;
        Quaternion l_OldRotation = transform.rotation;
        transform.localScale = new Vector3(PortalScale, PortalScale, PortalScale);
        transform.position = Position;
        transform.rotation = Quaternion.LookRotation(Normal);
        bool l_IsValid = true;
        for (int i = 0; i < m_ValidPoints.Count; i++)
        {
            Vector3 l_Direction = m_ValidPoints[i].position - ShootPosition;
            float l_Distance = l_Direction.magnitude;
            l_Direction.Normalize();
            Ray l_Ray = new Ray(ShootPosition, l_Direction);
            RaycastHit l_RaycastHit;
            if (Physics.Raycast(l_Ray, out l_RaycastHit, l_Distance + m_ValidPointOffset, _LayerMask.value))
            {
                if (l_RaycastHit.collider.tag == "Drawable")
                {
                    float l_DistanceToHit = Vector3.Distance(m_ValidPoints[i].position, l_RaycastHit.point);
                    if (l_DistanceToHit >= m_MinDistanceToValidPoint && l_DistanceToHit <= m_MaxDistanceToValidPoint)
                    {
                        float l_DotAngle = Vector3.Dot(Normal, l_RaycastHit.normal);
                        if (l_DotAngle < m_MinValidDotAngle)
                        {
                            l_IsValid = false;
                        }
                    }
                    else
                    {
                        l_IsValid = false;
                    }
                }
                else
                {
                    l_IsValid = false;
                }
            }
            else
            {
                l_IsValid = false;
            }

        }
        transform.position = l_OldPosition;
        transform.rotation = l_OldRotation;
        transform.localScale = l_OldPortalSize;
        return l_IsValid;
    }

    public void Reflect(Vector3 _Point, Quaternion _Rotation)
    {
        if (m_MirrorPortal.m_Reflected)
            return;
        m_MirrorPortal.m_Reflected = true;

        Vector3 l_LocalPoint = transform.InverseTransformPoint(_Point);
        l_LocalPoint.x = -l_LocalPoint.x;
        Vector3 l_WorldPoint = m_MirrorPortal.transform.TransformPoint(l_LocalPoint);

        Vector3 LineDirection = new Vector3(0, _Rotation.eulerAngles.y - m_MirrorPortal.transform.rotation.eulerAngles.y, 0);

        if ((int)Vector3.Dot(transform.rotation.eulerAngles, m_MirrorPortal.transform.rotation.eulerAngles)== 0)
            LineDirection.y -= 90;

        if (LineDirection.y > 90)
            LineDirection.y -= 180;
        if (LineDirection.y <= -90)
            LineDirection.y += 180;

        m_MirrorPortal.m_LineRenderer.transform.localRotation = Quaternion.Euler(LineDirection);

        Ray l_Ray = new Ray(l_WorldPoint, m_MirrorPortal.m_LineRenderer.transform.forward);

        float l_MaxDistance = m_MaxLaserDistance;
        RaycastHit l_RaycastHit;

        if (Physics.Raycast(l_Ray, out l_RaycastHit, m_MaxLaserDistance, m_LayerMask.value))
        {
            l_MaxDistance = Vector3.Distance(l_WorldPoint, l_RaycastHit.point);

            if (l_RaycastHit.collider.tag == "RefractionCube")
                l_RaycastHit.collider.GetComponent<RefractionCube>().Reflect();

            else if (l_RaycastHit.collider.tag == "Player")
                GameController.GetGameController().RestartLevel();

            else if (l_RaycastHit.collider.tag == "Turret")
                l_RaycastHit.collider.gameObject.SetActive(false);

            else if (l_RaycastHit.collider.tag == "LaserReceiver")
                l_RaycastHit.collider.gameObject.GetComponent<LaserReceiver>().SetRayReceiver();
        }

        m_MirrorPortal.m_LineRenderer.SetPosition(1, new Vector3(0, 0, l_MaxDistance));

        m_MirrorPortal.m_LineRenderer.transform.position = l_WorldPoint;
        m_MirrorPortal.m_LineRenderer.gameObject.SetActive(true);

    }
}
