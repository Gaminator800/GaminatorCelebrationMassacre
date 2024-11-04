using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Dash : MonoBehaviour
{
    public float dashSpeed;
    public AudioSource DashSound;
    Rigidbody rig;
    bool isDashing;


    

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
            isDashing = true;
        
    }

    private void FixedUpdate()
    {
        if(isDashing)
            Dashing();
        
    }

    private void Dashing()
    {
        rig.AddForce(transform.forward * dashSpeed, ForceMode.Impulse);
        isDashing = false;
        DashSound.Play();
    }
}
