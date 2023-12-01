using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionCube : MonoBehaviour
{
    [Range(0f, 1f)]
    public float m_DotTraversePortal = 0.42f;
    Rigidbody m_Rigidbody;
    public float m_PortalOffset = 0.8f;
    bool m_IsAttached = false;
    public bool m_IsInButton = false;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
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
        if (other.tag == "DeadTurretCubeZone")
        {
            gameObject.SetActive(false);
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
        float l_Scale = _Portal.m_MirrorPortal.transform.localScale.x / _Portal.transform.localScale.x;
        transform.localScale *= l_Scale;
        m_Rigidbody.isKinematic = false;
        m_Rigidbody.velocity = l_WorldVelocity;
    }

    public void SetAttached(bool Attached)
    {
        m_IsAttached = Attached;
    }
}
