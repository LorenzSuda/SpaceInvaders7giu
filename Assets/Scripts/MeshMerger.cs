using UnityEngine;

//SI VABBEH MA SE N'A SAI SALLA STA CLASSE, IN PRATICA: so tutte "librerie" su cui si basa per fare la fusione...
//...vabbeh, un giorno quando sarai grande.

//Il MeshMerger Trasforma in una geometria unica più geomterie: trasforma tutti i voxel figli in un'unica mesh
public class MeshMerger : MonoBehaviour
{
    //infatti il config. prende tutti i mesh filter figli e li unisce in uno solo (unendo tutti i triangoli).
    public void Configure(Material mat, bool destroyChildren = true, bool addCollider = true)  //gli passa un mat
    {
        Debug.Log($"Merging {gameObject}", this);
        
        if (!gameObject.GetComponent<MeshRenderer>()) //se non c'è...
            gameObject.AddComponent<MeshRenderer>(); //...lo aggiunge in pratica perché infatti fa..
        
        GetComponent<MeshRenderer>().material = mat; //...e gli assegna il mat passato al meshrender***

        // Create a single mesh joining all children meshes
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();//***+
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];//+++*clicca su combineInstance ma detta male qui sotto:
        int i = 0;
        while (i < meshFilters.Length) 
        {
            combine[i].mesh = meshFilters[i].sharedMesh; //gli da un ID alle mesh che "trova"?! (è trova? booh)
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix; //je da una transform e li mette in matrice: insomma li piazza me pare
            meshFilters[i].gameObject.SetActive(false); //disabilita i figli, così non li vedi più (e non li vedi più perché poi li distrugge [riattivandoli])
            i++;
        }

        // *-*-*-*NOTE: MeshFilter must be created after querying
        // the ComponentInChildren to avoid
        // returning also the current EMPTY NEW mesh filter
        MeshFilter mf;
        if (!gameObject.GetComponent<MeshFilter>()) 
        {
            gameObject.AddComponent<MeshFilter>(); 
        }
        mf = gameObject.GetComponent<MeshFilter>(); // e poi setti il mf al gameObject corrente

        // Create final mesh
        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combine); //QUI C'È LA FUSIONE VERA E PROPRIA!!! (fusione di CombineInstances)
        mf.sharedMesh = mesh;
        transform.gameObject.SetActive(true); //riattiva il gameObject corrente (quello che ha lo script MeshMerger)

        foreach (MeshFilter child in meshFilters) 
        {
            if (destroyChildren) 
            {
                // Destroy all children
                Destroy(child.gameObject);
            }
            else 
            {
                child.gameObject.SetActive(false);  //vedi se ridondante, commenta else e gioca.
            }
        }

        // Add global collider to detect hit by player bullet
        if (addCollider) 
        {
            BoxCollider bc = gameObject.AddComponent<BoxCollider>();
        }

        Destroy(this);
    }
}
//***DIGRESSIONI E NOTE:
//Quando MeshRenderer invoca material, sta accedendo a una proprietà ereditata dalla classe base Renderer.
//La proprietà material di Renderer restituisce il materiale attualmente assegnato al renderer e consente di modificarlo

//***+ ricorda, questo come er vangelo al momento: il MeshFilter definisce la forma dell'oggetto,
//mentre il MeshRenderer si occupa di renderizzarlo visivamente nella scena
//Contenere la mesh: Il MeshFilter ha una proprietà chiamata mesh o sharedMesh che contiene i dati della geometria
//(vertici, triangoli, UV, ecc.).
//Collaborare con il MeshRenderer: Il MeshFilter fornisce la geometria che il MeshRenderer utilizza
//per disegnare l'oggetto nella scena.

//+++*CombineInstance: È una struttura di Unity utilizzata per combinare più mesh in una singola mesh.
//Ogni elemento di questo array rappresenta una mesh da combinare, includendo informazioni come
//la mesh stessa e la sua trasformazione

#region GetCompInChild
//Il metodo GetComponentsInChildren<MeshFilter>() serve per ottenere tutti i componenti di tipo MeshFilter presenti nei GameObject figli
//(incluso il GameObject corrente, se ha un MeshFilter).
//Utilità:
//Raccogliere tutte le mesh: Questo metodo permette di raccogliere tutte le mesh associate ai figli di un oggetto,
//per poi combinarle in un'unica mesh.
//Iterare sui figli: È utile quando devi lavorare su tutti i componenti di un certo tipo presenti nella gerarchia di un GameObject.
//Nel contesto del tuo codice, viene usato per raccogliere tutti i MeshFilter dei figli, così da poter combinare le loro mesh in una mesh unica.

//*-*-*-*Il MeshFilter deve essere creato DOPO aver eseguito GetComponentsInChildren<MeshFilter>() 
//per evitare che il nuovo MeshFilter appena aggiunto venga incluso nei risultati della query.
//Quando chiami GetComponentsInChildren<MeshFilter>(),
//Unity restituisce tutti i MeshFilter presenti nei figli (e nel GameObject corrente, se applicabile).
//Se aggiungi un nuovo MeshFilter prima di questa chiamata,
//Unity includerà anche questo nuovo componente nell'elenco restituito, anche se è vuoto.
//Questo comportamento può causare problemi, come tentare di combinare una mesh vuota o non necessaria.
//Creandolo dopo, ti assicuri che il nuovo MeshFilter non interferisca con il processo di fusione delle mesh.
#endregion