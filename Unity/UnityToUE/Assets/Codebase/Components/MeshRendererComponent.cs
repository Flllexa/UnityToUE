using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nucleus
{
    public class MeshRendererComponent : ComponentInfo
    {
        public int mesh;
        public List<MaterialInfo> materials;

        public MeshRendererComponent()
        {
            materials = new List<MaterialInfo>();
        }
    }
}
