using UnityEngine;

public class MisteryShipManager : MonoBehaviour
{
    [SerializeField] Vector3 startPosition;
    [SerializeField] Vector2 endPosition;
    [SerializeField] Sprite ship;
    [SerializeField] Color color = Color.red;
    [SerializeField] float speed = 1000;
    [SerializeField] Material shipMaterial;
    [SerializeField] Material shipEmissiveMaterial;
    [SerializeField] GameManager manager;
    [SerializeField] int[] hitPoints = { 50, 100, 150, 200, 300 };
    
    GameObject shipShape;
    float deltaX = 1;
    float deltaY = 1;
    bool animate = false;
    Renderer[] renderers;
    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        shipShape = new GameObject();
        shipShape.name = "MisteryShip";
        shipShape.transform.parent = transform;
        PlayerDestroyOnHit pdh = shipShape.AddComponent<PlayerDestroyOnHit>();
        pdh.Manager = manager;

        int startX = (int)ship.textureRect.xMin;
        int startY = (int)ship.textureRect.yMin;
        int w = (int)ship.textureRect.width;
        int h = (int)ship.textureRect.height;

        Color[] pixels = ship.texture.GetPixels();
        float currentX = 0;
        float currentY = 0;

        for (int i = startX; i < startX + w; i++)
        {
            for (int j = startY; j < startY + h; j++)
            {
                Color c = ship.texture.GetPixel(i, j);
                if (c.r != 0 && c.g != 0 && c.b != 0)
                {
                    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    go.GetComponent<MeshRenderer>().material = shipEmissiveMaterial;
                    go.name = $"{i}:{j}:{pixels[(i * j) + i]}";
                    go.transform.position = new Vector3(currentX, currentY, 0);
                    go.transform.parent = shipShape.transform;
                }
                currentY += deltaY;
            }
            currentY = 0;
            currentX += deltaX;
        }

        MeshMerger mm = shipShape.AddComponent<MeshMerger>();
        mm.Configure(shipMaterial, false);
        transform.position = startPosition;
        transform.localScale = new Vector3(2, 2, 2);

        renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            r.enabled = false;
        }

        Animate();
    }

    void Animate()
    {
        foreach (Renderer r in renderers)
        {
            r.enabled = true;
        }
        transform.position = startPosition;
        animate = true;
    }

    private void Update()
    {
        if (!animate) return;
        transform.Translate(speed * Time.deltaTime * Vector3.left);

        if (Vector3.Distance(endPosition, transform.position) <= 10f)
        {
            HideShip();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            HideShip();
            int points = hitPoints[Random.Range(0, hitPoints.Length)];
            manager.DidHitEnemy(points);
        }
    }

    private void HideShip()
    {
        animate = false;
        foreach (Renderer r in renderers)
        {
            r.enabled = false;
        }
        Invoke("Animate", Random.Range(5, 15));
    }
}