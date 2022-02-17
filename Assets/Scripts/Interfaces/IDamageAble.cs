using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageAble
{
    void TakeDamage(float _damage);

    void TakeDamage(float _damage, Vector3 _direction);
}
