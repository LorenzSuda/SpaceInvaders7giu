using UnityEngine;
using UnityEngine.Audio;
//Se aggiungi [RequireComponent] a un GameObject, se manca AudioSource → Unity lo aggiunge automaticamente (evitan errori NullReferenceException). 
//Se provi a rimuoverlo manualmente → Unity blocca l'operazione (finché lo script è attaccato)
//se già esiste lo script...nisba nn fa niente
[RequireComponent(typeof(AudioSource))] 

//IMPORTANTE: non sembra ma è molto simile al codice dell'EnemyManager, solo che è il player
public class PlayerManager : MonoBehaviour
{
    [SerializeField] Vector3 startPosition;
    [SerializeField] float horizontalDelta = 90f; // distanza massima a sinistra e a destra
    [SerializeField] Sprite playerSprite;
    [SerializeField] Color color = Color.white;
    [SerializeField] float speed = 1000f;
    [SerializeField] Material playerMaterial;
    [SerializeField] Material playerEmissiveMaterial;
    [SerializeField] GameManager manager;
    [SerializeField] AudioSource fireAS;
    [SerializeField] AudioSource dieAS;
    [SerializeField] Vector3 bulletPosition1;
    [SerializeField] Vector3 bulletPosition2;
    
    
    private GameObject playerShape;
    private float deltaX = 1f;
    private float deltaY = 1f;
    private bool powerUp = false;
    private float powerUpTimer = 0f;
    private void Start()
    {
        // Create the player voxel model
        playerShape = new GameObject();
        playerShape.name = "Player";
        playerShape.transform.parent = transform;

        // Add destruction component
        PlayerDestroyOnHit pdh = playerShape.AddComponent<PlayerDestroyOnHit>();
        pdh.Manager = manager;

        // Read sprite pixels and create voxels
        int startX = (int)playerSprite.textureRect.xMin;
        int startY = (int)playerSprite.textureRect.yMin;
        int w = (int)playerSprite.textureRect.width;
        int h = (int)playerSprite.textureRect.height;

        Color[] pixels = playerSprite.texture.GetPixels();
        float currentX = 0f;
        float currentY = 0f;

        // Iterate through sprite pixels
        for (int i = startX; i < startX + w; i++)
        {
            for (int j = startY; j < startY + h; j++)
            {
                Color c = playerSprite.texture.GetPixel(i, j);
                if (c.r != 0 || c.g != 0 || c.b != 0) // Not black
                {
                    GameObject voxel = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    voxel.GetComponent<MeshRenderer>().material = playerEmissiveMaterial;
                    voxel.name = $"{i}:{j}";
                    voxel.transform.position = new Vector3(currentX, currentY, 0);
                    voxel.transform.parent = playerShape.transform;
                }
                currentY += deltaY;
            }
            currentY = 0f;
            currentX += deltaX;
        }

        // Merge all voxels into one mesh
        MeshMerger mm = playerShape.AddComponent<MeshMerger>();
        mm.Configure(playerMaterial, false);

        // Set initial position and scale
        transform.position = startPosition;
        transform.localScale = new Vector3(2f, 2f, 2f);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) //potevi usare pure GetButton horizontal (che va bene pure per altri controllers)
        {
            // left movement
            transform.Translate(Time.deltaTime * speed * -transform.right); //origin: transform.right * -1 è uguale 
                                                                                    // -transform.right: ebbene sì in unity non c'è transform.left, drogati maledetti.
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            // right movement
            transform.Translate(Time.deltaTime * speed * transform.right);
        }

        // Clampa la position (pos) dentro i lateral bounds
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, startPosition.x - horizontalDelta, startPosition.x + horizontalDelta); //rivedi come viene calcolato l'horzontalDelta
                                                                                                                 //perché 90 e non 100?
        transform.position = pos;

        //bullet timer
        if (powerUp)
        {
            powerUpTimer -= Time.deltaTime;
            if (powerUpTimer <= 0)
            {
                powerUp = false;
            }
        }
        
        // Handle shooting
        if (Input.GetKeyDown(KeyCode.Space)) //getButtonDown("Fire1") non funziona con il controller
        {
            // // Play fire sound : codice aggiuntivo
            // if (fireAS == null) return;
            // if (fireAS.clip == null) return;

            fireAS.PlayOneShot(fireAS.clip);

            // Create bullet: vedi esempio di EnemyManager
            GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bullet.tag = "PlayerBullet";
            bullet.transform.localScale = new Vector3(3f, 5f, 3f);
            bullet.transform.position = transform.position + bulletPosition1;
            BulletManager bm = bullet.AddComponent<BulletManager>();
            Destroy(bullet, 4f);

            if (powerUp)
            {
                GameObject bullet2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                bullet2.tag = "PlayerBullet";
                bullet2.transform.localScale = new Vector3(3f, 5f, 3f);
                bullet2.transform.position = transform.position + bulletPosition2;
                BulletManager bm2 = bullet2.AddComponent<BulletManager>();
                Destroy(bullet2, 4f);
            }
        }
    }

    public void StartPowerUp()
    {
        if (powerUp) return; 
        powerUp = true;
        powerUpTimer = 10f;
    }

    private void OnDisable()
    {
        // Play death sound when player is destroyed
        dieAS.Play();
    }
}