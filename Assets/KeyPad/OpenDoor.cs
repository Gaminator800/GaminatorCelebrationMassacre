using TMPro;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    private Animator anim;

    private bool IsAtDoor = false;

    [SerializeField] private TextMeshProUGUI CodeText;
    string codeTextValue = "";
    public string safeCode;
    public GameObject CodePannel;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        CodeText.text = codeTextValue;

        if (codeTextValue == safeCode)
        {
            anim.SetTrigger("OpenDoor");
            CodePannel.SetActive(false);
        }

        if (codeTextValue.Length >= 4)
        {
            codeTextValue = "";
        }

        if (Input.GetKey(KeyCode.E) && IsAtDoor == true)
        {
            CodePannel.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
        }

    }
    private void OnTriggerEnter(Collider other)
    {
      if(other.tag == "Player")
        {
            IsAtDoor = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        IsAtDoor = false;
        CodePannel.SetActive(false);
            Cursor.lockState = CursorLockMode.None;


    }

    public void AtDigits(string digit)
    {
        codeTextValue += digit;
    }
}
