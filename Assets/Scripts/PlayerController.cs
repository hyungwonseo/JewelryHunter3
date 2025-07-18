using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rbody;
    float axisH = 0.0f;
    public float speed = 3.0f;  // 이동 속도

    public float jump = 9.0f;       // 점프력
    public LayerMask groundLayer;   // 착지할 수 있는 레이어
    bool goJump = false;            // 점프 개시 플래그
    bool onGround = false;          // 지면에 서있는 플래그

    // 애니메이션 처리
    Animator animator; // 애니메이터
    public string stopAnime = "PlayerStop";
    public string moveAnime = "PlayerMove";
    public string jumpAnime = "PlayerJump";
    public string goalAnime = "PlayerGoal";
    public string deadAnime = "PlayerOver";
    string nowAnime = "";
    string oldAnime = "";

    public static string gameState = "playing"; // 게임 상태

    public int score;

    // Start is called before the first frame update
    void Start()
    {
        rbody = this.GetComponent<Rigidbody2D>();
        // Animator 가져오기
        animator = GetComponent<Animator>();
        nowAnime = stopAnime;
        oldAnime = stopAnime;

        gameState = "playing"; // 게임 중
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState != "playing")
        {
            return;
        }

        axisH = Input.GetAxisRaw("Horizontal");
        // 방향 조절
        if (axisH > 0.0f)
        {
            // 오른쪽 이동
            Debug.Log("오른쪽 이동");
            transform.localScale = new Vector2(1, 1);
        }
        else if (axisH < 0.0f)
        {
            // 왼쪽 이동
            Debug.Log("왼쪽 이동");
            transform.localScale = new Vector2(-1, 1);   
        }

        // 캐릭터 점프하기
        if (Input.GetButtonDown("Jump"))
        {
            Jump(); // 점프
        }
    }

    private void FixedUpdate()
    {
        if (gameState != "playing")
        {
            return;
        }

        // 착지 판정
        /*onGround = Physics2D.Linecast(transform.position,
                                      transform.position - (transform.up * 0.1f),
                                      groundLayer);*/
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down,
                                                   0.1f, groundLayer);
        onGround = hit.collider != null;

        if (onGround || axisH != 0)
        {            
            rbody.velocity = new Vector2(speed * axisH, rbody.velocity.y);
        }
        Debug.Log(" 점프! " + onGround + " " + goJump);
        if (onGround && goJump)
        {
            // 지면 위에서 점프키 눌림
            // 점프하기
            Debug.Log(" 점프! ");
            rbody.AddForce(new Vector2(0, jump), ForceMode2D.Impulse);  // 순간적인 힘 가하기
            goJump = false; // 점프 플래그 끄기
        }

        if (onGround)
        {
            // 지면 위
            if (axisH == 0)
            {
                nowAnime = stopAnime;       // 정지
            }
            else
            {
                nowAnime = moveAnime;       // 이동
            }
        }
        else
        {
            // 공중
            nowAnime = jumpAnime;
        }

        if (nowAnime != oldAnime)
        {
            oldAnime = nowAnime;
            animator.Play(nowAnime);        // 애니메이션 재생
        }
    }

    // 점프
    public void Jump()
    {
        goJump = true; // 점프 플래그 켜기
        Debug.Log(" 점프 버튼 눌림! ");
    }

    // 접촉 시작
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Goal")
        {
            Goal();        // 골
        }
        else if (collision.gameObject.tag == "Dead")
        {
            GameOver();     // 게임 오버
        }
        else if (collision.gameObject.tag == "ScoreItem")
        {
            ItemData item = collision.gameObject.GetComponent<ItemData>();
            score = item.value;
            Destroy(collision.gameObject);
        }
    }
    // 골
    public void Goal()
    {
        animator.Play(goalAnime);
        gameState = "gameclear";
        GameStop(); // 게임 중지
    }
    // 게임 오버
    public void GameOver()
    {
        animator.Play(deadAnime);

        gameState = "gameover";
        GameStop(); // 게임 중지
        // =====================
        // 게임 오버 연출
        // =====================
        // 플레이어의 충돌 판정 비활성
        GetComponent<CapsuleCollider2D>().enabled = false;
        // 플레이어를 위로 튀어 오르게 하는 연출
        rbody.AddForce(new Vector2(0, 5), ForceMode2D.Impulse);
    }

    // 게임 중지
    void GameStop()
    {
        // Rigidbody2D 가져오기
        Rigidbody2D rbody = GetComponent<Rigidbody2D>();
        // 속도를 0으로 하여 강제 정지
        rbody.velocity = new Vector2(0, 0);
    }
}