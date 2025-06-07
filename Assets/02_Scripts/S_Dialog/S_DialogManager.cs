//using DG.Tweening;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using TMPro;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;

//public class S_DialogManager : MonoBehaviour
//{
//    // 다이얼로그 정보
//    public int Trial;
//    public S_DialogStateEnum DialogState; // 다이얼로그 호출 시
//    public bool IsTutorial;

//    // 인스펙터 할당
//    [SerializeField] DialogData[] dialogArray;

//    // 컴포넌트
//    RectTransform panel_DialogBase;
//    Image image_Character;
//    Image image_DialogBase;
//    TMP_Text text_Name;
//    TMP_Text text_Dialog;
//    GameObject image_OKBtnBase;
//    GraphicRaycaster raycaster;

//    // 부가 변수
//    Coroutine typingCoroutine;
//    bool isFirst = true;
//    int currentDialogIndex = -1;
//    float typingSpeed = 0.05f;
//    bool isTypingEffect = false;

//    [Header("VFX")]
//    Vector3 posOffsetValue = new Vector3(0, 20, 0);

//    // 싱글턴
//    static S_DialogManager instance;
//    public static S_DialogManager Instance { get { return instance; } }

//    void Awake()
//    {
//        // 싱글턴
//        if (instance == null)
//        {
//            instance = this;
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    public void SetUp()
//    {
//        Transform[] transforms = GetComponentsInChildren<Transform>(true);
//        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);
//        Image[] images = GetComponentsInChildren<Image>(true);

//        panel_DialogBase = GetComponent<RectTransform>();
//        image_Character = Array.Find(images, c => c.gameObject.name.Equals("Image_Character"));
//        image_DialogBase = Array.Find(images, c => c.gameObject.name.Equals("Image_DialogBase"));
//        text_Name = Array.Find(texts, c => c.gameObject.name.Equals("Text_Name"));
//        text_Dialog = Array.Find(texts, c => c.gameObject.name.Equals("Text_Dialog"));
//        image_OKBtnBase = Array.Find(transforms, c => c.gameObject.name.Equals("Image_OKBtnBase")).gameObject;

//        text_Dialog = Array.Find(texts, c => c.gameObject.name.Equals("Text_Dialog"));

//        SetActiveObjects(true);

//        // 그래픽 레이케스터 찾기
//        raycaster = GetComponent<GraphicRaycaster>();
//        if (raycaster == null)
//        {
//            raycaster = GetComponentInParent<GraphicRaycaster>();
//        }

//        // 첫 대화는 마우스 클릭 없이 재생되어야하기에 다음 로직 추가
//        if (isFirst)
//        {
//            SetNextDialog();

//            isFirst = false;
//        }
//    }
//    void SetNextDialog()
//    {
//        SetActiveObjects(true);
//        currentDialogIndex++; // 다이얼로그 인덱스++
//        text_Name.text = dialogArray[currentDialogIndex].Name; // 이름 설정
//        image_Character.sprite = dialogArray[currentDialogIndex].Character; // 캐릭터 이미지 설정
//        panel_DialogBase.anchoredPosition = dialogArray[currentDialogIndex].DialogBasePos; // 다이얼로그 위치 설정
//        S_DialogInfoSystem.Instance.SetBtn(GetActiveBtn()); // 버튼 설정

//        typingCoroutine = StartCoroutine(OnTypingText());
//    }
//    void SetActiveObjects(bool visible)
//    {
//        panel_DialogBase.gameObject.SetActive(visible);
//        image_Character.gameObject.SetActive(visible);
//        image_DialogBase.gameObject.SetActive(visible);
//        text_Name.gameObject.SetActive(visible);
//        text_Dialog.gameObject.SetActive(visible);
//        image_OKBtnBase.SetActive(false); // 버튼은 별도로 활성화한다.
//    }
//    IEnumerator OnTypingText()
//    {
//        int index = 0;

//        isTypingEffect = true;

//        while (index < dialogArray[currentDialogIndex].Dialog.Length)
//        {
//            text_Dialog.text = dialogArray[currentDialogIndex].Dialog.Substring(0, index);

//            index++;

//            yield return new WaitForSeconds(typingSpeed);
//        }

//        isTypingEffect = false;

//        text_Dialog.text = dialogArray[currentDialogIndex].Dialog;

//        // OKBtn이거나 보여주기용 캔버스라면 활성화
//        if (GetActiveBtn() == S_ActivateUIEnum.NextBtn ||
//            GetActiveBtn() == S_ActivateUIEnum.CreatureCanvas ||
//            GetActiveBtn() == S_ActivateUIEnum.StatCanvas ||
//            GetActiveBtn() == S_ActivateUIEnum.LootCanvas)
//        {
//            image_OKBtnBase.SetActive(true);
//        }
//    }
//    public S_ActivateUIEnum GetActiveBtn()
//    {
//        return dialogArray[currentDialogIndex].ActivateUI;
//    }
//}

