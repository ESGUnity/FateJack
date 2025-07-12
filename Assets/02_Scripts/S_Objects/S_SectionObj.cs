using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class S_SectionObj : MonoBehaviour
{
    [Header("씬 오브젝트")]
    [SerializeField] public GameObject sprite_Character;
    [SerializeField] public GameObject sprite_MeetConditionEffect;
    [SerializeField] GameObject obj_Ground;

    [Header("연출")]
    Vector3 SECTION_SPAWN_POS = new Vector3(0, 0, 35f);
    Vector3 SECTION_ARRIVE_POS = new Vector3(0, 0, 0);
    Vector3 SECTION_EXIT_POS = new Vector3(0, 0, -35f);

    [HideInInspector] public Vector3 CHARACTER_ORIGIN_POS;
    protected Vector3 CHARACTER_EXIT_POS;

    public virtual void Awake()
    {
        CHARACTER_ORIGIN_POS = new Vector3(0f, 3.86f, -1.15f);
        CHARACTER_EXIT_POS = new Vector3(-15f, 3.86f, -1.15f);
    }

    public void SpawnSection(string sprite)
    {
        // 스프라이트 세팅
        var opHandle = Addressables.LoadAssetAsync<Sprite>($"Sprite_{sprite}");
        opHandle.Completed += OnCharacterSpriteLoadComplete;

        transform.position = SECTION_SPAWN_POS;

        transform.DOMove(SECTION_ARRIVE_POS, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart);
    }
    void OnCharacterSpriteLoadComplete(AsyncOperationHandle<Sprite> opHandle)
    {
        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            sprite_Character.GetComponent<SpriteRenderer>().sprite = opHandle.Result;
            sprite_MeetConditionEffect.GetComponent<SpriteRenderer>().sprite = opHandle.Result;
        }
    }
    public void ExitSection()
    {
        transform.DOMove(SECTION_EXIT_POS, S_GameFlowManager.PANEL_APPEAR_TIME).SetEase(Ease.OutQuart)
            .OnComplete(() => Destroy(gameObject));
    }
    public async Task ExitCharacter()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(sprite_Character.transform.DOLocalMove(CHARACTER_EXIT_POS, S_GameFlowManager.PANEL_APPEAR_TIME * 2).SetEase(Ease.OutQuart));

        await seq.AsyncWaitForCompletion();
    }
}
