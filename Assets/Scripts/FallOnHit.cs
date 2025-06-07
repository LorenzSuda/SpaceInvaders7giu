using UnityEngine;

public class FallOnHit : MonoBehaviour
{
    //se c'è una collisione si distrugge
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
        enabled = false; //Disabilita il componente per evitare ulteriori collisioni
    }
}
