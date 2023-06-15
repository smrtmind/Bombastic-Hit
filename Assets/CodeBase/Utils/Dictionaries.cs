using CodeBase.ObjectBased;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Utils
{
    public static class Dictionaries
    {
        public static Dictionary<Transform, CannonBall> CannonBalls { get; private set; } = new Dictionary<Transform, CannonBall>();
    }
}
