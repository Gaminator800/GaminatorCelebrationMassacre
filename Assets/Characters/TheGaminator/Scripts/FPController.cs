using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class FPController : MonoBehaviour
{
    public GameObject cam;
    public GameObject GaminatorPrefab;
    public Transform shotDirection;
    public Animator anim;
    public AudioSource[] footsteps;
    public AudioSource Jump;
    public AudioSource Land;
    public AudioSource AmmoPickUp;
    public AudioSource CanPickUp;
    public AudioSource triggerSound;
    public AudioSource deathSound;

    float speed = 0.1f;
    float Xsensitivity = 2;
    float Ysensitivity = 2;
    float MinimunX = -90;
    float MaximumX = 90;

    bool cursorIsLocked = true;
    bool lockedCursor = true;

    float x;
    float z;

    //Inventory
    int ammo = 0;
    int maxAmmo = 216;
    int health = 100;
    int maxHealth = 100;
    int ammoClip = 18;
    int ammoClipMax = 18;

    bool playingWalking = false;
    bool previouslyGrounded = true;

    Rigidbody rb;
    CapsuleCollider capsule;

    Quaternion cameraRot;
    Quaternion CharacterRot;

    public void TakeHit(float amount)
    {
        health = (int) Mathf.Clamp(health - amount, 0, maxHealth);
        //Debug.Log("Health: " + health);
        if (health <= 0)
        {
            Vector3 pos = new Vector3(this.transform.position.y,
                                        transform.position.z);
            GameObject gaminator = Instantiate(GaminatorPrefab, pos, this.transform.rotation);
            gaminator.GetComponent<Animator>().SetTrigger("Death");
            GameStats.gameOver = true;
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        capsule = this.GetComponent<CapsuleCollider>();

        cameraRot = cam.transform.localRotation;
        CharacterRot = this.transform.localRotation;
        

        health = maxHealth;
    }

   void ProcessCyborgHit()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(shotDirection.position, shotDirection.forward, out hitInfo, 200))
        {
            GameObject hitCyborg = hitInfo.collider.gameObject;
            if (hitCyborg.tag == "Cyborg")
            {
                if (Random.Range(0, 10) < 5)
                {
                    GameObject rdPrefab = hitCyborg.GetComponent<CyborgController>().ragdoll;
                    GameObject newRD = Instantiate(rdPrefab, hitCyborg.transform.position, hitCyborg.transform.rotation);
                    newRD.transform.Find("Root").GetComponent<Rigidbody>().AddForce(shotDirection.forward * 10000);
                    Destroy(hitCyborg);
                }
                else
                {
                    hitCyborg.GetComponent<CyborgController>().KillCyborg();
                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            anim.SetBool("Arm", !anim.GetBool("Arm"));

        if (Input.GetMouseButtonUp(0))
        {
            if (ammoClip > 0)
            {
                anim.SetTrigger("Fire");
                ProcessCyborgHit();
                ammoClip--;
            }
            else if (anim.GetBool("Arm"))
                triggerSound.Play();

                Debug.Log("Ammo Left in Clip:" + ammoClip);
            
        }


        if (Input.GetKey(KeyCode.R) && anim.GetBool("Arm"))
        {
            anim.SetTrigger("Reload");
            int amountNeeded = ammoClipMax - ammoClip;
            int ammoAvailable = amountNeeded < ammo ? amountNeeded : ammo;
            ammo -= ammoAvailable;
            ammoClip += ammoAvailable;
            Debug.Log("Ammo Left: " + ammo);
            Debug.Log("Ammo in Clip: " + ammoClip);

        }

        if (Mathf.Abs(x) > 0 || Mathf.Abs(z) > 0)
        {
            if (!anim.GetBool("Walking"))
            {
                anim.SetBool("Walking", true);
                InvokeRepeating("PlayFootStepAudio", 0, 0.4f);
            }
        }
        else if (anim.GetBool("Walking"))
        {
            anim.SetBool("Walking", false);
            CancelInvoke("PlayFootStepAudio");
            playingWalking = false;
        }

        bool grounded = IsGrounded();
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(0, 300, 0);
            Jump.Play();
            if (anim.GetBool("Walking"))
            {
                CancelInvoke("PlayFootStepAudio");
                playingWalking = false;
            }

        }
        else if (!previouslyGrounded && grounded)
        {
            Land.Play();
        }

        previouslyGrounded = grounded;

    }

    void PlayFootStepAudio()
    {
        AudioSource audioSource = new AudioSource();
        int n = Random.Range(1, footsteps.Length);

        audioSource = footsteps[n];
        audioSource.Play();
        footsteps[n] = footsteps[0];
        footsteps[0] = audioSource;
        playingWalking = true;
    }

    void FixedUpdate()
    {
        float yRot = Input.GetAxis("Mouse X") * Ysensitivity;
        float xRot = Input.GetAxis("Mouse Y") * Xsensitivity;

        cameraRot *= Quaternion.Euler(-xRot, 0, 0);
        CharacterRot *= Quaternion.Euler(0, yRot, 0);

        cameraRot = ClampRotationAroundXAxis(cameraRot);

        this.transform.localRotation = CharacterRot;
        cam.transform.localRotation = cameraRot;

        x = Input.GetAxis("Horizontal") * speed;
        z = Input.GetAxis("Vertical") * speed;

        transform.position += cam.transform.forward * z + cam.transform.right * x; //new Vector3(x * speed, 0, z * speed);

        UpdateCursorLock();
    }


    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
        angleX = Mathf.Clamp(angleX, MinimunX, MaximumX);
        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }

    bool IsGrounded()
    {
        RaycastHit hitInfo;
        if (Physics.SphereCast(transform.position, capsule.radius, Vector3.down, out hitInfo,
                (capsule.height / 2f) - capsule.radius + 0.1f))
        {
            return true;
        }
        return false;
    }

    public void SetCursorLock(bool value)
    {
        lockedCursor = value;
        if (!lockedCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Ammo" && ammo < maxAmmo)
        {

            ammo = Mathf.Clamp(ammo + 18, 0, maxAmmo);
            Debug.Log("Ammo:" + ammo);
            Destroy(col.gameObject);
            AmmoPickUp.Play();  
        }
        if (col.gameObject.tag == "Can" && health < maxHealth)
        {
            health = Mathf.Clamp(health + 25, 0, maxHealth);
            Debug.Log("Can: " + health);
            Destroy(col.gameObject);
            CanPickUp.Play();
        }
        else if (col.gameObject.tag == "Lava")
        {
            health = Mathf.Clamp(health - 50, 0, maxHealth);
            Debug.Log("Health Level: " + health);
            if (health <= 0)
                deathSound.Play();
        }

        
        else if (IsGrounded())
        {
            if (anim.GetBool("Walking") && !playingWalking)
            {
                InvokeRepeating("PlayFootStepAudio", 0, 0.4f);
            }

        }
    }



    public void UpdateCursorLock()
    {
        if (lockedCursor)
            InternalLockedUpdate();
    }

    public void InternalLockedUpdate()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
            cursorIsLocked = false;
        else if (Input.GetMouseButtonUp(0))
        cursorIsLocked = true;

        if (cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if(!cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

}
