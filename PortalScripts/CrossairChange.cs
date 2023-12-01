using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrossairChange : MonoBehaviour
{
    public Sprite m_SpriteBlank;
    public Sprite m_SpriteBlue;
    public Sprite m_SpriteOrange;
    public Sprite m_SpriteBlueOrange;
    private FPSController m_Player;
    private Image m_UIRenderer;

    // Start is called before the first frame update
    void Start()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player").GetComponent<FPSController>();
        m_UIRenderer = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_Player.m_BluePortal.isActiveAndEnabled && !m_Player.m_OrangePortal.isActiveAndEnabled)
            m_UIRenderer.sprite = m_SpriteBlank;
        else if (m_Player.m_BluePortal.isActiveAndEnabled && !m_Player.m_OrangePortal.isActiveAndEnabled)
            m_UIRenderer.sprite = m_SpriteBlue;
        else if (m_Player.m_OrangePortal.isActiveAndEnabled && !m_Player.m_BluePortal.isActiveAndEnabled)
            m_UIRenderer.sprite = m_SpriteOrange;
        else
            m_UIRenderer.sprite = m_SpriteBlueOrange;

    }
}
