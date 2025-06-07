using System;
using Unity.VisualScripting;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    
    [SerializeField] private float speed = 5f; // Speed of the power-up movement
    [SerializeField] private Vector3 movementDirection = Vector3.down; // Direction of the power-up movement
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Move the power-up in the specified direction at the specified speed
        transform.position += movementDirection * speed * Time.deltaTime;
     
        // // Check if the power-up has moved below a certain threshold (e.g., y = -10)
        // if (transform.position.y < -10f)
        // {
        //     // Destroy the power-up if it goes below the threshold
        //     Destroy(gameObject);
        // }
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("PlayerBullet"))
        {
            Destroy(other.gameObject);
            //Self destruct bullet
            Destroy(gameObject); 
            enabled = false;
            
            //cerchi il primo componente in scena col tipo X (GameManger)
            GameObject player = FindFirstObjectByType<GameManager>().player;
            player.GetComponent<PlayerManager>().StartPowerUp();
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject); 
            enabled = false;
            //ricorda other è il player: quindi usi other
            other.gameObject.GetComponent<PlayerManager>().StartPowerUp();
        }
    }
}
