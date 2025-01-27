using System.Collections.Generic;

namespace SET_MAIN_DETAIL
{
    public class OneRebarCage
    {
        public string CageName { get; private set; }
        public List<RebarInstance> RebarInstanceList { get; private set; }
        public int RebarInstancesCount { get { return RebarInstanceList.Count; } }


        public OneRebarCage(string cageName)
        {
            CageName = cageName;
        }

        public void AddInstance(RebarInstance element)
        {
            RebarInstanceList.Add(element);
        }
    }
}
