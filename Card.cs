using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // 카드 뒤집기 (너비 줄였다가 늘리기)

public class Card : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer cardRenderer; // 카드 오브젝트 넣을 곳

    [SerializeField]
    private Sprite animalSprite; // 동물 사진 넣을 곳

    [SerializeField]
    private Sprite backSprite; // 배경 사진 넣을 곳
    private bool isFlipped = false;  // 현재 뒤집어져있는지를 알려주는 스위치 만들기
    private bool isFlipping = false; // 현재 뒤집어지는 중인지를 알려주는 스위치 만들기
    private bool isMatched = false; // 매치 여부를 알려주는 스위치 만들기
    public int cardID;
    
    public void SetCardID(int id)
    {
        cardID = id; // 카드에 ID 설정
    }

    public void SetMatched() 
    {
        isMatched = true; // 매치가 되었다고 변경
    }

    public void SetAnimalSprite(Sprite sprite)
    {
        animalSprite = sprite; // 카드에 표시될 동물 이미지 설정
    }

    public void FlipCard()
    {

        isFlipping = true; // 카드가 현재 뒤집어지는 중임을 표시

        Vector3 orignalScale = transform.localScale; // 원래 크기
        Vector3 targetScale = new Vector3(0f, orignalScale.y, orignalScale.z); // X축(너비)을 0으로 만들기, 나머지 Y, Z축은 그대로

        transform.DOScale(targetScale, 0.2f).OnComplete(()=> // 0.2초 동안 변수에 지정된 대로 스케일을 변경
        {
            isFlipped = !isFlipped; // 앞면이면 뒷면으로, 뒷면이면 앞면으로
            if (isFlipped) {
                cardRenderer.sprite = animalSprite; // 동물 사진이 보이도록 뒤집기
            } else {
                cardRenderer.sprite = backSprite; // 배경 사진이 보이도록 뒤집기
            }

            transform.DOScale(orignalScale, 0.2f).OnComplete(()=> {
                isFlipping = false; // 뒤집어지는 중이 종료되었음을 알림
            }); // 미리 지정한 원래 크기로 0.2초만에 되돌리기

        }); 
    }

    void OnMouseDown() 
    {
        if (!isFlipping && !isMatched && !isFlipped) { // 뒤집어지는 중이 아닐 때, 매치가 되지 않은 상태에만, (매치되지 않은 채) 하나만 뒤집어진 상태가 아닌 경우에만
            // FlipCard(); // FlipCard 메서드를 호출하기
            GameManager.instance.CardClicked(this); // 카드를 클릭했을 때 뒤집고 매치 여부까지 확인하는 메서드를 작동함
        }
        Debug.Log("mouse down"); // 충돌 영역을 설정(Collider, 해야 작동함
    }
}
