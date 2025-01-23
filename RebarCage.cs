using Autodesk.Revit.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void AddInstance(Element instance)
        {

            string itemName = ParamHandler.GetParamValue(instance, "ADSK_Позиция");
            
            if (SimilarRebarsDict.Count == 0 || !SimilarRebarsDict.ContainsKey(itemName))
            {
                SimilarRebars similarRebars = new SimilarRebars(itemName);
                similarRebars.AddInstance(instance);
                SimilarRebarsDict.Add(itemName, similarRebars);
            }
            else
            {
                SimilarRebarsDict[itemName].AddInstance(instance);
            }
        }

        public void SetFlagAsMainRebar()
        {
            SimilarRebarsDict.First().Value.SetFlagAsMainRebar();  //потом если нужно будет, сделать более сложную логику выбора главной детали изделия
        }
    }
}
