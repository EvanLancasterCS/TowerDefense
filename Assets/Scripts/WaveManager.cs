using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;
    public GameObject enemy; // need to add system for variations for enemies later
    public float buildTime = 15f;
    
    public enum Phase { wave, build };
    public Phase currentPhase = Phase.build;
    public bool playingGame = false;
    public bool building = false;

    private List<GameObject> currentWave = new List<GameObject>();

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        StartCoroutine(GameLoop());
    }

    private void SwitchPhase(Phase newPhase)
    {
        if (currentPhase != newPhase)
        {
            currentPhase = newPhase;
        }
    }

    public void UI_EndBuild()
    {
        building = false;
    }

    IEnumerator GameLoop()
    {
        yield return new WaitForSeconds(1f);


        int towerIndex = TowerManager.instance.CreateTower(0);
        BaseTower tower = TowerManager.instance.GetUnplacedTower(towerIndex);
        TowerManager.instance.PlaceTower(new Coordinate(0, 0), tower);

        currentPhase = Phase.build;
        playingGame = true;

        float time = 0;
        float elapsed = 0;
        while (playingGame)
        {
            // starting build phase
            building = true;
            UIManager.instance.BeginBuild();

            // give player towers
            for (int i = 0; i < 3; i++)
            {
                int index = TowerManager.instance.CreateTower(1);
                BaseTower t = TowerManager.instance.GetUnplacedTower(index);
                UIManager.instance.CreateCard(t);
            }

            time = Time.fixedTime;
            while (currentPhase == Phase.build)
            {
                // in build phase
                yield return null;
                //elapsed = Time.fixedTime - time;
                if(!building)//(elapsed >= buildTime)
                    SwitchPhase(Phase.wave);
                //print("Time Left In Build: " + (buildTime - elapsed));
            }

            // starting wave phase, placeholder generation
            float radius = 30f;
            int numEnemies = 50;
            for(int i = 0; i < numEnemies; i++)
            {
                float random = Random.Range(0, 2 * Mathf.PI);
                float x = Mathf.Cos(random) * radius;
                float z = Mathf.Sin(random) * radius;
                GameObject newenemy = Instantiate(enemy);
                newenemy.transform.position = new Vector3(x, 1, z);
                currentWave.Add(newenemy);
            }

            time = Time.fixedTime;
            while (currentPhase == Phase.wave)
            {
                // in wave phase
                yield return null;
                for (int i = currentWave.Count - 1; i >= 0; i--)
                    if (currentWave[i] == null)
                        currentWave.RemoveAt(i);

                elapsed = Time.fixedTime - time;
                if (currentWave.Count == 0)
                    SwitchPhase(Phase.build);
                //print("Time Left In Wave: " + (waveTime - elapsed) + ", Number of enemies: " + currentWave.Count);
            }
        }
    }

    void Update()
    {
        
    }
}
