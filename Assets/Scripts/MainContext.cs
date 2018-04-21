using System;
using UnityEngine;



namespace UnityReorderableListEditor.V1.Context
{

    [Flags]
    public enum JusticeLeague
    {
        Batman = 1 << 0,
        Superman = 1 << 1,
        WonderWoman = 1 << 2,
        Flash = 1 << 3,
        GreenLantern = 1 << 4,
        AquaMan = 1 << 5
    }


    [Serializable]
    public struct MyData
    {
        [SerializeField] [Range (0f, 1f)] private float _floatInRange;
        [SerializeField] private string[] _stringArray;
    }


    [Serializable]
    public class MyGenericData<T>
    {
        [SerializeField] private T _value;
    }


    public class MainContext : MonoBehaviour
    {
        [SerializeField] private MyData _myData;

        [SerializeField] private MyData[] _myDataArray;

        [SerializeField] private AudioClip[] _clipArray;
    }

}