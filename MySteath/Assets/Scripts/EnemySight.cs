﻿using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class EnemySight : MonoBehaviour
{
	public float fieldOfViewAngle = 110f; // FOV角度
	public bool playerInSight;     // 是否发现角色
	public Vector3 personalLastSighting; //

	private NavMeshAgent nav; // 导航网格代理对象，用于自动寻找角色
	private SphereCollider col; // 触发器对象
	private Animator anim; // Animator对象，这里是EnemyAnimator
	private LastPlayerSighting lastPlayerSighting; //
	private GameObject player; // 角色游戏对象
	private Animator playerAnim; // 角色Animator对象，这里是PlayerAnimator
	private PlayerHealth playerHealth; // 角色生命值对象
	private HashIDs hash; // HashIDs对象
	private Vector3 previousSighting; // 上次发现角色时角色的位置


	void Awake()
	{
		nav = GetComponent<NavMeshAgent>();
		col = GetComponent<SphereCollider>();
		anim = GetComponent<Animator>();
		lastPlayerSighting = GameObject.FindWithTag(Tags.GameController).GetComponent<LastPlayerSighting>();
		player = GameObject.FindWithTag(Tags.Player);
		playerAnim = player.GetComponent<Animator>();
		playerHealth = player.GetComponent<PlayerHealth>();
		hash = GameObject.FindWithTag(Tags.GameController).GetComponent<HashIDs>();

		personalLastSighting = lastPlayerSighting.resetPosition;
		previousSighting = lastPlayerSighting.resetPosition;
	}

	float CalculatePathLength(Vector3 targetPosition)
	{
		NavMeshPath path = new NavMeshPath();
		if (nav.enabled)
			nav.CalculatePath(targetPosition, path);

		Vector3[] allWayPoints = new Vector3[path.corners.Length + 2];

		allWayPoints[0] = transform.position;
		allWayPoints[allWayPoints.Length - 1] = targetPosition;

		for (int i = 0; i < path.corners.Length; i++)
			allWayPoints[i + 1] = path.corners[i];

		float pathLength = 0;
		for (int i = 0; i < allWayPoints.Length - 1; i++)
			pathLength += Vector3.Distance(allWayPoints[i], allWayPoints[i + 1]);

		return pathLength;
	}

	void OnTriggerStay(Collider other)
	{
		if (other.gameObject == player)
		{
			playerInSight = false;

			Vector3 direction = other.transform.position - transform.position;
			float angle = Vector3.Angle(direction, transform.forward);

			Debug.Log(angle);
			if (angle < fieldOfViewAngle * 0.5f)
			{
				RaycastHit hit;
				if (Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, col.radius))
				{
					if (hit.collider.gameObject == player)
					{
						playerInSight = true;
						lastPlayerSighting.position = player.transform.position;
					}
				}
			}

			int state0 = playerAnim.GetCurrentAnimatorStateInfo(0).fullPathHash;
			int state1 = playerAnim.GetCurrentAnimatorStateInfo(1).fullPathHash;

			if (state0 == hash.locomotionState || state1 == hash.shoutState)
			{
				if (CalculatePathLength(player.transform.position) <= col.radius)
					personalLastSighting = player.transform.position;
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject == player)
			playerInSight = false;
	}

	void Update()
	{
		if (lastPlayerSighting.position != previousSighting)
			personalLastSighting = lastPlayerSighting.position;

		previousSighting = lastPlayerSighting.position;

		Debug.Log(playerInSight);
		if (playerHealth.health > 0f)
			anim.SetBool(hash.playerInSightBool, playerInSight);
		else
			anim.SetBool(hash.playerInSightBool, false);
	}

}
