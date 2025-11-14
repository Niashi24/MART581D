using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mart581d;
using Mart581d.Extensions;
using Unity.VisualScripting;
using UnityEngine;

[SelectionBase]
public class PlayerScript : MonoBehaviour
{

    public Rigidbody2D rbdy;
    public CircleCollider2D coll;
    public float radius = 2f;
    
    public Vector2 velocity;

    public float groundSpeed = 20f;
    public float jumpSpeed = 40f;

    public float airAcceleration = 20f;

    public float gravity = 80f;

    public float slideSpeed = 20f;
    
    public float slideDeceleration = 80f;

    public float wallJumpLockDuration = 0.2f;

    public float wallJumpDistance = 0.5f;
    public float jumpBufferDuration = 0.1f;
    public float coyoteTimeDuration = 0.1f;
    public float barkCooldownDuration = 1f;
    public float barkBufferDuration = 0.1f;
    public float barkForce = 60f;
    public float barkDistance = 4f;
    public float barkWidth = 3f;
    public float velocityBufferTime = 0.5f;
    
    // public float acceleration = 20f / 0.25f;
    // public float deceleration = 20f / 0.5f;

    public enum PlayerState
    {
        Ground,
        Air,
        WallSlide,
        Dead,
    }

    public PlayerState state;
    public PlayerInput input;

    public LayerMask groundMask;
    public Vector2 wallNormal;

    public float wallJumpLock = 0f;
    public float jumpBuffer = 0f;
    public float coyoteTime = 0f;
    public float barkCooldown = 0f;
    public float barkBuffer = 0f;
    public bool canBark = true;
    public Vector2 respawnLocation;
    public float respawnTimer;
    public AnimationClip deathAnimation;

    private Queue<(Vector2, float)> storedVelocity = new();

    public Action OnLand;
    public Action OnJump;
    public Action OnFall;
    public Action OnBark;
    public Action OnBarkJump;
    public Action<PlayerState, PlayerState> OnChangeState;
    public Action OnTakeDamage;
    

    void Start()
    {
        this.respawnLocation = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInput();
    }

    private void UpdateInput()
    {
        bool isKeyboard = true;

        if (isKeyboard)
        {
            var mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var directionToMouse = (mousePos - this.rbdy.position).normalized;
            
            this.input.Update
                (
                    Input.GetKey(KeyCode.Space),
                    Input.GetMouseButton(0),
                    new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")),
                    directionToMouse
                );
        }
        else
        {
            throw new NotImplementedException("implement gamepad support");
        }
    }

    private void StoreVelocity(Vector2 vel)
    {
        storedVelocity.Enqueue((this.rbdy.velocity, Time.time));
        while (true)
        {
            if (storedVelocity.Count == 0) break;
            var (_, time) = storedVelocity.Peek();
            if (!(Time.time - time > this.velocityBufferTime))
                break;
            
            storedVelocity.Dequeue();
        }
    }

    private Vector2 TakeStoredMomentum()
    {
        if (this.storedVelocity.Count == 0) return Vector2.zero;
        
        // this allocates but its not every frame so whatever
        var x = this.storedVelocity.Select(v => v.Item1.x)
            // surely there will be no issues with having both negative and positive values (<- clueless)
            .OrderBy(Mathf.Abs)
            .ToArray();
        
        var y = this.storedVelocity.Select(v => v.Item1.y)
            .OrderBy(Mathf.Abs)
            .ToArray();
        
        // to avoid weird values, we make sure this value (or higher) was in at least 10% of the queue (by sorting)
        // clamp index to bounds
        int i = Mathf.Clamp((int)(x.Length * 0.9f), 0, storedVelocity.Count - 1);
        
        this.storedVelocity.Clear();
        return new Vector2(x[i], y[i]);
    }

    private void FixedUpdate()
    {
        this.velocity += this.rbdy.velocity;
        StoreVelocity(this.rbdy.velocity);
        this.rbdy.velocity = Vector2.zero;

        if (this.state == PlayerState.Dead)
        {
            this.DeadUpdate();
            return;
        }

        jumpBuffer = Mathf.Max(0f, jumpBuffer - Time.deltaTime);

        var input = this.input.Take();
        
        this.Bark(input);

        switch (this.state)
        {
            case PlayerState.Ground:
                this.GroundUpdate(input);
                break;
            case PlayerState.Air:
                this.AirUpdate(input);
                break;
            case PlayerState.WallSlide:
                this.WallUpdate(input);
                break;
        }
    }

    private void DeadUpdate()
    {
        this.velocity = Vector2.zero;
        
        this.respawnTimer = Mathf.Max(0f, this.respawnTimer - Time.deltaTime);
        if (this.respawnTimer == 0f)
        {
            rbdy.position = respawnLocation;
            ChangeState(PlayerState.Ground);
        }
    }

    void Bark(PlayerInput input)
    {
        barkCooldown = Mathf.Max(0f, barkCooldown - Time.deltaTime);
        barkBuffer = Mathf.Max(0f, barkBuffer - Time.deltaTime);
        
        if (input.bark.JustPressed && barkCooldown > 0f)
        {
            // todo: should this buffer also store the current aim?
            barkBuffer = barkBufferDuration;
            return;
        }
        
        if (canBark && barkCooldown == 0f && (input.bark.JustPressed || barkBuffer > 0f))
        {
            barkCooldown = barkCooldownDuration;

            wallJumpLock = 0f;
            
            OnBark?.Invoke();
            
            // add force to player
            Vector2 force = barkForce * -input.aim;
            switch (this.state)
            {
                case PlayerState.Ground:
                    if (input.aim.y < 0f)
                    {
                        this.velocity = force;
                        canBark = false;
                        this.velocity += TakeStoredMomentum();
                        OnBarkJump?.Invoke();
                        ChangeState(PlayerState.Air);
                        // barkCooldown = barkCooldownDuration;
                    }
                    break;
                case PlayerState.Air:
                    this.velocity = force;
                    canBark = false;
                    OnBarkJump?.Invoke();
                    break;
                case PlayerState.WallSlide:
                    if (Vector2.Dot(wallNormal, force.normalized) > 0f)
                    {
                        this.velocity = force;
                        canBark = false;
                        this.velocity += TakeStoredMomentum();
                        OnBarkJump?.Invoke();
                        ChangeState(PlayerState.Air);
                    }
                    else
                    {
                        this.velocity.y += force.y;
                    }

                    break;
            }
            Debug.Log("here");
            
            // todo: trigger/push items in contact
            var overlaps = Physics2D.OverlapBoxAll(rbdy.position + input.aim * (this.barkDistance / 2), new Vector2(barkDistance, barkWidth), Mathf.Atan2(input.aim.y, input.aim.x) * Mathf.Rad2Deg, groundMask);
            foreach (var overlap in overlaps)
            {
                if (overlap.TryGetComponent<BarkTriggerScript>(out var trigger))
                {
                    trigger.Trigger(this);
                }
            }
        }
    }
    
    static RaycastHit2D[] hits = new [] { new RaycastHit2D() };
    static RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction, ContactFilter2D filter2D)
    {
        hits[0] = new RaycastHit2D();
        // hits[0].collider = null; hits[0].transform = null;
        Physics2D.CircleCast(origin, radius, direction.normalized, filter2D, hits, direction.magnitude);

        return hits[0];
    }

    private ContactFilter2D ContactFilter()
    {
        var filter = new ContactFilter2D().NoFilter();

        filter.useTriggers = false;
        filter.SetLayerMask(this.groundMask);

        // Debug.Log(filter);

        return filter;
    }

    private void GroundUpdate(PlayerInput input)
    {
        this.velocity.x = this.groundSpeed * input.move.x;
        this.velocity.y = 0f;
        // check if still grounded
        // Physics2D.Cir
        var hit = CircleCast(rbdy.position, this.radius - 0.1f, Vector2.down * 0.15f, ContactFilter());
        if (!hit)
        {
            this.velocity.x = this.groundSpeed * input.move.x;
            this.velocity.y = 0;
            coyoteTime = coyoteTimeDuration;
            this.velocity += TakeStoredMomentum();
            OnFall?.Invoke();
            ChangeState(PlayerState.Air);
            return;
        }

        if (input.jump.JustPressed || jumpBuffer > 0f)
        {
            if (jumpBuffer > 0f)
            {
                Debug.Log("jumped from buffer");
            }
            this.velocity.x = input.move.x * this.groundSpeed;
            Jump();
            ChangeState(PlayerState.Air);
            return;
        }
        
        // float dV = this.acceleration * this.input.move.x * Time.deltaTime;

        Vector2 dV = this.groundSpeed * input.move.x * Time.deltaTime * Vector2.right;
        rbdy.position += dV;
        
        // rbdy.MovePosition(rbdy.position + dV);
    }

    private void AirUpdate(PlayerInput input)
    {
        this.coyoteTime = Mathf.Max(0f, this.coyoteTime - Time.deltaTime);
        if (this.coyoteTime > 0f && input.jump.JustPressed)
        {
            this.velocity.x = input.move.x * this.groundSpeed;
            Jump();
            this.coyoteTime = 0f;
            return;
        }
        
        this.wallJumpLock = Mathf.Max(0, this.wallJumpLock - Time.deltaTime);
        if (this.wallJumpLock != 0)
        {
            input.move.x = this.wallNormal.x;
        }
        // add gravity
        this.velocity.y += -this.gravity * Time.deltaTime;
        
        // check if hit ground
        if (this.velocity.y <= 0)
        {
            var hit = CircleCast(rbdy.position, this.radius - 0.1f,
                Vector2.up * (this.velocity.y * Time.deltaTime), ContactFilter());
            if (hit && hit.normal.y > 0.1f)
            {
                // Debug.Log(rbdy.position);
                rbdy.position = rbdy.position.WithY(hit.centroid.y);
                this.velocity.y = 0;
                OnLand?.Invoke();
                ChangeState(PlayerState.Ground);
            }
        }
        else
        {
            var hit = CircleCast(rbdy.position, this.radius - 0.1f,
                Vector2.up * (this.velocity.y * Time.deltaTime), ContactFilter());
            if (hit && Vector2.Dot(hit.normal, Vector2.down) > 0.9f)
            {
                this.velocity.y = 0;
                rbdy.position = rbdy.position.WithY(hit.centroid.y);
            }
        }

        if (input.move.x != 0)
        {
            float accel = airAcceleration * Time.deltaTime;
            if (Mathf.Sign(this.velocity.x) != input.move.x || Mathf.Abs(this.velocity.x) < this.groundSpeed)
            {
                accel *= 8f;
            }

            this.velocity.x = Mathf.MoveTowards(this.velocity.x, this.groundSpeed * input.move.x, accel);
        }
        else
        {
            this.velocity.x = Mathf.MoveTowards(this.velocity.x, 0f, this.airAcceleration * Time.deltaTime);
        }

        // if (input.move.x != 0)
        {
            // this.velocity.x = this.groundSpeed * input.move.x;
            var wall = CircleCast(rbdy.position, this.radius - 0.1f,
                Vector2.right * (this.velocity.x * Time.deltaTime), ContactFilter());
            bool isWall = Mathf.Abs(wall.normal.x) > 0.99;
            if (wall && isWall)
            {
                rbdy.position = wall.centroid + wall.normal * 0.1f;
                this.velocity.x = 0;
                this.wallNormal = wall.normal;
                this.ChangeState(PlayerState.WallSlide);
                return;
            }
        }

        if (input.jump.JustPressed || jumpBuffer > 0f)
        {
            var leftWall = CircleCast(rbdy.position, this.radius - 0.1f,
                Vector2.left * (this.wallJumpDistance + 0.1f), ContactFilter());
            var rightWall = CircleCast(rbdy.position, this.radius - 0.1f,
                Vector2.right * (this.wallJumpDistance + 0.1f), ContactFilter());

            bool CanUseWall(RaycastHit2D wall)
            {
                return (this.wallJumpLock == 0 || Vector2.Dot(wall.normal, this.wallNormal) < 0.9f) &&
                       Mathf.Abs(wall.normal.y) < 0.1f;
            }


            bool canUseLeft = leftWall && CanUseWall(leftWall);
            bool canUseRight = rightWall && CanUseWall(rightWall);

            RaycastHit2D? wall;
            if (canUseLeft && !canUseRight)
                wall = leftWall;
            else if (!canUseLeft && canUseRight)
                wall = rightWall;
            else if (canUseLeft && canUseRight)
            {
                if (leftWall.distance < rightWall.distance)
                {
                    wall = leftWall;
                }
                else
                {
                    wall = rightWall;
                }
            }
            else
            {
                // buffer jump input
                if (jumpBuffer == 0f)
                    jumpBuffer = jumpBufferDuration;
                wall = null;
            }

            if (wall.HasValue)
            {
                // Debug.Lo
                this.wallNormal = wall.Value.normal;
                this.wallJumpLock = this.wallJumpLockDuration;
                this.velocity.x = this.wallNormal.x * this.groundSpeed;
                Jump();
                // should we return from here?
            }
        }

        rbdy.position += this.velocity * Time.deltaTime;
        // rbdy.MovePosition(rbdy.position + this.velocity * Time.deltaTime);

    }

    private void WallUpdate(PlayerInput input)
    {
        this.velocity.y = Mathf.MoveTowards(this.velocity.y, -slideSpeed, Time.deltaTime * this.slideDeceleration);
        this.velocity.x = 0f;
        
        // check if we've slid off the wall
        var wall = CircleCast(rbdy.position, this.radius * 0.95f, (-wallNormal * (this.radius * 0.06f)),
            ContactFilter());
        if (!wall)
        {
            this.velocity += TakeStoredMomentum();
            OnFall?.Invoke();
            ChangeState(PlayerState.Air);
        }
        
        // check if hit floor or ceiling
        var floorCeil = CircleCast(rbdy.position, this.radius * 0.95f, Vector2.up * (this.velocity.y * Time.deltaTime), ContactFilter());

        if (floorCeil)
        {
            this.velocity.y = 0;
            if (this.velocity.y > 0f)
            {
                this.rbdy.position = floorCeil.centroid - Vector2.up * (this.radius * 0.05f);
            }
            else
            {
                this.rbdy.position = floorCeil.centroid + Vector2.up * (this.radius * 0.05f);
                this.ChangeState(PlayerState.Ground);
            }
        }
        else
        {
            rbdy.position += Vector2.up * (velocity.y * Time.deltaTime);
        }
        
        if (input.jump.JustPressed || jumpBuffer > 0f)
        {
            this.wallJumpLock = this.wallJumpLockDuration;
            this.velocity.x = this.wallNormal.x * this.groundSpeed;
            Jump();
            ChangeState(PlayerState.Air);
        }

        if (input.move.x != 0f && Mathf.Sign(input.move.x) == Mathf.Sign(wallNormal.x))
        {
            this.velocity.x = input.move.x * groundSpeed;
            OnFall?.Invoke();
            ChangeState(PlayerState.Air);
        }
    }

    private void Jump()
    {
        this.jumpBuffer = 0f;
        this.velocity.y = this.jumpSpeed;
        this.velocity += this.TakeStoredMomentum();
        
        OnJump?.Invoke();
    }

    private void ChangeState(PlayerState state)
    {
        // OnExit
        switch (this.state)
        {
            case PlayerState.Air:
                if (state != PlayerState.Air)
                {
                    this.wallJumpLock = 0f;
                    this.coyoteTime = 0f;
                }
                break;
        }

        if (this.state == PlayerState.Ground && state == PlayerState.Air)
        {
            this.barkCooldown = 0f;
        }
        
        Debug.Log($"{this.state} -> {state}");
        OnChangeState?.Invoke(this.state, state);
        this.state = state;
        // OnEnter
        switch (this.state)
        {
            case PlayerState.Ground:
                this.velocity.y = 0f;
                canBark = true;
                break;
            case PlayerState.WallSlide:
                canBark = true;
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(transform.position, Vector3.right * (radius + this.wallJumpDistance));
        Gizmos.DrawRay(transform.position, Vector3.left * (radius + this.wallJumpDistance));
        
        // var overlaps = Physics2D.OverlapBoxAll(rbdy.position + input.aim * (this.barkDistance / 2), new Vector2(barkDistance, barkWidth), Mathf.Atan2(input.aim.y, input.aim.x), groundMask);

        var rotation = Mathf.Atan2(input.aim.y, input.aim.x) * Mathf.Rad2Deg;
        Gizmos.matrix = Matrix4x4.TRS(rbdy.position + input.aim * (this.barkDistance / 2),
            Quaternion.Euler(0, 0, rotation), new Vector3(barkDistance, barkWidth, 1));
        
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        Gizmos.matrix = Matrix4x4.identity;
    }

    // 1 = ready, 0 = just barked
    public float GetBarkCooldownPercent()
    {
        var inverse = this.barkCooldown / this.barkCooldownDuration;
        return 1 - inverse;
    }

    public void ResetBark()
    {
        this.canBark = true;
        this.barkCooldown = 0f;
    }

    public void SetRespawnLocation(Vector2 position)
    {
        this.respawnLocation = position;
    }

    public void TakeDamage()
    {
        // remove control
        // play hit animation
        // screen cover in
        // move player to respawn
        OnTakeDamage?.Invoke();
        this.respawnTimer = deathAnimation.length;
        ChangeState(PlayerState.Dead);
        // screen cover out
        // restore control
    }
}
