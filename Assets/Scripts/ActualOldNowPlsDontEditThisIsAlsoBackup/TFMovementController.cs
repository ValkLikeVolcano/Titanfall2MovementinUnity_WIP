using System.Collections;
using UnityEngine;
using KinematicCharacterController;

namespace N1C_Movement_Old
{
    public struct PlayerCharacterInputs
    {
        public float MoveAxisForward;
        public float MoveAxisRight;
        public Quaternion CameraRotation;
        public bool JumpDown;
        public bool SprintDown;
        public bool SprintUp;
        public bool CrouchDown;
        public bool CrouchUp;
    }

    public class TFMovementController : MonoBehaviour, ICharacterController
    {
        private KinematicCharacterMotor Motor;
        public Pilots p;

        [SerializeField] private Transform MeshRoot;
        [SerializeField] private TFMovementCamera PlayerCam;
        public Transform CameraFollowPoint;

        private float MaxStableSpeed;
        private float MinStableSlideSpeed;
        private float JumpUpSpeed;
        private float JumpScalableForwardSpeed;

        private float MaxDefaultSnappingAngle;
        private Collider[] _probedColliders = new Collider[8];
        private Vector3 _moveInputVector;
        private Vector3 _lookInputVector;

        //Wall Checks
        RaycastHit _wallHit;
        private bool _onWall;
        private bool _front, _canWallRun;

        Coroutine _tiltCamCoroutine;

        Ticker _wallRunTicker;

        //Jump Checks
        private bool _jumpRequested = false;
        private bool _jumpConsumed = false;
        private bool _doubleJumpConsumed = false;
        private bool _canWallJump = false;
        private Vector3 _wallJumpNormal;
        private bool _jumpedThisFrame = false;
        private float _timeSinceJumpRequested = Mathf.Infinity;
        private float _timeSinceLastAbleToJump = 0f;
        private Vector3 _internalVelocityAdd = Vector3.zero;
        private Vector3 DeltaHeight = Vector3.zero;

        //State Checks
        private bool _isSprinting = false;
        private bool _shouldBeSprinting = false;
        private bool _SprintCached = false;
        private bool _shouldBeCrouching = false;
        private bool _isCrouching = false;
        private bool _isChangingCrouchPosition = false;
        private bool _isVisuallyCrouching = false;
        private bool _isWallrunning = false;

        //Fall Stun
        private bool _getMag, _cutVelocity;
        private float _highestMag = 0f;

        private void Awake()
        {
            Motor = GetComponent<KinematicCharacterMotor>();
            Motor.CharacterController = this;
            MaxStableSpeed = p.MaxStableWalkSpeed;
            JumpUpSpeed = p.WalkJumpUpSpeed;
            JumpScalableForwardSpeed = p.WalkForwardSpeed;
            MinStableSlideSpeed = p.MaxStableCrouchSpeed + 0.4f;
            MaxDefaultSnappingAngle = Motor.MaxStableDenivelationAngle;
            _canWallRun = true;

            _wallRunTicker = new Ticker();
        }

        private void Update()
        {
            // so where am I going to write it XD like where should I check time 
            if (!Motor.GroundingStatus.IsStableOnGround && p.UseWallrun && _canWallRun)
            {
                _onWall = CheckWalls(out _wallHit);
                
                if(_onWall)
                {
                    float angle = Vector3.Angle(_wallHit.normal, Vector3.up);
                    print (angle);
                    if (angle > Motor.MaxStableSlopeAngle && angle < 120f)
                    {
                        //print($"on {_wallHit.collider.name}");

                        if (!_isWallrunning)
                        {
                            _isWallrunning = true;

                            float dotRightAndNormal = Vector3.Dot(_wallHit.normal, Motor.CharacterRight);

                            var left = dotRightAndNormal > 0;

                            if(_tiltCamCoroutine != null)
                                StopCoroutine(_tiltCamCoroutine);
                            _tiltCamCoroutine = StartCoroutine(PlayerCam.TiltCam(left ? -1 : 1));
                        }
                        //Front Approach
                    }
                }
                // CheckWalls(out _wallPoint,out _wallNormal, out _left, out _right, out _front);
            }
        }

        public void SetInputs(in PlayerCharacterInputs inputs)
        {
            Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputs.MoveAxisRight, 0f, inputs.MoveAxisForward), 1f);

            Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.forward, Motor.CharacterUp).normalized;

            if (cameraPlanarDirection.sqrMagnitude == 0f)
            {
                cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.up, Motor.CharacterUp).normalized;
            }
            Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Motor.CharacterUp);

            _moveInputVector = cameraPlanarRotation * moveInputVector;
            _lookInputVector = cameraPlanarDirection;

            if (inputs.JumpDown)
            {
                _timeSinceJumpRequested = 0f;
                _jumpRequested = true;
            }

            if (inputs.SprintDown)
            {
                _shouldBeCrouching = false;
                _shouldBeSprinting = true;
                _SprintCached = true;
            }
            else if (inputs.SprintUp)
            {
                _shouldBeSprinting = false;
                _SprintCached = false;
            }

            //Crouching
            if (inputs.CrouchDown)
            {
                _shouldBeCrouching = true;
                _shouldBeSprinting = false;

                if (!_isCrouching && Motor.GroundingStatus.IsStableOnGround)
                {
                    _isSprinting = false;
                    Motor.SetCapsuleDimensions(0.5f, p.CrouchedCapsuleHeight, p.CrouchedCapsuleHeight * 0.5f);
                    _isCrouching = true;
                }
            }
            else if (inputs.CrouchUp)
            {
                _shouldBeCrouching = false;
            }
        }

        #region ICharacterController
        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            if (_lookInputVector.sqrMagnitude > 0f && p.OrientationSharpness > 0f)
            {
                // Smoothly interpolate from current to target look direction
                Vector3 smoothedLookInputDirection = Vector3.Slerp(Motor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-p.OrientationSharpness * deltaTime)).normalized;

                // Set the current rotation (which will be used by the KinematicCharacterMotor)
                currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, Motor.CharacterUp);
            }

            Vector3 currentUp = (currentRotation * Vector3.up);

            Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, Vector3.up, 1 - Mathf.Exp(-p.OrientationSharpness * deltaTime));
            currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;

        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            #region Ground Movement

            if (Motor.GroundingStatus.IsStableOnGround && !_isWallrunning)
            {

                //Sliding
                if (_isCrouching && Motor.Velocity.magnitude >= MinStableSlideSpeed)
                {
                    //Increase Ground Stickyness
                    Motor.MaxStableDenivelationAngle = p.SlideGroundStickAngle;

                    Vector3 effectiveGroundNormal = Motor.GroundingStatus.GroundNormal;

                    currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocity.magnitude;


                    //Check Height Difference
                    if (Motor.LastGroundingStatus.IsStableOnGround && (DeltaHeight.y - transform.position.y) >= p.MinSlideHeightDiff)
                    {
                        //Add Velocity on Slope
                        if (currentVelocity.magnitude <= p.MaxStableSlideSpeed)
                        {
                            currentVelocity += Motor.CharacterForward * p.SlopeAccelerationSpeed * deltaTime;
                        }
                    }
                    else if (Motor.LastGroundingStatus.IsStableOnGround && (DeltaHeight.y - transform.position.y) < -p.MinSlideHeightDiff)
                    {
                        currentVelocity *= 1f / (1f + (p.SlopeUpDrag * deltaTime));
                    }

                    //Add Drag with Grace Time for Bhopping
                    else if (Motor.LastGroundingStatus.IsStableOnGround && ((DeltaHeight.y - transform.position.y) < p.MinSlideHeightDiff || (DeltaHeight.y - transform.position.y) > -p.MinSlideHeightDiff))
                    {
                        currentVelocity *= 1f / (1f + (p.SlideDrag * deltaTime));
                    }
                }
                //Walk
                else
                {
                    Motor.MaxStableDenivelationAngle = MaxDefaultSnappingAngle;

                    float currentVelocityMagnitude = currentVelocity.magnitude;

                    Vector3 effectiveGroundNormal = Motor.GroundingStatus.GroundNormal;

                    if (currentVelocityMagnitude > 0f && Motor.GroundingStatus.SnappingPrevented)
                    {
                        // Take the normal from where we're coming from
                        Vector3 groundPointToCharacter = Motor.TransientPosition - Motor.GroundingStatus.GroundPoint;
                        if (Vector3.Dot(currentVelocity, groundPointToCharacter) >= 0f)
                        {
                            effectiveGroundNormal = Motor.GroundingStatus.OuterGroundNormal;
                        }
                        else
                        {
                            effectiveGroundNormal = Motor.GroundingStatus.InnerGroundNormal;
                        }
                    }

                    // Reorient velocity on slope
                    currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocityMagnitude;

                    // Calculate target velocity
                    Vector3 inputRight = Vector3.Cross(_moveInputVector, Motor.CharacterUp);
                    Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * _moveInputVector.magnitude;
                    Vector3 targetMovementVelocity = reorientedInput * MaxStableSpeed;

                    // Smooth movement Velocity
                    currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-p.StableMovementSharpness * deltaTime));
                }
            }

            #endregion

            #region Wallrun Movement

            else if(_isWallrunning)
            {
                Motor.GroundDetectionExtraDistance = 2f;

                Vector3 currentVelOnPlane = new Vector3(currentVelocity.x, 0f, currentVelocity.z);

                _wallRunTicker.Tick(Time.deltaTime);

                if (_shouldBeCrouching || Motor.GroundingStatus.IsStableOnGround ||
                    currentVelOnPlane.magnitude < 3f || _wallRunTicker.Span >= p.WallRunTime || !_onWall)
                {
                    Motor.GroundDetectionExtraDistance = 0f;
                    _wallRunTicker.Reset();
                    _isWallrunning = false;
                    StartCoroutine(WallRunBanTime());
                    _doubleJumpConsumed = false;
                    StopCoroutine(_tiltCamCoroutine);
                    _tiltCamCoroutine = StartCoroutine(PlayerCam.TiltCam(0));
                    _highestMag = 0f;
                } 
                else
                {
                    Vector3 effectiveWallNormal = _wallHit.normal;

                    Vector3 direction = _wallHit.point - Motor.Transform.position;

                    float dotForwardAndInput = Vector3.Dot(Motor.CharacterForward, _moveInputVector);

                    currentVelocity += direction.normalized * p.WallStickiness * Mathf.Clamp(direction.magnitude / p.MaxWallDistance, 0f, 1f) * deltaTime;

                    Vector3 wallTanget = Vector3.Cross(effectiveWallNormal, Motor.CharacterUp);

                    var resultMoveInputVector = _moveInputVector;

                    // if going backwards
                    if(dotForwardAndInput < 0)
                    {
                        resultMoveInputVector *= .3f;
                    }

                    Vector3 inputRight = Quaternion.AngleAxis(-90, wallTanget) * resultMoveInputVector;
                    inputRight.y = inputRight.y * 0.25f;
                    
                    Vector3 targetMovementVelocity = inputRight * p.MaxStableWallSpeed;

                    if(_wallRunTicker.Span >= p.WallRunTime * 0.5f) currentVelocity += p.Gravity * 0.8f * deltaTime;

                    // Smooth movement Velocity
                    currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-p.WallAcceleration * deltaTime));

                }
            }

            #endregion

            #region Air Movement

            else
            {
                // Add move input
                if (_moveInputVector.sqrMagnitude > 0f)
                {
                    Vector3 addedVelocity = _moveInputVector * p.AirAccelerationSpeed * deltaTime;

                    Vector3 currentVelocityOnInputsPlane = Vector3.ProjectOnPlane(currentVelocity, Motor.CharacterUp);

                    // Limit air velocity from inputs
                    if (currentVelocityOnInputsPlane.magnitude < p.MaxAirMoveSpeed)
                    {
                        // clamp addedVel to make total vel not exceed max vel on inputs plane
                        Vector3 newTotal = Vector3.ClampMagnitude(currentVelocityOnInputsPlane + addedVelocity, p.MaxAirMoveSpeed);
                        addedVelocity = newTotal - currentVelocityOnInputsPlane;
                    }
                    else
                    {
                        // Make sure added vel doesn't go in the direction of the already-exceeding velocity
                        if (Vector3.Dot(currentVelocityOnInputsPlane, addedVelocity) > 0f)
                        {
                            addedVelocity = Vector3.ProjectOnPlane(addedVelocity, currentVelocityOnInputsPlane.normalized);
                        }
                    }

                    // Prevent air-climbing sloped walls
                    if (Motor.GroundingStatus.FoundAnyGround)
                    {
                        if (Vector3.Dot(currentVelocity + addedVelocity, addedVelocity) > 0f)
                        {
                            Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal), Motor.CharacterUp).normalized;
                            addedVelocity = Vector3.ProjectOnPlane(addedVelocity, perpenticularObstructionNormal);
                        }
                    }

                    // Apply added velocity
                    currentVelocity += addedVelocity;
                }

                // Gravity
                currentVelocity += p.Gravity * deltaTime;

                // Drag
                currentVelocity *= 1f / (1f + (p.Drag * deltaTime));
            }

            #endregion

            #region Handle jumping

            _jumpedThisFrame = false;
            _timeSinceJumpRequested += deltaTime;
            if (_jumpRequested)
            {
                // Handle double jump
                if (p.UseDoubleJump)
                {
                    if (!_doubleJumpConsumed && (p.AllowJumpingWhenSliding ? !Motor.GroundingStatus.FoundAnyGround : !Motor.GroundingStatus.IsStableOnGround))
                    {
                        Motor.ForceUnground(0.1f);

                        if (p.useCustomDJumpForces)
                        {
                            currentVelocity += (Motor.CharacterUp * p.DJumpUpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                            currentVelocity += (_moveInputVector * p.DJumpForwardSpeed);
                        }
                        else
                        {
                            currentVelocity += (Motor.CharacterUp * JumpUpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                            currentVelocity += (_moveInputVector * JumpScalableForwardSpeed);
                        }
                        // Add to the return velocity and reset jump state
                        _jumpRequested = false;
                        _doubleJumpConsumed = true;
                        _jumpedThisFrame = true;
                        _highestMag = 0f;
                    }
                }

                // See if we actually are allowed to jump
                if (_canWallJump || (!_jumpConsumed && ((p.AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround) || _timeSinceLastAbleToJump <= p.JumpPostGroundingGraceTime)))
                {
                    // Calculate jump direction before ungrounding
                    Vector3 jumpDirection = Motor.CharacterUp;
                    if (_canWallJump)
                    {
                        jumpDirection = _wallJumpNormal;
                    }
                    else if (Motor.GroundingStatus.FoundAnyGround && !Motor.GroundingStatus.IsStableOnGround)
                    {
                        jumpDirection = Motor.GroundingStatus.GroundNormal;
                    }

                    // Makes the character skip ground probing/snapping on its next update. 
                    // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
                    Motor.ForceUnground(0.1f);

                    // Add to the return velocity and reset jump state
                    if (jumpDirection == _wallJumpNormal)
                    {
                        currentVelocity += (jumpDirection * p.WallBounceForce) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                        currentVelocity += (Motor.CharacterUp * p.WallJumpUpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                        Motor.GroundDetectionExtraDistance = 0f;
                        _wallRunTicker.Reset();
                        _isWallrunning = false;
                        StartCoroutine(WallRunBanTime());
                        _doubleJumpConsumed = false;
                        StopCoroutine(_tiltCamCoroutine);
                        _tiltCamCoroutine = StartCoroutine(PlayerCam.TiltCam(0));
                        _highestMag = 0f;
                    }
                    else
                    {
                        currentVelocity += (jumpDirection * JumpUpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                        currentVelocity += (_moveInputVector * JumpScalableForwardSpeed);
                    }
                    _jumpRequested = false;
                    _jumpConsumed = true;
                    _jumpedThisFrame = true;
                }
            }

            // Reset wall jump
            _canWallJump = false;

            #endregion

            #region Misc

            //Counteract to High Velocities
            if (currentVelocity.magnitude >= 70) currentVelocity *= 1f / (1f + (5f * deltaTime));

            if (_getMag)
            {
                float currentMag = currentVelocity.y;
                if (currentMag < _highestMag) _highestMag = currentMag;
            }
            if (_cutVelocity)
            {
                currentVelocity = currentVelocity * p.StunStrenght;
                StartCoroutine(StunTime());
                _highestMag = 0f;
            }

            #endregion

            // Take into account additive velocity
            if (_internalVelocityAdd.sqrMagnitude > 0f)
            {
                currentVelocity += _internalVelocityAdd;
                _internalVelocityAdd = Vector3.zero;
            }
        }

        private bool CheckWalls(out RaycastHit resultHit/*out Vector3 wallPoint, out Vector3 wallNormal, out bool left, out bool right, out bool front*/)
        {
            // wallPoint = Vector3.zero;
            // wallNormal = Vector3.zero;
            // left = false;
            // right = false;
            // front = false;

            int rayCount = 8;
            bool hitAnything = false;
            
            resultHit = default;

            /*
             * shoot rays with rayCount amount, in a circle below the player
             * singleAngle = 360 / rayCount
            */
            float singleAngle = 360f / rayCount;

            float minDistance = float.MaxValue;

            for(int i = 0; i < rayCount; ++i)
            {
                Vector3 rayDirection = GetDirectionByIndex(singleAngle, i);

                if (!Physics.Raycast(Motor.Transform.position, rayDirection, out RaycastHit hit, p.MaxWallDistance, p.WallLayer)) continue;
                // else

                var distance = Vector3.Distance(hit.point, Motor.Transform.position);

                if(distance > minDistance) continue;
                // else current distance is smaller or equal to min Distance

                hitAnything = true;

                minDistance = distance;
                resultHit = hit;
            }

            return hitAnything;

            Vector3 GetDirectionByIndex(float singleAngleInDegrees, float index)
            {
                var angleInDegrees = singleAngleInDegrees * index;

                var angleInRadians = Mathf.Deg2Rad * angleInDegrees;

                var direction = new Vector3(Mathf.Cos(angleInRadians), 0f, Mathf.Sin(angleInRadians));

                return direction;
            }
        }

        public void BeforeCharacterUpdate(float deltaTime)
        {
            if (_isCrouching)
            {
                JumpUpSpeed = p.CrouchJumpUpSpeed;
                JumpScalableForwardSpeed = p.CrouchForwardSpeed;
                MaxStableSpeed = p.MaxStableCrouchSpeed;
            }
            else if (_isSprinting)
            {
                JumpUpSpeed = p.SprintJumpUpSpeed;
                JumpScalableForwardSpeed = p.SprintForwardSpeed;
                MaxStableSpeed = p.MaxStableSprintSpeed;
            }
            else
            {
                JumpUpSpeed = p.WalkJumpUpSpeed;
                JumpScalableForwardSpeed = p.WalkForwardSpeed;
                MaxStableSpeed = p.MaxStableWalkSpeed;
            }
        }

        public void AddVelocity(Vector3 velocity)
        {
            _internalVelocityAdd += velocity;
        }

        public void PostGroundingUpdate(float deltaTime)
        {
            if (Motor.GroundingStatus.IsStableOnGround && !Motor.LastGroundingStatus.IsStableOnGround)
            {
                OnLanded();
            }
            else if (!Motor.GroundingStatus.IsStableOnGround && Motor.LastGroundingStatus.IsStableOnGround)
            {
                OnLeaveStableGround();
            }
        }

        protected void OnLanded()
        {
            _getMag = false;
            if (_highestMag < -p.StunThreshold) _cutVelocity = true;
        }

        protected void OnLeaveStableGround()
        {
            _getMag = true;
        }

        public void AfterCharacterUpdate(float deltaTime)
        {
            // Handle jump-related values
            {
                // Handle jumping pre-ground grace period
                if (_jumpRequested && _timeSinceJumpRequested > p.JumpPreGroundingGraceTime)
                {
                    _jumpRequested = false;
                }

                if (p.AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround)
                {
                    // If we're on a ground surface, reset jumping values
                    if (!_jumpedThisFrame)
                    {
                        _doubleJumpConsumed = false;
                        _jumpConsumed = false;
                    }
                    _timeSinceLastAbleToJump = 0f;
                }
                else
                {
                    // Keep track of time since we were last able to jump (for grace period)
                    _timeSinceLastAbleToJump += deltaTime;
                }

                if (_isCrouching && Motor.Velocity.magnitude >= MinStableSlideSpeed)
                    DeltaHeight = transform.position;

                //Handle Crouching
                if (!_isCrouching && _shouldBeCrouching && Motor.GroundingStatus.IsStableOnGround)
                {
                    _isSprinting = false;
                    _isCrouching = true;

                    Motor.SetCapsuleDimensions(0.5f, p.CrouchedCapsuleHeight, p.CrouchedCapsuleHeight * 0.5f);
                }

                // Handle uncrouching
                if (_isCrouching && !_shouldBeCrouching)
                {
                    // Do an overlap test with the character's standing height to see if there are any obstructions
                    Motor.SetCapsuleDimensions(0.5f, 2f, 1f);
                    if (Motor.CharacterOverlap(
                        Motor.TransientPosition,
                        Motor.TransientRotation,
                        _probedColliders,
                        Motor.CollidableLayers,
                        QueryTriggerInteraction.Ignore) > 0)
                    {
                        // If obstructions, just stick to crouching dimensions
                        Motor.SetCapsuleDimensions(0.5f, p.CrouchedCapsuleHeight, p.CrouchedCapsuleHeight * 0.5f);
                        _shouldBeSprinting = false;
                    }
                    else
                    {
                        // If no obstructions, uncrouch
                        _isCrouching = false;

                        if (_SprintCached)
                        {
                            _shouldBeSprinting = true;
                        }
                    }
                }
            }

            if (_isCrouching && !_isVisuallyCrouching)
            {
                StartCoroutine(CrouchLerp());
            }
            else if (!_isCrouching && _isVisuallyCrouching)
            {
                StartCoroutine(StandUpLerp());
            }

            //Handle Sprinting
            if (_shouldBeSprinting && !_isCrouching && !_isSprinting)
            {
                _isSprinting = true;
                MaxStableSpeed = p.MaxStableSprintSpeed;
            }

            else if (_isSprinting && !_shouldBeSprinting)
            {
                MaxStableSpeed = p.MaxStableWalkSpeed;
                _isSprinting = false;
            }
        }

        public bool IsColliderValidForCollisions(Collider coll)
        {
            if (p.IgnoredColliders.Count == 0)
            {
                return true;
            }

            if (p.IgnoredColliders.Contains(coll))
            {
                return false;
            }

            return true;
        }

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {

        }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
            // We can wall jump only if we are not stable on ground and are moving against an obstruction
            if (!Motor.GroundingStatus.IsStableOnGround && !hitStabilityReport.IsStable)
            {
                float angle = Vector3.Angle(hitNormal, Vector3.up);
                if (angle > Motor.MaxStableSlopeAngle && angle < 120f)
                {
                    if(p.UseWallJump) _canWallJump = true;

                    _wallJumpNormal = hitNormal;
                }
            }
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {

        }

        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {

        }
        #endregion

        private IEnumerator CrouchLerp()
        {
            if (_isChangingCrouchPosition)
                yield break;


            _isChangingCrouchPosition = true;

            float EndHeight = 0.5f;

            while (MeshRoot.localScale.y >= EndHeight)
            {
                MeshRoot.localScale = Vector3.Slerp(MeshRoot.localScale, new Vector3(1f, EndHeight - 0.1f, 1f), p.CrouchDownSpeed * Time.deltaTime);

                yield return null;
            }

            MeshRoot.localScale = new Vector3(1f, 0.5f, 1f);

            _isVisuallyCrouching = true;

            _isChangingCrouchPosition = false;

            yield return null;
        }

        private IEnumerator StandUpLerp()
        {
            if (_isChangingCrouchPosition)
                yield break;

            _isChangingCrouchPosition = true;

            float EndHeight = 1f;

            while (MeshRoot.localScale.y <= EndHeight)
            {
                MeshRoot.localScale = Vector3.Slerp(MeshRoot.localScale, new Vector3(1f, EndHeight + 0.1f, 1f), p.CrouchDownSpeed * Time.deltaTime);

                yield return null;
            }

            MeshRoot.localScale = new Vector3(1f, 1f, 1f);

            _isVisuallyCrouching = false;

            _isChangingCrouchPosition = false;

            yield return null;
        }

        private IEnumerator WallRunBanTime()
        {
            _canWallRun = false;

            yield return new WaitForSeconds(p.WallRunBanTime);

            _canWallRun = true;
        }

        private IEnumerator StunTime()
        {
            _cutVelocity = true;

            yield return new WaitForSeconds(p.StunDuration);

            _cutVelocity = false;
        }
    }

    /// <summary>
	///     A floating point driven Ticker
	/// </summary>
	public class Ticker
    {
		public Ticker() => Span = 0f;

		public float Span { get; private set; }

		public void Tick(float delta)
		{
			Span += delta;
		}
		
		public bool TickMax(float delta, float max)
		{
			Span += delta;

			//if current Timer is not over max timer
			if (Span < max) return false;

			Span -= max;

			return true;
		}

		public void SetSpan(float span) => Span = span;

		public void Reset() => Span = 0f;
	}
}
