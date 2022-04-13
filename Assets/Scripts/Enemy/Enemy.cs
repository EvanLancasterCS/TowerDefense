using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float maxSpeed;

    [SerializeField] private GameObject healthBarPrefab;
    private HealthBar myHealthBar;

    private float currentHealth;
    private List<Coordinate> path;
    private bool followPath = false;
    private bool noPathFound = false;
    private Rigidbody myRigidbody;
    private float waypointDistance = 0.8f;

    private float pathUpdateTime = 0.1f; // intervals to check for if unit is stuck somewhere
    private float stuckThreshold = 0.05f; // distance required to have moved to not be stuck

    private float lastAttackTime = 0f;
    private float attackSpeed = 0.5f;

    private float attackRange = 1f;

    private int attackDmg = 5;

    private bool waitingForPath = false;

    private void Start()
    {
        currentHealth = maxHealth;
        myRigidbody = GetComponent<Rigidbody>();

        GameObject hB = Instantiate(healthBarPrefab, transform);
        myHealthBar = hB.GetComponent<HealthBar>();
        myHealthBar.Hide();

        StartCoroutine(FindPathToGoal());
    }

    public float GetMaxHealth() => maxHealth;
    public float GetMaxSpeed() => maxSpeed;

    public void WaveSetupStats(float health, float speed)
    {
        currentHealth = health;
        maxHealth = health;
        maxSpeed = speed;
    }

    public void Update()
    {
        UpdatePath();
        Vector3 baseLoc = ChunkManager.instance.GetBaseLocation().GetGamePos();

        // Follow path if there is a path found; if no path found, move towards goal and attack
        // Don't move if there's a path available, but we dont want to move
        if (followPath && path != null)
        {
            MoveTowards(path[0].GetGamePos());
        }
        else if (noPathFound)
        {
            MoveTowards(baseLoc);

            AttemptAttackFront();
        }
        else
        {
            StopMoving();
        }

        if (Vector3.Distance(transform.position, baseLoc) < 2f)
        {
            WaveManager.instance.PlayerTakeDamage(1);
            Destroy(gameObject);
        }
    }

    IEnumerator FindPathToGoal()
    {
        if (Time.timeSinceLevelLoad < 1)
            yield return new WaitForSeconds(1);
        SetWaypoint(ChunkManager.instance.GetBaseLocation());

        float sqrThreshold = Mathf.Pow(stuckThreshold, 2);
        Vector3 posOld = transform.position;
        while(true)
        {
            yield return new WaitForSeconds(pathUpdateTime);

            if ((transform.position - posOld).sqrMagnitude < sqrThreshold)
                SetWaypoint(ChunkManager.instance.GetBaseLocation());

            posOld = transform.position;
            
        }
    }

    // Raycasts forwards to check if tower exists, attacks if so
    public void AttemptAttackFront()
    {
        if (Time.time - lastAttackTime > attackSpeed)
        {
            RaycastHit hit;
            int mask = 0x01 << 6; // mask for tower only layer
            if (Physics.Raycast(transform.position, transform.forward, out hit, attackRange, mask))
            {
                HexInfo hex = hit.transform.parent.parent.GetComponent<HexInfo>(); // collider is the child of tower obj who is the child of the hex
                hex.TakeDamage(attackDmg);
                lastAttackTime = Time.time;
            }
        }
    }

    // Raycasts downwards to find hex tile underneath, returns coordinate of that hex
    public Coordinate GetCurrentPos()
    {
        RaycastHit hit;
        int mask = 0x01 << 3; // mask for hex only layer
        if(Physics.Raycast(transform.position, -Vector3.up, out hit, 50f, mask))
        {
            if(hit.transform != null)
            {
                HexInfo hI = hit.transform.GetComponent<HexInfo>();
                return new Coordinate(hI.x, hI.z);
            }
        }
        return null;
    }

    // Uses A* to find path from pos to end
    // Eventually need to attack towers if there is no path
    public void SetWaypoint(Coordinate end)
    {
        if (!waitingForPath)
        {
            waitingForPath = true;
            Coordinate myC = GetCurrentPos();
            RequestManager.RequestPath(end, myC, PathFound);
        }
    }

    public void PathFound(List<Coordinate> _path, bool success)
    {
        if(success)
        {
            noPathFound = false;
            path = null;

            _path.Reverse();
            path = _path;

            // find where we actually are on the path, since it may have changed due to waiting for queue
            float shortest = Vector3.Distance(transform.position, path[0].GetGamePos());
            int index = 0;
            for(int i = 1; i < path.Count; i++)
            {
                float dist = Vector3.Distance(transform.position, path[i].GetGamePos());
                if(dist < shortest)
                {
                    index = i;
                    shortest = dist;
                }
            }
            for (int i = 0; i <= index; i++)
                path.RemoveAt(0); // clear out path up to the point we are close to

            UpdatePath();
            followPath = true;
            waitingForPath = false;
        }
        else
        {
            noPathFound = true;
            followPath = false;
            path = null;
        }
    }

    // checks if the unit is at the most recent path waypoint
    // resizes path if so, destroys if at zero
    private void UpdatePath()
    {
        if (path != null)
        {
            Vector3 realPos = path[0].GetGamePos();
            Vector3 dist = transform.position - realPos;
            if (Mathf.Abs(dist.x) + Mathf.Abs(dist.z) < waypointDistance) // simple distance, if within perimeter of a small square in hexagon
            {
                path.RemoveAt(0);
                if (path.Count == 0)
                {
                    path = null;
                    followPath = false;
                }
            }
        }
    }

    public int DistanceFromGoal()
    {
        if (path != null)
            return path.Count;
        else
            return (int)(Vector3.Distance(transform.position, ChunkManager.instance.GetBaseLocation().GetGamePos()));
    }

    private void MoveTowards(Vector3 pos)
    {
        float angle = Mathf.Atan2(transform.position.z - pos.z, transform.position.x - pos.x);
        float xSpeed = -Mathf.Cos(angle) * maxSpeed;
        float zSpeed = -Mathf.Sin(angle) * maxSpeed;

        myRigidbody.velocity = Vector3.zero;
        myRigidbody.velocity += new Vector3(xSpeed, 0, zSpeed);
    }

    private void StopMoving()
    {
        myRigidbody.velocity = Vector3.zero;
    }

    public void TakeDamage(float amt)
    {
        currentHealth -= amt;
        if (currentHealth <= 0)
        {
            Die();
            ScoreTracker.inst.addToScore(10);
            UIManager.instance.SetPlayerScore();
        }

        myHealthBar.Show();
        myHealthBar.SetPercentage(currentHealth / (float)maxHealth);
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
