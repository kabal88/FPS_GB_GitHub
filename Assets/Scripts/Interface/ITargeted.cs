using System;
using UnityEngine;

namespace Geekbrains
{
    public interface ITargeted
    {
        Affiliation GetAffiliation();
        Transform GetTransform();
        event Action<Transform> OnDeath;
    }
}