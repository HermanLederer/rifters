﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviourPun
{
	//
	// Other components
	#region Other components
	private SphereCollider collider;
	private Rigidbody rigidbody;
	#endregion

	//
	// Editor Variables
	#region Editor variables
	public Player player;
	[Header("Movement")]
	public float maxSprintingSpeed = 12f;
	public float maxWalkingSpeed = 5f;
	public float maxFloatingSpeed = 2f;

	[Range(0f, 50f)] public float maxGroundAcceleration = 20f;
	[Range(0f, 50f)] public float maxGroundDeceleration = 10f;

	[Range(0f, 50f)] public float maxAirAcceleration = 5f;
	[Range(0f, 50f)] public float maxAirDeceleration = 5f;

	public float jumpPower = 8f;
	public float jumpCooldown = 0.1f;

	[Header("Prefabs")]
	public GameObject jumpingPlatfiormPrafab = null;
	#endregion

	//
	// Public variables
	#region Public variables
	[HideInInspector] public Vector3 spawnPoint;
	[HideInInspector] public Vector2 direction;
	#endregion

	//
	// Private variables
	#region Private variables
	private float nextJumpTime;
	private float _concuction;
	private Vector3 slopeNormal;
	private float slope;
	private bool isGrounded = true;
	#endregion

	//
	// Accessors
	#region Accessors
	private float Concuction
	{
		get
		{
			return Mathf.Clamp(_concuction - Time.time, 0, 1);
		}
		set
		{
			_concuction = Time.time + value;
		}
	}
	#endregion

	//--------------------------
	// MonoBehaviour events
	//--------------------------
	void Awake()
	{
		// Other components
		//player = GetComponent<Player>();
		collider = GetComponent<SphereCollider>();
		rigidbody = GetComponent<Rigidbody>();

		nextJumpTime = 0f;
		_concuction = 0f;
	}

	private void Start()
	{
		// Public variables
		spawnPoint = transform.position;
	}

	private void FixedUpdate()
	{
		//if (photonView.IsMine || player.isOfflinePlayer)
		//{
			// Controls
			ControlMovement();
		//}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!isGrounded)
		{
			Vector3 normal = collision.contacts[0].normal;

			if (Vector3.Angle(Vector3.up, normal) >= 90)
			{
				Debug.DrawRay(collision.contacts[0].point, normal, Color.red, 2f);
				rigidbody.AddForce(normal * rigidbody.velocity.magnitude * rigidbody.mass * collider.material.bounciness, ForceMode.Impulse);
			}
		}
	}

	//--------------------------
	// PlayerMovement methods
	//--------------------------
	private void ControlMovement()
	{
		// Direction
		float axisV = Input.GetAxisRaw("Vertical");
		float axisH = Input.GetAxisRaw("Horizontal");

		// Climbing the ground
		FollowGround();

		// Acceleration
		Vector3 horizontalVelocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);

		float targetSpeed = 0;

		if ((axisV != 0 || axisH != 0) && Concuction <= 0)
		{
			// rotating the model
			player.characterModel.transform.rotation = Quaternion.RotateTowards(player.characterModel.transform.rotation, player.orientationTransform.rotation, 600f * Time.deltaTime);

			// calculating direction vector
			Vector3 forwardMovement = player.orientationTransform.forward * axisV;
			Vector3 sidewaysMovement = player.orientationTransform.right * axisH;
			Vector3 targetDirection = forwardMovement + sidewaysMovement;
			targetDirection = targetDirection.normalized;

			// determining acceleration force and target speed
			float acceleration;
			if (isGrounded)
			{
				acceleration = maxGroundAcceleration;
				targetSpeed = maxSprintingSpeed;
				if (!Input.GetButton("Sprint")) targetSpeed = maxWalkingSpeed;
			}
			else
			{
				acceleration = maxAirAcceleration;
				targetSpeed = maxFloatingSpeed;
			}

			// accelerating
			Accelerate(targetDirection, acceleration * Time.fixedDeltaTime);
		}

		// Deceleration
		float deceleration;
		if (isGrounded) deceleration = maxGroundDeceleration;
		else deceleration = maxAirDeceleration;
		deceleration *= Time.fixedDeltaTime;

		float overhead = horizontalVelocity.magnitude - targetSpeed;
		Decelerate(Mathf.Clamp(overhead, 0, Mathf.Max(deceleration, deceleration * targetSpeed)));

		// Jumping
		if (isGrounded && Time.time >= nextJumpTime && Input.GetButton("Jump")) { Jump(); nextJumpTime = Time.time + jumpCooldown; }

		// Moving the model
		player.characterModel.transform.position = player.playerOrigin.position + Vector3.down * collider.radius;
	}

	private void Accelerate(Vector3 direction, float acceleration)
	{
		if (isGrounded) rigidbody.AddForce(direction * acceleration * rigidbody.mass, ForceMode.Impulse);
		else
		{
			float mag = Mathf.Max(new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z).magnitude, maxFloatingSpeed);
			rigidbody.AddForce(direction * acceleration * rigidbody.mass, ForceMode.Impulse);

			Vector3 vel = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
			vel = Vector3.ClampMagnitude(vel, mag);

			rigidbody.velocity = new Vector3(vel.x, rigidbody.velocity.y, vel.z);
		}
	}

	private void Decelerate(float deceleration)
	{
		// Only decelerates horizontally
		Vector3 horizontalVelocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
		horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, horizontalVelocity.magnitude - deceleration);
		rigidbody.velocity = new Vector3(horizontalVelocity.x, rigidbody.velocity.y, horizontalVelocity.z);
	}

	private void FollowGround()
	{
		isGrounded = false;

		if (rigidbody.velocity.y <= 0)
		{
			// Params
			float castingOffsetLength = 0.1f;
			float rayLength = castingOffsetLength  + 0.5f;

			Vector3 center = transform.position;
			center -= Vector3.down * castingOffsetLength;

			RaycastHit hit;
			if (Physics.SphereCast(center, collider.radius, Vector3.down, out hit, rayLength))
			{
				isGrounded = true;
			}
		}
	}
	
	private bool Jump()
	{
		rigidbody.AddForce(Vector3.up * jumpPower * rigidbody.mass, ForceMode.Impulse);
		return true;

		Vector3 center = transform.position + Vector3.up * collider.radius;

		RaycastHit hit;
		if (Physics.Raycast(center, -transform.up, out hit, 2f))
		{
			GameObject jumpingPlatfiorm = Instantiate(jumpingPlatfiormPrafab);
			jumpingPlatfiorm.GetComponent<PlayerJumpPlatform>().Shoot(hit.point);

			return true;
		}
		else
			return false;
	}
}