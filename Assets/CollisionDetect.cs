using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetect : MonoBehaviour {
    public MassPoint massPoint;

    private void OnTriggerEnter(Collider other) {
        RaycastHit hit;
        print(massPoint.index);
        if (Physics.Raycast(transform.position, Vector3.up, out hit)) {
            Debug.Log("Point of contact: "+ hit.point);
            // print(hit.point - this.transform.position);
            massPoint.velocity += (hit.point - this.transform.position) / 50;
        }
        // print(hit.point);
    }
}
