public interface IPlayerState
{
    void Enter(PlayerMove player);        //이 상태로 들어올 때 1번 실행
    void Update(PlayerMove player);       //이 상태인 동안 매 프레임 실행
    void FixedUpdate(PlayerMove player);  //이 상태인 동안 물리 스텝마다 실행
    void Exit(PlayerMove player);         //이 상태에서 나갈 때 1번 실행
}