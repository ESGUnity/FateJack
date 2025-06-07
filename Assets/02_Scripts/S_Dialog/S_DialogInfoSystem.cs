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
    GameObject image_UIBlockingBackground;
    GameObject image_DialogBase;
    TMP_Text text_Name;
    TMP_Text text_Dialog;
    GameObject image_NextBtn;
    TMP_Text text_Next;

    [Header("씬 오브젝트")]
    [SerializeField] GameObject sprite_WorldObjectBlockingBackground;
    [SerializeField] GameObject sprite_Foe;
    [SerializeField] GameObject sprite_Character_Store;

    [Header("모든 버튼 씬 오브젝트")]
    [SerializeField] Canvas statInfoCanvas;
    [SerializeField] Canvas skillInfoCanvas; 
    [SerializeField] Image image_BasicHitBtnBase; 
    [SerializeField] Image image_TwistBtnBase;
    [SerializeField] Image image_StandBtnBase;

    [Header("보조")]
    S_GameFlowStateEnum prevState;
    S_ActivateUIEnum currentBtn;
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

        image_UIBlockingBackground = Array.Find(transforms, c => c.gameObject.name.Equals("Image_UIBlockingBackground")).gameObject;
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

        InitPos();
    }

    #region 연출
    public void InitPos()
    {
        sprite_WorldObjectBlockingBackground.SetActive(false);
        sprite_WorldObjectBlockingBackground.GetComponent<SpriteRenderer>().DOFade(0f, 0f);
        image_UIBlockingBackground.SetActive(false);
        image_UIBlockingBackground.GetComponent<Image>().DOFade(0f, 0f);

        image_DialogBase.SetActive(false);
        SetBtn(S_ActivateUIEnum.None);
    }
    public void AppearBlockingPanel()
    {
        sprite_WorldObjectBlockingBackground.SetActive(true);
        image_UIBlockingBackground.SetActive(true);

        image_UIBlockingBackground.GetComponent<Image>().DOKill();
        image_UIBlockingBackground.GetComponent<Image>().DOFade(0.97f, DIALOG_APPEAR_TIME);
    }
    public void DisappearBlockingPanel()
    {
        image_UIBlockingBackground.GetComponent<Image>().DOKill();
        image_UIBlockingBackground.GetComponent<Image>().DOFade(0f, DIALOG_APPEAR_TIME)
            .OnComplete(() =>
            {
                sprite_WorldObjectBlockingBackground.SetActive(false);
                image_UIBlockingBackground.SetActive(false);
            });
    }
    #endregion
    #region 독백 관련
    Sequence monoSeq;
    public void StartMonologByStore(string text, float duration = 10)
    {
        S_HoverInfoSystem.Instance.SetPosByWorldObjectCharacter(sprite_Character_Store.GetComponent<SpriteRenderer>(), image_DialogBase.GetComponent<RectTransform>(), GetComponent<RectTransform>());

        SetBtn(S_ActivateUIEnum.None);

        text_Dialog.text = text;
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
    public async Task StartDialogByInGame(Queue<DialogData> keys) // 인게임 화면에서 대사를 출력하는 메서드
    {
        isCompleteDialog = false;

        // 스테이트 담아두기
        prevState = S_GameFlowManager.Instance.GameFlowState;
        S_GameFlowManager.Instance.GameFlowState = S_GameFlowStateEnum.Dialog;

        // 카메라 이동
        Camera.main.transform.DOKill();
        Camera.main.transform.DOMove(S_GameFlowManager.InGameCameraPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
        Camera.main.transform.DORotate(S_GameFlowManager.InGameCameraRot, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
        await S_GameFlowManager.PanelAppearTimeAsync();

        // 대사 위치
        S_HoverInfoSystem.Instance.SetPosByWorldObjectCharacter(sprite_Foe.GetComponent<SpriteRenderer>(), image_DialogBase.GetComponent<RectTransform>(), GetComponent<RectTransform>());

        // 이미지랑 버튼 활성화. 블락 패널도 활성화
        image_DialogBase.SetActive(true);
        AppearBlockingPanel();

        // 기존에 진행되던 시퀀스 죽이기
        if (diaSeq != null && diaSeq.IsActive())
        {
            diaSeq.Kill();
        }

        // 새로운 시퀀스 진행
        diaSeq = DOTween.Sequence();

        diaSeq.Append(image_DialogBase.GetComponent<Image>().DOFade(1f, DIALOG_APPEAR_TIME)).SetEase(Ease.OutQuart)
            .Join(text_Name.DOFade(1f, DIALOG_APPEAR_TIME)).SetEase(Ease.OutQuart)
            .Join(text_Dialog.DOFade(1f, DIALOG_APPEAR_TIME)).SetEase(Ease.OutQuart);

        // 대사 설정
        while (keys.Count > 0)
        {
            DialogData data = keys.Dequeue();
            text_Name.text = data.Name;
            text_Dialog.text = data.Dialog;

            // 버튼 설정
            SetBtn(data.ActivateUI);

            while (!isCompleteDialog)
            {
                await Task.Yield();
            }

            isCompleteDialog = false;
        }

        // 대사 종료 시
        // 스테이트 원래대로 되돌리기
        S_GameFlowManager.Instance.GameFlowState = prevState;

        // 버튼 쪽 초기화
        SetBtn(S_ActivateUIEnum.None);

        // 패널도 풀기
        DisappearBlockingPanel();

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
    }
    #endregion
    #region 보조
    public void SetBtn(S_ActivateUIEnum btn)
    {
        currentBtn = btn;

        // 각 UI의 기본값 세팅
        statInfoCanvas.sortingLayerName = "UI";
        statInfoCanvas.sortingOrder = 0;

        skillInfoCanvas.sortingLayerName = "UI";
        skillInfoCanvas.sortingOrder = 0;

        image_BasicHitBtnBase.GetComponent<Canvas>().sortingLayerName = "UI";
        image_BasicHitBtnBase.GetComponent<Canvas>().sortingOrder = 0;

        image_TwistBtnBase.GetComponent<Canvas>().sortingLayerName = "UI";
        image_TwistBtnBase.GetComponent<Canvas>().sortingOrder = 0;

        image_StandBtnBase.GetComponent<Canvas>().sortingLayerName = "UI";
        image_StandBtnBase.GetComponent<Canvas>().sortingOrder = 0;

        // 활성화할 버튼 세팅
        switch (btn)
        {
            case S_ActivateUIEnum.None: // 모든 버튼 끄기
                image_NextBtn.SetActive(false);
                break;
            case S_ActivateUIEnum.NextBtn:
                image_NextBtn.SetActive(true);
                break;
            case S_ActivateUIEnum.StatInfoCanvas:
                image_NextBtn.SetActive(true);
                statInfoCanvas.sortingLayerName = "Dialog";
                statInfoCanvas.sortingOrder = 1;
                break;
            case S_ActivateUIEnum.skillInfoCanvas:
                image_NextBtn.SetActive(true);
                skillInfoCanvas.sortingLayerName = "Dialog";
                skillInfoCanvas.sortingOrder = 1;
                break;
            case S_ActivateUIEnum.HitBtn:
                image_NextBtn.SetActive(false);
                image_BasicHitBtnBase.GetComponent<Canvas>().sortingLayerName = "Dialog";
                image_BasicHitBtnBase.GetComponent<Canvas>().sortingOrder = 1;
                break;
            case S_ActivateUIEnum.TwistBtn:
                image_NextBtn.SetActive(false);
                image_TwistBtnBase.GetComponent<Canvas>().sortingLayerName = "Dialog";
                image_TwistBtnBase.GetComponent<Canvas>().sortingOrder = 1;
                break;
            case S_ActivateUIEnum.StandBtn:
                image_NextBtn.SetActive(false);
                image_StandBtnBase.GetComponent<Canvas>().sortingLayerName = "Dialog";
                image_StandBtnBase.GetComponent<Canvas>().sortingOrder = 1;
                break;
        }
    }
    public void BlockAllClick()
    {
        sprite_WorldObjectBlockingBackground.SetActive(true);
        image_UIBlockingBackground.SetActive(true);
        image_UIBlockingBackground.GetComponent<Image>().DOFade(0f, 0f);
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
}
public struct DialogData
{
    public string Name;
    public string Dialog;
    public S_ActivateUIEnum ActivateUI;

    public DialogData(string name, string dialog, S_ActivateUIEnum activateUI)
    {
        Name = name;
        Dialog = dialog;
        ActivateUI = activateUI;
    }
}
public enum S_ActivateUIEnum
{
    None,
    NextBtn,
    StatInfoCanvas,
    skillInfoCanvas,
    HitBtn,
    TwistBtn,
    StandBtn
}
