using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace SET_MAIN_DETAIL
{
    public class RebarCages
    {
        public List<RebarInstance> RebarInstancesList = new List<RebarInstance>();
        public List<OneRebarCage> oneRebarCagesList = new List<OneRebarCage>();
        public OneRebarCage MainOneRebarCage;

        public string CageName { get; private set; }
        public bool RebarsCagesIsValidated { get; private set; }

        public int _allRebarsCount { get; private set; }
        public int _mainOneRebarCageCount { get { return MainOneRebarCage.RebarInstancesCount; } }
        public int CageCount
        {
            get
            {
                ValidateRebarsCages();
                if (RebarsCagesIsValidated)
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
            RebarInstancesList.Add(instance);

        }

        public void SetFlagAsMainRebar()
        {

        }

        private void ValidateRebarsCages()
        {
        }

        private int CalcRebarsCages()
        {
            return (int)_allRebarsCount / MainOneRebarCage.RebarInstancesCount;
        }

        public void DivideAllInstancesToCages()
        {
            //надо понять, что тут делать!!
        }


    }
}
