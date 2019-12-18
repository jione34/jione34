using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager gameManager;
    public float maxSpeed;
    public float jumpPower;
    public float sensitivity = 100;
    public float loudness = 0;
    public AudioManager audioManager;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    CapsuleCollider2D capsuleCollider;
    AudioSource _audio;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        _audio = GetComponent<AudioSource>();
    }

    void Start()
    {
        _audio.clip = Microphone.Start(null, true, 1, 4400);
        _audio.loop = true;
        while (!(Microphone.GetPosition(null) > 0)) { }
        _audio.Play();
    }

    void Update()
    {
        //점프
        loudness = GetAveragedVolume() * sensitivity * 2;
        if (loudness > 6 && !anim.GetBool("IsJumping") && loudness < 30)
        {
            jumpPower = loudness;
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("IsJumping", true);
            audioManager.PlaySound("JUMP");
        }


        //움직이기
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        //브레이크
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        //애니메이션 제어
        if (Mathf.Abs(rigid.velocity.x) < 0.5)
            anim.SetBool("IsWorking", false);
        else
            anim.SetBool("IsWorking", true);

        //방향전환
        if (Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

    }

    void FixedUpdate()
    {

        //오른쪽 왼쪽 속도제어
        if (rigid.velocity.x > maxSpeed)
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed * (-1))
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);

        //Landing Platform
        if (rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));

            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.5f)
                    anim.SetBool("IsJumping", false);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision) //충돌이벤트
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform);
            }
            else
                OnDamaged(collision.transform.position);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //점수 획득
        if (collision.gameObject.tag == "Item")
        {
            gameManager.stagePoint += 100;
            collision.gameObject.SetActive(false);
            audioManager.PlaySound("ITEM");
        }
        //스테이지 넘기기
        else if (collision.gameObject.tag == "Finish")
        {
            gameManager.NestStage();
            audioManager.PlaySound("FINISH");
        }
    }

    float GetAveragedVolume()
    {
        float[] data = new float[256];
        float a = 0;
        _audio.GetOutputData(data, 0);
        foreach (float s in data)
        {
            a += Mathf.Abs(s);
        }
        return a / 256;
    }

    void OnAttack(Transform enemy)
    {
        //포인트
        gameManager.stagePoint += 100;
        //적 죽음
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
        audioManager.PlaySound("HIT");
    }

    void OnDamaged(Vector2 targetPosition)
    {
        //피격이벤트
        gameManager.HealthDown();
        gameObject.layer = 11;
        spriteRenderer.color = new Color(1, 0.4f, 0.4f, 0.5f);

        int dirction = transform.position.x - targetPosition.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirction, 1) * 7, ForceMode2D.Impulse);

        //애니메이션
        anim.SetTrigger("doDamaged");

        Invoke("OffDamaged", 0.5f);
        audioManager.PlaySound("DAMAGED");
    }

    void OffDamaged() //무적이벤트 끄기
    {
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void OnDie() //플레이어 사망
    {
        spriteRenderer.color = new Color(1, 0.4f, 0.4f, 0.5f);
        spriteRenderer.flipY = true;
        capsuleCollider.enabled = false;
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        audioManager.PlaySound("DIE");

    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }
}