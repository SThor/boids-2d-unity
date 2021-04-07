using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    Vector3 velocity;
    Vector3 acceleration;
    public List<GameObject> allBoids;
    public float maxSpeed;
    public float maxForce;
    public GameObject target;
    public float behaviorDistance;
    public float wanderRadius;
    public float boidTooClose;

    void Start()
    {
        acceleration = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));
    }

    void FixedUpdate()
    {
        if (avoidWalls())
        {
            /*flee(new Vector3(0.5f, 0.5f));*/
            if (target.activeSelf)
            {
                seek(target.transform.position);
            }
            /*wander();*/
            avoidGroup();
        }
        computePosition();
    }

    void avoidGroup()
    {
        Vector3 sum = new Vector3();
        int count = 0;
        foreach(GameObject boid in allBoids)
        {
            float distance = Vector3.Distance(boid.transform.position, gameObject.transform.position);
            if(distance > 0 && distance < boidTooClose)
            {
                sum += (boid.transform.position - gameObject.transform.position).normalized;
                count++;
            }
        }

        if (count > 0)
        {
            sum /= count;
            sum = sum.normalized * maxForce;
            Vector3 steer = sum - velocity;
            steer = Vector3.ClampMagnitude(steer, maxForce);

            applyForce(steer);
        }
    }

    bool avoidWalls()
    {
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(gameObject.transform.position);
        float minX = 0.05f;
        float maxX = 1 - minX;
        float minY = 0.05f;
        float maxY = 1 - minY;
        bool res = true;
        Vector3 desired = new Vector3();
        if (viewportPosition.x < minX)
        {
            res = false;
            desired = new Vector3(1, velocity.y);
        }
        else if (viewportPosition.x > maxX)
        {
            res = false;
            desired = new Vector3(-1, velocity.y);
        }
        if (viewportPosition.y < minY)
        {
            res = false;
            desired = new Vector3(velocity.x, 1);
        }
        else if (viewportPosition.y > maxY)
        {
            res = false;
            desired = new Vector3(velocity.x, -1);
        }
        if (!res)
        {
            Vector3 steer = desired - velocity;
            steer = Vector3.ClampMagnitude(steer, maxForce);

            applyForce(steer);

        }
        return res;
    }

    void computePosition()
    {
        velocity += acceleration;
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        Quaternion newRotation = Quaternion.AngleAxis(angle, Vector3.forward);

        gameObject.transform.position += velocity;
        transform.rotation = newRotation;
        acceleration = Vector3.zero;
    }

    void applyForce(Vector3 force)
    {
        acceleration += force;
    }

    void wander()
    {
        Vector3 nextPosition = gameObject.transform.position + velocity;
        Vector3 wanderDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        Vector3 target = nextPosition + Vector3.ClampMagnitude(wanderDirection, wanderRadius);
        seek(target);
    }

    void seek(Vector3 target)
    {
        Vector3 desired = target - gameObject.transform.position;

        float distanceLeft = desired.magnitude;
        desired = desired.normalized;
        float multiplier;
        if (distanceLeft < behaviorDistance)
        {
            multiplier = map(0, behaviorDistance, 0,maxSpeed,distanceLeft);
        } else
        {
            multiplier = maxSpeed;
        }
        desired *= multiplier;
        

        Vector3 steer = desired - velocity;
        steer = Vector3.ClampMagnitude(steer, maxForce);

        applyForce(steer);
    }

    void flee(Vector3 target)
    {
        Vector3 desired = gameObject.transform.position - target;
        float distanceLeft = desired.magnitude;
        if (distanceLeft < behaviorDistance)
        {
            desired = desired.normalized;
            desired *= maxSpeed;

            Vector3 steer = desired - velocity;
            steer = Vector3.ClampMagnitude(steer, maxForce);

            applyForce(steer);
        }
    }

    public float map(float oldMin, float oldMax, float newMin, float newMax, float oldValue)
    {

        float oldRange = oldMax - oldMin;
        float newRange = newMax - newMin;
        float newValue = ((oldValue - oldMin) * newRange / oldRange) + newMin;

        return newValue;
    }
}
