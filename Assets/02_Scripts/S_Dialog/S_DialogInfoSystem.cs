using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class S_DialogInfoSystem : MonoBehaviour
{
    [Header("컴포넌트")]
    GameObject image_AllBlockingBackground;
    GameObject image_HitBtnBlockingBackground;
    GameObject image_ViewDeckBlockingBackground;
    GameObject image_StackCardBlockingBackground;
    GameObject image_TrinketBlockingBackground;
    GameObject image_DialogBase;
    TMP_Text text_Name;
    TMP_Text text_Dialog;

    GameObject image_NextBtn;
    TMP_Text text_Next;

    [Header("씬 오브젝트")]
    [SerializeField] GameObject sprite_Foe;
    [SerializeField] GameObject sprite_Character_Store;

    [Header("모든 버튼 씬 오브젝트")]
    [SerializeField] Image image_BasicHitBtnBase; 
    [SerializeField] Image image_TwistBtnBase;
    [SerializeField] Image image_StandBtnBase;
    [SerializeField] GameObject obj_ViewDeck;
    [SerializeField] GameObject obj_OwnedTrinketBase;
    [SerializeField] GameObject obj_StackCardsBase;
    [SerializeField] Canvas statInfoCanvas;
    [SerializeField] Canvas foeInfoCanvas;

    [Header("보조")]
    S_GameFlowStateEnum prevState;
    S_ActivateBtnEnum currentBtn;
    bool isCompleteDialog;
    const float DIALOG_APPEAR_TIME = 0.1f;

    // 싱글턴
    static S_DialogInfoSystem instance;
    public static S_DialogInfoSystem Instance { get { return instance; } }

    void Awake()
    {
        // 자식 오브젝트의 컴포넌트 가져오기
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);

        image_AllBlockingBackground = Array.Find(transforms, c => c.gameObject.name.Equals("Image_AllBlockingBackground")).gameObject;
        image_HitBtnBlockingBackground = Array.Find(transforms, c => c.gameObject.name.Equals("Image_HitBtnBlockingBackground")).gameObject;
        image_ViewDeckBlockingBackground = Array.Find(transforms, c => c.gameObject.name.Equals("Image_ViewDeckBlockingBackground")).gameObject;
        image_StackCardBlockingBackground = Array.Find(transforms, c => c.gameObject.name.Equals("Image_StackCardBlockingBackground")).gameObject;
        image_TrinketBlockingBackground = Array.Find(transforms, c => c.gameObject.name.Equals("Image_TrinketBlockingBackground")).gameObject;

        image_DialogBase = Array.Find(transforms, c => c.gameObject.name.Equals("Image_DialogBase")).gameObject;
        text_Name = Array.Find(texts, c => c.gameObject.name.Equals("Text_Name"));
        text_Dialog = Array.Find(texts, c => c.gameObject.name.Equals("Text_Dialog"));
        image_NextBtn = Array.Find(transforms, c => c.gameObject.name.Equals("Image_NextBtn")).gameObject;
        text_Next = Array.Find(texts, c => c.gameObject.name.Equals("Text_Next"));

        // 싱글턴
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region 독백 관련
    Sequence monoSeq;
    public void StartMonolog(SpriteRenderer targetSpriteRenderer, string name, string text, float duration = 10)
    {
        // 대사 위치
        S_HoverInfoSystem.Instance.SetPosByCharacterOrFoe(targetSpriteRenderer, image_DialogBase.GetComponent<RectTransform>(), GetComponent<RectTransform>());

        SetBtn(S_ActivateBtnEnum.None);

        text_Name.text = name;
        text_Dialog.text = WrapText(text);
        if (monoSeq != null && monoSeq.IsActive())
        {
            monoSeq.Kill();
        }

        image_DialogBase.SetActive(true);

        monoSeq = DOTween.Sequence();

        monoSeq.Append(image_DialogBase.GetComponent<Image>().DOFade(1f, DIALOG_APPEAR_TIME)).SetEase(Ease.OutQuart)
            .Join(text_Name.DOFade(1f, DIALOG_APPEAR_TIME)).SetEase(Ease.OutQuart)
            .Join(text_Dialog.DOFade(1f, DIALOG_APPEAR_TIME)).SetEase(Ease.OutQuart);

        if (duration > 999)
        {
            return;
        }
        else
        {
            monoSeq.AppendInterval(duration)
                .Append(image_DialogBase.GetComponent<Image>().DOFade(0f, DIALOG_APPEAR_TIME)).SetEase(Ease.OutQuart)
                .Join(text_Name.DOFade(0f, DIALOG_APPEAR_TIME)).SetEase(Ease.OutQuart)
                .Join(text_Dialog.DOFade(0f, DIALOG_APPEAR_TIME)).SetEase(Ease.OutQuart)
                .OnComplete(() =>
                {
                    image_DialogBase.SetActive(false);
                });
        }
    }
    public void EndMonolog()
    {
        if (!image_DialogBase.activeInHierarchy) return;

        if (monoSeq != null && monoSeq.IsActive())
        {
            monoSeq.Kill();

        }

        monoSeq = DOTween.Sequence();

        monoSeq.Append(image_DialogBase.GetComponent<Image>().DOFade(0f, DIALOG_APPEAR_TIME)).SetEase(Ease.OutQuart)
        .Join(text_Name.DOFade(0f, DIALOG_APPEAR_TIME)).SetEase(Ease.OutQuart)
        .Join(text_Dialog.DOFade(0f, DIALOG_APPEAR_TIME)).SetEase(Ease.OutQuart)
        .OnComplete(() =>
        {
            image_DialogBase.SetActive(false);
        });
    }
    #endregion
    #region 일반 다이얼로그 관련
    Sequence diaSeq;
    public async Task StartDialog(SpriteRenderer targetSpriteRenderer, Queue<DialogData> keys) // 인게임 화면에서 대사를 출력하는 메서드
    {
        isCompleteDialog = false;

        // 스테이트 담아두기
        prevState = S_GameFlowManager.Instance.GameFlowState;
        S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.Dialog;

        await S_GameFlowManager.WaitPanelAppearTimeAsync();

        // 대사 위치
        S_HoverInfoSystem.Instance.SetPosByCharacterOrFoe(targetSpriteRenderer, image_DialogBase.GetComponent<RectTransform>(), GetComponent<RectTransform>());

        // 이미지랑 버튼 활성화. 블락 패널도 활성화
        image_DialogBase.SetActive(true);
        OnBlockingPanel();

        // 다이얼로그 베이스 등장
        if (diaSeq != null && diaSeq.IsActive()) // 기존에 진행되던 시퀀스 죽이기
        {
            diaSeq.Kill();
        }
        diaSeq = DOTween.Sequence();
        diaSeq.Append(image_DialogBase.GetComponent<Image>().DOFade(1f, DIALOG_APPEAR_TIME)).SetEase(Ease.OutQuart)
            .Join(text_Name.DOFade(1f, DIALOG_APPEAR_TIME)).SetEase(Ease.OutQuart)
            .Join(text_Dialog.DOFade(1f, DIALOG_APPEAR_TIME)).SetEase(Ease.OutQuart);

        // 대사 설정
        while (keys.Count > 0)
        {
            DialogData data = keys.Dequeue();
            text_Name.text = data.Name;
            text_Dialog.text = WrapText(data.Dialog);

            // 버튼 설정
            SetBtn(data.ActivateUI);

            // 대사 완료까지 대기
            while (!isCompleteDialog)
            {
                await Task.Yield();
            }

            isCompleteDialog = false;
        }

        SetBtn(S_ActivateBtnEnum.None); // 버튼 쪽 초기화

        // 대사 종료 두트윈
        if (diaSeq != null && diaSeq.IsActive())
        {
            diaSeq.Kill();
        }

        diaSeq = DOTween.Sequence();
        diaSeq.Append(image_DialogBase.GetComponent<Image>().DOFade(0, DIALOG_APPEAR_TIME)).SetEase(Ease.OutQuart)
            .Join(text_Name.DOFade(0, DIALOG_APPEAR_TIME)).SetEase(Ease.OutQuart)
            .Join(text_Dialog.DOFade(0, DIALOG_APPEAR_TIME)).SetEase(Ease.OutQuart)
            .OnComplete(() =>
            {
                image_DialogBase.SetActive(false);
            });

        await diaSeq.AsyncWaitForCompletion();

        S_GameFlowManager.Instance.GameFlowState = prevState; // 스테이트 원래대로 되돌리기
        OffBlockingPanel(); // 패널도 풀기
    }
    public void SetBtn(S_ActivateBtnEnum btn)
    {
        currentBtn = btn;

        // 각 UI의 기본값 세팅
        // 월드 오브젝트 막기 패널 켜기
        OnBlockingPanel();
        // 카드 내기 버튼
        image_BasicHitBtnBase.GetComponent<Canvas>().sortingLayerName = "UI";
        image_BasicHitBtnBase.GetComponent<Canvas>().sortingOrder = 0;
        // 되돌리기 버튼
        image_TwistBtnBase.GetComponent<Canvas>().sortingLayerName = "UI";
        image_TwistBtnBase.GetComponent<Canvas>().sortingOrder = 0;
        // 스탠드 버튼
        image_StandBtnBase.GetComponent<Canvas>().sortingLayerName = "UI";
        image_StandBtnBase.GetComponent<Canvas>().sortingOrder = 0;
        // 덱 보기 오브젝트
        foreach (Transform t in obj_ViewDeck.GetComponentsInChildren<Transform>())
        {
            if (t.TryGetComponent<SpriteRenderer>(out var sprite))
            {
                sprite.sortingLayerName = "WorldObject";
            }
        }
        // 쓸만한 물건 오브젝트
        foreach (Transform t in obj_OwnedTrinketBase.GetComponentsInChildren<Transform>())
        {
            if (t.TryGetComponent<SpriteRenderer>(out var sprite))
            {
                sprite.sortingLayerName = "WorldObject";
            }
        }
        // 스택 카드 오브젝트
        foreach (Transform t in obj_StackCardsBase.GetComponentsInChildren<Transform>())
        {
            if (t.TryGetComponent<SpriteRenderer>(out var sprite))
            {
                sprite.sortingLayerName = "WorldObject";
            }
            if (t.TryGetComponent<MeshRenderer>(out var mesh))
            {
                mesh.sortingLayerName = "WorldObject";
            }
        }
        // 능력치 UI
        statInfoCanvas.sortingLayerName = "UI";
        statInfoCanvas.sortingOrder = 0;
        // 적 능력치(체력) UI
        foeInfoCanvas.sortingLayerName = "UI";
        foeInfoCanvas.sortingOrder = 0;

        // 활성화할 버튼 세팅
        switch (btn)
        {
            case S_ActivateBtnEnum.None: // 모든 버튼 끄기
                image_NextBtn.SetActive(false);
                break;
            case S_ActivateBtnEnum.Btn_Next:
                image_NextBtn.SetActive(true);
                break;
            case S_ActivateBtnEnum.Btn_Hit:
                image_NextBtn.SetActive(false);
                image_BasicHitBtnBase.GetComponent<Canvas>().sortingLayerName = "Dialog";
                image_BasicHitBtnBase.GetComponent<Canvas>().sortingOrder = 1;
                break;
            case S_ActivateBtnEnum.Btn_Twist:
                image_NextBtn.SetActive(false);
                image_TwistBtnBase.GetComponent<Canvas>().sortingLayerName = "Dialog";
                image_TwistBtnBase.GetComponent<Canvas>().sortingOrder = 1;
                break;
            case S_ActivateBtnEnum.Btn_Stand:
                image_NextBtn.SetActive(false);
                image_StandBtnBase.GetComponent<Canvas>().sortingLayerName = "Dialog";
                image_StandBtnBase.GetComponent<Canvas>().sortingOrder = 1;
                break;
            case S_ActivateBtnEnum.Obj_ViewDeck:
                image_NextBtn.SetActive(false);
                image_ViewDeckBlockingBackground.SetActive(false);
                foreach (Transform t in obj_ViewDeck.GetComponentsInChildren<Transform>())
                {
                    if (t.TryGetComponent<SpriteRenderer>(out var sprite))
                    {
                        sprite.sortingLayerName = "Dialog";
                    }
                }
                statInfoCanvas.sortingLayerName = "Dialog";
                statInfoCanvas.sortingOrder = 10;
                break;
            case S_ActivateBtnEnum.Obj_Trinket:
                image_NextBtn.SetActive(true);
                image_TrinketBlockingBackground.SetActive(false);
                foreach (Transform t in obj_OwnedTrinketBase.GetComponentsInChildren<Transform>())
                {
                    if (t.TryGetComponent<SpriteRenderer>(out var sprite))
                    {
                        sprite.sortingLayerName = "Dialog";
                    }
                }
                break;
            case S_ActivateBtnEnum.Obj_StackCards:
                image_NextBtn.SetActive(true);
                image_StackCardBlockingBackground.SetActive(false);
                foreach (Transform t in obj_StackCardsBase.GetComponentsInChildren<Transform>())
                {
                    if (t.TryGetComponent<SpriteRenderer>(out var sprite))
                    {
                        sprite.sortingLayerName = "Dialog";
                    }
                    if (t.TryGetComponent<MeshRenderer>(out var mesh))
                    {
                        mesh.sortingLayerName = "Dialog";
                    }
                }
                break;
            case S_ActivateBtnEnum.UI_Stat:
                image_NextBtn.SetActive(true);
                statInfoCanvas.sortingLayerName = "Dialog";
                statInfoCanvas.sortingOrder = 1;
                break;
            case S_ActivateBtnEnum.UI_FoeStat:
                image_NextBtn.SetActive(true);
                foeInfoCanvas.sortingLayerName = "Dialog";
                foeInfoCanvas.sortingOrder = 1;
                break;
        }
    }
    public void OnBlockingPanel() // 소팅레이어가 Dialog이면서 소팅오더가 1인 것만 클릭 가능하도록 나머지 월드오브젝트와 UI를 막는 패널
    {
        image_AllBlockingBackground.SetActive(true);
        image_HitBtnBlockingBackground.SetActive(true);
        image_ViewDeckBlockingBackground.SetActive(true);
        image_StackCardBlockingBackground.SetActive(true);

        image_AllBlockingBackground.GetComponent<Image>().DOKill();
        image_AllBlockingBackground.GetComponent<Image>().DOFade(0.85f, DIALOG_APPEAR_TIME);
    }
    public void OffBlockingPanel()
    {
        image_AllBlockingBackground.GetComponent<Image>().DOKill();
        image_AllBlockingBackground.GetComponent<Image>().DOFade(0f, DIALOG_APPEAR_TIME)
            .OnComplete(() =>
            {
                image_AllBlockingBackground.SetActive(false);
                image_HitBtnBlockingBackground.SetActive(false);
                image_ViewDeckBlockingBackground.SetActive(false);
                image_StackCardBlockingBackground.SetActive(false);
            });
    }
    public void ClickNextBtn()
    {
        if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Dialog)
        {
            isCompleteDialog = true;
            Debug.Log("들어오나");
        }
    }
    #endregion
    #region 보조
    string WrapText(string input, int maxLen = 20) // 20단어 기준으로 줄바꿈하는 메서드
    {
        var words = input.Split(' ');
        var result = "";
        var line = "";

        foreach (var word in words)
        {
            // 단어가 너무 길면 먼저 줄바꿈
            if (word.Length > maxLen)
            {
                if (line != "") { result += line + "\n"; line = ""; }
                result += word + "\n";
                continue;
            }

            // 현재 줄에 추가해도 되면 추가
            if ((line + " " + word).Trim().Length <= maxLen)
            {
                line = (line + " " + word).Trim();
            }
            else
            {
                // 안 되면 줄바꾸고 시작
                result += line + "\n";
                line = word;
            }
        }

        if (line != "") result += line;

        return result;
    }
    #endregion
}
public struct DialogData
{
    public string Name;
    public string Dialog;
    public S_ActivateBtnEnum ActivateUI;

    public DialogData(string name, string dialog, S_ActivateBtnEnum activateUI)
    {
        Name = name;
        Dialog = dialog;
        ActivateUI = activateUI;
    }
}
public enum S_ActivateBtnEnum
{
    None,
    Btn_Next,
    Btn_Hit,
    Btn_Twist,
    Btn_Stand,
    Obj_ViewDeck,
    Obj_Trinket,
    Obj_StackCards,
    UI_Stat,
    UI_FoeStat,
}
