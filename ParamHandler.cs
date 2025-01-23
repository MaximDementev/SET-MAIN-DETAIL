using Autodesk.Revit.DB;

namespace SET_MAIN_DETAIL
{
    internal static class ParamHandler
    {


        public static string GetParamValue(Element instance, string ParameterName)
        {
            Parameter param = GetParam(instance, ParameterName);

            string stringValue = "";
            if (param != null)
            {
                // Проверяем, что параметр имеет значение
                if (param.HasValue)
                {
                    stringValue = param.AsString();
                }
                return stringValue;
            }
            else throw new System.Exception($"Не удалось прочитать параметр {ParameterName}");
        }

        private static Parameter GetParam(Element instance, string ParameterName)
        {
            Parameter param = instance.LookupParameter(ParameterName);

            return param;
        }

        public static void SetParamValue(Element instance, string ParameterName, int value)
        {
            Parameter param = instance.LookupParameter(ParameterName);
            int existValue = param.AsInteger();
            if (existValue == value) return;
            param.Set(value);
        }
    }
}
