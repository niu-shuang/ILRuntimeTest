using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScene : MonoBehaviour
{
    private void Start()
    {
        ILRuntimeManager.Instance.Init("HotFixProj").Forget();
    }
    public async void StartILRuntime()
    {
        //await ILRuntimeManager.Instance.Init("HotFixProj");
        ILRuntimeManager.Instance.InvokeStaticFunc("HotFixProj.Main", "TestUniTask");
    }
}
