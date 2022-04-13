using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;
    public GameObject enemy; // need to add system for variations for enemies later
    [SerializeField] private GameObject spawnShipPrefab;
    public float buildTime = 15f;
    
    public enum Phase { wave, build };
    public Phase currentPhase = Phase.build;
    public bool playingGame = false;
    public bool building = false;

    private float currentDifficulty = 0f;
    private float difficultyRate = 2f; // minutes per difficulty setting
    private float difficultyIncreasePerWave = 0.2f;
    [SerializeField] private int playerHealth = 10;

    private List<GameObject> currentWave = new List<GameObject>();
    private List<GameObject> currentShips = new List<GameObject>();

    private bool pathsClear = false;
    private int pathsCalculated = 0;

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

    public void PlayerTakeDamage(int amt)
    {
        playerHealth -= amt;
        UIManager.instance.SetPlayerHealth(playerHealth);
        soundFX.inst.playerTowerTakeDamage();
        if(playerHealth <= 0)
        {
            playingGame = false;
        }
    }

    IEnumerator DifficultyLoop()
    {
        while(playingGame)
        {
            yield return null;
            /*if(!building)
                currentDifficulty += Time.deltaTime * (1 / 60f) / difficultyRate;
            else
                currentDifficulty += Time.deltaTime * (1 / 60f) / difficultyRate / 5f;*/
            UIManager.instance.UpdateDifficultySlider(currentDifficulty);
            bool changed = DifficultyInfo.instance.UpdateDifficulty(currentDifficulty);

            if(changed)
            {
                DifficultyHelper difficulty = DifficultyInfo.instance.GetDifficulty();
                difficulty.SetWorldTowerProbabilities();
                UIManager.instance.NotifyDifficultyChange(difficulty.GetName(), difficulty.GetColor());
                // notify player that difficulty has increased
            }
        }
    }

    IEnumerator GameLoop()
    {
        yield return null;

        GenerationManager.GenerateMap();
        Coordinate[] enemySpawnPoints = GenerationManager.currentMap.GetTendrilEnds();

        Vector3[] realSpawnPoints = new Vector3[enemySpawnPoints.Length];
        for (int i = 0; i < enemySpawnPoints.Length; i++)
            realSpawnPoints[i] = enemySpawnPoints[i].GetGamePos();

        currentPhase = Phase.build;
        playingGame = true;

        StartCoroutine(DifficultyLoop());

        yield return null;

        float time = 0;
        float elapsed = 0;
        while (playingGame)
        {
            // starting build phase
            building = true;
            UIManager.instance.BeginBuild();

            DifficultyInfo f = DifficultyInfo.instance;
            int numTowers = DifficultyInfo.instance.GetDifficulty().GetNumTowersGiven();

            // give player towers
            for (int i = 0; i < numTowers; i++)
            {
                int towerType = TowerPresets.RollForTower();
                int index = TowerManager.instance.CreateTower(towerType);
                BaseTower t = TowerManager.instance.GetUnplacedTower(index);
                UIManager.instance.CreateCard(t);
            }

            time = Time.fixedTime;
            while (currentPhase == Phase.build)
            {
                // in build phase
                yield return null;
                elapsed = Time.fixedTime - time;
                UIManager.instance.UpdateBuildTime(buildTime - elapsed);
                if (!building || buildTime - elapsed <= 0)
                {
                    SwitchPhase(Phase.wave);
                }
            }

            RequestPathsClearCheck(enemySpawnPoints);
            int checkLength = enemySpawnPoints.Length;
            while (pathsCalculated != checkLength)
            {
                yield return null;
                if(pathsClear == false)
                {
                    UIManager.instance.SetPassageErrorText(true);
                    building = true;

                    // wait for all the paths to get back to us so we dont interfere
                    while (pathsCalculated != checkLength)
                        yield return null;

                    // wait a sec for player to move things around
                    yield return new WaitForSeconds(4f);

                    RequestPathsClearCheck(enemySpawnPoints);
                }
            }

            UIManager.instance.SetPassageErrorText(false);
            UIManager.instance.EndBuild();
            InputHandler.instance.ClearAll();

            // starting wave phase, placeholder generation
            AStarManager.instance.ClearRecentPaths();

            WaveHelper waveHelper = DifficultyInfo.instance.GetWave();
            waveHelper.ResetWave();

            List<int> usedDirs = waveHelper.UsedDirs();

            float animationTime = 3f;
            soundFX.inst.playCrashLanding();
            for (int i = 0; i < usedDirs.Count; i++)
            {
                GameObject newShip = Instantiate(spawnShipPrefab);
                Vector3 spawn = realSpawnPoints[usedDirs[i]];
                SpawnShip shipScript = newShip.GetComponent<SpawnShip>();
                shipScript.StartAnimation(spawn, animationTime);
                currentShips.Add(newShip);

                UIManager.instance.SetSpawnIndicator(i, spawn);
            }

            yield return new WaitForSeconds(animationTime);

            soundFX.inst.playEnemyNoise();
            // spawn enemies.
            float disbursementTime = waveHelper.GetDisbursementTime(); // time between each enemy spawn
            List<int> enemyIDs = new List<int>();
            List<int> numSpawns = new List<int>();
            List<int> spawnLocs = new List<int>();
            WaveHelper.SpawnInformation waveInfo = waveHelper.GetNextSpawnInfo();
            while (waveInfo != null)
            {
                int enemyID = waveInfo.GetID();
                int numEnemies = waveInfo.GetNumSpawns();
                int spawnIndex = waveInfo.GetSpawnIndex();

                enemyIDs.Add(enemyID);
                numSpawns.Add(numEnemies);
                spawnLocs.Add(spawnIndex);

                waveInfo = waveHelper.GetNextSpawnInfo();
            }
            bool spawning = true;
            List<int> visitedSpawnLocs = new List<int>();
            // loop so we can have each pod spawn at the same time
            while (spawning)
            {
                spawning = false;
                visitedSpawnLocs.Clear();
                for (int i = 0; i < enemyIDs.Count; i++)
                {
                    if (numSpawns[i] <= 0 || visitedSpawnLocs.Contains(spawnLocs[i]))
                        continue;
                    spawning = true;

                    Vector3 spawnPos = new Vector3(realSpawnPoints[spawnLocs[i]].x, 1, realSpawnPoints[spawnLocs[i]].z);
                    GameObject enemyPrefab = ResourceLoader.instance.GetEnemyPrefab(enemyIDs[i]);
                    GameObject enemy = Instantiate(enemyPrefab);
                    enemy.transform.position = spawnPos;
                    Enemy enemyScript = enemy.GetComponent<Enemy>();
                    float healthMod = DifficultyInfo.instance.GetDifficulty().GetModifiedHealth(enemyScript.GetMaxHealth(), currentDifficulty);
                    float speedMod = DifficultyInfo.instance.GetDifficulty().GetModifiedSpeed(enemyScript.GetMaxSpeed(), currentDifficulty);
                    enemyScript.WaveSetupStats(healthMod, speedMod);

                    currentWave.Add(enemy);
                    numSpawns[i] -= 1;
                    visitedSpawnLocs.Add(spawnLocs[i]);
                }

                yield return new WaitForSeconds(disbursementTime);
            }

            UIManager.instance.ClearSpawnIndicators();

            // loop, wait until enemies are all dead or player is dead
            time = Time.fixedTime;
            while (currentPhase == Phase.wave)
            {
                // in wave phase
                yield return null;
                for (int i = currentWave.Count - 1; i >= 0; i--)
                    if (currentWave[i] == null)
                        currentWave.RemoveAt(i);

                UIManager.instance.SetWaveText(currentWave.Count);

                elapsed = Time.fixedTime - time;
                if (currentWave.Count == 0)
                    SwitchPhase(Phase.build);
                //print("Time Left In Wave: " + (waveTime - elapsed) + ", Number of enemies: " + currentWave.Count);
            }

            currentDifficulty += DifficultyInfo.instance.GetDifficulty().GetDifficultyIncrease();

            // can probably make them self destruct or something
            foreach (GameObject obj in currentShips)
                Destroy(obj);
        }
    }

    private void RequestPathsClearCheck(Coordinate[] spawns)
    {
        pathsCalculated = 0;
        pathsClear = true;
        AStarManager.instance.ClearRecentPaths();
        foreach (Coordinate c in spawns)
        {
            RequestManager.RequestPath(ChunkManager.instance.GetBaseLocation(), c, PathsClearCheckCallback);
        }
    }

    private void PathsClearCheckCallback(List<Coordinate> path, bool success)
    {
        if(!success)
            pathsClear = false;
        pathsCalculated += 1;
    }

    void Update()
    {
        
    }
}
