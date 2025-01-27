using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

using Accord.MachineLearning;
using Accord.Math.Distances;
using Accord.Math;

namespace SET_MAIN_DETAIL
{
    public class RebarCage
    {
        public Dictionary<string, SimilarRebars> SimilarRebarsDict = new Dictionary<string, SimilarRebars>();
        public string CageName { get; private set; }
        public bool ValidatedSimilarRebarsCount { get; private set; }
        public int CageCount 
        {
            get { if (ValidatedSimilarRebarsCount) 
                    return SimilarRebarsDict.Values.ToList().FirstOrDefault().RebarInstancesCount; 
                else return 0; }
        }

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

            ValidateSimilarRebarsCount();

            SimilarRebarsDict.First().Value.SetFlagAsMainRebar();  //потом если нужно будет, сделать более сложную логику выбора главной детали изделия
        }


        private void ValidateSimilarRebarsCount()
        {

            ValidatedSimilarRebarsCount = true;
            int similarRebarsCount = SimilarRebarsDict.Values.ToList().FirstOrDefault().RebarInstancesCount;
            foreach (var SemRebs in SimilarRebarsDict.Values)
            {
                if (similarRebarsCount % SemRebs.RebarInstancesCount != 0 && SemRebs.RebarInstancesCount % similarRebarsCount != 0)
                {
                    ValidatedSimilarRebarsCount = false;
                    break;
                }
            }
        }

        private void DBSCANing()
        {
            if (!ValidatedSimilarRebarsCount) return;

            List<double[]> coordinatesList = new List<double[]>();

            string coordinatesAsString = "";

            foreach (var item in SimilarRebarsDict.Values.ToList())
            {
                item.RebarInstanceList.ForEach(inst => 
                {
                    LocationPoint point = (LocationPoint)inst._instance.Location;
                    double[] pointCoordinates =  new double[] 
                    {
                        (int)UnitUtils.ConvertFromInternalUnits(point.Point.X, UnitTypeId.Millimeters),
                        (int)UnitUtils.ConvertFromInternalUnits(point.Point.Y, UnitTypeId.Millimeters),
                        //(int)UnitUtils.ConvertFromInternalUnits(point.Point.Z, UnitTypeId.Millimeters)
                    };
                    coordinatesAsString += $"X={pointCoordinates[0]}, Y={pointCoordinates[1]}\n";
                    coordinatesList.Add(pointCoordinates);
                });
            }

            double[][] coordinates = coordinatesList.ToArray();


            //-------------------

            // Инициализация MeanShift
            MeanShift meanShift = new MeanShift();

            meanShift.Bandwidth = 100;
            meanShift.MaxIterations = 1000;  // или другое значение


            // Обучение модели
            meanShift.Learn(coordinates);

            // Определение кластеров для точек
            int clastersCount = meanShift.Clusters.Count;
            //------------------


            //-------------------
            TaskDialog.Show("Кластеры", $"Всего изделий {clastersCount}"
                + $"\n{coordinatesAsString}"
                );

        }
    }
}
