using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour {
    public ParticleSystem ImpactEffect;
    public Combattent launcher;

    private float cacheTime;

    public void Launch(Transform muzzle, float speed, float fuse, Combattent launcher)
    {
        this.launcher = launcher;

        transform.rotation = muzzle.transform.rotation;
        transform.position = muzzle.transform.position;

        Rigidbody projBody = GetComponent<Rigidbody>();

        projBody.useGravity = false;
        projBody.velocity = transform.forward * speed;

        gameObject.SetActive(true);

        cacheTime = Time.time + fuse;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Projectile Collided");


        //TODO this should only ignore it's launcher's heirarchy
        if (collision.transform.GetComponentInParent<Combattent>() == launcher || collision.transform.GetComponent<Combattent>() == launcher) return;

        ParticleSystem HitEffect = GameObject.Instantiate(ImpactEffect);

        HitEffect.transform.position = collision.contacts[0].point;
        HitEffect.transform.rotation = Quaternion.Euler(collision.contacts[0].normal);
        HitEffect.transform.parent = collision.gameObject.transform;
        HitEffect.Play(true);

        AudioSource impactSound = HitEffect.gameObject.GetComponent<AudioSource>();
        if(impactSound != null) impactSound.Play();

        LightBlip blip = HitEffect.GetComponentInChildren<LightBlip>();
        if(blip != null)
        {
            blip.Blip(0.5f); // HitEffect.main.duration);
        }

        GameObject.Destroy(HitEffect.gameObject, 0.5f);
        Cache();
    }

    private void Update()
    {
        if (cacheTime > Time.time) return;

        Cache();
    }

    private void Cache()
    {
        gameObject.SetActive(false);
        ProjectileWeapon.projectileCache.Add(this);
    }
}
