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
        public static void Test()
        {
            int i = 10;
            int b = i + 1;
            Debug.Log(b);
        }

        public static async void TestVoid()
        {
            int i = 0;
            await Task.Delay(500);
            i++;
            Debug.Log(i);
        }

        /*
        public static async void TestUniTask()
        {
            int value = await IntTask();
            Debug.Log($"Get value task value : { value }");
            TestVoid();
        }

        private static async UniTask<int> IntTask()
        {
            await UniTask.Delay(100);
            Debug.LogError("int task awaited");
            return 1;
        }*/

    }
}
