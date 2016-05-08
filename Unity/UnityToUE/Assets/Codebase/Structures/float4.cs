using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Nucleus
{
    public struct float4
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public float4(Quaternion vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
            w = vector.w;
        }
        public float4(Vector4 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
            w = vector.w;
        }
    }
}
