using UnityEngine;
using J;
using UnityEngine.Networking;
using System.IO;
using LitJson;
using System;
using Cysharp.Threading.Tasks;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.CLR.Method;
using System.Linq;
using System.Collections;

public class ILRuntimeManager : SingletonMonoBehaviour<ILRuntimeManager>
{
    private ILRuntime.Runtime.Enviorment.AppDomain appdomain;
    private MemoryStream dllMS;
    private MemoryStream pdbMS;

    public async UniTask<bool> Init(string patchName, DividableProgress progress = null)
    {
        appdomain = new ILRuntime.Runtime.Enviorment.AppDomain(  );
        if (dllMS != null)
        {
            dllMS.Dispose();
        }
        if (pdbMS != null)
        {
            pdbMS.Dispose();
        }

#if UNITY_ANDROID && !UNITY_EDITOR
            string dllFilePath = Application.streamingAssetsPath + $"/{patchName}.dll";
#else
        string dllFilePath = "file:///" + Application.streamingAssetsPath + $"/{patchName}.dll";
#endif
        byte[] dll;

        UnityWebRequest dllReq = UnityWebRequest.Get(dllFilePath);
        await dllReq.SendAsObservable(progress?.Divide(.4f)).ToUniTask();
        if (dllReq.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(dllReq.error);
            return false;
        }
        dll = dllReq.downloadHandler.data;
        dllReq.Dispose();



#if UNITY_ANDROID && !UNITY_EDITOR
            string pdbFilePath = Application.streamingAssetsPath + $"/{patchName}.pdb";
#else
        string pdbFilePath = "file:///" + Application.streamingAssetsPath + $"/{patchName}.pdb";
#endif
        var req = UnityWebRequest.Get(pdbFilePath);
        await req.SendAsObservable(progress?.Divide(.4f)).ToUniTask();
        if (req.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(req.error);
            return false;
        }
        byte[] pdb = req.downloadHandler.data;

        dllMS = new MemoryStream(dll);
        pdbMS = new MemoryStream(pdb);

        appdomain.LoadAssembly(dllMS, pdbMS, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());

        OnRuntimeInited();
        progress?.Report(1f);
        return true;
    }

    private void OnRuntimeInited()
    {
#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
        //由于Unity的Profiler接口只允喧疒主线程使用，为了避免出襾E＃枰嫠逫LRuntime主线程的线程ID才能正确将函数运行耗时报告给Profiler
        appdomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
        appdomain.DebugService.StartDebugService(56000);
#endif
        appdomain.DelegateManager.RegisterMethodDelegate<UniRx.Unit>();
        appdomain.DelegateManager.RegisterFunctionDelegate<Boolean>();
        appdomain.DelegateManager.RegisterMethodDelegate<Int32>();
        appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<float>>((action) =>
        {
            return new UnityEngine.Events.UnityAction<float>((a) =>
            {
                ((System.Action<float>)action)(a);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction>((act) =>
        {
            return new UnityEngine.Events.UnityAction(() =>
            {
                ((System.Action)act)();
            });
        });

        appdomain.DelegateManager.RegisterFunctionDelegate<IProgress<Single>, IEnumerator>();


        appdomain.RegisterCrossBindingAdaptor(new IAsyncStateMachineClassInheritanceAdaptor());
        appdomain.RegisterCrossBindingAdaptor(new CoroutineAdapter());
        JsonMapper.RegisterILRuntimeCLRRedirection(appdomain);
        ILRuntime.Runtime.Generated.CLRBindings.Initialize(appdomain);
    }

    public static void DoCoroutine(IEnumerator coroutine)
    {
        Instance.StartCoroutine(coroutine);
    }

    public void InitHotFixProj()
    {
        InvokeStaticFunc("HotFixProj.Main", "InitializeHotfixProj");
    }

    public IType GetClassType(string type)
    {
        return appdomain.LoadedTypes.GetOrDefault(type);
    }
    public ILTypeInstance ILInstantiate(IType type)
    {
        return ((ILType)type).Instantiate();
    }

    public void InvokeInstanceMethod(IMethod method, ILTypeInstance instance, params object[] args)
    {
        using (var ctx = appdomain.BeginInvoke(method))
        {
            ctx.PushObject(instance);
            if (args.Length > 0)
            {
                foreach (var arg in args)
                {
                    ctx.PushObject(arg);
                }
            }
            ctx.Invoke();
        }
    }

    public void InvokeStaticFunc(string type, string method, params object[] args)
    {
        appdomain.Invoke(type, method, null, args);
    }


    protected override void SingletonOnDestroy()
    {
        //InvokeStaticFunc("HotFixProj.Main", "Dispose");
        dllMS?.Dispose();
        pdbMS?.Dispose();
    }
}