using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AsteroidStats
{
	[Tooltip("Identifier name for this script")]
	public string name;
	[Tooltip("Designated size of object")]
	public int size;
	[Tooltip("The cost of having the object in the scene")]
	public int weight;
	[Tooltip("The score given upon destroying the object")]
	public int score;
	[Tooltip("The amount of damage the asteroid deals on impact")]
	public float damage;
	[Tooltip("The score color of the object")]
	public Color color;
	[Tooltip("The sprite of the object")]
	public Sprite sprite;
	[Tooltip("The max amount of damage the object can take")]
	public float HP;
	[Tooltip("The modifier that dictates the velocity of the object")]
	public float speed;
	[Tooltip("How many objects will this object split into when destroyed")]
	public int splitCount;
	[Tooltip("What object this object will split into when destroyed. -1 is none")]
	public int splitIndex;
	[Tooltip("The lifetime of the object. -1 is infinity")]
	public float lifetime;
	[Tooltip("How large will the object be")]
	public Vector3 scale;


	public AsteroidStats(AsteroidStats stats)
	{
		name = stats.name;
		size = stats.size;
		weight = stats.weight;
		score = stats.score;
		damage = stats.damage;
		color = stats.color;
		sprite = stats.sprite;
		HP = stats.HP;
		speed = stats.speed;
		splitCount = stats.splitCount;
		splitIndex = stats.splitIndex;
		lifetime = stats.lifetime;
		scale = stats.scale;
	}
}
