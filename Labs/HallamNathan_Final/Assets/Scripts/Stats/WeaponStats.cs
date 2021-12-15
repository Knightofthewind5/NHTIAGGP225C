using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponStats
{
	[Tooltip("Identifier name for this script")]
	public string weaponName;
	[Tooltip("The HUD sprite for this weapon")]
	public Sprite icon;
	[Tooltip("The prefab for the projectile")]
	public GameObject projectilePrefab;
	[Tooltip("How much damage this weapon deals")]
	public int damage;
	[Tooltip("The speed the projectile will move at")]
	public float speed;
	[Tooltip("The amount of time the projectile will live for")]
	public float lifetime;
	[Tooltip("The amount of time the weapon needs to fire another shot")]
	public float fireRate;
	[Tooltip("The sound that will play when firing this weapon")]
	public AudioClip fireSound;
	[Tooltip("The amount of knockback this projectile will deal on impact")]
	public float mass;
	[Tooltip("The amount of objects the projectile can pierce")]
	public float life;

	public WeaponStats(WeaponStats stats)
	{
		weaponName = stats.weaponName;
		icon = stats.icon;
		projectilePrefab = stats.projectilePrefab;
		damage = stats.damage;
		speed = stats.speed;
		lifetime = stats.lifetime;
		fireRate = stats.fireRate;
		fireSound = stats.fireSound;
		mass = stats.mass;
		life = stats.life;
	}
}
