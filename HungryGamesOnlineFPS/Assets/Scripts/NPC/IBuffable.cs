using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuffable
{
    void Buff(float damageMultiplier, float speedMultiplier);

    void Debuff(float damageDebuff, float speedDebuff);

    void Damage(float damage);
}
