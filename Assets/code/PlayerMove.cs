using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    //컴포넌트 관리
    public Animator anim;

    public Rigidbody2D rb;
    public SpriteRenderer sr;
    public BoxCollider2D col;

    //좌우 이동 변수
    public float speed;
    public float Warkspeed;
    public float RunSpeed;
    public float movement;
    public bool run = false;

    //점프 변수
    public float jumpforce = 12f;
    public Vector2 boxsize;
    public LayerMask Lground;
    public bool isGround;

    //대쉬 변수
    public float Dashcooltime; 
    public float Dashcooltime_; 
    public float Dashtime; 
    public float Dashtime_; 

    public float standGravity;
    public float DashForce;
    public float PlayerDir;

    //상태 머신 변수
    public IPlayerState currentState;

    //입력 (조종자 - 지금은 키보드, 나중엔 AI도 꽂을 수 있음)
    public IPlayerInput input;


    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
        currentState = new NolmarState();
        input = new KeyboardInput();
        standGravity = rb.gravityScale;
    }

    void Start()
    {
        boxsize = col.bounds.size;   //바닥 감지용 상자 크기 = 콜라이더 크기
        Dashcooltime_ = Dashcooltime;
    }

    public void ChangeState(IPlayerState newState)
    {
        currentState.Exit(this);    //이전 상태 뒷정리
        currentState = newState;
        currentState.Enter(this);   //새 상태 준비
    }
    

    void Update()
    {
        //① 감각 수집 - 입력과 바닥 감지를 먼저
        input.Tick();
        isGround = Physics2D.BoxCast(transform.position, boxsize, 0f, Vector2.down, 0.1f, Lground);

        //② 판단은 상태에게
        currentState.Update(this);

        //플레이어 쿨타임 관리
        Dashcooltime_ -= Time.deltaTime;

        // 플레이어가 바라보는 방향
        if (movement != 0) PlayerDir = movement;
    }
    void FixedUpdate() { currentState.FixedUpdate(this);}
}
