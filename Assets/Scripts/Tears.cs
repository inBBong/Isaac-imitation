using UnityEngine;

public class Tears : MonoBehaviour
{
    public Animator TearAnimator;

    public float speed = 10;
    public float damage = 1;
    Vector2 direction;
    public Vector2 Direction
    {
        set
        {
            direction = value.normalized;
        }

    }

    enum State
    {
        MOVE = 0,
        DESTROY
    }
    State state;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //this.Direction = new Vector2(1, 0);
        state = State.MOVE;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.MOVE)
        {
            transform.Translate(direction * speed * Time.deltaTime);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Wall"|| collision.tag == "Enemy")
        {
            if (state == State.DESTROY) return;
            state = State.DESTROY;
            TearAnimator.SetTrigger("Destroy");
            GetComponent<Collider2D>().enabled = false;
        }

    }
    public void OnDestroyAnimationEnd()
    {
        gameObject.SetActive(false);
    }
    void OnEnable()
    {
        state = State.MOVE;
        TearAnimator.ResetTrigger("Destroy");

        GetComponent<Collider2D>().enabled = true;
    }
}
