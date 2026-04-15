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

    public float speed = 3;
    public SpriteRenderer bodyRenderer;
    public GameObject tearPrefab;

    Vector3 move;
    Direction bodydir;
    Direction headdir;
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        move = Vector3.zero;
        if (Input.GetKey(KeyCode.A))
        {
            move += new Vector3(-1, 0, 0);
            bodydir = Direction.LEFT;
        }
        if (Input.GetKey(KeyCode.D))
        {
            move += new Vector3(1, 0, 0);
            bodydir = Direction.RIGHT;
        }
        if (Input.GetKey(KeyCode.W))
        {
            move += new Vector3(0, 1, 0);
            bodydir = Direction.UP;
        }
        if (Input.GetKey(KeyCode.S))
        {
            move += new Vector3(0, -1, 0);
            bodydir = Direction.DOWN;
        }

        move = move.normalized;
        
        if (move.x < 0)
        {
            bodyRenderer.flipX = true;
        }
        if (move.x > 0)
        {
            bodyRenderer.flipX = false;
        }

        bool isMoving = move.magnitude > 0;
        if (isMoving)
        {                    
            if (bodydir == Direction.RIGHT|| bodydir==Direction.LEFT)
            {
                GetComponent<Animator>().SetTrigger("MoveRight");
            }
            if (bodydir == Direction.UP|| bodydir == Direction.DOWN)
            {
                GetComponent<Animator>().SetTrigger("MoveFront");

            }
                
        }
        else
        {
            
            if (bodydir == Direction.RIGHT || bodydir == Direction.LEFT)
            {
                GetComponent<Animator>().SetTrigger("StopRight");

            }
            if (bodydir == Direction.UP || bodydir == Direction.DOWN)
            {
                GetComponent<Animator>().SetTrigger("StopFront");

            }
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            headdir=Direction.LEFT;
            Shoot();
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            headdir = Direction.RIGHT;
            Shoot();
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            headdir = Direction.UP;
            Shoot();
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            headdir = Direction.DOWN;
            Shoot();
        }



    }
    void Shoot()
    {
        GameObject newTear = Instantiate<GameObject>(tearPrefab);
        newTear.transform.position=transform.position+new Vector3(0,0.4f,0);
        if (headdir == Direction.LEFT)
        {
            newTear.GetComponent<Tears>().Direction = new Vector2(-1, 0);
        }
        if (headdir == Direction.RIGHT)
        {
            newTear.GetComponent<Tears>().Direction = new Vector2(1, 0);
        }
        if (headdir == Direction.UP)
        {
            newTear.GetComponent<Tears>().Direction = new Vector2(0, 1);
        }
        if (headdir == Direction.DOWN)
        {
            newTear.GetComponent<Tears>().Direction = new Vector2(0, -1);
        }

    }

    private void FixedUpdate()
    {
        transform.Translate(move * speed * Time.fixedDeltaTime);
    }
}
