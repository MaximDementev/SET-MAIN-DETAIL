using Autodesk.Revit.DB;

namespace SET_MAIN_DETAIL
{
    public class RebarInstance
    {
        private Document _doc;
        private string _mainPartOfProduct_ParamName = "ADSK_Главная деталь изделия";
        private string _rebarProductMark_ParamName = "ADSK_Марка изделия";
        private string _rebarPosition_ParamName = "ADSK_Позиция";
        private string _rebarDetailPrefix_ParamName = "ADSK_Деталь_Префикс";
        private string _rebarName_ParamName = "ADSK_Наименование";
        private string _rebarLength_ParamName = "ADSK_Длина арматуры";
        private string _rebarShape_ParamName = "ADSK_Форма арматуры";
        private string _rebarInstanceMark;


        private string _rebarProductMark;
        private string _rebarPosition;
        private string _rebarDetailPrefix;
        private string _rebarName;
        private int _mainPartOfProduct = 3;
        private double _rebarLength = 0;
        private int _rebarShape = 0;

        public Element _instance { get; private set; }

        public RebarInstance (Document doc, Element instance)
        { 
            _doc = doc;
            _instance = instance;  
        }

        //------------------------------------------------------------------------------


        //GET PARAM VALUE
        public string GetRebarInstanceMark()
        {
            if (_rebarInstanceMark == null) 
            {
                string mark = GetRebarDetailPrefix();
                mark += $"_{GetRebarName()}";
                mark += $"_{GetRebarLength()}";
                mark += $"_{GetRebarPosition()}";
                mark += $"_{GetRebarShape()}";
                _rebarInstanceMark = mark ;
            }
            return _rebarInstanceMark;
        }

        public string GetRebarProductMark()
        {
            if (_rebarProductMark == null) 
            _rebarProductMark = ParamHandler.GetStringParamValue(_doc, _instance, _rebarProductMark_ParamName);
            return _rebarProductMark;
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

        //SET

        public void SetMainPartOfProduct(int value)
        {
            _mainPartOfProduct = value;
        }

        //UPDATE PARAM VALUE

        public void SetAllParamValue()
        {
            if (_mainPartOfProduct != 3)
                ParamHandler.SetIntParamValue(_instance, _mainPartOfProduct_ParamName, _mainPartOfProduct);
        }

    }
}
