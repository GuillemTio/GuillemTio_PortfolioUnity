using UnityEngine;

public class SpawnerCompanionCube : MonoBehaviour
{
    public Transform m_SpawnPosition;
    public GameObject m_CompanionCubePrefab;

    public void Spawn()
    {
        GameObject l_CompanionCube = GameObject.Instantiate(m_CompanionCubePrefab);
        l_CompanionCube.transform.position = m_SpawnPosition.position;
        l_CompanionCube.transform.rotation = m_SpawnPosition.rotation;
    }
}
