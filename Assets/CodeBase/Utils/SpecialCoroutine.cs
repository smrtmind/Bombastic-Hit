using System.Collections;
using UnityEngine;

namespace CodeBase.Utils
{
    public static class SpecialCoroutine
    {
        public static IEnumerator LookAt(Transform who, Transform where)
        {
            while (true)
            {
                who.LookAt(where.position);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
