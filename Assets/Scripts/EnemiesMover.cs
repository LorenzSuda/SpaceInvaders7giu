using UnityEngine;


[RequireComponent(typeof(GameManager))]

//le astronavi si muovono in blocco
public class EmemiesMover : MonoBehaviour
{
    float deltaX = 5; //lunghezza dello step
    float step = 1; //scatta: è lo scatto in blocco delle astronavi
    int direction = 1; // direzione dello spostamento: 1 = destra, -1 = sinistra

    [SerializeField] float gameOverLowerY = -18;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Move", step, step); //visto che è un mov. ripetitivo parte nello start
        //con un invoke repeat.
    }

    void Move()
    {
        transform.position += Vector3.right * deltaX * direction;
        
        //se supera 20 unità o -20 in x cambia direzione
        if (transform.position.x > 20 && direction == 1) 
        {
            CancelInvoke();

            direction *= -1;
            transform.position -= Vector3.up;

            step -= 0.1f;

            InvokeRepeating("Move", step, step);
        }
        else if (transform.position.x < -20 && direction == -1)
        {
            CancelInvoke();

            direction *= -1;
            transform.position -= Vector3.up;

            step -= 0.1f;

            if (step >= 0.1)
            {
                InvokeRepeating("Move", step, step);
            }
            else
            {
                CancelInvoke();

                GetComponent<GameManager>().GameOver();

                enabled = false;
            }
        }

        if (transform.position.y < gameOverLowerY)
        {
            //GameOver
            CancelInvoke();

            GetComponent<GameManager>().GameOver();

            enabled = false;
        }
    }
}