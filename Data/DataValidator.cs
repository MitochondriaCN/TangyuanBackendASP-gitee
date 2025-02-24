using System.Globalization;
using System.Text.RegularExpressions;

namespace TangyuanBackendASP.Data
{
    public enum CountryCode
    {
        China,
        UnitedStates,
        India,
        UnitedKingdom
    }

    public static class DataValidator
    {
        /// <summary>
        /// 判断电话号码是否合法，国家码合法值在ISO 3166-1中定义。
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="countryCode">ISO 3166-1二位国家编码，如CN。</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static bool IsPhoneNumberValid(string phoneNumber, string countryCode)
        {
            // 根据国家码选择相应的正则表达式
            string pattern = countryCode switch
            {
                "CN" => @"^(\+?0?86\-?)?1[345789]\d{9}$", // 中国
                "US" => @"^\+?1?\d{10}$", // 美国
                "IN" => @"^\+?91?\d{10}$", // 印度
                _ => throw new NotSupportedException("Unsupported country code.")
            };

            // 使用正则表达式判断电话号码是否合法
            return Regex.IsMatch(phoneNumber, pattern);
        }
    }
}
