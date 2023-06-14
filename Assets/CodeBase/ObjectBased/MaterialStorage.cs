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

        public ColorData GetColorData(ColorType type)
        {
            if (type == ColorType.Random)
                return colorDatas[Random.Range(0, colorDatas.Length)];

            foreach (var colorData in colorDatas)
            {
                if (colorData.Type == type)
                    return colorData;
            }

            return null;
        }
    }

    [Serializable]
    public class ColorData
    {
        [field: SerializeField] public ColorType Type { get; private set; }
        [field: SerializeField] public Material Material { get; private set; }
    }
}
