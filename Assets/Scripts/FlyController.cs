using UnityEngine;

public class FlyController : Monster
{
    protected override void Move()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        transform.Translate(dir * speed * Time.deltaTime);
    }
}