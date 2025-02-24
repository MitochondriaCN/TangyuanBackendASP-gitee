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
        /// 判断电话号码是否合法，支持的国家在<see cref="CountryCode"/>枚举中定义。
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="countryCode"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static bool IsPhoneNumberValid(string phoneNumber, CountryCode countryCode)
        {
            // 根据国家码选择相应的正则表达式
            string pattern = countryCode switch
            {
                CountryCode.China => @"^(\+?0?86\-?)?1[345789]\d{9}$", // 中国
                CountryCode.UnitedStates => @"^\+?1?\d{10}$", // 美国
                CountryCode.India => @"^\+?91?\d{10}$", // 印度
                _ => throw new NotSupportedException("不支持的国家码")
            };

            // 使用正则表达式判断电话号码是否合法
            return Regex.IsMatch(phoneNumber, pattern);
        }
    }
}
