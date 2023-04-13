using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacle : Entity
{
    private float speed = 3.5f;
    private Vector3 direction;
    private SpriteRenderer sprite;

    Animator anim;
    Collider2D col;
    
    void Start()
    {
        lives = 3;
        direction = transform.right;
        sprite = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }
    private void Move()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + transform.up * 0.1f + transform.right * direction.x * 0.7f, 0.1f);
        if(colliders.Length > 0) direction *= -1f;
        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, Time.deltaTime);
        sprite.flipX = direction.x < 0.0f;
    }
    void Update()
    {
        if(lives > 0)
        {
            Move();
        }
    }
    IEnumerator BackToNormalColor()
    {
        yield return new WaitForSeconds(0.3f);
        sprite.color = Color.white; 
    }
    void OnCollisionEnter2D(Collision2D collision) 
    {
        if(lives > 0 && collision.gameObject == Player.Instance.gameObject)
        {
            Player.Instance.GetDamage();
        }
    }
    public override void Die()
    {
        col.isTrigger = true;
        anim.SetTrigger("die");
    }
}
