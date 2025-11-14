using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Mart581d.Extensions;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public PlayerScript player;
    public Animator animator;
    public SpriteRenderer sprite;
    public AnimationClip idle;
    public AnimationClip jumpStart;
    public AnimationClip barkJump;
    public AnimationClip land;

    public float idleTimeLength = 5f;

    // public AnimationClip idle;
    // public AnimationClip jump;
    // public AnimationClip wallJump;
    // public AnimationClip fall;
    // public AnimationClip land;

    private bool inAnimation = false;
    public float idleTimer = 0f;
    public float playingIdleTimer = 0f;

    [CanBeNull] private Coroutine _playerAfterCoroutine;

    private void Start()
    {
        player.OnJump += PlayJump;
        player.OnBarkJump += PlayBarkJump;
        player.OnLand += Land;
        player.OnFall += () => animator.Play("Fall");
        player.OnChangeState += OnChangeState;
        player.OnTakeDamage += TakeDamage;
    }

    private void PlayIfNotPlaying(string name)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(name)) return;
        
        animator.Play(name);
    }

    private void LateUpdate()
    {
        if (player.velocity.x > 0f)
            sprite.flipX = false;
        else if (player.velocity.x < 0f)
            sprite.flipX = true;
    
        // return color to normal after damage
        sprite.color = Color.Lerp(sprite.color, Color.white, Time.deltaTime * 5);

        if (inAnimation) return;

        if (player.state == PlayerScript.PlayerState.Ground)
        {
            if (Mathf.Abs(player.velocity.x) > 0.1f)
            {
                PlayIfNotPlaying("Run");
            }
            else
            {
                playingIdleTimer = Mathf.Max(0f, playingIdleTimer - Time.deltaTime);
                idleTimer = Mathf.Max(0f, idleTimer - Time.deltaTime);

                if (playingIdleTimer != 0f) return;
                
                if (idleTimer == 0f)
                {
                    animator.Play("Idle");
                    playingIdleTimer = idle.length;
                    idleTimer = playingIdleTimer + idleTimeLength;
                }
                else
                {
                    PlayIfNotPlaying("IdleStill");
                }
            }
        } else if (player.state == PlayerScript.PlayerState.WallSlide)
        {
            PlayIfNotPlaying("WallSlide");
            sprite.flipX = player.wallNormal.x < 0f;
        }
    }

    private void Land()
    {
        if (_playerAfterCoroutine != null) StopCoroutine(_playerAfterCoroutine);
        
        animator.Play("JumpLand");
        _playerAfterCoroutine = StartCoroutine(PlayAfter(land.length, "Idle"));
    }

    private void PlayJump()
    {
        if (_playerAfterCoroutine != null) StopCoroutine(_playerAfterCoroutine);

        animator.Play("JumpStart");
        _playerAfterCoroutine = StartCoroutine(PlayAfter(jumpStart.length, "Fall"));
    }

    private void PlayBarkJump()
    {
        if (_playerAfterCoroutine != null) StopCoroutine(_playerAfterCoroutine);
        
        animator.Play("BarkJump");
        _playerAfterCoroutine = StartCoroutine(PlayAfter(barkJump.length, "Fall"));
    }

    private IEnumerator FallAfter(float seconds)
    {
        inAnimation = true;
        yield return new WaitForSeconds(seconds);
        animator.Play("Fall");
        inAnimation = false;
    }

    private IEnumerator PlayAfter(float seconds, string stateName)
    {
        inAnimation = true;
        yield return new WaitForSeconds(seconds);
        animator.Play(stateName);
        inAnimation = false;
    }

    private void OnChangeState(PlayerScript.PlayerState from, PlayerScript.PlayerState to)
    {
        if (to == PlayerScript.PlayerState.WallSlide && inAnimation)
        {
            StopCoroutine(_playerAfterCoroutine);
            inAnimation = false;
        }
    }

    private void TakeDamage()
    {
        animator.Play("Death");
        sprite.color = Color.red;
    }
}
