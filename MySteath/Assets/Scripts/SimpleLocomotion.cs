﻿using UnityEngine;
using System.Collections;

public class SimpleLocomotion
{
	public float speedDampTime = 0.1f;
	public float angularSpeedDampTime = 0.7f;
	public float angleResponseTime = 0.6f;

	private readonly Animator anim;
	private readonly HashIDs hash;

	public SimpleLocomotion(Animator animator, HashIDs hashIDs)
	{
		anim = animator;
		hash = hashIDs;
	}

	public void Do(float speed, float angle)
	{
		float angularSpeed = angle / angleResponseTime;

		anim.SetFloat(hash.speedFloat, speed, speedDampTime, Time.deltaTime);
		anim.SetFloat(hash.angularSpeedFloat, angularSpeed, angularSpeedDampTime, Time.deltaTime);
	}

}
