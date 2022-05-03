using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New " + nameof(PilotData), menuName = "CharacterController/Create New " + nameof(PilotData))]
public class PilotData : ScriptableObject
{
	public float maxStableCrouchSpeed    = 5f;
	public float maxStableWalkSpeed      = 10f;
	public float maxStableSprintSpeed    = 15f;
	public float stableMovementSharpness = 15f;
	public float orientationSharpness    = 10f;
	public float crouchDownSpeed         = 10f;


	[Space(10)]
	[Header("Sliding")]
	public float maxStableSlideSpeed = 20f;
	public float slopeAccelerationSpeed = 10f;
	public float slideDrag              = 0.7f;
	public float slopeUpDrag            = 1.2f;
	[Range(-0.1f, 0.2f)]
	public float minSlideHeightDiff = 0.05f;
	[Range(0f, 89f)]
	public float slideGroundStickAngle = 60f;


	[Space(10)]
	[Header("Air Movement")]
	public float maxAirMoveSpeed = 15f;
	public float airAccelerationSpeed = 15f;
	public float drag                 = 0.1f;


	[Space(10)]
	[Header("Jumping")]
	public bool allowJumpingWhenSliding;
	[Space(5)]
	public float sprintJumpUpSpeed = 10f;
	public float walkJumpUpSpeed   = 10f;
	public float crouchJumpUpSpeed = 5f;
	[Space(5)]
	public float sprintForwardSpeed = 10f;
	public float walkForwardSpeed   = 10f;
	public float crouchForwardSpeed = 5f;
	[Space(5)]
	public float jumpPreGroundingGraceTime = 0.1f;
	public float jumpPostGroundingGraceTime = 0.1f;
	[Space(5)]
	public bool useDoubleJump;
	public bool  useCustomDJumpForces;
	public float dJumpUpSpeed      = 10f;
	public float dJumpForwardSpeed = 10f;
	[Space(5)]
	public bool useWallJump;
	public float wallBounceForce = 10f;
	public float wallJumpUpSpeed = 5f;

	[Space(10)]
	[Header("Wallrunning")]
	public bool useWallRun;
	public float     maxWallDistance    = 0.8f;
	public float     maxStableWallSpeed = 30f;
	public float     wallAcceleration   = 10f;
	public float     wallRunTime        = 3f;
	public float     wallStickiness     = 40f;
	public float     wallRunBanTime     = 1f;
	public LayerMask wallLayer;


	[Space(10)]
	[Header("FallStun")]
	public float stunThreshold = 55f;


	[Range(0f, 1f)]
	[Tooltip("Multiplies Current Velocity")]
	public float stunStrength = 0.2f;
	public float stunDuration = 0.2f;


	[Space(10)]
	[Header("Misc")]
	public List<Collider> ignoredColliders = new();
	public float   bonusOrientationSharpness = 10f;
	public Vector3 gravity                   = new(0, -30f, 0);
	public float   crouchedCapsuleHeight     = 1f;
}
