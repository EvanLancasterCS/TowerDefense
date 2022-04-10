using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnShip : MonoBehaviour
{
    private float hOffset = 20f;
    private float vOffset = 40f;
    [SerializeField] private ParticleSystem explosionPS;
    [SerializeField] private ParticleSystem flyingPS;
    [SerializeField] private ParticleSystem smokePS;

    public void StartAnimation(Vector3 endPos, float timeToAnimate)
    {
        StartCoroutine(Animate(endPos, timeToAnimate));
    }

    IEnumerator Animate(Vector3 endPos, float timeToAnimate)
    {
        Vector3 startPos = endPos + new Vector3(hOffset, vOffset, hOffset);
        Vector3 vecDiff = endPos - startPos;
        //Vector3 direction = vecDiff.normalized;
        float speed = vecDiff.magnitude / timeToAnimate;
        transform.position = startPos;
        transform.LookAt(endPos);

        flyingPS.Play();

        float time = 0;
        while(time < timeToAnimate)
        {
            transform.position += transform.forward * Time.deltaTime * speed;

            yield return null;
            time += Time.deltaTime;
        }

        transform.position = endPos;

        // explosion
        flyingPS.Stop();
        explosionPS.Play();
        smokePS.Play();

        yield return null;
    }
}
