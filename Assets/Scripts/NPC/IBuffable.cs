using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuffable
{
    void Buff(float damageMultiplierBuff, float speedMultiplierBuff, float armorBuff);

    void Debuff(float damageMultiplierDebuff, float speedMultiplierDebuff, float armorDebuff);

    void Damage(int damage);
}
