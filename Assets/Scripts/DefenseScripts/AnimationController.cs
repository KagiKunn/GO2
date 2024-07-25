using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public void OnAnimationEvent()
    {
        if (gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            EnemyMovement firstScript = GetComponent<EnemyMovement>();
            if (firstScript != null)
            {
                firstScript.isAttack();
            }
        }
        else if (gameObject.layer == LayerMask.NameToLayer("Ally"))
        {
            AllyScan secondScript = GetComponent<AllyScan>();
            if (secondScript != null)
            {
                secondScript.isAttack();
            }
        }
    }
}