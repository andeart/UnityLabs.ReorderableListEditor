using System;
using UnityEngine;



namespace UnityReorderableListEditor.V1.Context
{
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

        [SerializeField] private ScriptableObject[] _myAssetArray;

        // TODO: Handle serialization of generic type arrays.
        [SerializeField] private MyGenericData<int>[] _myGenericDataIntArray;
    }

}