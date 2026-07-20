using UnityEngine;
using UnityEngine.InputSystem;

public class NolmarState : IPlayerState // 좌우움직임 + 점프
{
    public void Enter(PlayerMove player) {} 
      
    public void Update(PlayerMove player) // 입출력 구현
    {
        // 좌우이동 입력
        player.movement = player.input.MoveInput;

        // 플레이어 달리기 입력 & 속도 조절
        player.run = player.input.RunHeld;
        player.speed = player.run ? player.RunSpeed : player.Warkspeed;

        // 조합 공격 (S+K) - 구체적인 조합을 단독 입력보다 먼저 검사!
        if (player.input.IsBuffered("s") && player.input.IsBuffered("k"))
        {
            player.input.Consume("s");
            player.input.Consume("k");
            player.ChangeState(new AttackState(1));
            return;
        }

        // 대쉬 입력 (쿨타임 다 돌았을 때만)
        if (player.input.IsBuffered("l") && player.Dashcooltime_ <= 0f)
        {
            player.input.Consume("l");
            player.Dashcooltime_ = player.Dashcooltime;   //쿨타임 리셋
            player.ChangeState(new DashState());
            return;
        }

        // 플레이어 점프
        if (player.input.IsBuffered("k") && player.isGround)
        {
            player.input.Consume("k");
            player.rb.linearVelocity = new Vector2(player.rb.linearVelocity.x, player.jumpforce);
            player.anim.SetBool("isjump", true);
        }

        //플레이어가 아래로 추락하고있다면 플레이어 애니메이션 상태 변경
        if (player.rb.linearVelocity.y < -0.3f) player.anim.SetBool("isjump", true);

        // 플레이어 바라보는 방향으로 플립
        if (player.movement != 0f) player.sr.flipX = player.movement < 0f;

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
    }     

    public void Update(PlayerMove player)
    {
        // DashState.Update 안에서
        AnimatorStateInfo info = player.anim.GetCurrentAnimatorStateInfo(0);

        if (info.IsName("Dash") && info.normalizedTime >= 1f)
        {
            player.ChangeState(new NolmarState());
            return;
        }
    } 
    
    public void FixedUpdate(PlayerMove player) {}

    public void Exit(PlayerMove player)
    {
        // 플레이어 중력값 되돌려두기
        player.rb.gravityScale = player.standGravity;

        // 플레이어 애니메이션 전환
        player.anim.SetBool("isDash", false);
    }
}

public class AttackState : IPlayerState // 공격
{
    public int attacktype;
    public AttackState(int type) { attacktype = type;}

    public void Enter(PlayerMove player) 
    {
        player.anim.SetBool("isAttack", true);

        if (attacktype == 1) player.anim.SetFloat("attack_", 0);
    }     

    public void Update(PlayerMove player) //공격 입력
    {
       AnimatorStateInfo info = player.anim.GetCurrentAnimatorStateInfo(0);

        if (info.IsName("isAttack") && info.normalizedTime >= 1f)
        {
            player.ChangeState(new NolmarState());
            return;
        }
    } 
    
    public void FixedUpdate(PlayerMove player) {}

    public void Exit(PlayerMove player)
    {
        player.anim.SetBool("isAttack", false);
    }
}

