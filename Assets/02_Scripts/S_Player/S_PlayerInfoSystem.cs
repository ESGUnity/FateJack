using DG.Tweening;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class S_PlayerInfoSystem : MonoBehaviour
{
    [Header("컴포넌트")]
    [SerializeField] GameObject pos_Player;
    [SerializeField] GameObject pos_Foe;

    [Header("프리팹")]
    [SerializeField] GameObject prefab_PlayerVFX;

    [Header("UI")]
    Vector3 playerSpriteOriginPos = new Vector3(0, 0, -5.7f);
    Vector2 playerSpriteHidePos = new Vector3(0, 0, -6f);
    //Vector3 playerVFXPos = new Vector3(0, -300);
    Vector3 harmVFXPos = new Vector3(0, 4f, 0); // 임시

    // 싱글턴
    static S_PlayerInfoSystem instance;
    public static S_PlayerInfoSystem Instance { get { return instance; } }

    void Awake()
    {
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

    public async Task PlayerVFXAsync(S_PlayerVFXEnum vfx, GameObject target = null) // 카드 위에 표시되는 각종 버프 및 디버프 VFX
    {
        GameObject go = Instantiate(prefab_PlayerVFX);
        if (target == null)
        {
            await go.GetComponent<S_PlayerVFX>().VFXAsync(vfx, pos_Player);
        }
        else
        {
            await go.GetComponent<S_PlayerVFX>().VFXAsync(vfx, target);
        }
    }
    public async Task HarmVFXAsync(S_PlayerVFXEnum vfx) // 공격 VFX
    {
        GameObject go = Instantiate(prefab_PlayerVFX);
        await go.GetComponent<S_PlayerVFX>().VFXAsync(vfx, pos_Foe);
    }
}

public enum S_PlayerVFXEnum
{
    Burst,
    Perfect,
    Delusion,
    First,
    Expansion,
    ColdBlood,
    Add_Weight,
    Subtract_Weight,
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
    ResistanceCurse, // 면역카드 전용
    Exclusion,
}