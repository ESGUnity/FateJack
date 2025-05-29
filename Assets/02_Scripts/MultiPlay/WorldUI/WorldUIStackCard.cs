using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class WorldUIStackCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{   
    // UI 관련
    public Card cardInfo;
    public CardEffect cardEffect;
    public PRS OriginPRS;
    public int OriginOrder;
    const float POINTER_ENTER_ANIMATION_TIME = 0.3f;

    // 카드 시각 효과
    [SerializeField] SpriteRenderer stackCardEffectImage;
    [SerializeField] TMP_Text stackCardNumber;
    [SerializeField] TMP_Text stackCardEffectText1;
    [SerializeField] TMP_Text stackCardEffectText2;
    [SerializeField] SpriteRenderer stackCardEnterEdge;

    public void SetCardInfo(Card card)
    {
        // 카드 정보 설정
        cardInfo = card;
        cardEffect = CardEffectList.FindCardEffectToKey(card.CardEffectKey.ToString());

        // 카드 텍스트 설정
        //stackCardEffectImage = // 나중에 어드레서블로 카드 이미지 가져오는 등 그렇게 하자
        stackCardNumber.text = card.Number.ToString();
        stackCardEffectText1.text = cardEffect.Name.ToString();
        stackCardEffectText2.text = cardEffect.Name.ToString();
    }
    public void SetOrder(int order)
    {
        GetComponent<SpriteRenderer>().sortingOrder = order;
        stackCardEffectImage.sortingOrder = order + 1;
        stackCardNumber.GetComponent<MeshRenderer>().sortingOrder = order + 2;
        stackCardEffectText1.GetComponent<MeshRenderer>().sortingOrder = order + 3;
        stackCardEffectText2.GetComponent<MeshRenderer>().sortingOrder = order + 4;
        stackCardEnterEdge.sortingOrder = order + 5;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Transform>().DOLocalMove(OriginPRS.Pos + new Vector3(0, 0.2f, 0), POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);
        GetComponent<Transform>().DOScale(OriginPRS.Scale + new Vector3(0.1f, 0.1f, 0.1f), POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);
        SetOrder(300);
        stackCardEnterEdge.DOFade(1f, 0.2f).SetEase(Ease.OutQuart);

        WorldUICardInfo.Instance.DisplayCardInfo(true, cardInfo);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Transform>().DOLocalMove(OriginPRS.Pos, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);
        GetComponent<Transform>().DOScale(OriginPRS.Scale, POINTER_ENTER_ANIMATION_TIME).SetEase(Ease.OutQuart);
        SetOrder(OriginOrder);
        stackCardEnterEdge.DOFade(0f, 0.2f).SetEase(Ease.OutQuart);

        WorldUICardInfo.Instance.DisplayCardInfo(false);
    }
}
