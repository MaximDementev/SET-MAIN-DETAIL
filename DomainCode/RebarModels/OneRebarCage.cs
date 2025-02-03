using System;
using System.Collections.Generic;
using System.Linq;

namespace SET_MAIN_DETAIL
{
    public class OneRebarCage
    {
        public RebarInstance MainRebarInstance;
        public string CageName { get; private set; }
        public List<RebarInstance> RebarInstanceList = new List<RebarInstance>();
        public int RebarInstancesCount { get 
            { 
                if(RebarInstanceList.Count != null) return RebarInstanceList.Count;
                return 0; 
            
            } }

        public OneRebarCage(string cageName)
        {
            CageName = cageName;
        }
        public DimensionBox DimensionBox {get; set;}

        public void AddInstance(RebarInstance element)
        {
            RebarInstanceList.Add(element);
        }

        public void AddMainRebar(RebarInstance rebarInstance)
        {
            MainRebarInstance = rebarInstance;
        }

        public void MakeDimensionsBox()
        {
            List<Autodesk.Revit.DB.XYZ> minPointList = new List<Autodesk.Revit.DB.XYZ>();
            List<Autodesk.Revit.DB.XYZ> maxPointList = new List<Autodesk.Revit.DB.XYZ>();
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
            MainRebarInstance = RebarInstanceList.FirstOrDefault(elem =>
            elem.GetRebarInstanceName() == RebarInstanceName);

            MainRebarInstance.SetMainPartOfProduct(1);
        }
    }
}
