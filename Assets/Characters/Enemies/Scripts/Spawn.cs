using UnityEngine;
using UnityEngine.AI;

public class Spawn : MonoBehaviour
{
    public GameObject cyborgPrefab;
    public int number;
    public float spawnRadius;
    public bool SpawnOnStart = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(SpawnOnStart)
            SpawnAll();
    }

    void SpawnAll()
    {
        for (int i = 0; i < number; i++)
        {
            Vector3 randomPoint = this.transform.position + Random.insideUnitSphere * spawnRadius;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 10.0f, NavMesh.AllAreas))
            {
                Instantiate(cyborgPrefab, hit.position, Quaternion.identity);
            }
            else
                i--;
        }
    }


    void OnTriggerEnter(Collider collider)
    {
        if(!SpawnOnStart && collider.gameObject.tag == "Player")
            SpawnAll(); 
    }
}
