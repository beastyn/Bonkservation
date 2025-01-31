using Player.Actions;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] BonkAction bonkAction;

    int isBonkingHash;

    void OnEnable()
    {
        this.bonkAction.ActionEvent += OnBonkActionEvent;     
        
    }
    void OnDisable()
    {
        this.bonkAction.ActionEvent -= OnBonkActionEvent;
    }

    void Start()
    {
        this.isBonkingHash = Animator.StringToHash("IsBonking");

    }

    void OnBonkActionEvent()
    {
        this.animator.SetTrigger(this.isBonkingHash);
    }
}
