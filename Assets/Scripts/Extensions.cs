using UnityEngine;

namespace RyanGQ.RunOrDie
{
    public static class Extensions
    {
        public static void SetLayerRecursively(this GameObject obj, int layer)
        {
            obj.layer = layer;
            foreach (Transform child in obj.GetComponentInChildren<Transform>(true))
                SetLayerRecursively(child.gameObject, layer);
        }
    }
}
