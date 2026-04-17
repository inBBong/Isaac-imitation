using System.Collections;
using System.Collections.Generic;
using UnityEngine;
enum Direction
{
    UP = 0,
    DOWN,
    LEFT,
    RIGHT
}
public class PlayerController : MonoBehaviour
{

    [Header("Components")]
    public SpriteRenderer bodyRenderer;
    public Animator bodyAnimator;
    public Animator headAnimator;

    [Header("Setting")]
    public float speed = 3;
    public float headFollowDelay = 0.3f; // 머리가 따라오는 지연시간
    public float shootCooldown = 0.3f;
    public GameObject tearPrefab;

    Vector3 move;
    Direction bodydir;
    Direction headdir;

    float headFollowTimer = 0f;
    float shootTimer = 0f;
    bool isAttacking=false;
    bool isMoving = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        HandleMove();
        HandleAttackDirection();
        HandleHeadFollow();      // 머리 지연 추적
        HandleShootTimer();      // 공격 쿨타임
    }
    void HandleMove()
    {
        move = Vector3.zero;
        if (Input.GetKey(KeyCode.A)) { move.x -= 1; bodydir = Direction.LEFT; }
        if (Input.GetKey(KeyCode.D)) { move.x += 1; bodydir = Direction.RIGHT; }
        if (Input.GetKey(KeyCode.W)) { move.y += 1; bodydir = Direction.UP; }
        if (Input.GetKey(KeyCode.S)) { move.y -= 1; bodydir = Direction.DOWN; }
        move = move.normalized;

        isMoving = move.magnitude > 0;
        bodyAnimator.SetBool("IsMoving", isMoving);

        // 좌우 판단
        if (bodydir == Direction.LEFT || bodydir == Direction.RIGHT)
        {
            bodyAnimator.SetInteger("DirX", 1);
            bodyRenderer.flipX = (bodydir == Direction.LEFT);
        }
        else
        {
            bodyAnimator.SetInteger("DirX", 0);
            bodyRenderer.flipX = false;
        }
        bodyRenderer.flipY = false;
        // 이동 시 머리 추적 타이머 리셋
        //if (isMoving) headFollowTimer = headFollowDelay;
    }
    void HandleAttackDirection()
    {
        bool attackInput = false;
        // 방향키로 즉시 머리 방향 전환
        if (Input.GetKey(KeyCode.UpArrow)) {SetHeadDirection(Direction.UP); attackInput = true; }
        if (Input.GetKey(KeyCode.DownArrow)) {SetHeadDirection(Direction.DOWN); attackInput = true; }
        if (Input.GetKey(KeyCode.LeftArrow)) {SetHeadDirection(Direction.LEFT); attackInput = true; }
        if (Input.GetKey(KeyCode.RightArrow)) {SetHeadDirection(Direction.RIGHT); attackInput = true; }

        if (attackInput)
        {
            isAttacking = true;
            headFollowTimer = headFollowDelay; // 공격 중엔 머리 추적 타이머 리셋

            // 쿨다운마다 발사
            if (shootTimer <= 0f)
            {
                Shoot();
                shootTimer = shootCooldown;
            }
        }
    }
    void HandleShootTimer()
    {
        if (shootTimer > 0f) shootTimer -= Time.deltaTime;
    }
    void SetHeadDirection(Direction dir)
    {
        headdir = dir;
        switch (dir)
        {
            case Direction.DOWN: headAnimator.SetInteger("Headdir", 0); break;
            case Direction.UP: headAnimator.SetInteger("Headdir", 1); break;
            case Direction.LEFT: headAnimator.SetInteger("Headdir", 2); break;
            case Direction.RIGHT: headAnimator.SetInteger("Headdir", 3); break;
        }
    }
    void HandleHeadFollow()
    {
        if (isAttacking)
        {
            // 방향키 안 누르는 순간 isAttacking 해제
            if (!Input.GetKey(KeyCode.UpArrow) &&
                !Input.GetKey(KeyCode.DownArrow) &&
                !Input.GetKey(KeyCode.LeftArrow) &&
                !Input.GetKey(KeyCode.RightArrow))
            {
                isAttacking = false;
            }
            return; // 공격 중엔 머리 추적 안 함
        }
        if(isMoving)
        {// 공격 중이 아닐 때, 움직일때만 몸통 방향 추적
        headFollowTimer -= Time.deltaTime;
            if (headFollowTimer <= 0f)
            {
                SetHeadDirection(bodydir);// 지연 후 머리가 몸통 방향 따라감
            }
        }
                             
    }
    void Shoot()
    {
        GameObject newTear = GetComponent<ObjectPool>().Get();
        if (newTear != null)
        {
            newTear.transform.position = transform.position + new Vector3(0, 0.4f, 0);
            if (headdir == Direction.LEFT) newTear.GetComponent<Tears>().Direction = new Vector2(-1, 0);
            else if (headdir == Direction.RIGHT) newTear.GetComponent<Tears>().Direction = new Vector2(1, 0);
            else if (headdir == Direction.UP) newTear.GetComponent<Tears>().Direction = new Vector2(0, 1);
            else if (headdir == Direction.DOWN) newTear.GetComponent<Tears>().Direction = new Vector2(0, -1);
        }
    }

    private void FixedUpdate()
    {
        transform.Translate(move * speed * Time.fixedDeltaTime);
    }
}
