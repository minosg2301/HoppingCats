using System;
using System.Collections.Generic;

namespace moonNest
{
    [Serializable]
    public class NavigationData : BaseData
    {
        public List<NavigationPath> paths = new List<NavigationPath>();

        public NavigationData(string name) : base(name)
        {

        }
    }

    [Serializable]
    public class NavigationPath : BaseData
    {
        public int gameEventId;
        public float delayTime = 0;

        public NavigationPath(string name) : base(name)
        {

        }
    }

}


