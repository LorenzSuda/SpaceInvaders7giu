using UnityEngine;

//in questo caso tutti i movimenti sono gestiti da un rigidbody (fisica, quindi fixed update)
// enum BulletType
// {
//     Enemy, Player    
// } considera che lo puoi fare pure così fancedo un controllo delle collisioni coll'enum.

public class EnemyBulletManager : MonoBehaviour
{
    Rigidbody rb; 
    float physicsSpeed = 50; // Speed of the bullet
    Vector3 movementDirection = Vector3.down;

    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (!rb)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.useGravity = false; //gliela toglie perché la forza di gravità è "reale" quindi il bullet subirebbe un accelerazione (sarebbe brutto ^^)
        rb.isKinematic = true; //in realtà è più per la tua memoria, perché se isKinematic è true non viene influenzato dalla gravità di default
        rb.interpolation = RigidbodyInterpolation.Extrapolate; //fluidifica movimento ma nn alla precisione di un interpolate
    }

    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + movementDirection * physicsSpeed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!enabled) return;

        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Barrier"))
        {
            //Self destruct bullet
            Destroy(gameObject); //ricorda che fai il controllo del tipo di GO no? nn fare il drogato
            enabled = false;
        }
        else if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            Destroy(collision.gameObject);
            //Self destruct bullet
            Destroy(gameObject); //ricorda che fai il controllo del tipo di GO no? nn fare il drogato
            enabled = false;
        }
    }
}