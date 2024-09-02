using System.Collections;
using System.Collections.Generic;
using TMPro; // TextMeshProGUI (텍스트)를 사용하기 위해 만들기
using UnityEngine;
using UnityEngine.SceneManagement; // 재시작 버튼을 위해 만들기
using UnityEngine.UI; // 슬라이더를 사용하기 위해 만들기

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    private List<Card> allCards;
    private Card flippedCard;

    private bool isFlipping = false; // 지금 뒤집히고 있지 않은 상황

    [SerializeField]
    private Slider timeoutSlider; // 제한시간 슬라이더와 관련된 변수

    [SerializeField]
    private TextMeshProUGUI timeoutText; // 제한시간 텍스트와 관련된 변수

    [SerializeField]
    private TextMeshProUGUI gameOverText; // 게임종료 시 띄우는 텍스트를 받는 변수

    [SerializeField]
    private GameObject gameOverPanel; // 게임종료 이후 패널의 등장과 관련된 변수
    private bool isGameOver = false; // 게임종료는 기본적으로 false 값

    [SerializeField]
    private float timeLimit = 60f; // 최대 시간은 60초
    private float currentTime; // 현재 남은 시간을 저장하는 변수

    private int totalMatches = 10; // 총 찾아야 하는 카드 매칭의 수
    private int matchesFound = 0; // 현재까지 찾은 카드 매칭의 수

    void Awake() // Start보다 먼저
    {
        if (instance == null) {
            instance = this;
        } // public static GameManager instance를 언제든지 사용
    } 

    // Start is called before the first frame update
    void Start()
    {
        Board board = FindObjectOfType<Board>(); // 보드를 가져와 변수에 저장
        allCards = board.GetCards(); // 보드에서 모든 카드를 가져와서 변수에 저장
        currentTime = timeLimit; // 시작하자마자의 현재 남은 시간은 제한시간과 동일
        SetCurrentTimeText(); // 현재 남은 시간을 텍스트와 연동하는 메서드 (시작했을 때만 연동됨)
        StartCoroutine("FlipAllCardsRoutine"); // FlipAllCardsRoutine(모든 카드 초반에 뒤집기)를 실행
    }
    
    void SetCurrentTimeText() 
    {
        int timeSec = Mathf.CeilToInt(currentTime); // 실수(현재 남은 시간)를 정수로 바꾸기
        timeoutText.SetText(timeSec.ToString()); // 정수(timeSec)를 문자열로 바꾸기
    }

    IEnumerator FlipAllCardsRoutine() {
        isFlipping = true; // 지금 뒤집히고 있는 상황
        yield return new WaitForSeconds(0.5f); // 시작(뒤집기)하기에 앞서 0.5초 대기하기
        FlipAllCards(); // 카드 뒤집기
        yield return new WaitForSeconds(3f); // (뒤집은 카드의 동물을 보여주기 : 암기의 시간) 3초 대기하기
        FlipAllCards(); // 다시 카드 뒤집기
        yield return new WaitForSeconds(3f); // 진정한 게임의 시작에 앞서 0.5초 대기하기
        isFlipping = false; // 지금 뒤집히지 않는 상황

        yield return StartCoroutine("CountDownTimerRoutine");
    }

    IEnumerator CountDownTimerRoutine()
    {
        while (currentTime > 0) {
            currentTime -= Time.deltaTime; // 현재 남은 시간이 점차 줄어들도록 설정
            timeoutSlider.value = currentTime / timeLimit; // 슬라이더의 값은 현재 남은 시간을 제한 시간으로 나눈 값과 동일
            SetCurrentTimeText(); // 현재 남은 시간을 텍스트와 연동하는 메서드 (시작하고 나서 카운트다운과 함께 연동됨)
            yield return null; // 바로 다음 프레임에 이어서 위의 작업을 다시 한다는 의미
        } // 시간이 더 이상 남아 있지 않으면 탈출

        GameOver(false); // GameOver 메서드의 인수를 false로 넣기 (게임 실패)
    }

    void FlipAllCards()
    {
        foreach (Card card in allCards) { // 모든 카드를 순회함
            card.FlipCard(); // 모든 카드를 하나씩 뒤집기 (순회하니까 결국에는 모두 뒤집기인 셈)
        }
    }

    public void CardClicked(Card card) // 카드를 클릭하면
    {
        if (isFlipping || isGameOver) { // 카드를 뒤집는 중'이거나' 게임이 종료되면
            return; // (카드를 클릭하더라도) 카드를 뒤집거나 매치여부를 확인하는 행동을 취하지 않음
        }

        card.FlipCard(); // 카드를 뒤집기

        if (flippedCard == null) { // 처음 뒤집은 카드라면
            flippedCard = card; // 현재 카드가 곧 뒤집힌 카드
        } else {
            StartCoroutine(CheckMatchRoutine(flippedCard, card));// 이후 뒤집은 카드라면 매치 여부를 확인해야 함

        }
    }

    IEnumerator CheckMatchRoutine(Card card1, Card card2) {

        isFlipping = true; // 매치 여부를 확인하는 중이기 때문

        if (card1.cardID == card2.cardID){ // 첫번째 카드와 두번째 카드의 ID가 동일한 경우
            Debug.Log("Same");
            card1.SetMatched();
            card2.SetMatched(); // 첫번째와 두번째 카드 모두 매치된 상태로 변경
            matchesFound++; // matchesFound(현재까지 찾은 카드 매칭의 수)를 1씩 더해주기

            if (matchesFound == totalMatches) { // 현재까지 찾은 카드 매칭의 수가 전체 찾아야 하는 카드 매칭의 수와 동일하면
                GameOver(true); // 게임을 종료
            }
        } else { // 다른 경우
            Debug.Log("Differ");
            yield return new WaitForSeconds(1f); // 1초 정도만 보여주고
            card1.FlipCard(); // 첫번째 카드 뒤집기
            card2.FlipCard(); // 두번째 카드 뒤집기
            yield return new WaitForSeconds(0.4f); // 0.4초 정도 대기
        }
        isFlipping = false; // 매치 여부 확인이 종료
        flippedCard = null; // 초기화하기 : (동일하든 다르든) 새로운 두 쌍을 또 비교해야 하기 때문
    }

    void GameOver (bool success) { // GameOver 메서드가 호출된 경우

        if (!isGameOver) { // 게임종료가 아닌 경우 (일단 게임종료를 호출한 상황에서는 게임종료가 된 것은 아니기 때문)
        
        isGameOver = true; // 게임종료로 바꿔버리기
        StopCoroutine("CountDownTimerRoutine"); // CountDownTimerRoutine을 중단하기 위함

        if (success) { // 성공하면
            gameOverText.SetText("Good job"); // 굿잡을 출력
        } else { // 실패하면
            gameOverText.SetText("Game Over"); // 게임오버를 출력
        }
        Invoke("ShowGameOverPanel", 2f); // 2초 뒤에 ShowGameOverPanel 메소드 활성화
        }
    }

    void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true); // 게임종료 패널을 활성화
    }

    public void Restart() // 재시작 버튼을 누르면
    {
        SceneManager.LoadScene("SampleScene"); // SampleScene(처음 화면)으로 돌아가기
    }
}
