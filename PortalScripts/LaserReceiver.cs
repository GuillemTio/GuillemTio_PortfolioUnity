using UnityEngine;
using UnityEngine.Events;

public class LaserReceiver : MonoBehaviour
{
    public UnityEvent m_OpenEvent;
    public UnityEvent m_CloseEvent;
    bool m_DoorIsOpen = false;
    bool m_IsReceivingRay;

    private void LateUpdate()
    {
        if (m_IsReceivingRay && !m_DoorIsOpen)
        {
            ActivateEvent(m_OpenEvent);
            m_DoorIsOpen = true;
            Debug.Log("OPEN");
        }
        else if (!m_IsReceivingRay && m_DoorIsOpen)
        {
            ActivateEvent(m_CloseEvent);
            m_DoorIsOpen = false;
            Debug.Log("CLOSE");
        }
        m_IsReceivingRay = false;
    }

    private void ActivateEvent(UnityEvent _Event)
    {
        _Event.Invoke();
    }

    public void SetRayReceiver()
    {
        m_IsReceivingRay = true;
    }
}
