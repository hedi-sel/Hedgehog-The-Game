using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {
    // This class informs the Gameplay when an obstacle is hit by the character

    private void OnTriggerEnter (Collider other) {
        if (other.gameObject.tag == "Player") //We check if the object that just entered is the Hedgehog
            Gameplay.Instance.HitObstacle();
    }
}
