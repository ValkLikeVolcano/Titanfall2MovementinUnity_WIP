﻿using KinematicCharacterController;
using UnityEngine;

namespace N1C_Movement
{
	public class CharacterController : MonoBehaviour, ICharacterController
	{
		public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
		{
			throw new System.NotImplementedException();
		}
		public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
		{
			throw new System.NotImplementedException();
		}
		public void BeforeCharacterUpdate(float deltaTime)
		{
			throw new System.NotImplementedException();
		}
		public void PostGroundingUpdate(float deltaTime)
		{
			throw new System.NotImplementedException();
		}
		public void AfterCharacterUpdate(float deltaTime)
		{
			throw new System.NotImplementedException();
		}
		public bool IsColliderValidForCollisions(Collider coll) => throw new System.NotImplementedException();
		public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
		{
			throw new System.NotImplementedException();
		}
		public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
		{
			throw new System.NotImplementedException();
		}
		public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
		{
			throw new System.NotImplementedException();
		}
		public void OnDiscreteCollisionDetected(Collider hitCollider)
		{
			throw new System.NotImplementedException();
		}
	}
}
