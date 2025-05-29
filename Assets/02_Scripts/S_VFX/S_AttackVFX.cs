using UnityEngine;

public class S_AttackVFX : MonoBehaviour
{
    string attackStateName = "Attack";
    float motionTime;

    void Start()
    {
        GetMotionTime();

        DestroySelf();
    }
    public float GetMotionTime()
    {
        Animator animator = GetComponent<Animator>();
        RuntimeAnimatorController controller = animator.runtimeAnimatorController;

        foreach (AnimationClip clip in controller.animationClips)
        {
            if (clip.name == attackStateName) // ���� �̸��� Ŭ�� �̸��� �����ϴٰ� ����
            {
                motionTime = clip.length;
                return motionTime;
            }
        }

        motionTime = 0f;
        return motionTime;
    }
    public int GetHitTimeByMs(float hitTimeRatio) // 0���� 1 ����.
    {
        Animator animator = GetComponent<Animator>();
        RuntimeAnimatorController controller = animator.runtimeAnimatorController;

        foreach (AnimationClip clip in controller.animationClips)
        {
            if (clip.name == attackStateName) // ���� �̸��� Ŭ�� �̸��� �����ϴٰ� ����
            {
                motionTime = clip.length;
                return Mathf.RoundToInt(motionTime * hitTimeRatio * 1000);
            }
        }

        motionTime = 0f;
        return 0;
    }
    void DestroySelf()
    {
        Destroy(gameObject, motionTime);
    }
}