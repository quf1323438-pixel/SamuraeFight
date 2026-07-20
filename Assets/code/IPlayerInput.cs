//조종자 약속: 사람이든 AI든 이 질문들에 답할 수 있으면 캐릭터를 조종할 수 있다
public interface IPlayerInput
{
    float MoveInput { get; }          //-1(왼쪽), 0, 1(오른쪽)
    bool RunHeld { get; }             //달리기 키를 누르고 있는가

    void Tick();                      //매 프레임 1번 호출, 입력 수집 + 버퍼 갱신
    bool IsBuffered(string action);   //최근 6프레임 안에 이 키가 눌렸는가
    void Consume(string action);      //사용한 입력 지우기 (중복 발동 방지)
}
