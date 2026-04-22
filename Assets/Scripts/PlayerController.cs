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
    public SpriteRenderer headRenderer;
    public SpriteRenderer playerRenderer;
    public Animator bodyAnimator;
    public Animator headAnimator;
    public Animator playerAnimator;

    [Header("Setting")]
    public float speed = 3;
    public float MaxHp = 3;
    public float headFollowDelay = 0.3f; // 머리가 따라오는 지연시간
    public float shootCooldown = 0.3f;
    public float hitCooldown = 1.5f;
    public GameObject tearPrefab;

    bool isDead = false;
    bool isInvincible = false;

    float HitTime = 1.0f;
    float DeadTime = 2.0f;

    Vector3 move;
    Direction bodydir;
    Direction headdir;
        
    float headFollowTimer = 0f;
    float shootTimer = 0f;
    bool isAttacking=false;
    bool isMoving = false;
    float hp;
    MonsterSpawner spawner;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hp = MaxHp;
        spawner = FindAnyObjectByType<MonsterSpawner>();
        spawner.Spawn("Fly"); // 테스트용
        spawner.Spawn("Fly"); // 테스트용
        spawner.Spawn("Fly"); // 테스트용
        spawner.Spawn("Fly"); // 테스트용
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            HandleMove();
            HandleAttackDirection();
            HandleHeadFollow();      // 머리 지연 추적
            HandleShootTimer();      // 공격 쿨타임
        }
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
        if (Input.GetKey(KeyCode.UpArrow)) {SetHeadDirection(Direction.UP); attackInput = true; headAnimator.SetBool("Shoot",true); }
        if (Input.GetKey(KeyCode.DownArrow)) {SetHeadDirection(Direction.DOWN); attackInput = true; headAnimator.SetBool("Shoot", true); }
        if (Input.GetKey(KeyCode.LeftArrow)) {SetHeadDirection(Direction.LEFT); attackInput = true; headAnimator.SetBool("Shoot", true); }
        if (Input.GetKey(KeyCode.RightArrow)) {SetHeadDirection(Direction.RIGHT); attackInput = true; headAnimator.SetBool("Shoot", true); }

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
    public void TakeDamage(float damage)
    {
        if (isDead || isInvincible) return;  // 무적/사망 중엔 피격 무시

        hp -= damage;

        if (hp <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(HitRoutine());
        }
    }
    IEnumerator HitRoutine()
    {
        isInvincible = true;

        // 피격 애니메이션 재생
        bodyRenderer.enabled = false;
        headRenderer.enabled = false;
        playerRenderer.enabled = true;

        bodyAnimator.enabled = false;   // Body 애니메이터 끄기
        headAnimator.enabled = false;   // Head 애니메이터 끄기
        playerAnimator.SetTrigger("IsHit");

        // 깜빡임 효과
        StartCoroutine(BlinkRoutine());

        // 피격 애니메이션이 끝날 때까지 대기
        yield return new WaitForSeconds(HitTime); // 피격 애니메이션 길이에 맞게 조정


        // 피격 애니메이션 끝 - 복구
        bodyRenderer.enabled = true;
        headRenderer.enabled = true;
        playerRenderer.enabled = false;
        bodyAnimator.enabled = true;
        headAnimator.enabled = true;

        yield return new WaitForSeconds(hitCooldown- HitTime);
        isInvincible = false;
    }
    IEnumerator BlinkRoutine()
    {
        float blinkDuration = hitCooldown;
        float blinkInterval = 0.1f;
        float timer = 0f;

        while (timer < blinkDuration)
        {
            if (timer < HitTime)// 피격애니메이션이 실행되는동안 Player 렌더러를 번갈아 끄고 켜기
                playerRenderer.enabled = !playerRenderer.enabled;
            else
            {   // Body, Head 렌더러를 번갈아 끄고 켜기
                playerRenderer.enabled = false;
                bodyRenderer.enabled = !bodyRenderer.enabled;
                headRenderer.enabled = !headRenderer.enabled;
            }

            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }

        bodyRenderer.enabled = true;  // 깜빡임 끝나면 반드시 켜기
        headRenderer.enabled = true;  // 깜빡임 끝나면 반드시 켜기
    }
    void Die()
    {
        isDead = true;
        move = Vector3.zero;
        bodyRenderer.enabled = false;
        headRenderer.enabled = false;
        playerRenderer.enabled = true;
        GetComponent<Collider2D>().enabled = false; // 충돌 끄기
        bodyAnimator.enabled = false;   // Body 애니메이터 끄기
        headAnimator.enabled = false;   // Head 애니메이터 끄기
        playerAnimator.SetTrigger("IsDead");

        // 사망 애니메이션 길이에 맞게 조정
        StartCoroutine(DieRoutine());
    }
    IEnumerator DieRoutine()
    {
        yield return new WaitForSeconds(DeadTime); // 사망 애니메이션 길이
        // 나중에 게임오버 화면 전환 추가
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
}
