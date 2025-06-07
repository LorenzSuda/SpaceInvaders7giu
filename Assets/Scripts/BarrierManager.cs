using UnityEngine;

public class BarrierManager : MonoBehaviour
{
    float deltaX = 1;
    float deltaY = 1;
    
    //anche qui dice della storia dell'interfaccia unica per il config. (refactorization)
    public void Configure(Sprite barrierShape, Color color, Material newMat)
    {
        int startX = (int)barrierShape.textureRect.xMin;
        int startY = (int)barrierShape.textureRect.yMin;

        int w = (int)barrierShape.textureRect.width;
        int h = (int)barrierShape.textureRect.height;

        //Debug.Log($"Image 1 size: {w}x{h} from:{startX}x{startY} to {startX*w}x{startY*h}");

        //Prepare to read pixels colors for frame1
        Color[] pixels = barrierShape.texture.GetPixels();

        float currentX = 0;
        float currentY = 0;

        //iterate image i -> stessa giostra del player e dell'enemy manager
        for (int i = startX; i < startX + w; i++)
        {
            for (int j = startY; j < startY + h; j++)
            {
                //Debug.LogFormat("EM step [0]:[1]-> [2] value: {3}",i,j, (i * j) + j, pixels[(i * j) + i]);
                Color c = barrierShape.texture.GetPixel(i, j);
                if (c.r != 0 && c.g != 0 && c.b != 0)
                {
                    //not black
                    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    go.tag = "Barrier";
                    go.AddComponent<FallOnHit>();
                    go.GetComponent<MeshRenderer>().material = newMat;
                    go.GetComponent<MeshRenderer>().material.color = color;
                    go.name = $"{i}:{j}:{pixels[(i * j) + i]}"; //rivedi spiegaz. interpolazione stringhe dentro enemy manager
                    go.transform.parent = transform; //make object child of this go
                    go.transform.position = new Vector3(currentX, currentY, 0);
                    //Debug.LogFormat("Img1 block at: {0} {1}", i, j);
                }
                currentY += deltaY;
            } 
            currentY = 0;
            currentX += deltaX;
        }
    }
}