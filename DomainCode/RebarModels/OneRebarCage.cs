using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SET_MAIN_DETAIL
{
    public class OneRebarCage
    {
        #region Public Fields
        public RebarInstance MainRebarInstance { get; private set; }
        public string CageName { get; private set; }
        public bool RebarsCagesIsValidated { get; private set; }

        public DimensionBox DimensionBox { get; set; }

        public List<RebarInstance> RebarInstanceList = new List<RebarInstance>();
        public int RebarInstancesCount 
        { 
            get
            { 
                if(RebarInstanceList != null) return RebarInstanceList.Count;
                return 0;             
            } 
        }
        #endregion

        #region Constructor
        public OneRebarCage(string cageName)
        {
            CageName = cageName;
        }

        #endregion

        //---------------- Methods -----------------------

        public void AddInstance(RebarInstance element)
        {
            RebarInstanceList.Add(element);
        }

        public void AddInstances (List<FamilyInstance> elementList)
        {
            elementList.ForEach(elem => 
            {
                RebarInstance rebarInstance = new RebarInstance(elem);

                RebarInstanceList.Add(rebarInstance); 
            });
        }

        public void AddMainRebar(RebarInstance rebarInstance)
        {
            MainRebarInstance = rebarInstance;
        }

        public void MakeDimensionsBox()
        {
            List<XYZ> minPointList = new List<XYZ>();
            List<XYZ> maxPointList = new List<XYZ>();
            RebarInstanceList.ForEach(element => 
            {
                minPointList.Add(element.GetBoundingBox_MinPoint());
                maxPointList.Add(element.GetBoundingBox_MaxPoint());
            });
            DimensionBox = new DimensionBox(minPointList, maxPointList);
        }

        public bool CheckElementIsInsideDimensionBox(RebarInstance rebarInstance)
        {
            return DimensionBox.CheckPointIsInside(rebarInstance.GetLocationPoint());
        }

        public void SetMainRebarInstance (string RebarInstanceName)
        {
            if (MainRebarInstance != null && MainRebarInstance.GetRebarInstanceName() == RebarInstanceName) return;

            MainRebarInstance = RebarInstanceList.FirstOrDefault(elem =>
            elem.GetRebarInstanceName() == RebarInstanceName);

            MainRebarInstance.SetMainPartOfProduct(1);
        }

        public bool Validate(OneRebarCage MainRebarCage)
        {
            if (MainRebarCage.RebarInstancesCount == RebarInstanceList.Count)
             RebarsCagesIsValidated = true;
            else RebarsCagesIsValidated = false;

            return RebarsCagesIsValidated;

        }
    }
}
