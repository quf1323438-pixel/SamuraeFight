using System.Collections.Generic;
using UnityEngine.InputSystem;

//키보드 조종자: 매 프레임 키를 읽고, 눌린 순간을 버퍼(선입력 쪽지)에 기록한다
public class KeyboardInput : IPlayerInput
{
    private const int BufferFrames = 6;   //쪽지 유효기간 = 6프레임 (약 0.1초)

    //"키 이름" -> 남은 프레임 수
    private Dictionary<string, int> buffer = new Dictionary<string, int>();

    public float MoveInput { get; private set; }
    public bool RunHeld { get; private set; }

    public void Tick()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        //── 누르고 있는 "상태"들 (isPressed) ──
        MoveInput = 0f;
        if (kb.aKey.isPressed) MoveInput = -1f;
        else if (kb.dKey.isPressed) MoveInput = 1f;

        RunHeld = kb.shiftKey.isPressed;

        //── 기존 쪽지들 유효기간 깎기, 0이면 버리기 ──
        foreach (var key in new List<string>(buffer.Keys))
            if (--buffer[key] <= 0) buffer.Remove(key);

        //── 눌린 "사건"들 (wasPressedThisFrame) -> 쪽지 붙이기 ──
        if (kb.kKey.wasPressedThisFrame) buffer["k"] = BufferFrames;   //점프/공격
        if (kb.sKey.wasPressedThisFrame) buffer["s"] = BufferFrames;   //조합용
        if (kb.lKey.wasPressedThisFrame) buffer["l"] = BufferFrames;   //대쉬
    }

    public bool IsBuffered(string action) => buffer.ContainsKey(action);

    public void Consume(string action) => buffer.Remove(action);
}
