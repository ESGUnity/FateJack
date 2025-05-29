using Unity.Netcode;
using UnityEngine;

public class Dealer : NetworkBehaviour
{
    Animator animator;

    // ∞£¿Ã ΩÃ±€≈œ
    public static Dealer LocalInstance {  get; private set; }
    void Awake()
    {
        LocalInstance = this;

        animator = GetComponent<Animator>();
    }

    [ClientRpc]
    public void DoAttackClientRpc(ulong targetId)
    {
        animator.ResetTrigger("Attack");
        animator.SetTrigger("Attack");
    }
}
