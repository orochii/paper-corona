using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour {
    [SerializeField] private float moveSpeed = 1f;
    private Rigidbody2D rbody;
    private Animator anim;
    private SpriteRenderer sprite;
    private Vector3 targetPosition;
    private Vector3 lastPosition;
    private int tries = 0;
    private bool moving;

    private int b_walkingHash;

    public void SetSortingLayer(int l) {
        sprite.sortingOrder = l;
    }

    private void Awake() {
        rbody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        targetPosition = transform.position;
        b_walkingHash = Animator.StringToHash("walking");
    }

    private void Update() {
        if (Interactable.Busy) return;
        if (GameState.Instance.UIOpen) return;
        if (PlayerInteraction.Instance.HasCurrent) return;
        // Open inventory
        if (Input.GetMouseButtonDown(1)) {
            PlayerInteraction.Instance.OpenInventory();
        }
        // Move character
        if (Input.GetMouseButtonDown(0)) {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    public void MoveTo(Vector3 tpos) {
        targetPosition = tpos;
    }

    public bool IsMoving {
        get {
            float minMove = moveSpeed * 2 * Time.fixedDeltaTime;
            Vector2 deltaPos = targetPosition - transform.position;
            return deltaPos.sqrMagnitude > minMove * minMove;
        }
    }

    private void FixedUpdate() {
        float minMinMove = moveSpeed * 0.2f * Time.fixedDeltaTime; // :)
        if (moving) {
            if (Vector3.Distance(lastPosition,transform.position) < minMinMove) {
                tries++;
                if (tries > 8) targetPosition = transform.position;
            } else {
                tries = 0;
            }
        }
        float minMove = moveSpeed * 2 * Time.fixedDeltaTime;
        Vector2 deltaPos = targetPosition - transform.position;
        if (deltaPos.sqrMagnitude > minMove*minMove) {
            rbody.velocity = deltaPos.normalized * moveSpeed;
            // Flip
            sprite.flipX = rbody.velocity.x < 0;
            anim.SetBool(b_walkingHash, true);
            lastPosition = transform.position;
            moving = true;
        } else {
            rbody.velocity = Vector2.zero;
            anim.SetBool(b_walkingHash, false);
            moving = false;
        }
    }

    internal void PlayAnimation(string v) {
        anim.Play(v);
    }
}