using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Pilot", menuName = "CharacterController/Create New Pilot")]
public class Pilots : ScriptableObject
{
    [Header("Ground Movement")]
    public float MaxStableCrouchSpeed = 5f;
    public float MaxStableWalkSpeed = 10f;
    public float MaxStableSprintSpeed = 15f;
    public float StableMovementSharpness = 15f;
    public float OrientationSharpness = 10f;
    public float CrouchDownSpeed = 10f;

    [Space(10)]
    [Header("Sliding")]
    public float MaxStableSlideSpeed = 20f;
    public float SlopeAccelerationSpeed = 10f;
    public float SlideDrag = 0.7f;
    public float SlopeUpDrag = 1.2f;
    [Range(-0.1f, 0.2f)]
    public float MinSlideHeightDiff = 0.05f;
    [Range(0f, 89f)]
    public float SlideGroundStickAngle = 60f;

    [Space(10)]
    [Header("Air Movement")]
    public float MaxAirMoveSpeed = 15f;
    public float AirAccelerationSpeed = 15f;
    public float Drag = 0.1f;

    [Space(10)]
    [Header("Jumping")]
    public bool AllowJumpingWhenSliding = false;
    [Space(5)]
    public float SprintJumpUpSpeed = 10f;
    public float WalkJumpUpSpeed = 10f;
    public float CrouchJumpUpSpeed = 5f;
    [Space(5)]
    public float SprintForwardSpeed = 10f;
    public float WalkForwardSpeed = 10f;
    public float CrouchForwardSpeed = 5f;
    [Space(5)]
    public float JumpPreGroundingGraceTime = 0.1f;
    public float JumpPostGroundingGraceTime = 0.1f;
    [Space(5)]
    public bool UseDoubleJump = false;
    public bool useCustomDJumpForces = false;
    public float DJumpUpSpeed = 10f;
    public float DJumpForwardSpeed = 10f;
    [Space(5)]
    public bool UseWallJump = false;
    public float WallBounceForce = 10f;
    public float WallJumpUpSpeed = 5f;

    [Space(10)]
    [Header("Wallrunning")]
    public bool UseWallrun = false;
    public float MaxWallDistance = 0.8f;
    public float MaxStableWallSpeed = 30f;
    public float WallAcceleration = 10f;
    public float WallRunTime = 3f;
    public float WallStickiness = 40f;
    public float WallRunBanTime = 1f;
    public LayerMask WallLayer;

    [Space(10)]
    [Header("FallStun")]
    public float StunThreshold = 55f;
    [Range(0f, 1f)]
    [Tooltip("Multiplies Current Velocity")]
    public float StunStrenght = 0.2f;
    public float StunDuration = 0.2f;

    [Space(10)]
    [Header("Misc")]
    public List<Collider> IgnoredColliders = new List<Collider>();
    public float BonusOrientationSharpness = 10f;
    public Vector3 Gravity = new Vector3(0, -30f, 0);
    public float CrouchedCapsuleHeight = 1f;

}
