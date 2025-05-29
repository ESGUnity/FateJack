using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class S_DialogSystem : MonoBehaviour
{
    // 다이얼로그 정보
    public int Trial;
    public S_DialogStateEnum DialogState; // 다이얼로그 호출 시
    public bool IsTutorial;

    // 인스펙터 할당
    [SerializeField] DialogData[] dialogArray;

    // 컴포넌트
    RectTransform panel_DialogBase;
    Image image_Character;
    Image image_DialogBase;
    TMP_Text text_Name;
    TMP_Text text_Dialog;
    GameObject image_OKBtnBase;

    GraphicRaycaster raycaster;
    EventSystem eventSystem;

    // 부가 변수
    Coroutine typingCoroutine;
    bool isFirst = true;
    int currentDialogIndex = -1;
    float typingSpeed = 0.05f;
    bool isTypingEffect = false;

    void Update()
    {
        if ((GetActiveBtn() == S_ActivateUIEnum.DeckInfoBtn ||
            GetActiveBtn() == S_ActivateUIEnum.HitBtn ||
            GetActiveBtn() == S_ActivateUIEnum.DeterminationHitBtn ||
            GetActiveBtn() == S_ActivateUIEnum.TwistBtn) &&
            Input.GetMouseButtonDown(0))
        {
            PointerEventData pointerData = new PointerEventData(eventSystem)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count > 0)
            {
                GameObject firstHit = results[0].gameObject;

                if (firstHit.layer == LayerMask.NameToLayer("UI"))
                {
                    Debug.Log("첫 번째 UI 충돌: " + firstHit.name);
                    ClickOKBtn();
                }
            }
        }

        // 마우스 클릭 시 타이핑 효과 중단하기
        if (Input.GetMouseButtonDown(0))
        {
            if (isTypingEffect)
            {
                isTypingEffect = false;

                // 타이핑 효과를 중지하고 대사 전체를 즉시 출력하고 확인 버튼 활성화
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                    typingCoroutine = null;
                }
                text_Dialog.text = dialogArray[currentDialogIndex].Dialog;

                // OKBtn이거나 보여주기용 캔버스라면 활성화
                if (GetActiveBtn() == S_ActivateUIEnum.OKBtn ||
                    GetActiveBtn() == S_ActivateUIEnum.CreatureCanvas ||
                    GetActiveBtn() == S_ActivateUIEnum.StatCanvas ||
                    GetActiveBtn() == S_ActivateUIEnum.LootCanvas)
                {
                    image_OKBtnBase.SetActive(true);
                }
            }
        }
    }
    public void SetUp()
    {
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);
        Image[] images = GetComponentsInChildren<Image>(true);

        panel_DialogBase = GetComponent<RectTransform>();
        image_Character = Array.Find(images, c => c.gameObject.name.Equals("Image_Character"));
        image_DialogBase = Array.Find(images, c => c.gameObject.name.Equals("Image_DialogBase"));
        text_Name = Array.Find(texts, c => c.gameObject.name.Equals("Text_Name"));
        text_Dialog = Array.Find(texts, c => c.gameObject.name.Equals("Text_Dialog"));
        image_OKBtnBase = Array.Find(transforms, c => c.gameObject.name.Equals("Image_OKBtnBase")).gameObject;

        SetActiveObjects(true);

        // 그래픽 레이케스터 찾기
        raycaster = GetComponent<GraphicRaycaster>();
        if (raycaster == null)
        {
            raycaster = GetComponentInParent<GraphicRaycaster>();
        }

        // 이벤트 시스템 찾기
        eventSystem = EventSystem.current;

        // 첫 대화는 마우스 클릭 없이 재생되어야하기에 다음 로직 추가
        if (isFirst)
        {
            SetNextDialog();

            isFirst = false;
        }
    }
    void SetNextDialog()
    {
        SetActiveObjects(true);
        currentDialogIndex++; // 다이얼로그 인덱스++
        text_Name.text = dialogArray[currentDialogIndex].Name; // 이름 설정
        image_Character.sprite = dialogArray[currentDialogIndex].Character; // 캐릭터 이미지 설정
        panel_DialogBase.anchoredPosition = dialogArray[currentDialogIndex].DialogBasePos; // 다이얼로그 위치 설정
        S_DialogManager.Instance.SetBtn(GetActiveBtn()); // 버튼 설정

        typingCoroutine = StartCoroutine(OnTypingText());
    }
    void SetActiveObjects(bool visible)
    {
        panel_DialogBase.gameObject.SetActive(visible);
        image_Character.gameObject.SetActive(visible);
        image_DialogBase.gameObject.SetActive(visible);
        text_Name.gameObject.SetActive(visible);
        text_Dialog.gameObject.SetActive(visible);
        image_OKBtnBase.SetActive(false); // 버튼은 별도로 활성화한다.
    }
    IEnumerator OnTypingText()
    {
        int index = 0;

        isTypingEffect = true;

        while (index < dialogArray[currentDialogIndex].Dialog.Length)
        {
            text_Dialog.text = dialogArray[currentDialogIndex].Dialog.Substring(0, index);

            index++;

            yield return new WaitForSeconds(typingSpeed);
        }

        isTypingEffect = false;

        text_Dialog.text = dialogArray[currentDialogIndex].Dialog;

        // OKBtn이거나 보여주기용 캔버스라면 활성화
        if (GetActiveBtn() == S_ActivateUIEnum.OKBtn ||
            GetActiveBtn() == S_ActivateUIEnum.CreatureCanvas ||
            GetActiveBtn() == S_ActivateUIEnum.StatCanvas ||
            GetActiveBtn() == S_ActivateUIEnum.LootCanvas)
        {
            image_OKBtnBase.SetActive(true);
        }
    }
    public void ClickOKBtn()
    {
        if (dialogArray.Length > currentDialogIndex + 1) // 다음 대화가 존재한다면
        {
            SetNextDialog();
        }
        else // 다음 대화가 없다면
        {
            SetActiveObjects(false);

            S_DialogManager.Instance.IsCompleteDialog = true;

            Destroy(gameObject);
        }
    }
    public S_ActivateUIEnum GetActiveBtn()
    {
        return dialogArray[currentDialogIndex].ActiveBtn;
    }
}

[Serializable]
public struct DialogData
{
    public string Name;
    public Sprite Character;
    [TextArea(3, 5)] public string Dialog;
    public Vector2 DialogBasePos;
    public S_ActivateUIEnum ActiveBtn;
}