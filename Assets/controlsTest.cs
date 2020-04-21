using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class controlsTest : MonoBehaviour {

	public GameObject player;
	void FixedUpdate () {
		int speed = (int) (player.GetComponent<Rigidbody2D>().velocity.x /2 *10);
		GetComponent<Text>().text = "SPEED:\n" + speed;
	}
}
