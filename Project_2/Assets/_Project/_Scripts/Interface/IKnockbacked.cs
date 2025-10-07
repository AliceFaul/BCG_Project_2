using UnityEngine;

public interface IKnockbacked
{
    void Knockback(Transform obj, float knockbackForce, float stunTime);
}
