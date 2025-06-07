using UnityEngine;

public class PlayerDestroyOnHit : MonoBehaviour
{
    private GameManager manager; //prova senza "a fa un Action eventualmente" ma guarda prima il video! 30 aprile 2025 2h45 (sembra un delegato) 
    
    //se te senti abbastanza matto lascia il get e set così. SE NON FUNZIA fallo come l'ha messo lui.
    public GameManager Manager
    {
        // Se il manager è null, lancia un'eccezione (se tutto ciò che è sistra dei "??" nn è null...sennò exception)
        get => manager ?? throw new System.Exception("Manager non inizializzato!");
        set => manager = value ?? throw new System.ArgumentNullException(nameof(value), "Manager non può essere null!");
    }
   // public GameManager Manager { get => manager; set => manager = value; } 
    
    private void OnCollisionEnter(Collision collision)
    {
        if (!enabled) return;
        
        //stesso ragionamento di prima, se è un bullet nemico...
        if (collision.gameObject.CompareTag("EnemyBullet"))
        {
            Debug.Log("Hit by Enemy bullet", this);
            
            //...bla bla...
            GetComponentInParent<PlayerManager>().enabled = false;//okkio questo è messo così apposta per rimarcare che devi scrivere
            GetComponentInParent<MeshRenderer>().enabled = false;//codice che crea delle "interdipendenze forti" fra parentele -> se cambi
                                                                 //la gerarchia e poi il "figlio" nn ha il padre (PlayerManager) nn funzia.
                                                                 //Vedi minuto 2h:45 del 30 aprile 2025, per farlo cogli action ma lo rivedrai prox lez.! 
            
            // Distruggi l'oggetto dopo 2 secondi
            Destroy(gameObject, 2);
            
            
            bool first = true;
            foreach (Transform t in transform) //stessa giostra della classe DestroyOnHit 
            {
                Debug.Log(t.name);
                if (first)  //questo lo puoi mettere anche così: if(t != transform) -> rivedi sempre DestroyOnHit e poi il Destroy fallo su sto milionesimo commento
                {
                    first = false;
                    continue;
                }

                // Attiva i voxel e aggiungi fisica
                t.gameObject.SetActive(true);
                Rigidbody rb = t.gameObject.AddComponent<Rigidbody>();
                rb.AddExplosionForce(20, t.transform.position, 100, 0, ForceMode.Impulse);
            }

            // Notifica il GameManager
            Manager.GameOver();

            enabled = false;
        }
    }
}