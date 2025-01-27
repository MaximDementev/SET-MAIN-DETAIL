using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

using Accord.MachineLearning;
using Accord.Math.Distances;
using Accord.Math;

namespace SET_MAIN_DETAIL
{
    public class RebarCages
    {
        public Dictionary<string, SimilarRebars> SimilarRebarsDict = new Dictionary<string, SimilarRebars>();
        private int _allRebarsCount;
        public List<OneRebarCage> oneRebarCages = new List<OneRebarCage>();
        public OneRebarCage MainOneRebarCage;
        public string CageName { get; private set; }
        public bool ValidatedRebarsCages { get; private set; }
        public int CageCount 
        {
            get 
            {
                ValidateRebarsCages();
                if (ValidatedRebarsCages) 
                    return CalcRebarsCages(); 
                else return 0; 
            }
        }

        public RebarCages(string cageName)
        {
            CageName = cageName;
            MainOneRebarCage = new OneRebarCage(CageName);
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

        public void AddInstanceToMainOneRebarCage(RebarInstance instance)
        {
            MainOneRebarCage.AddInstance(instance);
        }

        public void SetFlagAsMainRebar()
        {

            ValidateRebarsCages();

            SimilarRebarsDict.First().Value.SetFlagAsMainRebar();  //потом если нужно будет, сделать более сложную логику выбора главной детали изделия
        }

        private void ValidateRebarsCages()
        {

            ValidatedRebarsCages = true;
            SimilarRebarsDict.Values.ToList().ForEach(simRebs =>
            {
                _allRebarsCount = _allRebarsCount + simRebs.RebarInstancesCount;
            });
            if(_allRebarsCount % MainOneRebarCage.RebarInstancesCount != 0)
                ValidatedRebarsCages = false;
        }
        private int CalcRebarsCages()
        {
            return (int) _allRebarsCount / MainOneRebarCage.RebarInstancesCount;
        }

        public void DivideAllInstancesToCages()
        {
            //надо понять, что тут делать!!
        }


    }
}
