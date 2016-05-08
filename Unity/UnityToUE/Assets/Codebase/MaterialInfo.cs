using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nucleus
{
    public class MaterialInfo
    {
        public List<MaterialPropertyInfo> properties;
        public string name;

        public MaterialInfo()
        {
            properties = new List<MaterialPropertyInfo>();
        }
    }
}
