using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;

public class S_CameraManager : MonoBehaviour
{
    public Transform camTransform;

    public static Vector3 InGameCameraPos { get; set; }
    public static Vector3 InGameCameraRot { get; set; }
    public static Vector3 DeckCameraPos { get; set; }
    public static Vector3 DeckCameraRot { get; set; }

    // 흔들림 강도 및 속도
    float shakeAmount = 0.15f;
    float shakeSpeed = 0.3f;

    bool isShaking = true;
    Vector3 originalPos;
    Vector3 originalRot;
    float shakeTime = 0f;

    // 싱글턴
    static S_CameraManager instance;
    public static S_CameraManager Instance { get { return instance; } }

    void Awake()
    {
        InGameCameraPos = new Vector3(0, 22.7f, -13f);
        InGameCameraRot = new Vector3(60f, 0, 0);
        DeckCameraPos = new Vector3(0, 27f, -10.9f);
        DeckCameraRot = new Vector3(85f, 0, 0);

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
    void Start()
    {
        originalPos = camTransform.localPosition;
        originalRot = camTransform.eulerAngles;

        StartHandHeld();
    }
    void Update()
    {
        if (isShaking)
        {
            shakeTime += Time.deltaTime;

            // 사인 웨이브 기반의 흔들림 적용 (localPosition 기준)
            float shakeX = Mathf.Sin(shakeTime * shakeSpeed) * shakeAmount;
            float shakeY = Mathf.Cos(shakeTime * shakeSpeed * 1.2f + Mathf.PI / 2f) * shakeAmount;

            camTransform.localPosition = originalPos + new Vector3(shakeX, shakeY, 0);
        }
    }
    public void StartHandHeld()
    {
        isShaking = true;
    }
    public void StopHandHeld()
    {
        // 흔들림 중지하고 원위치 복귀
        isShaking = false;
    }
    public void MoveToPosition(Vector3 targetPos, Vector3 targetRot, float duration)  // 지정 위치로 이동 + 흔들림 잠시 중지 후 재개
    {
        StopHandHeld();

        originalPos = targetPos;
        originalRot = targetRot;

        // Tween으로 부드럽게 이동
        camTransform.DOKill();
        camTransform.DOMove(targetPos, duration).SetEase(Ease.OutQuart);
        camTransform.DORotate(targetRot, duration).SetEase(Ease.OutQuart)
            .OnComplete(() =>
            {
                originalPos = camTransform.localPosition;
                originalRot = camTransform.localEulerAngles;

                shakeTime = 0f;

                StartHandHeld();
            });
    }
    public void ShakeCamera(float power)
    {
        StopHandHeld();

        Sequence seq = DOTween.Sequence();

        seq.Append(camTransform.DOShakePosition(S_EffectActivator.Instance.GetEffectLifeTime() / 2, power))
            .Append(camTransform.DOMove(originalPos, S_EffectActivator.Instance.GetEffectLifeTime() / 4))
            .Join(camTransform.DORotate(originalRot, S_EffectActivator.Instance.GetEffectLifeTime() / 4))
                 .OnComplete(() =>
                 {
                     shakeTime = 0f;

                     StartHandHeld();
                 });
    }
}
