using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public AudioSource Eject;
    public AudioSource Insert;
    public AudioSource Slider;
    public AudioSource Shot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Fire()
    {
        Shot.Play();
    }

    public void Inset()
    {
        Insert.Play();
    }

    public void eject()
    {
        Eject.Play();
    }

    public void slider()
    {
        Slider.Play();
    }


}
