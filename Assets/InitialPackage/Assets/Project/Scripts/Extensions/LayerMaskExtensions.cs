using UnityEngine;

namespace Project
{
    public static class LayerMaskExtensions
    {
        public static int GetFirstLayerNumber(this LayerMask layerMask)
        {
            for (int i = 0; i < 32; i++)
            {
                if (layerMask == (layerMask | (1 << i)))
                {
                    // the layer mask includes this layer, so it must be the one we're looking for
                    return i;
                }
            }

            return -1;
        }
    }
}