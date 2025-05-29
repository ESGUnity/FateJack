using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TossedCardPool : NetworkBehaviour
{
    public GameObject TossedCardPrefab;  
    int poolSize = 10;
    Vector3 tossedCardOriginPos = new Vector3(0, 0.01f, 14f);
    Vector3 tossedCardOriginRot = new Vector3(90, 0, 0);

    Queue<GameObject> cardPool = new Queue<GameObject>();

    // 싱글턴
    static TossedCardPool instance;
    public static TossedCardPool Instance { get { return instance; } }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        // 카드 풀을 초기화 (비활성화된 카드로 큐 채우기)
        for (int i = 0; i < poolSize; i++)
        {
            GameObject card = Instantiate(TossedCardPrefab);
            card.transform.position = tossedCardOriginPos;
            card.transform.eulerAngles = tossedCardOriginRot;
            card.SetActive(false);  // 처음에는 모두 비활성화
            cardPool.Enqueue(card);
        }
    }

    public GameObject GetTossedCard()
    {
        GameObject card = cardPool.Dequeue();
        card.SetActive(true); // 카드 활성화
        return card;
    }

    public void ReturnCard(GameObject card)
    {
        card.transform.position = tossedCardOriginPos;
        card.transform.eulerAngles = tossedCardOriginRot;
        card.SetActive(false); // 비활성화
        cardPool.Enqueue(card); // 카드 풀에 반환
    }
}
