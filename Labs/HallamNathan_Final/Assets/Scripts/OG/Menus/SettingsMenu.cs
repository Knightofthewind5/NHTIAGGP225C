using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
	public AudioMixer audioMixer;

	public Slider volumeSlider;

	public float vol;

	public void Start()
	{
		vol = PlayerPrefs.GetFloat("volume");
		audioMixer.SetFloat("volume", vol);
		volumeSlider.value = vol;
	}

	public void Update()
	{
		PlayerPrefs.SetFloat("volume", vol);
		audioMixer.SetFloat("volume", vol);
	}

	public void SetVolume (float volume)
	{
		vol = volume;

		audioMixer.SetFloat("volume", vol);
		PlayerPrefs.SetFloat("volume", vol);
	}
}
