using System.Collections.Generic;

namespace moonNest
{
    public class Profiles : SingletonScriptObject<Profiles>
    {
        public List<ProfileConfig> profiles = new List<ProfileConfig>();
    }
}