using System.Collections.Generic;

namespace moonNest
{
    public class NavigationAsset : SingletonScriptObject<NavigationAsset>
    {
        public List<NavigationData> navigationDatas;


        public NavigationData FindNavigationData(int id) => navigationDatas.Find(_ => _.id == id);
    }

}

