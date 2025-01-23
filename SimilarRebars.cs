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
        public List<Element> RebarInstanceList { get; private set; }

        public SimilarRebars(string rebarName)
        {
            RebarName = rebarName;
            RebarInstanceList = new List<Element>();
        }

        public void AddInstance(Element element)
        {
            RebarInstanceList.Add(element);
        }

        public void SetFlagAsMainRebar()
        {
            foreach (var item in RebarInstanceList)
            {
                ParamHandler.SetParamValue(item, "ADSK_Главная деталь изделия", 1);
            }
        }

        public int GetInstanceCount()
        {
            return RebarInstanceList.Count;
        }
    }
}
