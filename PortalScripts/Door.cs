using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Door : MonoBehaviour
{

    public Animation m_DoorAnimation;
    public AnimationClip m_OpenDoorAnimationClip;
    public AnimationClip m_CloseDoorAnimationClip;
    public AnimationClip m_StayOpenAnimationClip;
    public AnimationClip m_StayClosedAnimationClip;

    private void Start()
    {
    }

    void Update()
    {
    }

    public void OpenDoor()
    {
        m_DoorAnimation.CrossFade(m_OpenDoorAnimationClip.name, 0.1f);
        m_DoorAnimation.CrossFadeQueued(m_StayOpenAnimationClip.name, 0.1f);
    }

    public void CloseDoor()
    {
        //m_DoorAnimation.CrossFade(m_CloseDoorAnimationClip.name, 0.1f);
        m_DoorAnimation.CrossFade(m_StayClosedAnimationClip.name, 0.1f);
    }
}
