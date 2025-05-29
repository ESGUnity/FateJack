using DG.Tweening;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class S_ResultInfoSystem : MonoBehaviour
{
    // ������
    [SerializeField] GameObject resultItem;

    // ������Ʈ
    GameObject panel_ResultInfoBase;
    GameObject layoutGroup_ResultListBase;
    TMP_Text text_FinalGoldReward;
    GameObject panel_ResultOKBtnBase;

    // �г� ���� ����
    Vector2 resultHidePos = new Vector2(0, 650);
    Vector2 resultOriginPos = new Vector2(0, -210);
    Vector2 okBtnHidePos = new Vector2(0, -100);
    Vector2 okBtnOriginPos = new Vector2(0, 55);

    // ����
    const float RESULT_VFX_TIME = 0.3f;

    // �̱���
    static S_ResultInfoSystem instance;
    public static S_ResultInfoSystem Instance { get { return instance; } }

    void Awake()
    {
        // �ڽ� ������Ʈ�� ������Ʈ ��������
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);

        // ������Ʈ �Ҵ�
        panel_ResultInfoBase = Array.Find(transforms, c => c.gameObject.name.Equals("Panel_ResultInfoBase")).gameObject;
        layoutGroup_ResultListBase = Array.Find(transforms, c => c.gameObject.name.Equals("LayoutGroup_ResultListBase")).gameObject;
        text_FinalGoldReward = Array.Find(texts, c => c.gameObject.name.Equals("Text_FinalGoldReward"));
        panel_ResultOKBtnBase = Array.Find(transforms, c => c.gameObject.name.Equals("Panel_OKBtnBase")).gameObject;

        // �̱���
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

    public void InitPos()
    {
        panel_ResultInfoBase.GetComponent<RectTransform>().anchoredPosition = resultHidePos;
        panel_ResultOKBtnBase.GetComponent<RectTransform>().anchoredPosition = okBtnHidePos;
    }
    public void AppearResult() // �г� ����
    {
        // �г� ��ġ �ʱ�ȭ
        panel_ResultInfoBase.GetComponent<RectTransform>().anchoredPosition = resultHidePos;
        panel_ResultInfoBase.SetActive(true);

        // �г� ���� ��Ʈ��
        panel_ResultInfoBase.GetComponent<RectTransform>().DOKill(); // ��Ʈ�� �� Ʈ�� �ʱ�ȭ
        panel_ResultInfoBase.GetComponent<RectTransform>().DOAnchorPos(resultOriginPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
    }
    public void DisappearResult() // �г� ����
    {
        // �г� ���� ��Ʈ��
        panel_ResultInfoBase.GetComponent<RectTransform>().DOKill(); // ��Ʈ�� �� Ʈ�� �ʱ�ȭ
        panel_ResultInfoBase.GetComponent<RectTransform>().DOAnchorPos(resultHidePos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart)
            .OnComplete(() => panel_ResultInfoBase.SetActive(false));
    }
    public void AppearResultOKBtn() // �г� ����
    {
        // �г� ��ġ �ʱ�ȭ
        panel_ResultOKBtnBase.GetComponent<RectTransform>().anchoredPosition = okBtnHidePos;
        panel_ResultOKBtnBase.SetActive(true);

        // �г� ���� ��Ʈ��
        panel_ResultOKBtnBase.GetComponent<RectTransform>().DOKill(); // ��Ʈ�� �� Ʈ�� �ʱ�ȭ
        panel_ResultOKBtnBase.GetComponent<RectTransform>().DOAnchorPos(okBtnOriginPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
    }
    public void DisappearResultOKBtn() // �г� ����
    {
        // �г� ���� ��Ʈ��
        panel_ResultOKBtnBase.GetComponent<RectTransform>().DOKill(); // ��Ʈ�� �� Ʈ�� �ʱ�ȭ
        panel_ResultOKBtnBase.GetComponent<RectTransform>().DOAnchorPos(okBtnHidePos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart)
            .OnComplete(() => panel_ResultOKBtnBase.SetActive(false));
    }
    public void InitResultPanel()
    {
        foreach (Transform go in layoutGroup_ResultListBase.transform)
        {
            Destroy(go.gameObject);
        }

        text_FinalGoldReward.text = "0";
    }
    public async Task CalcResultGoldAsync(int health, int determination, int omen, int robbery, int greed)
    {
        int gold = 0;
        GameObject go1 = Instantiate(resultItem, layoutGroup_ResultListBase.transform);
        go1.GetComponent<S_ResultBase>().SetResultBase("������ óġ ���", "5");
        go1.transform.localScale = Vector3.zero;
        gold += 5;
        ChangeGoldVFXTween(int.Parse(text_FinalGoldReward.text), gold, text_FinalGoldReward);
        await go1.transform.DOScale(Vector3.one, RESULT_VFX_TIME).SetEase(Ease.OutBounce).AsyncWaitForCompletion();

        if (S_GameFlowManager.Instance.CurrentTrial % 3 == 0)
        {
            GameObject go2 = Instantiate(resultItem, layoutGroup_ResultListBase.transform);
            go2.GetComponent<S_ResultBase>().SetResultBase("������ ������ óġ �߰� ���", "5");
            go2.transform.localScale = Vector3.zero;
            gold += 5;
            ChangeGoldVFXTween(int.Parse(text_FinalGoldReward.text), gold, text_FinalGoldReward);
            await go2.transform.DOScale(Vector3.one, RESULT_VFX_TIME).SetEase(Ease.OutBounce).AsyncWaitForCompletion();
        }

        GameObject go3 = Instantiate(resultItem, layoutGroup_ResultListBase.transform);
        go3.GetComponent<S_ResultBase>().SetResultBase("���� ü�� 1�� ���", health.ToString());
        go3.transform.localScale = Vector3.zero;
        gold += health;
        ChangeGoldVFXTween(int.Parse(text_FinalGoldReward.text), gold, text_FinalGoldReward);
        await go3.transform.DOScale(Vector3.one, RESULT_VFX_TIME).SetEase(Ease.OutBounce).AsyncWaitForCompletion();

        if (determination > 0)
        {
            GameObject go4 = Instantiate(resultItem, layoutGroup_ResultListBase.transform);
            go4.GetComponent<S_ResultBase>().SetResultBase("���� ���� 1�� ���", determination.ToString());
            go4.transform.localScale = Vector3.zero;
            gold += determination;
            ChangeGoldVFXTween(int.Parse(text_FinalGoldReward.text), gold, text_FinalGoldReward);
            await go4.transform.DOScale(Vector3.one, RESULT_VFX_TIME).SetEase(Ease.OutBounce).AsyncWaitForCompletion();
        }

        if (omen > 0)
        {
            GameObject go5 = Instantiate(resultItem, layoutGroup_ResultListBase.transform);
            go5.GetComponent<S_ResultBase>().SetResultBase("���� ī�忡 ���� ���", determination.ToString());
            go5.transform.localScale = Vector3.zero;
            gold += omen * 2;
            ChangeGoldVFXTween(int.Parse(text_FinalGoldReward.text), gold, text_FinalGoldReward);
            await go5.transform.DOScale(Vector3.one, RESULT_VFX_TIME).SetEase(Ease.OutBounce).AsyncWaitForCompletion();
        }

        if (robbery > 0)
        {
            GameObject go6 = Instantiate(resultItem, layoutGroup_ResultListBase.transform);
            go6.GetComponent<S_ResultBase>().SetResultBase("���� ī�忡 ���� ���", determination.ToString());
            go6.transform.localScale = Vector3.zero;
            gold += robbery * 2;
            ChangeGoldVFXTween(int.Parse(text_FinalGoldReward.text), gold, text_FinalGoldReward);
            await go6.transform.DOScale(Vector3.one, RESULT_VFX_TIME).SetEase(Ease.OutBounce).AsyncWaitForCompletion();
        }

        if (greed > 0)
        {
            GameObject go7 = Instantiate(resultItem, layoutGroup_ResultListBase.transform);
            go7.GetComponent<S_ResultBase>().SetResultBase("Ž�� ī�忡 ���� ���", determination.ToString());
            go7.transform.localScale = Vector3.zero;
            gold += greed * 2;
            ChangeGoldVFXTween(int.Parse(text_FinalGoldReward.text), gold, text_FinalGoldReward);
            await go7.transform.DOScale(Vector3.one, RESULT_VFX_TIME).SetEase(Ease.OutBounce).AsyncWaitForCompletion();
        }
    }
    void ChangeGoldVFXTween(int oldValue, int newValue, TMP_Text statText)
    {
        int currentNumber = oldValue;
        DOTween.To
            (
                () => currentNumber,
                x => { currentNumber = x; statText.text = currentNumber.ToString(); },
                newValue,
                RESULT_VFX_TIME
            ).SetEase(Ease.OutQuart);
    }
    public void ClickOKBtn()
    {
        if (S_GameFlowManager.Instance.GameFlowState == S_GameFlowStateEnum.None) return;

        InitResultPanel();
        S_GameFlowManager.Instance.StartStore();
    }
}
