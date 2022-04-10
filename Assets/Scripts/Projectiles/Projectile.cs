using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform target;
    private string tarTag;

    private float lifetime;
    private float speed;
    private float damage;
    private float spread;

    private float currentLife;

    private List<ProjectileEffect> effects = new List<ProjectileEffect>();
    private Dictionary<ProjectileEffect, object> effectTrackers = new Dictionary<ProjectileEffect, object>();

    private bool dead = false;
    private List<Transform> collisions = new List<Transform>();

    private void Awake()
    {
        currentLife = Time.fixedTime;
    }

    public void SetProjectile(Transform tar, string tag, float lt, float sp, float dmg, float sprd)
    {
        target = tar;
        tarTag = tag;
        lifetime = lt;
        speed = sp;
        damage = dmg;
        spread = sprd;

        SetDirection();
    }

    public bool TrackerExists(ProjectileEffect id)
    {
        return effectTrackers.ContainsKey(id);
    }

    public void UpdateTracker(ProjectileEffect id, object tracker)
    {
        effectTrackers[id] = tracker;
    }

    public object GetTracker(ProjectileEffect id)
    {
        return effectTrackers[id];
    }

    public void AttachEffect(ProjectileEffect effect)
    {
        effects.Add(effect);
        effect.ActivateCreation(transform);
    }

    public void SetDirection()
    {
        float angle = Mathf.Atan2(target.position.x-transform.position.x, target.position.z-transform.position.z);
        angle *= Mathf.Rad2Deg;
        float spreadAngle = Random.Range(-spread / 2f, spread / 2f);

        transform.eulerAngles = new Vector3(0, angle + spreadAngle, 0);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.transform.tag == tarTag && !dead && !collisions.Contains(collision.transform))
        {
            collisions.Add(collision.transform);
            collision.gameObject.SendMessage("TakeDamage", damage);

            dead = true;
            foreach (ProjectileEffect ef in effects)
                dead = dead == true ? ef.ActivateOnHit(transform, collision.transform) : false; // stays false after being set false

            if(dead)
                Destroy(gameObject);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.transform.tag == tarTag && !dead && collisions.Contains(collision.transform))
        {
            collisions.Remove(collision.transform);
        }
    }

    private void Update()
    {
        if(target != null)
            foreach (ProjectileEffect ef in effects)
                ef.ActivateTraveling(transform, target);

        transform.position += transform.forward * Time.deltaTime * speed;

        if (Time.fixedTime - currentLife > lifetime)
        {
            effects.Clear();
            Destroy(gameObject);
        }
    }
}
