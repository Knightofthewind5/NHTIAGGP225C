using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTest : MonoBehaviour
{
	ParticleSystem PS;

	// Start is called before the first frame update
	void Start()
	{
		PS = GetComponentInChildren<ParticleSystem>();
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{

		}
	}
}
