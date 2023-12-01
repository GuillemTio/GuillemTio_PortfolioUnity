using UnityEngine;

public class LaserCreator : MonoBehaviour
{
    public LineRenderer m_LineRenderer;
    public float m_MaxLaserDistance;
    public LayerMask m_LayerMask;

    void Start()
    {

    }

    private void Update()
    {
        Ray l_Ray = new Ray(m_LineRenderer.transform.position, m_LineRenderer.transform.forward);
        float l_MaxDistance = m_MaxLaserDistance;
        RaycastHit l_RaycastHit;

        if (Physics.Raycast(l_Ray, out l_RaycastHit, m_MaxLaserDistance, m_LayerMask.value))
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
    }
}
