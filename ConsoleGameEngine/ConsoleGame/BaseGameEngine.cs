// 같은 질문을 했는데

// AI 임의로 구성한 내용을 파악하고 이 부분을 A로 만들어서 만들어지는 결과를 일정한 수준으로 맞추게 유도하는 작업.

// # ㅁ -> 가로행이 밀려서 그림이 그려진다

// '상속'
// 내가 만들고 싶은 것을 클래스 만들어주세요.
// abstract 추상 : 기능을 구현하지는 않을거지만 너가 이 내용을 무조건 구현해야해.

abstract class BaseGameEngine
{
    // 입력기능     Input
    // 갱신기능     Update
    // 렌더링기능   Rendering

    abstract public void Input();

    abstract public void Update();

    abstract public void Rendering();
}

// MyGameEngine : 키밀림X , 화면이 깜빡이지도 않는 3요소가 있는