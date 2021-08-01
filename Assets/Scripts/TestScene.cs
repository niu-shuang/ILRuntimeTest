using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScene : MonoBehaviour
{
    private async void Start()
    {
        var suc = await ILRuntimeManager.Instance.Init("HotFixProj");
    }
    public void StartILRuntime()
    {
        //await ILRuntimeManager.Instance.Init("HotFixProj");
        ILRuntimeManager.Instance.InvokeStaticFunc("HotFixProj.Main", "TestVoid");
    }
}
