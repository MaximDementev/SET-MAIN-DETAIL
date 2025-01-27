using System.Collections.Generic;

namespace SET_MAIN_DETAIL
{
    public class SimilarRebars
    {
        public string RebarName { get; private set; }
        public int RebarInstancesCount { get { return RebarInstanceList.Count;} }

        public List<RebarInstance> RebarInstanceList { get; private set; }

        public SimilarRebars(string rebarName)
        {
            RebarName = rebarName;
            RebarInstanceList = new List<RebarInstance>();
        }

        public void AddInstance(RebarInstance element)
        {
            RebarInstanceList.Add(element);
        }

        public void SetFlagAsMainRebar()
        {
            RebarInstanceList.ForEach(el => el.SetMainPartOfProduct(1));
        }
    }
}
