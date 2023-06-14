using UnityEngine;

namespace CodeBase.Utils
{
    public static class Tags
    {
        [field: SerializeField] public static string Weapon { get; private set; } = "Weapon";
        [field: SerializeField] public static string Enemy { get; private set; } = "Enemy";
    }
}
