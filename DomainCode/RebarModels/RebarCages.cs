using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System;

namespace SET_MAIN_DETAIL
{
    public class RebarCages
    {
        #region Public Fields
        public List<RebarInstance> RebarInstancesList = new List<RebarInstance>();
        public List<OneRebarCage> OneRebarCagesList = new List<OneRebarCage>();
        public OneRebarCage MainOneRebarCage;

        public string CageName { get; private set; }
        public bool RebarsCagesIsValidated { get; private set; }

        public int CageCount
        {
            get
            {
                return OneRebarCagesList.Count;
            }
        }
        #endregion


        #region Constructor
        public RebarCages(string cageName)
        {
            CageName = cageName;
            MainOneRebarCage = new OneRebarCage(CageName);
        }
        #endregion

        //------------- Methods -------------------

        public void AddInstance(RebarInstance instance)
        {
            RebarInstancesList.Add(instance);
        }

        public void SetMainOneRebarCage(OneRebarCage oneRebarCage)
        {
            if (oneRebarCage == null) return;
            OneRebarCagesList.Clear();
            MainOneRebarCage = oneRebarCage;
        }

        public void DivideAllInstancesToCages()
        {
            MainOneRebarCage.MakeDimensionsBox();

            double dimensionBoxRadius = MainOneRebarCage.DimensionBox.GetRadius();

            foreach (RebarInstance rebarInstance in RebarInstancesList)
            {
                bool isAddedToExistingCage = false;

                foreach (OneRebarCage cage in OneRebarCagesList)
                {
                    if (cage.CheckElementIsInsideDimensionBox(rebarInstance))
                    {
                        cage.AddInstance(rebarInstance);
                        isAddedToExistingCage = true;
                        break;
                    }
                }

                if (!isAddedToExistingCage)
                {
                    OneRebarCage newOneRebarCage = new OneRebarCage(rebarInstance.GetRebarCageName());
                    newOneRebarCage.DimensionBox = new DimensionBox(rebarInstance.GetLocationPoint(), dimensionBoxRadius);
                    newOneRebarCage.AddInstance(rebarInstance);
                    OneRebarCagesList.Add(newOneRebarCage);
                }
            }
        }

        //многопоточный
        public void ValidateRebarsCages()
        {
            bool allCagesIsValidated = true;

            //многопоточный процесс
            OneRebarCagesList.ForEach(cage => 
            {
                if (!cage.Validate(MainOneRebarCage)) allCagesIsValidated = false;
            });

            RebarsCagesIsValidated = allCagesIsValidated;
        }

        //многопоточный
        public void SetFlagAsMainRebar(int value)
        {
            //многопоточный процесс
            RebarInstancesList.ForEach(rebarInst =>
            {
                rebarInst.SetMainPartOfProduct(value);
            });
        }

        //многопоточный
        public void SetMainRebarInstance()
        {
            string MainRebarInstanceName = MainOneRebarCage.MainRebarInstance.GetRebarInstanceName();

            //многопоточный процесс
            foreach (OneRebarCage cage in OneRebarCagesList)
            {
                cage.SetMainRebarInstance(MainRebarInstanceName);
            }
        }
    }
}
