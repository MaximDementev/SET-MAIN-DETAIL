using Autodesk.Revit.DB;
using System.Xml.Linq;

namespace SET_MAIN_DETAIL
{
    internal static class ParamHandler
    {
        public static string GetStringParamValue(Document doc, Element instance, string ParameterName)
        {
            string value = "";
            try
            {
                Parameter param = GetParam(doc, instance, ParameterName);
                value = param.AsString();
            }
            catch { }
            return value;
        }

        public static int GetIntParamValue(Document doc, Element instance, string ParameterName)
        {
            Parameter param = GetParam(doc, instance, ParameterName);

            if (param != null && param.HasValue)
                return param.AsInteger();
            throw new System.Exception($"Не удалось прочитать параметр {ParameterName}");
        }

        public static double GetDoubleParamValue(Document doc, Element instance, string ParameterName)
        {
            Parameter param = GetParam(doc, instance, ParameterName);

            if (param == null || !param.HasValue)
                throw new System.Exception($"Не удалось прочитать параметр {ParameterName}");

            double valueInFeet = param.AsDouble();
            double valueInMillim = UnitUtils.ConvertFromInternalUnits(valueInFeet, UnitTypeId.Millimeters);
            return valueInMillim;
        }

        private static Parameter GetParam(Document doc, Element instance, string ParameterName)
        {
            Parameter parameterInstance = instance.LookupParameter(ParameterName);
            if (parameterInstance != null)
            { return parameterInstance; }

            ElementId typeId = instance.GetTypeId();
            if (typeId == ElementId.InvalidElementId)
                return null;

            Element typeElement = doc.GetElement(typeId);
            if (typeElement == null)
                return null;

            Parameter parameterType = typeElement.LookupParameter(ParameterName);
            if (parameterType == null)
                return null;
            return parameterType;
        }

        public static void SetIntParamValue(Element instance, string ParameterName, int value)
        {
            Parameter param = instance.LookupParameter(ParameterName);
            int existValue = param.AsInteger();
            if (existValue == value && value != 0) return;
            param.Set(value);
        }
    }
}
