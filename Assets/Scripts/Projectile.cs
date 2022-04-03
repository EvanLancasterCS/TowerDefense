using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 target;
    private string tarTag;

    private float lifetime;
    private float speed;
    private float damage;
    private float spread;

    private float currentLife;

    private List<ProjectileEffect> effects = new List<ProjectileEffect>();

    private bool hasCollided = false;

    private void Awake()
    {
        currentLife = Time.fixedTime;
    }

    public void SetProjectile(Vector3 tar, string tag, float lt, float sp, float dmg, float sprd)
    {
        target = tar;
        tarTag = tag;
        lifetime = lt;
        speed = sp;
        damage = dmg;
        spread = sprd;

        SetDirection();
    }

    public void AttachEffect(ProjectileEffect effect)
    {
        effects.Add(effect);
    }

    public void SetDirection()
    {
        float angle = Mathf.Atan2(target.x-transform.position.x, target.z-transform.position.z);
        angle *= Mathf.Rad2Deg;
        float spreadAngle = Random.Range(-spread / 2f, spread / 2f);

        transform.eulerAngles = new Vector3(0, angle + spreadAngle, 0);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.transform.tag == tarTag && !hasCollided)
        {
            collision.gameObject.SendMessage("TakeDamage", damage);

            foreach (ProjectileEffect ef in effects)
                ef.Activate(collision.transform.position);

            Destroy(gameObject);

            hasCollided = true;
        }
    }

    private void Update()
    {
        if (Time.fixedTime - currentLife > lifetime)
        {
            Destroy(gameObject);
        }

        transform.position += transform.forward * Time.deltaTime * speed;
    }
}
