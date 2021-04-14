using System;
using System.Collections.Generic;
using System.Reflection;

namespace ILRuntime.Runtime.Generated
{
    class CLRBindings
    {

//will auto register in unity
#if UNITY_5_3_OR_NEWER
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
        static private void RegisterBindingAction()
        {
            ILRuntime.Runtime.CLRBinding.CLRBindingUtils.RegisterBindingAction(Initialize);
        }


        /// <summary>
        /// Initialize the CLR binding, please invoke this AFTER CLR Redirection registration
        /// </summary>
        public static void Initialize(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            System_Runtime_CompilerServices_AsyncVoidMethodBuilder_Binding.Register(app);
            //Cysharp_Threading_Tasks_CompilerServices_AsyncUniTaskMethodBuilder_1_Int32_Binding.Register(app);
            Cysharp_Threading_Tasks_UniTask_1_Int32_Binding.Register(app);
            Cysharp_Threading_Tasks_UniTask_1_Int32_Binding_Awaiter_Binding.Register(app);
            System_String_Binding.Register(app);
            UnityEngine_Debug_Binding.Register(app);
            Cysharp_Threading_Tasks_UniTask_Binding.Register(app);
            Cysharp_Threading_Tasks_UniTask_Binding_Awaiter_Binding.Register(app);
        }

        /// <summary>
        /// Release the CLR binding, please invoke this BEFORE ILRuntime Appdomain destroy
        /// </summary>
        public static void Shutdown(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
        }
    }
}
