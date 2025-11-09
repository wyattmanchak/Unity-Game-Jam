using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    private bool needsReset;
    private Animator anim;
    
    void Start()
    {
        anim = transform.GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            anim.SetTrigger("OpenDoor");
            needsReset = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemy")
        {
            needsReset = false;
        }
    }
}
