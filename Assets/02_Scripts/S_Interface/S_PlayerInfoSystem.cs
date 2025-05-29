using DG.Tweening;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class S_PlayerInfoSystem : MonoBehaviour
{
    [Header("�� ������Ʈ")]
    [SerializeField] Animator animator_BurstVFX;
    [SerializeField] Animator animator_CleanHitVFX;
    [SerializeField] Animator animator_DelusionVFX;
    [SerializeField] Animator animator_FirstVFX;
    [SerializeField] Animator animator_ExpansionVFX;

    [Header("������Ʈ")]
    GameObject image_Player;

    [Header("������")]
    [SerializeField] GameObject prefab_PlayerVFX;

    [Header("UI")]
    Vector2 playerImageHidePos = new Vector2(0, -160);
    Vector2 playerImageOriginPos = new Vector2(0, 85);
    Vector2 playerVFXPos = new Vector2(0, -300);
    Vector2 harmVFXPos = new Vector2(0, 350);

    // �̱���
    static S_PlayerInfoSystem instance;
    public static S_PlayerInfoSystem Instance { get { return instance; } }

    void Awake()
    {
        // �ڽ� ������Ʈ�� ������Ʈ ��������
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);

        // ������Ʈ �Ҵ�
        image_Player = Array.Find(transforms, c => c.gameObject.name.Equals("Image_Player")).gameObject;

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
        image_Player.GetComponent<RectTransform>().anchoredPosition = playerImageHidePos;
        image_Player.SetActive(false);
    }
    public void AppearPlayerImage()
    {
        image_Player.SetActive(true);

        // ��Ʈ������ ���� �ִϸ��̼� �ֱ�
        image_Player.GetComponent<RectTransform>().DOKill(); // ��Ʈ�� �� Ʈ�� �ʱ�ȭ
        image_Player.GetComponent<RectTransform>().DOAnchorPos(playerImageOriginPos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
    }
    public void DisappearPlayerImage()
    {
        image_Player.GetComponent<RectTransform>().DOKill(); // ��Ʈ�� �� Ʈ�� �ʱ�ȭ
        image_Player.GetComponent<RectTransform>().DOAnchorPos(playerImageHidePos, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart)
            .OnComplete(() => image_Player.SetActive(false));
    }

    public async Task PlayerVFXAsync(S_PlayerVFXEnum vfx) // ���� ���� �� ����� VFX
    {
        GameObject go = Instantiate(prefab_PlayerVFX);
        await go.GetComponent<S_PlayerVFX>().VFXAsync(vfx, playerVFXPos);
    }
    public async Task HarmVFXAsync(S_PlayerVFXEnum vfx) // ���� VFX
    {
        GameObject go = Instantiate(prefab_PlayerVFX);
        await go.GetComponent<S_PlayerVFX>().VFXAsync(vfx, harmVFXPos);
    }
    public void ChangeSpecialAbilityVFX()
    {
        if (S_PlayerStat.Instance.IsBurst) animator_BurstVFX.SetTrigger("DoBurst");
        else animator_BurstVFX.SetTrigger("DoNone");

        if (S_PlayerStat.Instance.IsCleanHit) animator_CleanHitVFX.SetTrigger("DoCleanHit");
        else animator_CleanHitVFX.SetTrigger("DoNone");

        if (S_PlayerStat.Instance.IsDelusion) animator_DelusionVFX.SetTrigger("DoDelusion");
        else animator_DelusionVFX.SetTrigger("DoNone");

        if (S_PlayerStat.Instance.IsFirst != S_FirstEffectEnum.None) animator_FirstVFX.SetTrigger("DoFirst");
        else animator_FirstVFX.SetTrigger("DoNone");

        if (S_PlayerStat.Instance.IsExpansion) animator_ExpansionVFX.SetTrigger("DoExpansion");
        else animator_ExpansionVFX.SetTrigger("DoNone");
    }
}

public enum S_PlayerVFXEnum
{
    Add_StackSum,
    Subtract_StackSum,
    Add_Limit,
    Subtract_Limit,
    Add_Strength,
    Subtract_Strength,
    Add_Mind,
    Subtract_Mind,
    Add_Luck,
    Subtract_Luck,
    Harm_Strength,
    Harm_Mind,
    Harm_Luck,
    Harm_Strength_Mind,
    Harm_Strength_Luck,
    Harm_Mind_Luck,
    Harm_Carnage,
    Add_Health,
    Subtract_Health,
    Add_Determination,
    Subtract_Determination,
    Add_Gold,
    Subtract_Gold,
    Creation,
    Undertow,
    Guidance,
    Cursed,
    ResistanceCurse, // �鿪ī�� ����
    Exclusion,
}