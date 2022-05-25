using UnityEngine;

namespace Project
{
    public static class StoreHelper
    {
        public static TargetStoreType GetTargetStore()
        {
            var targetStore = TargetStoreType.None;

#if UNITY_AMAZON
            targetStore = TargetStoreType.Amazon;
#elif SAYKIT
            targetStore = TargetStoreType.SayGames;
#endif
            
            return targetStore;
        }

        public static string GetDefineByStore(TargetStoreType storeType)
        {
            string define = string.Empty;
            
            switch (storeType)
            {
                case TargetStoreType.None:
                    define = string.Empty;
                    break;
                
                case TargetStoreType.Amazon:
                    define = "UNITY_AMAZON";
                    break;
                
                case TargetStoreType.SayGames:
                    define = "SAYKIT";
                    break;
                
                default:
                    Debug.Log($"Not found define for {nameof(TargetStoreType)} {storeType}");
                    break;
            }

            return define;
        }
    }
}