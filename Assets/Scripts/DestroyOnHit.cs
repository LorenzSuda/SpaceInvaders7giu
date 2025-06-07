using UnityEngine;

//DestroyOnHit serve per distruggere l'oggetto a cui è associata quando viene colpito da un proiettile (tag `"PlayerBullet"`)
//Inoltre, disabilita il componente 'EnemyManager`, nasconde il `MeshRenderer`,
//e aggiunge effetti visivi come forze esplosive ai figli dell'oggetto. (rb.AddExplosionForce)
public class DestroyOnHit : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        //Self destruct on hit
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            GetComponentInParent<AudioSource>().Play();
            Debug.Log("hit by player bullet", this);
            
            //Take away from hierarchy to avoid moved with the others by EnemiesMover script
            transform.parent.parent = null;
            GetComponentInParent<EnemyManager>().enabled = false;
            GetComponentInParent<MeshRenderer>().enabled = false;
            GetComponentInParent<EnemyManager>().WasHit();
            
            //In Unity, un oggetto Transform rappresenta la gerarchia degli oggetti nella scena, e la proprietà transform di un oggetto restituisce
            //il suo Transform (la gerarchia!)
            //Quindi la riga foreach (Transform t in transform) itera su tutti i figli (child) dell'oggetto a cui è associato lo script.
            foreach (Transform t in transform)
            {
                Debug.Log(t.name);
                t.gameObject.SetActive(true); //IMPORTANTE! è qui che attivi tutti i voxels, tutti i cubetti!
                //okkio qui inoltre se vuoi lavorare solo sui figli devi mettere un if(t != transform)
                
                Rigidbody rb = t.gameObject.AddComponent<Rigidbody>();
                if (rb)
                    rb.AddExplosionForce(20, t.transform.position, 100, 0, ForceMode.Impulse);
            }
            Destroy(transform.parent.gameObject, 3);// tempo di distruzione dei voxels poi il padre viene distrutto dal figlio
            Destroy(this);
        }
    }
}