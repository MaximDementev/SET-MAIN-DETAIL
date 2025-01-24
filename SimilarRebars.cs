using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SET_MAIN_DETAIL
{
    public class SimilarRebars
    {
        public string RebarName { get; private set; }
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

        public int GetInstanceCount()
        {
            return RebarInstanceList.Count;
        }
    }
}
