using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_DialogInfoSystem : MonoBehaviour
{
    [Header("컴포넌트")]
    GameObject image_BlackBackground;
    GameObject image_HitBtnBlockingBackground;
    [HideInInspector] public GameObject image_ViewDeckBlockingBackground;
    GameObject image_ViewUsedBlockingBackground;
    GameObject image_FieldCardBlockingBackground;

    GameObject image_DialogBase;
    TMP_Text text_Name;
    TMP_Text text_Dialog;
    GameObject image_NextBtn;
    TMP_Text text_Next;

    [Header("모든 버튼 씬 오브젝트")]
    [SerializeField] Image image_HitBtnBase; 
    [SerializeField] Image image_StandBtnBase;
    [SerializeField] GameObject obj_ViewDeck;
    [SerializeField] GameObject obj_StackCardsBase;
    [SerializeField] Canvas statInfoCanvas;
    [SerializeField] Canvas foeInfoCanvas;

    [Header("보조")]
    S_GameFlowStateEnum prevState;
    S_ActivateBtnEnum currentBtn;
    bool isCompleteDialog;
    const float DIALOG_APPEAR_TIME = 0.15f;
    Vector3 DIALOG_SCALE_AMOUNT = new Vector3(1.05f, 1.05f, 1.05f);

    public bool IsCompletedTotaly;

    // 싱글턴
    static S_DialogInfoSystem instance;
    public static S_DialogInfoSystem Instance { get { return instance; } }

    void Awake()
    {
        // 자식 오브젝트의 컴포넌트 가져오기
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);

        image_BlackBackground = Array.Find(transforms, c => c.gameObject.name.Equals("Image_BlackBackground")).gameObject;
        image_HitBtnBlockingBackground = Array.Find(transforms, c => c.gameObject.name.Equals("Image_HitBtnBlockingBackground")).gameObject;
        image_ViewDeckBlockingBackground = Array.Find(transforms, c => c.gameObject.name.Equals("Image_ViewDeckBlockingBackground")).gameObject;
        image_FieldCardBlockingBackground = Array.Find(transforms, c => c.gameObject.name.Equals("Image_FieldCardBlockingBackground")).gameObject;
        image_ViewUsedBlockingBackground = Array.Find(transforms, c => c.gameObject.name.Equals("Image_ViewUsedBlockingBackground")).gameObject;

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
    S_GameFlowStateEnum prevGameFlowState;
    void Update()
    {
        S_GameFlowStateEnum currentState = S_GameFlowManager.Instance.GameFlowState;
        if (prevGameFlowState != currentState)
        {
            if (currentState == S_GameFlowStateEnum.None)
            {
                EndMonolog();
            }

            prevGameFlowState = currentState;
        }
    }

    #region 독백 관련
    Sequence monoSeq;
    public void StartMonolog(string name, string text, float duration)
    {
        //// 대사 위치
        //S_HoverInfoSystem.Instance.SetPosByCharacterOrFoe(targetSpriteRenderer, image_DialogBase.GetComponent<RectTransform>(), GetComponent<RectTransform>());

        // 버튼 제거
        image_NextBtn.SetActive(false);

        text_Name.text = name;
        text = S_TextHelper.ParseText(text);
        text = S_TextHelper.WrapText(text, 30);
        text_Dialog.text = text;
        if (monoSeq != null && monoSeq.IsActive())
        {
            monoSeq.Kill();
        }

        image_DialogBase.SetActive(true);

        monoSeq = DOTween.Sequence();
        monoSeq.Append(image_DialogBase.GetComponent<Image>().DOFade(1f, DIALOG_APPEAR_TIME)).SetEase(Ease.OutQuart)
            .Join(text_Name.DOFade(1f, DIALOG_APPEAR_TIME)).SetEase(Ease.OutQuart)
            .Join(text_Dialog.DOFade(1f, DIALOG_APPEAR_TIME)).SetEase(Ease.OutQuart)
            .Join(image_DialogBase.transform.DOScale(DIALOG_SCALE_AMOUNT, DIALOG_APPEAR_TIME).SetEase(Ease.OutQuart))
            .Append(image_DialogBase.transform.DOScale(Vector3.one, DIALOG_APPEAR_TIME).SetEase(Ease.OutQuart));

        // 사운드
        S_AudioManager.Instance.PlaySFX(SFXEnum.Dialog);

        if (duration > 99)
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

        monoSeq.Append(image_DialogBase.GetComponent<Image>().DOFade(0f, 0)).SetEase(Ease.OutQuart)
        .Join(text_Name.DOFade(0f, 0)).SetEase(Ease.OutQuart)
        .Join(text_Dialog.DOFade(0f, 0)).SetEase(Ease.OutQuart)
        .OnComplete(() =>
        {
            image_DialogBase.SetActive(false);
        });
    }
    #endregion
    #region 일반 다이얼로그 관련
    Sequence diaSeq;
    public async Task StartDialog(Queue<DialogData> keys) // 인게임 화면에서 대사를 출력하는 메서드
    {
        isCompleteDialog = false;
        IsCompletedTotaly = false; 

        // 스테이트 담아두기
        prevState = S_GameFlowManager.Instance.GameFlowState;
        S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.Dialog;

        await S_GameFlowManager.WaitPanelAppearTimeAsync();

        //// 대사 위치
        //S_HoverInfoSystem.Instance.SetPosByCharacterOrFoe(targetSpriteRenderer, image_DialogBase.GetComponent<RectTransform>(), GetComponent<RectTransform>());

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
            // 사운드
            S_AudioManager.Instance.PlaySFX(SFXEnum.Dialog);

            if (diaSeq.IsActive())
            {
                diaSeq.Join(image_DialogBase.transform.DOScale(DIALOG_SCALE_AMOUNT, DIALOG_APPEAR_TIME).SetEase(Ease.OutQuart))
                    .Append(image_DialogBase.transform.DOScale(Vector3.one, DIALOG_APPEAR_TIME).SetEase(Ease.OutQuart));
            }
            else
            {
                diaSeq.Append(image_DialogBase.transform.DOScale(DIALOG_SCALE_AMOUNT, DIALOG_APPEAR_TIME).SetEase(Ease.OutQuart))
                    .Append(image_DialogBase.transform.DOScale(Vector3.one, DIALOG_APPEAR_TIME).SetEase(Ease.OutQuart));
            }
            DialogData data = keys.Dequeue();
            text_Name.text = data.Name;

            string text = data.Dialog;
            text = S_TextHelper.ParseText(text);
            text = S_TextHelper.WrapText(text, 30);
            text_Dialog.text = text;

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

        S_GameFlowManager.Instance.GameFlowState = prevState; // 스테이트 원래대로 되돌리기
        OffBlockingPanel(); // 패널도 풀기

        await diaSeq.AsyncWaitForCompletion();

        IsCompletedTotaly = true;
    }
    public void SetBtn(S_ActivateBtnEnum btn)
    {
        currentBtn = btn;

        // 각 UI의 기본값 세팅
        // 월드 오브젝트 막기 패널 켜기
        OnBlockingPanel();
        // 카드 내기 버튼
        image_HitBtnBase.GetComponent<Canvas>().sortingLayerName = "UI";
        image_HitBtnBase.GetComponent<Canvas>().sortingOrder = 0;
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
        // 필드 카드 오브젝트
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
        statInfoCanvas.sortingOrder = 1;
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
            case S_ActivateBtnEnum.Btn_Next_NoBlackBackground:
                image_NextBtn.SetActive(true);
                image_BlackBackground.GetComponent<Image>().DOKill();
                image_BlackBackground.GetComponent<Image>().DOFade(0, 0);
                break;
            case S_ActivateBtnEnum.Btn_Hit:
                image_NextBtn.SetActive(false);
                image_HitBtnBase.GetComponent<Canvas>().sortingLayerName = "Dialog";
                image_HitBtnBase.GetComponent<Canvas>().sortingOrder = 1;
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
            case S_ActivateBtnEnum.Obj_FieldCards:
                image_NextBtn.SetActive(true);
                image_FieldCardBlockingBackground.SetActive(false);
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
        image_BlackBackground.SetActive(true);
        image_HitBtnBlockingBackground.SetActive(true);
        image_ViewDeckBlockingBackground.SetActive(true);
        image_FieldCardBlockingBackground.SetActive(true);

        image_BlackBackground.GetComponent<Image>().DOKill();
        image_BlackBackground.GetComponent<Image>().DOFade(0.85f, DIALOG_APPEAR_TIME);
    }
    public void OffBlockingPanel()
    {
        image_BlackBackground.GetComponent<Image>().DOKill();
        image_BlackBackground.GetComponent<Image>().DOFade(0, DIALOG_APPEAR_TIME)
            .OnComplete(() =>
            {
                image_BlackBackground.SetActive(false);
                image_HitBtnBlockingBackground.SetActive(false);
                image_ViewDeckBlockingBackground.SetActive(false);
                image_FieldCardBlockingBackground.SetActive(false);
            });
    }
    public void ClickNextBtn()
    {
        if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.Dialog)
        {
            isCompleteDialog = true;
        }
    }
    #endregion
    public async Task StartQueueDialog(List<DialogData> dialogList)
    {
        Queue<DialogData> dialogs = new Queue<DialogData>();

        for (int i = 0; i < dialogList.Count; i++)
        {
            dialogs.Enqueue(dialogList[i]);
        }

        await StartDialog(dialogs);
    }
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
    Btn_Next_NoBlackBackground,
    Btn_Hit,
    Btn_Stand,
    Obj_ViewDeck,
    Obj_FieldCards,
    UI_Stat,
    UI_FoeStat,
}
