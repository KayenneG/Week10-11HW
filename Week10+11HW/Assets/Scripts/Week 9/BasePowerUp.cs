using UnityEngine;

public class BasePowerUp : MonoBehaviour
{
    public AudioSource poSound;
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("PO collision");
        if (collision.gameObject.CompareTag("Player"))
        {
            PowerUp();
        }
    }
    public virtual void PowerUp()
    {
        poSound.Play();
    }
}
