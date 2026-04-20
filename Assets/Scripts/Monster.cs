using UnityEngine;

public abstract class Monster : MonoBehaviour
{
    [Header("Stats")]
    public float maxHp;
    float hp;
    public float speed;
    public float damage;

    // 자신을 꺼낸 풀 참조
    [HideInInspector] public ObjectPool myPool;

    protected Transform player;
    protected Animator animator;
    protected Rigidbody2D rb;

    protected virtual void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // 풀에서 꺼낼 때 초기화
    protected virtual void OnEnable()
    {
        hp = maxHp; // HP 초기화
    }

    protected abstract void Move();

    public virtual void TakeDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0) Die();
    }

    protected virtual void Die()
    {
        if (myPool != null)
            gameObject.SetActive(false); // 풀로 반환
        else
            Destroy(gameObject); // 풀 없으면 그냥 삭제
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Tears"))
        {
            TakeDamage(other.GetComponent<Tears>().damage);
        }

        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().TakeDamage(damage);
        }
    }

    void Update()
    {
        Move();
    }
}