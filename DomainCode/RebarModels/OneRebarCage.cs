using System;
using System.Collections.Generic;

namespace SET_MAIN_DETAIL
{
    public class OneRebarCage
    {
        private RebarInstance _mainRebarInstance;
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

        public void AddInstance(RebarInstance element)
        {
            RebarInstanceList.Add(element);
        }

        internal void AddMainRebar(RebarInstance rebarInstance)
        {
            _mainRebarInstance = rebarInstance;
        }

        internal void GetDimensionsBox()
        {
            throw new NotImplementedException();
        }
    }
}
