using DG.Tweening;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class WorldUIPlayerStack : MonoBehaviour
{
    Vector3 stackCardSpawnPoint = new Vector3(0, 0.01f, -15f);
    Vector3 startPoint = new Vector3(-2f, 1.9f, 0);
    Vector3 endPoint = new Vector3(2f, -1.9f, 0);
    const float CARD_ANIM_TIME = 0.7f;

    [SerializeField] GameObject stackCardPrefab;
    List<GameObject> stackCardGameObjectList = new();

    public void InitStackCard()
    {
        for (int i = stackCardGameObjectList.Count - 1; i >= 0; i--)
        {
            Destroy(stackCardGameObjectList[i]);
        }

        stackCardGameObjectList.Clear();
    }
    public async Task SetPlayerStack(Card card, bool isRemove = false)
    {
        if (!isRemove)
        {
            GameObject go = Instantiate(stackCardPrefab);
            go.transform.position = stackCardSpawnPoint;
            go.GetComponent<WorldUIStackCard>().SetCardInfo(card);
            stackCardGameObjectList.Add(go);
            go.transform.SetParent(transform, true);

            await AlignmentStackCard();
        }
    }
    async Task AlignmentStackCard()
    {
        List<PRS> originCardPRS = SetStackCardPos(stackCardGameObjectList.Count);
        List<Task> tweenTask = new List<Task>();

        for (int i = 0; i < stackCardGameObjectList.Count; i++)
        {
            stackCardGameObjectList[i].GetComponent<WorldUIStackCard>().OriginPRS = originCardPRS[i];
            stackCardGameObjectList[i].GetComponent<WorldUIStackCard>().OriginOrder = i * 10;
            stackCardGameObjectList[i].GetComponent<WorldUIStackCard>().SetOrder(i * 10);
            Task move = stackCardGameObjectList[i].GetComponent<Transform>().DOLocalMove(originCardPRS[i].Pos, CARD_ANIM_TIME).SetEase(Ease.OutQuart).AsyncWaitForCompletion();
            Task scale = stackCardGameObjectList[i].GetComponent<Transform>().DOScale(originCardPRS[i].Scale, CARD_ANIM_TIME).SetEase(Ease.OutQuart).AsyncWaitForCompletion();

            tweenTask.Add(Task.WhenAll(move, scale));
        }

        await Task.WhenAll(tweenTask);
    }
    List<PRS> SetStackCardPos(int cardCount)
    {
        float[] lerps = new float[cardCount];
        List<PRS> results = new List<PRS>(cardCount);

        if (cardCount < 7)
        {
            float interval = 1f / 5f;
            for (int i = 0; i < cardCount; i++)
            {
                lerps[i] = interval * i;
            }
        }
        else if (cardCount >= 7)
        {
            float interval = 1f / (cardCount - 1);
            for (int i = 0; i < cardCount; i++)
            {
                lerps[i] = interval * i;
            }
        }

        for (int i = 0; i < cardCount; i++)
        {
            Vector3 pos = Vector3.Lerp(startPoint, endPoint, lerps[i]);
            Vector3 rot = Vector3.zero;
            Vector3 scale = new Vector3(0.8f, 0.8f, 0.8f);
            results.Add(new PRS(pos, rot, scale));
        }

        return results;
    }
}
