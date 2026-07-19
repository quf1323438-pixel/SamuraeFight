using UnityEngine;
using UnityEngine.InputSystem;

public class NolmarState : IPlayerState // 좌우움직임 + 점프
{
    public void Enter(PlayerMove player) {} 
      
    public void Update(PlayerMove player) // 입출력 구현
    {
        // 좌우이동 입력
        player.movement = 0f;
        if (Keyboard.current.aKey.isPressed) player.movement = -1f;
        else if (Keyboard.current.dKey.isPressed) player.movement = 1f;

        //대쉬 입력
        if (Keyboard.current.lKey.wasReleasedThisFrame) player.ChangeState(new DashState());

        // 플레이어 달리기 입력 & 속도 조절
        player.run = Keyboard.current.shiftKey.isPressed;
        player.speed = player.run ? player.RunSpeed : player.Warkspeed;

        // 플레이어 점프
        if (Keyboard.current.kKey.wasPressedThisFrame && player.isGround)
        {
            Debug.Log("ss");
            player.rb.linearVelocity = new Vector2(player.rb.linearVelocity.x, player.jumpforce);
            player.anim.SetBool("isjump", true);
        }

        //플레이어가 아래로 추락하고있다면 플레이어 애니메이션 상태 변경
        if (player.rb.linearVelocity.y < -0.3f) player.anim.SetBool("isjump", true);

        // 플레이어 바라보는 방향으로 플립
        if (player.movement != 0f) player.sr.flipX = player.movement < 0f; 

        if (player.isGround) Debug.Log("qq");

        // 플레이어 바닥 감지 - 상태가 읽기 전에 먼저 갱신
        player.isGround = Physics2D.BoxCast(player.transform.position, player.boxsize, 0f, 
        Vector2.down, 0.1f, player.Lground);


        //----------------------------------------------------------------------
        //애니메이션
        //----------------------------------------------------------------------

        // 플레이어 점프 애니매이션 전환
        if (player.rb.linearVelocity.y >= 0) player.anim.SetFloat("jump", 0);  
        else player.anim.SetFloat("jump", 1);

        // 플레이어 애니메이션 상태 전환
        if (player.isGround && player.rb.linearVelocity.y <= 0f) player.anim.SetBool("isjump", false); 
        
        //이동 애니메이션 전환
        if(player.movement == 0f) player.anim.SetFloat("speed", 0);
        else if (player.run && player.movement != 0f) player.anim.SetFloat("speed", 1f);
        else player.anim.SetFloat("speed", 0.5f);

    }    
    
    public void FixedUpdate(PlayerMove player) // 물리적 구현
    {
        player.rb.linearVelocity = new Vector2(player.movement * player.speed, 
        player.rb.linearVelocity.y);
    }

    public void Exit(PlayerMove player) {}
}

public class DashState : IPlayerState // 대쉬
{
    public void Enter(PlayerMove player) 
    {
        // 플레이어 기존 중력값 초기화
        player.rb.gravityScale = 0f;

        // 플레이어 애니메이션 전환
        player.anim.SetBool("isDash", true);

        //기존 벡터값 초기화
        player.rb.linearVelocity = Vector2.zero;

        player.rb.linearVelocity = new Vector2(player.DashForce * player.PlayerDir, player.rb.linearVelocity.y);
        player.ChangeState(new NolmarState());
    }     

    public void Update(PlayerMove player) {} 
    
    public void FixedUpdate(PlayerMove player) {}

    public void Exit(PlayerMove player)
    {
        // 플레이어 중력값 되돌려두기
        player.rb.gravityScale = player.standGravity;

        // 플레이어 애니메이션 전환
        player.anim.SetBool("isDash", false);
    }
}

