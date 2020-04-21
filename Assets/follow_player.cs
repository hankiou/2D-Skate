using UnityEngine;

public class follow_player : MonoBehaviour {

	public Transform player;
	public float smooth = 3f;
	public float smoothSpeed = 0.650f;
	public float rotationOffsetX = 11.2f;
	public Vector3 Offset;

	void FixedUpdate ()
	{
		Offset.x = 3f + (player.GetComponent<Rigidbody2D>().velocity.x / 10);


		float tempPos = transform.position.y;
		Vector2 position = player.position + Offset;
		if(player.position.y + Offset.y > transform.position.y) position.y = tempPos;
		Vector2 smoothedPosition = Vector2.Lerp(transform.position, position, smoothSpeed);
		transform.position = smoothedPosition;

	}
}
