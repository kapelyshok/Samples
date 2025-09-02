using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtomicApps.Infrastructure.Bootstrap
{
    public interface IState : IExitableState
    {
        void Enter();
    }
}
