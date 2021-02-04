using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour {
    // This class informs the Gameplay when the finish line is hit by the character

    private void OnTriggerEnter (Collider other) {
        if (other.gameObject.tag == "Player") //We check if it the hedgehog that just entered
            Gameplay.Instance.CrossFinishLine();
    }
}
