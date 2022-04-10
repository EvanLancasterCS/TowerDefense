using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem explosionPS;
    [SerializeField] private SphereCollider myCollider;
    private int damage = 0;
    private float lifetime = 0.3f;
    private string tarTag = "";
    private List<Transform> collisions = new List<Transform>();

    void Start()
    {
        explosionPS.Play();
        StartCoroutine(WaitAndDie());
    }

    private IEnumerator WaitAndDie()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    public void Set(int dmg, float radius, string targetTag)
    {
        damage = dmg;
        tarTag = targetTag;
        myCollider.radius = radius;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == tarTag &&  !collisions.Contains(other.transform))
        {
            collisions.Add(other.transform);
            other.gameObject.SendMessage("TakeDamage", damage);
        }
    }
}
