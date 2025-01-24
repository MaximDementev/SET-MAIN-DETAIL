using System.Collections.Generic;
using System.Linq;


namespace SET_MAIN_DETAIL
{
    public class RebarCage
    {
        public Dictionary<string, SimilarRebars> SimilarRebarsDict = new Dictionary<string, SimilarRebars>();

        public string CageName { get; private set; }

        public RebarCage(string cageName)
        {
            CageName = cageName;
        }

        public void AddInstance(RebarInstance instance)
        {

            string InstanceMark = instance.GetRebarInstanceMark();
            
            if (SimilarRebarsDict.Count == 0 || !SimilarRebarsDict.ContainsKey(InstanceMark))
            {
                SimilarRebars similarRebars = new SimilarRebars(InstanceMark);
                similarRebars.AddInstance(instance);
                SimilarRebarsDict.Add(InstanceMark, similarRebars);
            }
            else
            {
                SimilarRebarsDict[InstanceMark].AddInstance(instance);
            }
        }

        public void SetFlagAsMainRebar()
        {
            SimilarRebarsDict.First().Value.SetFlagAsMainRebar();  //потом если нужно будет, сделать более сложную логику выбора главной детали изделия
        }
    }
}
