using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;

namespace DefaultNamespace
{
    public static partial class BuildExtentions
    {
        public static Transform[] GetChilds(this Transform parent)
        {
            List<Transform> ret = new List<Transform>();
            if (null == parent)
                return null;

            foreach (Transform child in parent.transform)
            {
                if (null == child)
                    continue;
                ret.Add(child);
                ret.AddRange(child.GetChilds());
            }

            return ret.ToArray();
        }
    }
}