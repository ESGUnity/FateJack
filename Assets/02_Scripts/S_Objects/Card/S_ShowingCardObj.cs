using DG.Tweening;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class S_ShowingCardObj : S_CardObj
{
    [SerializeField] Material mat_DissolveEngraving;
    Vector3 showingPos = new Vector3(0f, -1f, -2f);
    Vector3 deckPos = new Vector3(0f, 0f, -9f);

    const float MOVE_TIME = 0.3f;
    const float WAIT_TIME = 0.35f;
    const float CHANGE_TIME = 1.1f;
    const float MIN_VALUE = 0.05f;
    const float MAX_VALUE = 1f;

    [HideInInspector] public Vector3 CARD_ROT;

    public async Task MoveShowingPos()
    {
        await transform.DOLocalMove(showingPos, MOVE_TIME).AsyncWaitForCompletion();
        await Task.Delay(Mathf.RoundToInt(WAIT_TIME * 1000));
    }
    public async Task MoveDeckPos()
    {
        await transform.DOMove(deckPos, MOVE_TIME).AsyncWaitForCompletion();
        Destroy(gameObject);
    }
    public async Task ChangeEngraving(List<S_EngravingEnum> prevEngraving)
    {
        Material newMat = Instantiate(mat_DissolveEngraving);
        sprite_Engraving.material = newMat;

        // 사라짐 (0 -> 1)
        if (prevEngraving.Count > 0)
        {
            sprite_Engraving.material.SetFloat("_DissolveStrength", MIN_VALUE);
            await sprite_Engraving.material.DOFloat(MAX_VALUE, "_DissolveStrength", CHANGE_TIME).AsyncWaitForCompletion();
        }
        else
        {
            sprite_Engraving.material.SetFloat("_DissolveStrength", MAX_VALUE);
        }

        // 카드 정보 변경
        SetCardInfo(CardInfo);

        sprite_Engraving.material = newMat;
        // 나타남 (1 -> 0)
        if (CardInfo.Engraving.Count > 0)
        {
            sprite_Engraving.material.SetFloat("_DissolveStrength", MAX_VALUE);
            await sprite_Engraving.material.DOFloat(MIN_VALUE, "_DissolveStrength", CHANGE_TIME).AsyncWaitForCompletion();
        }

        // 약간의 시간 텀 후 후속 작업
        await Task.Delay(300);
    }
}
