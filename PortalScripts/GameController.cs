using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{

    static GameController m_GameController = null;
    public GameObject m_DestroyObjects;
    public FPSController m_Player;

    List<Turret> m_Turrets;
    List<RefractionCube> m_RefractionCubes;
    //List<Enemy> m_Enemies;
    static bool m_AlreadyInitialized = false;

    static public GameController GetGameController()
    {
        if(m_GameController == null && !m_AlreadyInitialized)
        {
            GameObject l_GameObject = new GameObject("GameController");
            m_GameController = l_GameObject.AddComponent<GameController>();
            m_GameController.m_DestroyObjects = new GameObject("DestroyObjects");
            m_GameController.m_DestroyObjects.transform.SetParent(l_GameObject.transform);
            m_GameController.m_Turrets = new List<Turret>();
            m_GameController.m_RefractionCubes = new List<RefractionCube>();
            //m_GameController.m_Enemies = new List<Enemy>();
            GameController.DontDestroyOnLoad(l_GameObject);
            m_AlreadyInitialized = true;
        }
        return m_GameController;
    }

    public void RestartLevel()
    {
        m_Player.RestartLevel();
        //foreach (Enemy l_Enemy in m_Enemies)
        //{
        //    l_Enemy.RestartLevel();
        //}
        foreach (Turret l_Turret in m_Turrets)
        {
            l_Turret.RestartLevel();
        }
        foreach (RefractionCube l_RefractionCube in m_RefractionCubes)
        {
            l_RefractionCube.RestartLevel();
        }
        GameObject[] l_CompanionCubes = GameObject.FindGameObjectsWithTag("CompanionCube");
        foreach (GameObject _Cube in l_CompanionCubes)
        {
            Destroy(_Cube);
        }
        m_Player.ResetAttachedObject();
        GameObject[] l_Doors = GameObject.FindGameObjectsWithTag("Door");
        foreach (GameObject _Door in l_Doors)
        {
            _Door.GetComponent<Door>().CloseDoor();
        }
        DestroyLevelObjects();
    }

    void DestroyLevelObjects()
    {
        Transform[] l_Transforms = m_DestroyObjects.GetComponentsInChildren<Transform>();
        foreach (Transform l_Transform in l_Transforms)
        {
            if (l_Transform != m_DestroyObjects.transform)
            {
                GameObject.Destroy(l_Transform.gameObject);
            }
            
        }
    }

    private void Update()
    {
        
    }

    public void AddTurret(Turret _Turret)
    {
        m_Turrets.Add(_Turret);
    }

    public void AddRefractionCube(RefractionCube _Cube)
    {
        m_RefractionCubes.Add(_Cube);
    }

    public void GoToLevel1()
    {
        DestroyLevelObjects();
        SceneManager.LoadSceneAsync("Level1Scene");
    }
    public void GoToLevel2()
    {
        DestroyLevelObjects();
        SceneManager.LoadSceneAsync("Level2Scene");
    }
    public void GoToMenu()
    {
        DestroyLevelObjects();
        GameObject.Destroy(m_Player.gameObject);
        SceneManager.LoadSceneAsync("MainMenuScene");
    }

}
