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

    // �̱���
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
        // ī�� Ǯ�� �ʱ�ȭ (��Ȱ��ȭ�� ī��� ť ä���)
        for (int i = 0; i < poolSize; i++)
        {
            GameObject card = Instantiate(TossedCardPrefab);
            card.transform.position = tossedCardOriginPos;
            card.transform.eulerAngles = tossedCardOriginRot;
            card.SetActive(false);  // ó������ ��� ��Ȱ��ȭ
            cardPool.Enqueue(card);
        }
    }

    public GameObject GetTossedCard()
    {
        GameObject card = cardPool.Dequeue();
        card.SetActive(true); // ī�� Ȱ��ȭ
        return card;
    }

    public void ReturnCard(GameObject card)
    {
        card.transform.position = tossedCardOriginPos;
        card.transform.eulerAngles = tossedCardOriginRot;
        card.SetActive(false); // ��Ȱ��ȭ
        cardPool.Enqueue(card); // ī�� Ǯ�� ��ȯ
    }
}
