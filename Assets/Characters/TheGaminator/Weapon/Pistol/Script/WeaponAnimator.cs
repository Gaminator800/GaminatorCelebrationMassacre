using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimator : MonoBehaviour
{
    public Animator anim;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("Fire");
        }

        if (Input.GetKey(KeyCode.R))
            anim.SetTrigger("Reload");

    }
}
