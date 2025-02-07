using Autodesk.Revit.DB;

namespace SET_MAIN_DETAIL
{
    public class RebarInstance
    {
        private Document _doc;
        private string _mainPartOfProduct_ParamName = "ADSK_Главная деталь изделия";
        private string _rebarCageName_ParamName = "ADSK_Марка изделия";
        private string _rebarPosition_ParamName = "ADSK_Позиция";
        private string _rebarDetailPrefix_ParamName = "ADSK_Деталь_Префикс";
        private string _rebarName_ParamName = "ADSK_Наименование";
        private string _rebarLength_ParamName = "ADSK_Длина арматуры";
        private string _rebarShape_ParamName = "ADSK_Форма арматуры";
        private string _rebarInstanceName; 


        private string _rebarCageName;
        private string _rebarPosition;
        private string _rebarDetailPrefix;
        private string _rebarName;
        private int _mainPartOfProduct = 2;
        private double _rebarLength = 0;
        private int _rebarShape = 0;

        public FamilyInstance _instance { get; private set; }

        public RebarInstance(FamilyInstance instance)
        {
            _doc = instance.Document;
            _instance = instance;
        }

        private string point
        {
            get
            {
                LocationPoint point = (LocationPoint)_instance.Location;
                return $"{(int)UnitUtils.ConvertFromInternalUnits(point.Point.X, UnitTypeId.Millimeters)}_" +
                    $"{(int)UnitUtils.ConvertFromInternalUnits(point.Point.Y, UnitTypeId.Millimeters)}_" +
                    $"{(int)UnitUtils.ConvertFromInternalUnits(point.Point.Z, UnitTypeId.Millimeters)}";
            }
        }

        //------------------------------------------------------------------------------


        //GET PARAM VALUE
        public string GetRebarInstanceName()
        {
            if (_rebarInstanceName == null)
            {
                string mark = GetRebarDetailPrefix();
                mark += $"_{GetRebarName()}";
                mark += $"_{GetRebarLength()}";
                mark += $"_{GetRebarPosition()}";
                mark += $"_{GetRebarShape()}";

                _rebarInstanceName = mark;
            }
            return _rebarInstanceName;
        }

        public string GetRebarCageName()
        {
            if (_rebarCageName == null)
                _rebarCageName = ParamHandler.GetStringParamValue(_doc, _instance, _rebarCageName_ParamName);
            return _rebarCageName;
        }

        public string GetRebarPosition()
        {
            if (_rebarPosition == null)
                _rebarPosition = ParamHandler.GetStringParamValue(_doc, _instance, _rebarPosition_ParamName);
            return _rebarPosition;
        }

        public int GetMainPartOfProduct()
        {
            if (_mainPartOfProduct == 3)
                _mainPartOfProduct = ParamHandler.GetIntParamValue(_doc, _instance, _mainPartOfProduct_ParamName);
            return _mainPartOfProduct;
        }

        public string GetRebarDetailPrefix()
        {
            if (_rebarDetailPrefix == null)
                _rebarDetailPrefix = ParamHandler.GetStringParamValue(_doc, _instance, _rebarDetailPrefix_ParamName);
            return _rebarDetailPrefix;
        }

        public string GetRebarName()
        {
            if (_rebarName == null)
                _rebarName = ParamHandler.GetStringParamValue(_doc, _instance, _rebarName_ParamName);
            return _rebarName;
        }

        public double GetRebarLength()
        {
            if (_rebarLength == 0)
                _rebarLength = ParamHandler.GetDoubleParamValue(_doc, _instance, _rebarLength_ParamName);
            return _rebarLength;
        }

        public int GetRebarShape()
        {
            if (_rebarShape == 0)
                _rebarShape = ParamHandler.GetIntParamValue(_doc, _instance, _rebarShape_ParamName);
            return _rebarShape;
        }

        //Location & BoundingBox
        public Autodesk.Revit.DB.XYZ GetLocationPoint()
        {
            LocationPoint locationPoint = _instance.Location as LocationPoint;
            if (locationPoint != null) return locationPoint.Point;
            else throw new System.Exception("Элемент не хранит координаты");
        }

        public Autodesk.Revit.DB.XYZ GetBoundingBox_MaxPoint()
        {
            return GetBoundingBox().Max;
        }

        public Autodesk.Revit.DB.XYZ GetBoundingBox_MinPoint()
        {
           return GetBoundingBox().Min;
        }

        public BoundingBoxXYZ GetBoundingBox()
        {
            BoundingBoxXYZ boundingBoxXYZ = _instance.get_BoundingBox(_doc.ActiveView);
            if (boundingBoxXYZ != null) return _instance.get_BoundingBox(_doc.ActiveView);
            else throw new System.Exception("Элемент не имеет тела");
        }


        //SET

        public void SetMainPartOfProduct(int value)
        {
            _mainPartOfProduct = value;
        }

        //UPDATE PARAM VALUE

        public void SetAllParamValue()
        {
            if (_mainPartOfProduct != 2)
                ParamHandler.SetIntParamValue(_instance, _mainPartOfProduct_ParamName, _mainPartOfProduct);
        }
    }
}
