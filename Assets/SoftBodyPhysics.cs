using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftBodyPhysics : MonoBehaviour {
    public GameObject massPointObj;
    // Mesh Properties
    public float g;
    public int width;
    public int height;

    private Mesh startMesh;
    private Mesh endMesh;
    private MeshRenderer renderer;
    private MassPoint[] massPoint;
    private Vector3[] vertexArray;
    private GameObject[] collisionPoints;
    // Spring Properties
    public float springConstant;
    public float length;
    public float dampingFactor; 
    public float massOfPoint;

    void Start() {
        CreateMesh();
        massPoint = new MassPoint[startMesh.vertices.Length]; // Define the size of the array same size of the mesh.
        for (int i = 0; i < startMesh.vertices.Length; i++) { // Loop through the all the vertices in the mesh and define a mass-point with index and intial position for each one of them.
            massPoint[i] = new MassPoint(i, transform.TransformPoint(startMesh.vertices[i]));
            collisionPoints[i].GetComponent<CollisionDetect>().massPoint = massPoint[i]; 
        }
    }

    void CreateMesh() {
        vertexArray = new Vector3[(width+1)*(height+1)];
        collisionPoints = new GameObject[(width+1)*(height+1)];
        startMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = startMesh;
        int i = 0;
        for (int y = 0; y <= height; y++) {
            for (int x = 0; x <= width; x++) {
                vertexArray[i] = new Vector3(x*length,-y*length,0);
                collisionPoints[i] = Instantiate(massPointObj, vertexArray[i], Quaternion.identity);
                collisionPoints[i].transform.localScale = new Vector3(0.05f,0.05f,0.05f);
                i += 1;
            }
        }
        startMesh.vertices = vertexArray;
    }

    void FixedUpdate() {
        for (int i = 0; i < massPoint.Length; i++) { // loops through ever mass-point
            Vector3 force = Vector3.zero;
            int n = 0;
            for (int y = 0; y <= 2; y++) { // Loops through every connected mass-point.
                for (int x = 0; x <= 2; x++) {
                    if (n != width + 2 && i - (width + 2) + n >= 0 && i - (width + 2) + n < massPoint.Length) {
                        if ( i % (width + 1) == 0 && x == 0 ) { // Far left and checking to the left stop
                            n += 1;
                            continue;
                        }
                        if ( (i+1) % (width + 1) == 0 && x == 2 ) { // Far right and checking to the right stop
                            n += 1;
                            continue;
                        }
                        if ( i <= width && y == 0 ) { // At top and checking above stop
                            n += 1;
                            continue;
                        }
                        if ( i >= massPoint.Length - width && y == 2 ) { // At buttom and checking bellow stop
                            n += 1;
                            continue;
                        } else {
                            force += SpringForce(massPoint[i], massPoint[i - (width + 2) + n]); // Calculated the spring forced for the linked mass-point
                        }
                    }
                    n += 1;
                }
                n += 2;
            }
            CollisionDetection(collisionPoints[i].transform.position);
            Vector3 pushVector = CollisionVelocity(collisionPoints[i].transform.position);
            massPoint[i].force = force + new Vector3(0,massOfPoint*g,0);
            massPoint[i].velocity += (massPoint[i].force / massOfPoint) + pushVector; 
            vertexArray[i] += massPoint[i].velocity; 
            massPoint[i].position = vertexArray[i];
            collisionPoints[i].transform.position = vertexArray[i];
        }
        startMesh.Clear();
        startMesh.vertices = vertexArray;
    }

    Vector3 SpringForce(MassPoint massPointA, MassPoint massPointB) {
        Vector3 positionA = massPointA.position;
        Vector3 positionB = massPointB.position;
        Vector3 direction = (positionB - positionA).normalized;
        Vector3 velocityDiff = massPointB.velocity - massPointA.velocity;
        float extention = Vector3.Distance(positionA, positionB) - length;
        float dampingForce = DampingForce(direction, velocityDiff);
        float springForce  = springConstant * extention;
        Vector3 force = (springForce + dampingForce) * direction;
        // Debug.DrawLine(positionA, positionA + (force), Color.green);
        return force;
    }

    float DampingForce(Vector3 direction, Vector3 velocityDiff) {
        return Vector3.Dot(direction, velocityDiff) * dampingFactor;
    }

    Vector3 CollisionVelocity(Vector3 position) {
        Vector3 pushVector = Vector3.zero;
        // if (position.y < -5) { // Bellow the floor
        //     pushVector = new Vector3(0,-5 - position.y,0);
        // }
        // if (position.y > 5) { // Above the ceiling
        //     pushVector = new Vector3(0,5 - position.y,0);
        // }
        // if (position.x < -11) { // To the left
        //     pushVector = new Vector3(-10 - position.x,0,0);
        // }
        // if (position.x > 11) { // To the right
        //     pushVector = new Vector3(10 - position.x,0,0);
        // }

        // if (position.y < -2.5 && position.y > -3.5 && position.x < 1 && position.x > -3.5) { // Bellow the floor
        //     pushVector = new Vector3(0,-2.5f - position.y,0);
        // }
        return pushVector;
    }

    void CollisionDetection(Vector3 position) {
        // if (Physics.Linecast(Vector3.zero, position))
        // {
        //     Debug.Log("blocked");
        // }
    }
    // Collision Detection
    // cast a ray from the origin to the point and check if it colides with and even or odd amounnt of surfaces 
    // if odd then it is in something 
    // if even it is not 
    // if odd loop through each face of the shade the it has collided with checking which side is the closest 
    // push the point out to the closest point with that push velocity.
}

    Vector3 SeperationForce(Vector3 positionA, Vector3 positionB) {
        // Push vectors away from each other if they are closer than a given distance.
    }

public class MassPoint : MonoBehaviour {
    public int index;
    public Vector3 position;
    public Vector3 velocity;
    public Vector3 force;
    public GameObject sphere;

    public MassPoint(int _index, Vector3 _position) {
        index = _index;
        position = _position;
    }
}
