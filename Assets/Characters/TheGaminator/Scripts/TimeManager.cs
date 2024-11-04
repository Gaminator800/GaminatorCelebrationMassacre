using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public float slowDownFactor = 0.5f;
    public float slowDownLength = 5f;
    public bool slowmo = false;
    public AudioSource slowmoLine;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        slowTrigger();
        Time.fixedDeltaTime = Time.timeScale * .02f;
    }



    private void ExitSlowMo()
    {
        Time.timeScale = 1;
        slowmo = false;
    }

    public void slowMo()
    {
        slowmo = true;
        Time.timeScale = slowDownFactor;
        slowmoLine.Play();
    }

    void slowTrigger()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (slowmo)
                ExitSlowMo();

            else
                slowMo();
            
        }
    }
}
