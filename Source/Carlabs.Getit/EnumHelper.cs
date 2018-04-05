namespace Carlabs.Getit
{
    /// <summary>
    /// Helper class that will allow us to pass a string but
    /// keep the idea that it's really a non-quoted ENUM in the
    /// gQL relm. So the idea would be to pass this as a param
    /// value not a string and it will be rendered without being
    /// quoted
    /// </summary>
    /// <example>
    /// Creating Instance -
    ///     EnumHelper GqlEnumEnabled = new EnumHelper().Enum("ENABLED");
    ///     EnumHelper GqlEnumDisabled = new EnumHelper("DISABLED");
    ///     GqlEnumDisabled.Enum("SOMETHNG_ENUM");
    /// 
    /// In use -
    ///     Creating a Dictionary for a Select (gQL Parameters)
    ///     Dictionary &lt;string, object&gt; mySubDict = new Dictionary&lt;string, object&gt;
    ///     {
    ///         {"subMake", "honda"},
    ///         {"subState", "ca"},
    ///         {"__debug", GqlEnumDisabled},
    ///         {"SuperQuerySpeed", GqlEnumEnabled }
    ///     };
    /// </example>
    public class EnumHelper
    {
        private string _str;

        public EnumHelper(string enumStr = "")
        {
            _str = enumStr;
        }

        public EnumHelper Enum(string enumStr)
        {
            _str = enumStr;
            return this;
        }

        public override string ToString()
        {
            return _str;
        }
    }
}
