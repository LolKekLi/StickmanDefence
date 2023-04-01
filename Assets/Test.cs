using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class Test : SerializedMonoBehaviour
{

    public enum MyEnum
    {
        ikpjbnsid,
        dsfsdfsdf,
    }

    
    [Serializable]
    public class Testt
    {
        public MyEnum MyEnum;
        public int i = 0;
    }

    [BoxGroup("ReadOnly table")]
    private MyEnum[,] _testts = new MyEnum[3,3];
    
    [BoxGroup("ReadOnly table")]
    [TableMatrix(IsReadOnly = true)]
    public int[,] ReadOnlyTable = new int[5, 5];
}
