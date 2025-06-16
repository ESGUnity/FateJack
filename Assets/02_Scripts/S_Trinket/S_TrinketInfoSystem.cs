using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_TrinketInfoSystem : MonoBehaviour
{
    [Header("프리팹")]
    [SerializeField] GameObject prefab_StackTrinketObj;

    [Header("씬 오브젝트")]
    GameObject pos_OwnedTrinketBase;
    TMP_Text text_TrinketCount;

    [Header("연출 관련")]
    Vector3 TRINKET_START_POS = new Vector3(-2.3f, 0, 0);
    Vector3 TRINKET_END_POS = new Vector3(2.3f, 0, 0);
    const float STACK_Z_VALUE = -0.02f;
    Vector3 STACK_TRINKET_ORIGIN_SCALE = new Vector3(0.65f, 0.65f, 0.65f);

    [Header("소유한 능력 리스트")]
    List<GameObject> ownedTrinketObjs = new();

    // 싱글턴
    static S_TrinketInfoSystem instance;
    public static S_TrinketInfoSystem Instance { get { return instance; } }

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
    #region 쓸만한 물건 관리
    public void AddTrinketObj(S_Trinket tri, Vector3 pos) // 쓸만한 물건 추가하기
    {
        GameObject go = Instantiate(prefab_StackTrinketObj, pos_OwnedTrinketBase.transform, true);
        go.transform.position = pos;
        go.GetComponent<S_StackTrinketObj>().SetTrinketInfo(tri);
        ownedTrinketObjs.Add(go);

        AlignmentTrinkets();

        UpdateTotalOwnedTrinketCount();
    }
    void AlignmentTrinkets() // 쓸만한 물건 정렬
    {
        List<PRS> originCardPRS = SetTrinketsPos(ownedTrinketObjs.Count);
        List<Task> tweenTask = new List<Task>();

        for (int i = 0; i < ownedTrinketObjs.Count; i++)
        {
            // PRS 설정
            ownedTrinketObjs[i].GetComponent<S_StackTrinketObj>().OriginPRS = originCardPRS[i];

            // 소팅오더 설정
            ownedTrinketObjs[i].GetComponent<S_StackTrinketObj>().OriginOrder = (i + 1) * 10;
            ownedTrinketObjs[i].GetComponent<S_StackTrinketObj>().SetOrder(ownedTrinketObjs[i].GetComponent<S_StackTrinketObj>().OriginOrder);

            // 카드의 위치 설정
            ownedTrinketObjs[i].transform.DOKill();
            ownedTrinketObjs[i].transform.DOLocalMove(ownedTrinketObjs[i].GetComponent<S_StackTrinketObj>().OriginPRS.Pos, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart);
            ownedTrinketObjs[i].transform.DOLocalRotate(ownedTrinketObjs[i].GetComponent<S_StackTrinketObj>().OriginPRS.Rot, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart);
            ownedTrinketObjs[i].transform.DOScale(ownedTrinketObjs[i].GetComponent<S_StackTrinketObj>().OriginPRS.Scale, S_EffectActivator.Instance.GetHitAndSortCardsTime()).SetEase(Ease.OutQuart);
        }

        UpdateTrinketObjState();
    }
    List<PRS> SetTrinketsPos(int cardCount) // 쓸만한 물건 위치 설정하는 메서드
    {
        List<PRS> results = new List<PRS>(cardCount);
        float interval = 1f / (S_PlayerTrinket.MAX_Trinket - 1f);

        for (int i = 0; i < cardCount; i++)
        {
            Vector3 pos = Vector3.Lerp(TRINKET_START_POS, TRINKET_END_POS, interval * i);
            pos = new Vector3(pos.x, pos.y, i * STACK_Z_VALUE);

            Vector3 rot = Vector3.zero;

            Vector3 scale = STACK_TRINKET_ORIGIN_SCALE;

            results.Add(new PRS(pos, rot, scale));
        }

        return results;
    }
    public void RemoveTrinketObj(S_Trinket tri) // 덱에서 카드 제거하기
    {
        GameObject removeObj = null;
        foreach (GameObject go in ownedTrinketObjs)
        {
            if (go.GetComponent<S_StackTrinketObj>().TrinketInfo == tri)
            {
                removeObj = go;
                break;
            }
        }

        ownedTrinketObjs.Remove(removeObj);
        removeObj.GetComponent<S_StackTrinketObj>().DeleteTrinketVFX();

        AlignmentTrinkets();

        UpdateTotalOwnedTrinketCount();
    }
    #endregion
    #region 효과
    public void SwapLeftTrinketObjIndex(S_Trinket tri)
    {
        int index = ownedTrinketObjs.IndexOf(GetTrinketObj(tri));

        if (index == 0)
        {
            GameObject firstTri = ownedTrinketObjs[0];

            // 요소들을 왼쪽으로 한 칸씩 이동
            for (int i = 0; i < ownedTrinketObjs.Count - 1; i++)
            {
                ownedTrinketObjs[i] = ownedTrinketObjs[i + 1];
            }

            // 마지막 자리에 처음 요소 넣기
            ownedTrinketObjs[ownedTrinketObjs.Count - 1] = firstTri;
        }
        else
        {
            // 일반적인 왼쪽 교체
            GameObject tempGo = ownedTrinketObjs[index - 1];
            ownedTrinketObjs[index - 1] = ownedTrinketObjs[index];
            ownedTrinketObjs[index] = tempGo;
        }

        // 위치 바꿨으니 정렬
        AlignmentTrinkets();
    }
    public void UpdateTrinketObjState() // 쓸만한 물건 오브젝트의 ActivatedCount와 MeetConditionEffect 효과 업데이트
    {
        foreach (GameObject go in ownedTrinketObjs)
        {
            go.GetComponent<S_StackTrinketObj>().UpdateTrinketObj();
        }
    }
    public void BouncingTrinketObjVFX(S_Trinket trinket)
    {
        foreach (GameObject go in ownedTrinketObjs)
        {
            if (go.GetComponent<S_StackTrinketObj>().TrinketInfo == trinket)
            {
                go.GetComponent<S_StackTrinketObj>().BouncingVFX();
            }
        }
    }
    #endregion
    #region 보조
    void UpdateTotalOwnedTrinketCount()
    {
        text_TrinketCount.text = $"{S_PlayerTrinket.Instance.OwnedTrinketList.Count} / {S_PlayerTrinket.MAX_Trinket}";
    }
    GameObject GetTrinketObj(S_Trinket tri)
    {
        foreach (GameObject go in ownedTrinketObjs)
        {
            if (go.GetComponent<S_StackTrinketObj>().TrinketInfo == tri)
            {
                return go;
            }
        }

        Debug.Log("소유하지 않은 쓸만한 물건을 왜 찾는가");
        return null;
    }
    #endregion
}
