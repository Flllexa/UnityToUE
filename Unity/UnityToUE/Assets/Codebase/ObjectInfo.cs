using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Nucleus
{
    public class ObjectInfo
    {
        public string name;
        public List<ObjectInfo> children;
        public List<ComponentInfo> components;

        public float3 position;
        public float3 scale;
        public float4 rotation;

        public bool active;

        public ObjectInfo()
        {
            children = new List<ObjectInfo>();
            components = new List<ComponentInfo>();
        }
    }
}
