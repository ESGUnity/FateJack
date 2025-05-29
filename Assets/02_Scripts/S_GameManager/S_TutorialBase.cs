using UnityEngine;

public abstract class S_TutorialBase : MonoBehaviour
{
    public abstract void Enter();
    public abstract void Execute(S_TutorialManager controller);
    public abstract void Exit();
}
