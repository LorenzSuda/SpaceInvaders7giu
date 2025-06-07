using UnityEngine;
//Considera come esercizio di rifattorizzazione il fatto che puoi fare tutto senza ripetere il codice e
//considerando che i 2 sprite usati hanno una buona metà di voxels in comune
//quindi pensa a uno sprite padre che conteniene i voxels comuni + 2 sprite figli che contengono i voxels diversi (3 spritz in tutto!)
//istanzi solo quelli della parte in comune e poi accendi e spegni i figli come come qui: goShape2.SetActive(false);
public class EnemyManager : MonoBehaviour
{
    [SerializeField] PrimitiveType voxel = PrimitiveType.Cube; //ER CUBETTO ^^ . 
                                                                //se metti GameObject a posto di PrimitiveType ci puoi mette tutte le forme che vuoi a posto dei cubibus

    bool animate = false; 

    GameObject goShape1;
    GameObject goShape2;

    float deltaX = 1;
    float deltaY = 1;
    float fireDelay = 0;

    GameManager gameManager;

    int hitPoints;

    //2 immagini:i 2 sprite delle navette, materiale, punti, game manager con cui parla
    public void Configure(Sprite frame1Shape, Sprite frame2Shape, Material newMat, int points, GameManager manager)
    {
        //solita config. dei valori
        hitPoints = points;
        gameManager = manager;
        fireDelay = Random.Range(5, 10); //tempo fra un colpo e l'altro

        //crea la prima forma (quella che riceve il colpo) okkio no voxel i cubetti stanno dopo
        goShape1 = new GameObject();
        goShape1.AddComponent<DestroyOnHit>();
        goShape1.name = "Frame1";
        goShape1.transform.parent = transform;
        //...e la seconda forma ( pure questa riceve il colpo!) okkio no voxel i cubetti stanno dopo
        goShape2 = new GameObject();
        goShape2.AddComponent<DestroyOnHit>();
        goShape2.name = "Frame2";
        goShape2.transform.parent = transform; //IMP!: parent è il gameobject padre
        
        
        //leggi le coordinate dei pixel del 1° sprite. Degli sprite okkio!
        int startX = (int)frame1Shape.textureRect.xMin; //passa mouse su XMin e YMin: fatto? ohoooh!
        int startY = (int)frame1Shape.textureRect.yMin;
        int w = (int)frame1Shape.textureRect.width;
        int h = (int)frame1Shape.textureRect.height;
        //Debug.Log($"Image 1 size: {w}x{h} from: {startX}x{startY} to {startX+w}x{startY+h}");
        
        //1.PROCEDURA DI LETTURA "DEL 1° SPRITE"->TEX: Prepare to read pixels colors for frame1
        Color[] pixels = frame1Shape.texture.GetPixels(); //getpixels legge i pixel della texture e li mette in un array di colori

        float currentX = 0;
        float currentY = 0;

        //iterate for all the pixels of the image 1
        for (int i = startX; i < startX + w; i++)
        {
            for (int j = startY; j < startY + h; j++)
            {
                Debug.LogFormat("EM step {0}:{1}->{2} value: {3}",i,j, (i * j)+j, pixels[(i * j) + 1]);
                //in teoria sta riga (Color c..) è "rifattorizzabile" rispetto a sopra GetpixelS vs Getpixel (la "s"):
                //GetPixelS prende una stringa unica di pixel
                //GetPixel divide i pixel in righe e colonne
                //in teoria dovresti riusare GetPixels anche qui e fare il modulo per andare a capo coi pixel...rivedi video 25:49 30 apr
                Color c = frame1Shape.texture.GetPixel(i, j); 
                if (c.r != 0 && c.g != 0 && c.b != 0) //controlla se il colore è diverso da nero: così sei sicuro che non è trasparente
                {
                    //VOXELS. primo sprite: 
                    GameObject go = GameObject.CreatePrimitive(voxel);
                    go.GetComponent<MeshRenderer>().material = newMat;
                    
                    //IMPORTANTE IMPORTANTISSIMO INTERPOLAZIONE DI STRINGHE! :
                    //Se i = 2, j = 3 e pixels[8] = Color.red, il nome assegnato sarà: "2:3:Color.red%" -> PERCHé pixels: è UNA STRUCT DI COLORI, OKKIO!!!
                    go.name = $"{i}:{j}:{pixels[(i * j) + i]}%";                                           //è un interpolaz.
                                                                                                           //i e j vengono inseriti direttamente
                                                                                                           // "%" per rendere univoco il nome del cubetto 
                                                                                                           //NEL CASO CI FOSSERO PROBLEMI COL SINGOLO VOXEL!!!
                    go.transform.parent = goShape1.transform; //make object child of goShape1
                    go.transform.position = new Vector3(currentX, currentY, 0);
                    //Debug.LogFormat("Img1 block at: {0} {1}", i, j);
                }
                currentY += deltaY;
            }
            currentY = 0;
            currentX += deltaX;
        }
        currentX = 0;
        currentY = 0;

        startX = (int)frame2Shape.textureRect.xMin;
        startY = (int)frame2Shape.textureRect.yMin;
        w = (int)frame2Shape.textureRect.width;
        h = (int)frame2Shape.textureRect.height;
        //Debug.Log($"Image 2 size: {w}x{h} from: {startx}x{starty} to {startX + w}x{starty + h}");

        
        //2.PROCEDURA DI LETTURA "DEL 2° SPRITE"->TEX: Prepare to read pixels colors for frame2
        pixels = frame2Shape.texture.GetPixels();
        
        for (int i = startX; i < startX + w; i++)
        {
            for (int j = startY; j < startY + h; j++)
            {
                Color c = frame2Shape.texture.GetPixel(i, j);
                if (c.r != 0 && c.g != 0 && c.b != 0)
                {
                    //VOXELS. secondo sprite:
                    GameObject go = GameObject.CreatePrimitive(voxel);
                    go.GetComponent<MeshRenderer>().material = newMat;
                    go.name = $"{i}:{j}:{pixels[(i * j) + 1]}%";
                    go.transform.parent = goShape2.transform; //make object child of goShape2
                    go.transform.position = new Vector3(currentX, currentY, 0);
                    //Debug.LogFormat("Img2 block at: {0} {1}",i,j);
                }
                currentY += deltaY;
            }

            currentY = 0;
            currentX += deltaX;
        }
        
        //mergia le geometrie degli sprite per velocizzare il rendering
        MeshMerger mr1 = goShape1.AddComponent<MeshMerger>();
        mr1.Configure(newMat, false);

        MeshMerger mr2 = goShape2.AddComponent<MeshMerger>();
        mr2.Configure(newMat, false);

        //hide second frame !temporarily!
        goShape2.SetActive(false);

        InvokeRepeating("Fire", fireDelay, fireDelay); //prova a rifarlo colle coroutine: bello vé? 

        animate = true; //per far partire l'animazione
        ////ogni nemico sparerà a un intervallo pesudocasuale impostato su Configure. 
    }
    
    //accendi e spegni gli sprite
    void Update()
    {
        if (!animate) return;
        //Debug.Log(Mathf.FloorToInt(Time.time));
        //ANVEDI! in 2 righe di codice c'è l'accendi/spegni!
        goShape1.SetActive(Mathf.FloorToInt(Time.time) % 2 == 0); //prendi il tempo assoluto e lo converti in intero FloorToInt,
                                                                  //poi se il 2° è pari (% 2 == 0) allora accendi sprite1!
        goShape2.SetActive(!goShape1.activeSelf);                 //altrimenti quando % 2 == 1 ossia il tempo è dispari, accendi sprite2;
    }

    private void Fire()
    {
        if (Random.Range(1, 100) > 80) //81<=x<=99 (sparo casuale)
        {
            GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Cube); //spara parallelepipedi
            bullet.tag = "EnemyBullet";
            bullet.transform.localScale = new Vector3(2, 4, 2); //spara parallelepipedi (non è 1,1,1 no cubo, ma cubo è un parall vabeh fermate ^^)
            bullet.transform.position = transform.position;
            bullet.AddComponent<EnemyBulletManager>(); //per gestire la collisione del projectile
            Destroy(bullet, 4);
        }
    }

    public void WasHit() //alieno colpito
    {
        gameManager.DidHitEnemy(hitPoints);
    }
}