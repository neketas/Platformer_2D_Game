using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : Entity
{
   
    [SerializeField] float speed = 3f;
    [SerializeField] int lives;
    [SerializeField] int health;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] Image[] hearts;
    [SerializeField] Sprite aliveHearts;
    [SerializeField] Sprite deadHearts;

    [SerializeField] AudioSource jumpSound;
    [SerializeField] AudioSource walkingSound;
    [SerializeField] AudioSource attackSound;
    [SerializeField] AudioSource hurtSound;

    bool isGrounded = false;
    bool levelCompleted = false;

    public bool isAttacking = false;
    public bool isRecharged = true;

    public Transform attackPosition;
    public float attackrange;
    public LayerMask enemy;
    

    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sprite;

    public GameObject[] gems;
    public GameObject[] doorGems;
    public GameObject[] keys;
    public GameObject gameOverScreen;
    public GameObject winScreen;

    int coinsCount;
    public TMP_Text coinsText;

    public static Player Instance {get;set;}

    States State
    {
        get {return (States)anim.GetInteger("state");}
        set {anim.SetInteger("state", (int)value);}
    }
    void Awake()
    {
       
        Instance = this;
        lives = 5;
        health = lives;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        isRecharged = true;
    }
    void Update()
    {
        if(isGrounded && !isAttacking) State = States.idle;

        if(!isAttacking && Input.GetButton("Horizontal"))
            Run();
        if(!Input.GetButton("Horizontal"))
            walkingSound.Stop();
        if(!isAttacking && isGrounded && Input.GetButtonDown("Jump"))
            Jump();
        if(Input.GetButtonDown("Fire1"))
            Attack();

        if(health > lives)
            health = lives;

        for (int i = 0; i < hearts.Length; i++)
        {
            if(i < health)
                hearts[i].sprite = aliveHearts;
            else
                hearts[i].sprite = deadHearts;

            if(i < lives)
                hearts[i].enabled = true;
            else
                hearts[i].enabled = false;
        }
        if(doorGems[0].activeSelf && doorGems[1].activeSelf && doorGems[2].activeSelf && doorGems[3].activeSelf && !levelCompleted)
        {
           
            winScreen.SetActive(true);
            levelCompleted = true;
        }
    }
    void FixedUpdate()
    {
        CheckGround();
    }
    void Run()
    {
        if(isGrounded) State = States.run;

        Vector3 direction = transform.right * Input.GetAxis("Horizontal");
        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);
        sprite.flipX = direction.x < 0.0f;
        walkingSound.Play();
    }
    void Jump()
    {
        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        jumpSound.Play();
    }
    void CheckGround()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, 0.3f);
        isGrounded = collider.Length > 1;

        if(!isGrounded) State = States.jump;
    }
    void Attack()
    {
        if(isGrounded && isRecharged)
        {
            State = States.attack;
            isAttacking = true;
            isRecharged = false;
            attackSound.Play();
            StartCoroutine(AttackAnimation());
            StartCoroutine(AttackCooldown());
        }
    }
    void OnAttack()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPosition.position, attackrange, enemy);
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].GetComponent<Entity>().GetDamage();
            StartCoroutine(EnemyOnAttack(colliders[i]));
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosition.position, attackrange);
    }
    public void GetDamage()
    {
        hurtSound.Play();
        lives -= 1;
        Debug.Log(lives);
        if(lives < 1)
        {
            Die();
            gameOverScreen.SetActive(true);
        }
    }
    IEnumerator AttackAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }
    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(1f);
        isRecharged = true;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            sprite.color = Color.red; 
            GetDamage();
            StartCoroutine(BackToNormalColor());
        }
        if (other.gameObject.CompareTag("Coin"))
        {
            coinsCount++;
            coinsText.text = coinsCount.ToString();
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("Water"))
        {
            Die();
            gameOverScreen.SetActive(true);
        }
        if (other.gameObject.name == "KeyBlue")
        {
            keys[0].SetActive(true);
            Destroy(other.gameObject);
        }
        if (other.gameObject.name == "KeyRed")
        {
            keys[1].SetActive(true);
            Destroy(other.gameObject);
        }
        if (other.gameObject.name == "KeyGreen")
        {
            keys[2].SetActive(true);
            Destroy(other.gameObject);
        }
        if (other.gameObject.name == "GemBlue")
        {
            gems[0].SetActive(true);
            Destroy(other.gameObject);
        }
        if (other.gameObject.name == "GemPurple")
        {
            gems[1].SetActive(true);
            Destroy(other.gameObject);
        }
        if (other.gameObject.name == "GemRed")
        {
            gems[3].SetActive(true);
            Destroy(other.gameObject);
        }
        if (other.gameObject.name == "GemOrange")
        {
            gems[2].SetActive(true);
            Destroy(other.gameObject);
        }
        if (other.gameObject.name == "Doors")
        {
            if(gems[0].activeSelf)
            {
                Destroy(gems[0]);
                doorGems[0].SetActive(true);
            }
            if(gems[1].activeSelf)
            {
                Destroy(gems[1]);
                doorGems[1].SetActive(true);
            }
            if(gems[2].activeSelf)
            {
                Destroy(gems[2]);
                doorGems[2].SetActive(true);
            }
            if(gems[3].activeSelf)
            {
                Destroy(gems[3]);
                doorGems[3].SetActive(true);
            }
        }
        
    }
    void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.name == "GateRed" && keys[1].activeSelf)
        {
            Destroy(other.gameObject);
            Destroy(keys[1]);
        }
        if (other.gameObject.name == "GateBlue" && keys[0].activeSelf)
        {
            Destroy(other.gameObject);
            Destroy(keys[0]);
        }
        if (other.gameObject.name == "GateGreen" && keys[2].activeSelf)
        {
            Destroy(other.gameObject);
            Destroy(keys[2]);
        }
    }
    IEnumerator BackToNormalColor()
    {
        yield return new WaitForSeconds(0.3f);
        sprite.color = Color.white; 
    }
    IEnumerator EnemyOnAttack(Collider2D enemy)
    {
        SpriteRenderer enemySprite = enemy.GetComponentInChildren<SpriteRenderer>();
        enemySprite.color = Color.red;
        yield return new WaitForSeconds(0.3f);
        enemySprite.color = Color.white;
    }
    
}
public enum States
{
    idle,
    run,
    jump,
    attack
}
