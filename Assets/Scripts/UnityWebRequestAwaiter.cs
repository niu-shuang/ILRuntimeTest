using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

public class UnityWebRequestAwaiter : INotifyCompletion
{
    private UnityWebRequestAsyncOperation asyncOp;

    public UnityWebRequestAwaiter(UnityWebRequestAsyncOperation asyncOp)
    {
        this.asyncOp = asyncOp;
    }

    public bool IsCompleted { get { return asyncOp.isDone; } }

    public void GetResult() { }

    public void OnCompleted(Action continuation)
    {
        asyncOp.completed += _ => { continuation(); };
    }

}

public static class ExtensionMethods
{
    public static UnityWebRequestAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
    {
        return new UnityWebRequestAwaiter(asyncOp);
    }
}