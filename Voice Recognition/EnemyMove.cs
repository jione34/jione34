using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    CapsuleCollider2D capsuleCollider;
    public int normalMove;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        normalMove = -1;
    }

    void Update()
    {
        //애니메이션 제어
        if (Mathf.Abs(rigid.velocity.x) < 0.5)
            anim.SetBool("IsWorking", false);
        else
            anim.SetBool("IsWorking", true);

        if (normalMove == 1)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;
         

    }


    void FixedUpdate()
    {
        rigid.velocity = new Vector2(normalMove, rigid.velocity.y);

        //타일 위에 고정
        Vector2 frontposition = new Vector2(rigid.position.x + normalMove * 0.3f, rigid.position.y);
        Debug.DrawRay(frontposition, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontposition, Vector3.down, 1, LayerMask.GetMask("Platform"));
        if (rayHit.collider == null)
        {
            normalMove *= -1;
        }
    }

    public void OnDamaged()
    {
        spriteRenderer.color = new Color(1, 0.4f, 0.4f, 0.5f);
        spriteRenderer.flipY = true;
        capsuleCollider.enabled = false;
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        Invoke("OffActive", 3);
    }

    void OffActive()
    {
        gameObject.SetActive(false);
    }
}
