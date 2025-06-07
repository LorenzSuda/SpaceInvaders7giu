using UnityEngine;

public class BulletManager : MonoBehaviour
{
    Rigidbody rb;
    float physicSpeed = 80;
    Vector3 movementDirection = Vector3.up;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (!rb) // è = a dire -> rb = null. [oper. bool sovrascritto da unity] ...se capiva + senza commento che con ^^
        {
            rb = gameObject.AddComponent<Rigidbody>(); //se il RB non c'è lo aggiunge
        }

        rb.useGravity = false; //prova senza gravity: isKinematic da stesso effetto
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Extrapolate; //crea dei frame intermedi per evitare il lag
        //(ricorda che l'interpolazione fluidifica i movimenti.Extrapol:variante +leggera)
    }

    //infatti poi fa il fixed update perché non mischia la fisica con la trasform! "Mai incrociare i flussi" (come il vangelo!)
    private void FixedUpdate()
    {
        rb.MovePosition(transform.position +
                        movementDirection * physicSpeed *
                        Time.fixedDeltaTime); //sposta il bullet verso "l'up a 80 unità al secondo"
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!enabled) return;
        //Self destruct bullet
        Destroy(gameObject);
        enabled = false;
    }
    //ricorda che è pure la solita storia per distruggere anche il component che gestisce le collisioni così non ne rileva altre
}