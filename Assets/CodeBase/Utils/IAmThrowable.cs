using CodeBase.ObjectBased;
using UnityEngine;

namespace CodeBase.Utils
{
    public interface IAmThrowable
    {
        public PhysicsObject PhysicsObject { get; }

        public void Throw(Vector3 force);
    }
}
