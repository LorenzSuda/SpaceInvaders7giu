using UnityEngine;
using TMPro;
//rendilo più parametrico: cioè lui fa 3 volte le stesse 3 cose per 3 le astro
public class GameManager : MonoBehaviour
{

//Enemies setup
    [SerializeField] Material enemyMaterial; //così configuri i materiali tutti da qui

//Enemy 1st row top
    [SerializeField] Sprite enemy1;//
    [SerializeField] Sprite enemy1b;
    [SerializeField] int enemy1Cols = 11;
    [SerializeField] int enemy1Rows = 1;
    [SerializeField] int enemy1Points = 30;

//Enemy 2nd and 3rd row middle
    [SerializeField] Sprite enemy2;
    [SerializeField] Sprite enemy2b;
    [SerializeField] int enemy2Cols = 11;
    [SerializeField] int enemy2Rows = 2;
    [SerializeField] int enemy2Points = 20;

//Enemy 4th and 5th row bottom
    [SerializeField] Sprite enemy3;
    [SerializeField] Sprite enemy3b;
    [SerializeField] int enemy3Cols = 11;
    [SerializeField] int enemy3Rows = 2;
    [SerializeField] int enemy3Points = 10;
    [SerializeField] GameObject enemyGO;
    [SerializeField] float deltax = 1;
    [SerializeField] float deltay = 1;

    float currentY = 0;

//Barriers setup
    [SerializeField] Material barrierMaterial;
    [SerializeField] GameObject barrierGO;
    [SerializeField] Sprite barrier;
    [SerializeField] int barriers = 4;
    [SerializeField] Vector3 barriersDelta = new Vector3(1, 0, 0); //lui ha scritto barries
    [SerializeField] Vector3 barriersStart;

//Player
    [SerializeField] public GameObject player;
    
//POwerUp    
    [SerializeField] public GameObject powerUp;
    
//Score
    int score = 0;

//GUI
    //per scrivere il punteggio a schermo
    [SerializeField] TMP_Text scoreText; 
    [SerializeField] TMP_Text scoreBestText;
    
//Gamer over    
    [SerializeField] GameObject gameOverCanvas;
    
//ScriptableObject    
 public Stats stats; //per vedere se funziona il SO
 public Stats [] stats2; //per vedere se funziona il SO array

    void Start()
    {
        //imposto il punteggio 
        scoreBestText.text = $"hi-Score:\n{PlayerPrefs.GetInt("score")}"; 
        scoreText.text = $"Score:\n{score}";

        //gli do un colore
        Material e1Material = new Material(enemyMaterial);
        e1Material.color = new Color(0, 1, 0);

        //perogni riga e colonna di nemici
        for (int i = 0; i < enemy1Rows; i++)
        {
            for (int j = 0; j < enemy1Cols; j++)
            {
               //Debug.Log("Loop GM: " + i + " j:" + j);
               GameObject go = Instantiate(enemyGO); //istanzio il nemico
               go.SetActive(true);

               //accede al nemico e lo configura
               EnemyManager em = go.GetComponent<EnemyManager>();
               em.Configure(enemy1, enemy1b, e1Material, enemy1Points, this); //rendilo più parametrico: cioè lui fa 3 volte le stesse 3 cose per 3 le astro
                                                                                //2h:02 lez. 28 apr suggerimenti per "non fare this" e legare GM troppo coi scpritztz
               go.transform.position = new Vector3(j * deltax, currentY, 0);
               go.transform.parent = transform;
            }
            currentY -= deltay;
        }
        Debug.Log($"DY: {deltay}");
        Material e2Material = new Material(enemyMaterial);
        e2Material.color = new Color(1, 1, 0.5f);
        for (int i = 0; i < enemy2Rows; i++)
        {
            for (int j = 0; j < enemy2Cols; j++)
            {
                GameObject go = Instantiate(enemyGO);
                go.SetActive(true);
                EnemyManager em = go.GetComponent<EnemyManager>();
                em.Configure(enemy2, enemy2b, e2Material, enemy2Points, this); 
                go.transform.position = new Vector3(j * deltax, currentY, 0);
                go.transform.parent = transform;
            }
            currentY -= deltay;
        }
        Debug.Log($"DY: {deltay}");
        Material e3Material = new Material(enemyMaterial);
        e2Material.color = new Color(1, 0.5f, 0f);
        for (int i = 0; i < enemy2Rows; i++)
        {
            for (int j = 0; j < enemy2Cols; j++)
            {
                GameObject go = Instantiate(enemyGO);
                go.SetActive(true);
                EnemyManager em = go.GetComponent<EnemyManager>();
                //em.Configure(enemy3, enemy3b, e3Material, enemy3Points, this); 
                go.transform.position = new Vector3(j * deltax, currentY, 0);
                go.transform.parent = transform;
            }
            currentY -= deltay;
        }
        
        for (int i = 0; i < barriers; i++)
        {
            GameObject go = Instantiate(barrierGO);
            go.name = $"Barrier #{i}"; // per dare un nome al barrier sullo schermo
            BarrierManager em = go.GetComponent<BarrierManager>();
            //em.Configure(barrier,new Color(1,1,1), barrierMaterial);
            go.transform.position = new Vector3(barriersStart.x + i * barriersDelta.x, barriersStart.y, barriersStart.z); //rivedi formula l'hai rivista? 
        }                                                                                                                //non mentire a te stesso!
    }
    
    public void DidHitEnemy(int newPoints) 
    {
        score += newPoints;
        scoreText.text = $"Score:\n{score}";
    }

    public void GameOver() {
        player.GetComponent<PlayerManager>().enabled = false;
        gameOverCanvas.SetActive(true);
        int lastScore = PlayerPrefs.GetInt("score", 0);
        if (score > lastScore) 
        {
            //salvo il punteggio: PRIMA VOLTA CHE LO FAI vedi appunti 28 apr 2025
            PlayerPrefs.SetInt("score", score);
            PlayerPrefs.Save();
        }
    }

}
