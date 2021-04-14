using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IDisposableInterfaceAdapter : CrossBindingAdaptor
{
    public override Type BaseCLRType => typeof(IDisposable);

    public override Type AdaptorType => typeof(IDisposableAdapter);

    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    {
        return new IDisposableAdapter(appdomain, instance);
    }

    public class IDisposableAdapter : IDisposable, CrossBindingAdaptorType
    {
        private ILTypeInstance instance;
        private ILRuntime.Runtime.Enviorment.AppDomain appDomain;
        private IMethod mDispose;
        public IDisposableAdapter()
        {
        }

        public IDisposableAdapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            this.instance = instance;
            this.appDomain = appdomain;
        }
        public ILTypeInstance ILInstance => instance;

        public void Dispose()
        {
            if(mDispose == null)
            {
                mDispose = instance.Type.GetMethod("Dispose", 0);
            }
            appDomain.Invoke(mDispose, instance);
        }
    }
}
