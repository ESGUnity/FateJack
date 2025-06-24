using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;

public class S_ShowingCardObj : S_CardObj
{
    [SerializeField] Material mat_Origin;
    [SerializeField] Material mat_Dissolve;
    Vector3 showingPos = new Vector3(0f, 0f, -2f);
    Vector3 deckPos = new Vector3(0f, 0f, -9f);

    const float MOVE_TIME = 0.3f;
    const float CHANGE_TIME = 0.8f;
    const float MIN_VALUE = 0.05f;
    const float MAX_VALUE = 1f;

    public async Task MoveShowingPos()
    {
        await transform.DOLocalMove(showingPos, MOVE_TIME).AsyncWaitForCompletion();
    }
    public async Task MoveDeckPos()
    {
        await transform.DOMove(deckPos, MOVE_TIME).AsyncWaitForCompletion();
    }
    public async Task ChangeCardEffect()
    {
        Material newMat = Instantiate(mat_Dissolve);
        sprite_CardEffect.material = newMat;

        newMat.SetFloat("_DissolveStrength", MIN_VALUE);

        // 사라짐 (0 -> 1)
        await newMat.DOFloat(MAX_VALUE, "_DissolveStrength", CHANGE_TIME).AsyncWaitForCompletion();

        SetCardInfo(CardInfo);

        // 나타남 (1 -> 0)
        await newMat.DOFloat(MIN_VALUE, "_DissolveStrength", CHANGE_TIME).AsyncWaitForCompletion();

        // 원래 매터리얼로 되돌림
        sprite_CardEffect.material = mat_Origin;

        // 약간의 시간 텀 후 후속 작업
        await Task.Delay(300);
    }
    public async Task ChangeEngraving()
    {
        Material newMat = Instantiate(mat_Dissolve);
        sprite_Engraving.material = newMat;

        newMat.SetFloat("_DissolveStrength", MIN_VALUE);

        // 사라짐 (0 -> 1)
        await newMat.DOFloat(MAX_VALUE, "_DissolveStrength", CHANGE_TIME).AsyncWaitForCompletion();

        SetCardInfo(CardInfo);

        if (CardInfo.Engraving != S_EngravingEnum.None)
        {
            // 나타남 (1 -> 0)
            await newMat.DOFloat(MIN_VALUE, "_DissolveStrength", CHANGE_TIME).AsyncWaitForCompletion();
        }

        // 원래 매터리얼로 되돌림
        sprite_Engraving.material = mat_Origin;

        // 약간의 시간 텀 후 후속 작업
        await Task.Delay(300);
    }
}
