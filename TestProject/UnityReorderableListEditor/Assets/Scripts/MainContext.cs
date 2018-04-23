using System;
using UnityEngine;



namespace UnityReorderableListEditor.V1.Context
{

    [Serializable]
    public struct MyData
    {
        [SerializeField]
        private string _name;
        [SerializeField] [Range (0f, 1f)]
        private float _floatInRange;
    }


    public class MainContext : MonoBehaviour
    {
        [SerializeField]
        private int[] _integers;

        [SerializeField]
        private MyData[] _dataObjects;

        [SerializeField]
        private ScriptableObject[] _scriptableObjects;
    }

}