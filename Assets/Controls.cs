using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Controls : MonoBehaviour {

	public BoxCollider2D lowBoard;
	public BoxCollider2D highBoard;

	public float pushForce;
	public float jumpForce;
//	public float maxPushSpeed = 2f;
	public float pushCooldown;
	private bool onBoard;
//	private bool perpendicular;
	private float timeLastPush;
	private Animator animator;

	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck1;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_GroundCheck2;	
//	[SerializeField] private Transform m_CeilingCheck;							// A position marking where to check for ceilings
//	[SerializeField] private Collider2D m_CrouchDisableCollider;				// A collider that will be disabled when crouching
	const float k_GroundedRadius = 0.02f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;

	// Events
	public UnityEvent OnLandEvent;
	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }
	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;

	///////////////  -- CONTROLS --  ///////////////
	private KeyCombo c_push= new KeyCombo(new string[] {"keyA"}); 		// PUSH
	private KeyCombo c_crouch= new KeyCombo(new string[] {"down"}); 		// CROUCH
//	private KeyCombo c_perpendicular= new KeyCombo(new string[] {"right"}); 		// PERPENDICULAR
	private KeyCombo c_ollie= new KeyCombo(new string[] {"down", "up"}); 	// OLLIE
	private KeyCombo c_kickflip= new KeyCombo(new string[] {"down", "upright"}); 	// KICKFLIP
	private KeyCombo c_shoveit= new KeyCombo(new string[] {"left", "right"}); 	// SHOVE-IT
	private KeyCombo c_powerslide= new KeyCombo(new string[] {"JoyLeftClick", "right"}); 	// POWERSLIDE
	////////////////////////////////////////////////

	void Start(){
		onBoard = false;
		animator = GetComponent<Animator>();
	}

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}
	
	void FixedUpdate ()
	{
		GroundChecker();
		SlopeCheck();
		ColliderAdapter();
		ControlsManage();
	}

	void ControlsManage(){
		// PUSH
		if (c_push.Check()) Push();
		// CROUCH
		if (c_crouch.Check()) animator.SetBool("crouch", true);
		else animator.SetBool("crouch", false);

		// ON BOARD
		if(onBoard && m_Grounded){
			// OLLIE
			if (c_ollie.Check()) Ollie();
			if (c_shoveit.Check()) Shoveit();
			if (c_kickflip.Check()) Kickflip();
			if (c_powerslide.Check()) Powerslide();
			if (Input.GetButton("KeyB")){
				Brake();
			}
		}
	}

	void Push(){
		if(m_Grounded){ // Can only push while grounded
			if(!onBoard) onBoard = true; // 0 -> 1, char is getting on the board
			if(Time.time >= timeLastPush + pushCooldown){
				animator.SetTrigger("push");
				if(onBoard) m_Rigidbody2D.AddForce(new Vector2(pushForce, 0), ForceMode2D.Force); // Give forward force
				timeLastPush = Time.time; // Update cooldown
			}
		}
	}

	void Brake(){
		
	}





	///////////////////  ----- TRICKS -----  ///////////////////
	void Ollie(){
		animator.SetTrigger("ollie");
		if(m_Rigidbody2D.velocity.x < 1) m_Rigidbody2D.AddForce(new Vector2(5, jumpForce), ForceMode2D.Impulse);
		else m_Rigidbody2D.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
	}

	void Kickflip(){
		animator.SetTrigger("kickflip");
		m_Rigidbody2D.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
	}

	void Shoveit(){
		Debug.Log("shove-it");
		animator.SetTrigger("shove-it");
		timeLastPush = Time.time + 0.06f; // Cannot push during animation
		m_Rigidbody2D.AddForce(new Vector2(0, 15), ForceMode2D.Impulse);
	}

	void Powerslide(){
//		perpendicular = true;
		if(m_Rigidbody2D.velocity.x > 2f) animator.SetTrigger("perpendicular");
	}









	private void GroundChecker()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck1.position, k_GroundedRadius, ~0);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				animator.SetBool("grounded", true);
				if (!wasGrounded)
					OnLandEvent.Invoke();
				return;
			}
			
		}
		Collider2D[] colliders2 = Physics2D.OverlapCircleAll(m_GroundCheck2.position, k_GroundedRadius, ~0);
		for (int j = 0; j < colliders2.Length; j++)
		{
			if (colliders2[j].gameObject != gameObject)
			{
				m_Grounded = true;
				animator.SetBool("grounded", true);
				if (!wasGrounded)
					OnLandEvent.Invoke();
				return;
			}
		}
		animator.SetBool("grounded", false);
	}

	private void ColliderAdapter(){
		if(m_Grounded){
			highBoard.enabled = false;
			lowBoard.enabled = true; 
		}
		else{
			highBoard.enabled = true;
			lowBoard.enabled = false;
		}
	}

	private void SlopeCheck(){
		Collider2D[] colliders2 = Physics2D.OverlapCircleAll(m_GroundCheck2.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders2.Length; i++)
		{
			if (colliders2[i].gameObject != gameObject)
			{
				transform.rotation = colliders2[i].transform.rotation;
				return;
			}
		}
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck1.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				transform.rotation = colliders[i].transform.rotation;
				return;
			}
		}
	}
}
