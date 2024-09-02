using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{   
    [SerializeField]
    private GameObject cardPrefab;

    [SerializeField]
    private Sprite[] cardSprites; // 여러 동물 카드가 들어가는 리스트 만들기 (동물 카드를 모두 여기에 집어넣기)

    private List<int> cardIDList = new List<int>();
    private List<Card> cardList = new List<Card>();

    // Start is called before the first frame update
    void Start()
    {   
        GenerateCardID(); // 카드 ID 생성기 실행하기
        ShuffleCardID(); // 카드 섞는 메서드 실행하기
        InitBoard(); // InitBoard(카드 펼치기) 메서드 실행하기
    }

    void ShuffleCardID()
    {
        int cardCount = cardIDList.Count;
        for (int i = 0; i < cardCount; i++){ 
            int randomIndex = Random.Range(i, cardCount); // i부터 cardCount(전체 수)까지 랜덤으로 숫자를 뽑음
            int temp = cardIDList[randomIndex]; // temp라는 변수에 랜덤하게 뽑은 수 저장하기
            cardIDList[randomIndex] = cardIDList[i]; // 랜덤하게 뽑은 위치에는 현재 번호의 값을 넣고
            cardIDList[i] = temp; // 현재 번호 위치에는 랜덤하게 뽑은 값을 넣는 것 : 즉, 서로 뒤바꾸는 셈 

            // Debug.Log(cardIDList[i]); // 잘 섞였는지 확인하는 디버깅
        }
    }
    void GenerateCardID()
    {
        for (int i = 0; i < cardSprites.Length; i++) { // 00, 11, 22, ..., 99
            cardIDList.Add(i);
            cardIDList.Add(i);
        } // 각 동물 카드의 ID를 두 번씩 추가하여 매칭 쌍을 만들기
    }
    void InitBoard()
    {
        float spaceY = 1.8f; // 세로 간격
        float spaceX = 1.3f; // 가로 간격
        int rowCount = 5; // 세로 줄 수
        int colCount = 4; // 가로 줄 수
        int cardIndex = 0; // 리스트에서의 몇번째를 꺼낼지를 결정하는 인덱스

        for (int row = 0; row < rowCount; row++) { // rowCount로 지정한 수만큼 반복
            for (int col = 0; col < colCount; col++) { // colCount로 지정한 수만큼 반복
                float posX = (col - (colCount / 2)) * spaceX + (spaceX / 2); // 현재 열번호(col)와 전체 열번호(colCount) 그리고 가로 간격을 활용하여 각각의 X좌표를 구하는 공식 만들기
                float posY = (row - (int)(rowCount / 2)) * spaceY; // 현재 행번호(row)와 전체 행번호(rowCount) 그리고 세로 간격을 활용하여 각각의 Y좌표를 구하는 공식 만들기
                Vector3 pos = new Vector3(posX, posY, 0f); // posX와 posY를 위치 변수로 지정하기
                GameObject cardObject = Instantiate(cardPrefab, pos, Quaternion.identity); // 카드를 pos의 위치에 (회전은 생략한 것이나 다름 없음)
                
                Card card = cardObject.GetComponent<Card>(); // 생성된 카드 오브젝트(도형)에서 Card.cs라는 스크립트(cs 파일)를 가져오기
                int cardID = cardIDList[cardIndex++]; // 섞이고 쌍으로 만들어진 카드들의 ID를 하나씩 가져오기 위함 (더 이상의 중복이나 랜덤의 발생을 막기 위함)
                card.SetCardID(cardID); // 앞서 저장된 변수를 카드 ID로 지정함
                card.SetAnimalSprite(cardSprites[cardID]); // 리스트에서 앞서 저장된 변수 번째에 있는 것을 동물 이미지로 지정

                cardList.Add(card);
            }
        }
    }
    public List<Card> GetCards() 
    {
        return cardList;
    }
}
