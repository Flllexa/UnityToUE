using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nucleus
{
    public class SceneInfo
    {
        public string name;
        public List<ObjectInfo> rootObjects;
        public List<AssetInfo> usedAssets;

        public SceneInfo()
        {
            rootObjects = new List<ObjectInfo>();
            usedAssets = new List<AssetInfo>();
        }
    }
}
