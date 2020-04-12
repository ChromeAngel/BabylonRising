using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ProjectileWeapon : MonoBehaviour {

    public GameObject projectilePrefab;

    public static List<Projectile> projectileCache;

    public float secondBetweenShots;
    public float launchSpeed;

    public float nextShotTime;

    private AudioSource launchSound;

    // Use this for initialization
    void Start () {
        if (projectileCache == null) projectileCache = new List<Projectile>();

        launchSound = GetComponent<AudioSource>();

        nextShotTime = Time.time;
    }
	
	// Update is called once per frame
	void Update () {
        if (Time.time < nextShotTime) return;
        if (Time.timeScale == 0f) return; // dont fire when paused
        if (RadialMenu.IsOpen) return; //dont fire when in the menu
        if (!Input.GetButton("Fire1")) return;

        nextShotTime = Time.time + secondBetweenShots;

        Projectile projectile = CreateProjectile();

        launchSound.Play();
        projectile.Launch(transform, launchSpeed, 20f, GetComponentInParent<Combattent>());
    }

    private Projectile CreateProjectile()
    {
        if (projectileCache.Count == 0) return GameObject.Instantiate(projectilePrefab).GetComponent<Projectile>();

        var result = projectileCache.First();

        projectileCache.Remove(result);

        return result;
    }
}
