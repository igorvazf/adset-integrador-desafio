using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AdSetIntegrador.Web.Utils
{
    public static class EnumHelper
    {
        public static string GetEnumDisplayName(Enum enumValue)
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<DisplayAttribute>()?
                            .Name ?? enumValue.ToString();
        }
    }
}
