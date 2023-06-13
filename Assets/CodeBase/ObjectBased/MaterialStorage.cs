using System;
using UnityEngine;
using static Codebase.Utils.Enums;
using Random = UnityEngine.Random;

namespace CodeBase.ObjectBased
{
    [CreateAssetMenu(fileName = "MaterialStorage", menuName = "ScriptableObjects/MaterialStorage")]
    public class MaterialStorage : ScriptableObject
    {
        [SerializeField] private ColorData[] colorDatas;

        public ColorData GetRandomColorData() => colorDatas[Random.Range(0, colorDatas.Length)];
    }

    [Serializable]
    public class ColorData
    {
        [field: SerializeField] public ColorType Type { get; private set; }
        [field: SerializeField] public Material Material { get; private set; }
    }
}
