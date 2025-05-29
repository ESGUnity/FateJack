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
            if (clip.name == attackStateName) // 상태 이름과 클립 이름이 동일하다고 가정
            {
                motionTime = clip.length;
                return motionTime;
            }
        }

        motionTime = 0f;
        return motionTime;
    }
    public int GetHitTimeByMs(float hitTimeRatio) // 0부터 1 사이.
    {
        Animator animator = GetComponent<Animator>();
        RuntimeAnimatorController controller = animator.runtimeAnimatorController;

        foreach (AnimationClip clip in controller.animationClips)
        {
            if (clip.name == attackStateName) // 상태 이름과 클립 이름이 동일하다고 가정
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