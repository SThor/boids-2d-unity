using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScript : MonoBehaviour
{
    public GameObject boidPrefab;
    public List<GameObject> boids = new List<GameObject>();
    public GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 pos = Camera.main.ScreenToWorldPoint(mousePos);
            GameObject boid = Instantiate(boidPrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
            boid.GetComponent<Boid>().allBoids = boids;
            boid.GetComponent<Boid>().target = target;
            boids.Add(boid);
        }
        if (Input.GetMouseButtonDown(1))
        {
            target.SetActive(!target.activeSelf);            
        }
    }
}
