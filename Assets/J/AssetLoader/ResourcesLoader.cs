namespace J
{
    using System;
    using System.Linq;
    using UniRx;
    using UnityEngine;
    using static AssetLoaderInstance;
    public static class ResourcesLoader
    {
        public static readonly LoadDelegate Load;

        static ResourcesLoader()
        {
            Load = ToLoadDelegate();
        }

        public static LoadDelegate ToLoadDelegate()
        {
            return entry =>
            {
                string path = entry.NormBundleName;
                if (string.IsNullOrEmpty(path))
                    return Observable.Throw<UnityEngine.Object>(
                        new AssetNotFoundException(entry),
                        Scheduler.MainThreadIgnoreTimeScale);
                return Resources.LoadAsync(path).AsAsyncOperationObservable()
                        .Select(op =>
                        {
                            return op.asset;
                        });
            };
        }
    }
}
