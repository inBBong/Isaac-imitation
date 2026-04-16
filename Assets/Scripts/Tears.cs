using UnityEngine;

public class Tears : MonoBehaviour
{
    public float speed = 10;
    public float damage = 1;
    Vector2 direction;
    public Vector2 Direction
    {
        set
        {
            direction=value.normalized;
        }

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //this.Direction = new Vector2(1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(direction*speed*Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag=="Wall")
        {
            gameObject.SetActive(false);
        }
    }

}
