using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFixProj
{
    public class Main
    {
        public static async void TestUniTask()
        {
            int value = await IntTask();
            Debug.Log($"Get value task value : { value }");
        }

        private static async UniTask<int> IntTask()
        {
            await UniTask.Delay(1000);
            return 1;
        }
    }
}
