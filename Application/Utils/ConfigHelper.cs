using System;
using System.Collections.Generic;
using System.Text;
using AccountManager.Domain.Entities.Public;

namespace AccountManager.Application.Utils
{
    public static class ConfigHelper
    {
        public static object GetDataTypeValue(string strValue, string dataType)
        {
            switch (dataType)
            {
                case "number":
                    if (long.TryParse(strValue, out var intValue))
                    {
                        return intValue;
                    }
                    else if (decimal.TryParse(strValue, out var decimalValue))
                    {
                        return decimalValue;
                    }
                    else
                    {
                        return (object)default(int);
                    }
                case "boolean":
                    bool.TryParse(strValue, out var boolValue);
                    return (object)boolValue;

            }

            return strValue;
        }
    }
}
