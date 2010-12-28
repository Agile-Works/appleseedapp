// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XslHelper.cs" company="--">
//   Copyright � -- 2010. All Rights Reserved.
// </copyright>
// <summary>
//   XslHelper object, designed to be imported into an XSLT transform
//   via XsltArgumentList.AddExtensionObject(...). Provides transform with
//   access to various Appleseed functions, such as BuildUrl(), IsInRoles(), data
//   formatting, etc. (Jes1111)
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.Framework.Helpers
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Security;
    using System.Xml;
    using System.Xml.XPath;

    using Appleseed.Framework.Security;
    using Appleseed.Framework.Site.Configuration;
    using Appleseed.Framework.Users.Data;

    /// <summary>
    /// XslHelper object, designed to be imported into an XSLT transform
    ///   via XsltArgumentList.AddExtensionObject(...). Provides transform with 
    ///   access to various Appleseed functions, such as BuildUrl(), IsInRoles(), data 
    ///   formatting, etc. (Jes1111)
    /// </summary>
    public class XslHelper
    {
        #region Constants and Fields

        /// <summary>
        /// The portal settings.
        /// </summary>
        private readonly PortalSettings portalSettings;

        /// <summary>
        /// The membership user.
        /// </summary>
        private readonly MembershipUser user;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="XslHelper"/> class. 
        /// </summary>
        public XslHelper()
        {
            if (HttpContext.Current != null)
            {
                this.portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];

                var users = new UsersDB();
                this.user = users.GetSingleUser(HttpContext.Current.User.Identity.Name, this.portalSettings.PortalAlias);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds to URL.
        /// </summary>
        /// <param name="url">
        /// The URL.
        /// </param>
        /// <param name="paramKey">
        /// The key of the URL.
        /// </param>
        /// <param name="paramValue">
        /// The value of the URL.
        /// </param>
        /// <returns>
        /// A string value...
        /// </returns>
        public string AddToUrl(string url, string paramKey, string paramValue)
        {
            if (url.IndexOf(paramKey) == -1)
            {
                if (url.IndexOf("?") > 0)
                {
                    url = string.Format("{0}&{1}={2}", url.Trim(), paramKey.Trim(), paramValue.Trim());
                }
                else
                {
                    url = url.Trim();
                    url = string.Format("{0}/{1}_{2}{3}", url.Substring(0, url.LastIndexOf("/")), paramKey.Trim(), paramValue.Trim(), url.Substring(url.LastIndexOf("/")));
                }
            }

            return url;
        }

        /// <summary>
        /// Builds the URL.
        /// </summary>
        /// <param name="targetPage">
        /// The target page.
        /// </param>
        /// <param name="pageId">
        /// The page ID.
        /// </param>
        /// <returns>
        /// A string value...
        /// </returns>
        public string BuildUrl(string targetPage, int pageId)
        {
            // targetPage = System.Text.RegularExpressions.Regex.Replace(targetPage,@"[\.\$\^\{\[\(\|\)\*\+\?!'""]",string.Empty);
            // targetPage = targetPage.Replace(" ","_").ToLower();
            // return Appleseed.HttpUrlBuilder.BuildUrl("~/" + targetPage + ".aspx", tabID);
            return HttpUrlBuilder.BuildUrl(string.Concat("~/", this.Clean(targetPage), ".aspx"), pageId);
        }

        /// <summary>
        /// Builds the URL.
        /// </summary>
        /// <param name="targetPage">
        /// The target page.
        /// </param>
        /// <param name="pageId">
        /// The page ID.
        /// </param>
        /// <param name="pathTrace">
        /// The path trace.
        /// </param>
        /// <returns>
        /// The build URL.
        /// </returns>
        public string BuildUrl(string targetPage, int pageId, string pathTrace)
        {
            return HttpUrlBuilder.BuildUrl(
                string.Concat("~/", this.Clean(targetPage), ".aspx"), pageId, this.Clean(pathTrace));
        }

        /// <summary>
        /// Builds the URL.
        /// </summary>
        /// <param name="pageId">
        /// The page ID.
        /// </param>
        /// <param name="pathTrace">
        /// The path trace.
        /// </param>
        /// <returns>
        /// A string value...
        /// </returns>
        public string BuildUrl(int pageId, string pathTrace)
        {
            return HttpUrlBuilder.BuildUrl(pageId, this.Clean(pathTrace));
        }

        /// <summary>
        /// Builds the URL.
        /// </summary>
        /// <param name="pageId">
        /// The page ID.
        /// </param>
        /// <returns>
        /// The build URL.
        /// </returns>
        public string BuildUrl(int pageId)
        {
            return HttpUrlBuilder.BuildUrl(pageId);
        }

        /// <summary>
        /// C2s the F.
        /// </summary>
        /// <param name="c">
        /// The c.
        /// </param>
        /// <returns>
        /// A double value...
        /// </returns>
        public double C2F(double c)
        {
            return (1.8 * c) + 32;
        }

        /// <summary>
        /// Checks the roles.
        /// </summary>
        /// <param name="authRoles">
        /// The auth roles.
        /// </param>
        /// <returns>
        /// A Boolean value...
        /// </returns>
        public bool CheckRoles(string authRoles)
        {
            return PortalSecurity.IsInRoles(authRoles);
        }

        /// <summary>
        /// Desktops the tabs XML.
        /// </summary>
        /// <returns>
        /// A System.Xml.XPath.XPathNodeIterator value...
        /// </returns>
        public XPathNodeIterator DesktopTabsXml()
        {
            return this.portalSettings.PortalPagesXml.CreateNavigator().Select("*");
        }

        /// <summary>
        /// F2s the C.
        /// </summary>
        /// <param name="f">
        /// The f.
        /// </param>
        /// <returns>
        /// A double value...
        /// </returns>
        public double F2C(double f)
        {
            return (f - 32) / 1.8;
        }

        /// <summary>
        /// Formats the date time.
        /// </summary>
        /// <param name="dateStr">
        /// The date STR.
        /// </param>
        /// <param name="dataCulture">
        /// The data culture.
        /// </param>
        /// <param name="formatStr">
        /// The format STR.
        /// </param>
        /// <returns>
        /// A string value...
        /// </returns>
        public string FormatDateTime(string dateStr, string dataCulture, string formatStr)
        {
            try
            {
                return this.FormatDateTime(
                    dateStr, dataCulture, this.portalSettings.PortalDataFormattingCulture.Name, formatStr);
            }
            catch
            {
                return dateStr;
            }
        }

        /// <summary>
        /// Formats the date time.
        /// </summary>
        /// <param name="dateStr">
        /// The date STR.
        /// </param>
        /// <param name="formatStr">
        /// The format STR.
        /// </param>
        /// <returns>
        /// A string value...
        /// </returns>
        public string FormatDateTime(string dateStr, string formatStr)
        {
            try
            {
                return this.FormatDateTime(
                    dateStr, 
                    this.portalSettings.PortalDataFormattingCulture.Name, 
                    this.portalSettings.PortalDataFormattingCulture.Name, 
                    formatStr);
            }
            catch
            {
                return dateStr;
            }
        }

        /// <summary>
        /// Formats the date time.
        /// </summary>
        /// <param name="dateStr">
        /// The date STR.
        /// </param>
        /// <returns>
        /// A string value...
        /// </returns>
        public string FormatDateTime(string dateStr)
        {
            try
            {
                return DateTime.Parse(dateStr).ToLongDateString();
            }
            catch
            {
                return dateStr;
            }
        }

        /// <summary>
        /// Formats the date time.
        /// </summary>
        /// <param name="dateStr">
        /// The date STR.
        /// </param>
        /// <param name="dataCulture">
        /// The data culture.
        /// </param>
        /// <param name="outputCulture">
        /// The output culture.
        /// </param>
        /// <param name="formatStr">
        /// The format STR.
        /// </param>
        /// <returns>
        /// A string value...
        /// </returns>
        public string FormatDateTime(string dateStr, string dataCulture, string outputCulture, string formatStr)
        {
            try
            {
                DateTime conv = dataCulture.ToLower() == this.portalSettings.PortalDataFormattingCulture.Name.ToLower()
                                    ? DateTime.ParseExact(
                                        dateStr,
                                        "mm/dd/yyyy hh:mm:ss",
                                        new CultureInfo(dataCulture, false),
                                        DateTimeStyles.AdjustToUniversal)
                                    : DateTime.Parse(dateStr, new CultureInfo(dataCulture, false), DateTimeStyles.None);

                return outputCulture.ToLower() == this.portalSettings.PortalDataFormattingCulture.Name.ToLower() ? conv.ToString(formatStr) : conv.ToString(formatStr, new CultureInfo(outputCulture, false));
            }
            catch
            {
                return dateStr;
            }
        }

        /// <summary>
        /// Formats the money.
        /// </summary>
        /// <param name="myAmount">
        /// My amount.
        /// </param>
        /// <param name="myCurrency">
        /// My currency.
        /// </param>
        /// <returns>
        /// A string value...
        /// </returns>
        public string FormatMoney(string myAmount, string myCurrency)
        {
            try
            {
                // Jonathan - im not sure what namespace this comes from?
                // TODO: FIX TIHS
                return myAmount;

                // return new Money(Decimal.Parse(myAmount, CultureInfo.InvariantCulture.NumberFormat), myCurrency).ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Formats the number.
        /// </summary>
        /// <param name="numberStr">
        /// The number STR.
        /// </param>
        /// <param name="dataCulture">
        /// The data culture.
        /// </param>
        /// <param name="formatStr">
        /// The format STR.
        /// </param>
        /// <returns>
        /// A string value...
        /// </returns>
        public string FormatNumber(string numberStr, string dataCulture, string formatStr)
        {
            try
            {
                return this.FormatNumber(
                    numberStr, dataCulture, this.portalSettings.PortalDataFormattingCulture.Name, formatStr);
            }
            catch
            {
                return numberStr;
            }
        }

        /// <summary>
        /// Formats the number.
        /// </summary>
        /// <param name="numberStr">
        /// The number STR.
        /// </param>
        /// <param name="dataCulture">
        /// The data culture.
        /// </param>
        /// <param name="outputCulture">
        /// The output culture.
        /// </param>
        /// <param name="formatStr">
        /// The format STR.
        /// </param>
        /// <returns>
        /// A string value...
        /// </returns>
        public string FormatNumber(string numberStr, string dataCulture, string outputCulture, string formatStr)
        {
            try
            {
                var conv = dataCulture.ToLower() == this.portalSettings.PortalDataFormattingCulture.Name.ToLower() ? Double.Parse(numberStr) : Double.Parse(numberStr, new CultureInfo(dataCulture, false));

                return outputCulture.ToLower() == this.portalSettings.PortalDataFormattingCulture.Name.ToLower() ? conv.ToString(formatStr) : conv.ToString(formatStr, new CultureInfo(outputCulture, false));
            }
            catch
            {
                return numberStr;
            }
        }

        /// <summary>
        /// Formats the temp.
        /// </summary>
        /// <param name="tempStr">
        /// The temp STR.
        /// </param>
        /// <param name="dataScale">
        /// The data scale.
        /// </param>
        /// <param name="outputScale">
        /// The output scale.
        /// </param>
        /// <returns>
        /// A string value...
        /// </returns>
        public string FormatTemp(string tempStr, string dataScale, string outputScale)
        {
            try
            {
                double conv;

                if (dataScale == outputScale)
                {
                    conv = double.Parse(tempStr, new CultureInfo(string.Empty));
                    return conv.ToString("F0") + Convert.ToChar(176) + outputScale;
                }
                
                if (outputScale.ToUpper() == "C")
                {
                    conv = this.F2C(double.Parse(tempStr, new CultureInfo(string.Empty)));
                    return conv.ToString("F0") + Convert.ToChar(176) + "C";
                }

                conv = this.C2F(double.Parse(tempStr, new CultureInfo(string.Empty)));
                return conv.ToString("F0") + Convert.ToChar(176) + "F";
            }
            catch
            {
                return tempStr;
            }
        }

        /// <summary>
        /// Localizes the specified text key.
        /// </summary>
        /// <param name="textKey">
        /// The text key.
        /// </param>
        /// <param name="translation">
        /// The translation.
        /// </param>
        /// <returns>
        /// A string value...
        /// </returns>
        public string Localize(string textKey, string translation)
        {
            return General.GetString(textKey, translation);
        }

        /// <summary>
        /// Pages the ID.
        /// </summary>
        /// <returns>
        /// A string value...
        /// </returns>
        public string PageID()
        {
            return this.portalSettings.ActivePage.PageID.ToString();
        }

        /// <summary>
        /// Portals the alias.
        /// </summary>
        /// <returns>
        /// A string value...
        /// </returns>
        public string PortalAlias()
        {
            return this.portalSettings.PortalAlias;
        }

        /// <summary>
        /// Portals the content language.
        /// </summary>
        /// <returns>
        /// A string value...
        /// </returns>
        public string PortalContentLanguage()
        {
            return this.portalSettings.PortalContentLanguage.Name;
        }

        /// <summary>
        /// Portals the data formatting culture.
        /// </summary>
        /// <returns>
        /// A string value...
        /// </returns>
        public string PortalDataFormattingCulture()
        {
            return this.portalSettings.PortalDataFormattingCulture.Name;
        }

        /// <summary>
        /// Portals the full path.
        /// </summary>
        /// <returns>
        /// A string value...
        /// </returns>
        public string PortalFullPath()
        {
            return this.portalSettings.PortalFullPath;
        }

        /// <summary>
        /// Portals the ID.
        /// </summary>
        /// <returns>
        /// A string value...
        /// </returns>
        public string PortalID()
        {
            return this.portalSettings.PortalID.ToString();
        }

        /// <summary>
        /// Portals the layout path.
        /// </summary>
        /// <returns>
        /// A string value...
        /// </returns>
        public string PortalLayoutPath()
        {
            return this.portalSettings.PortalLayoutPath;
        }

        /// <summary>
        /// Portals the name.
        /// </summary>
        /// <returns>
        /// A string value...
        /// </returns>
        public string PortalName()
        {
            return this.portalSettings.PortalName;
        }

        /// <summary>
        /// Portals the title.
        /// </summary>
        /// <returns>
        /// A string value...
        /// </returns>
        public string PortalTitle()
        {
            return this.portalSettings.PortalTitle;
        }

        /// <summary>
        /// Portals the UI language.
        /// </summary>
        /// <returns>
        /// A string value...
        /// </returns>
        public string PortalUILanguage()
        {
            return this.portalSettings.PortalUILanguage.Name;
        }

        /// <summary>
        /// Selected options.
        /// </summary>
        /// <param name="metadataXml">
        /// The metadata XML.
        /// </param>
        /// <returns>
        /// A string value...
        /// </returns>
        public string SelectedOptions(string metadataXml)
        {
            var selectedOptions = string.Empty;

            // Create a xml Document
            var xmlDoc = new XmlDocument();

            if (!string.IsNullOrEmpty(metadataXml))
            {
                try
                {
                    xmlDoc.LoadXml(metadataXml);
                    var foundNode1 = xmlDoc.SelectSingleNode("options/option1/selected");

                    if (foundNode1 != null)
                    {
                        selectedOptions += foundNode1.InnerText;
                    }

                    var foundNode2 = xmlDoc.SelectSingleNode("options/option2/selected");

                    if (foundNode2 != null)
                    {
                        selectedOptions += " - " + foundNode2.InnerText;
                    }

                    var foundNode3 = xmlDoc.SelectSingleNode("options/option3/selected");

                    if (foundNode3 != null)
                    {
                        selectedOptions += " - " + foundNode3.InnerText;
                    }
                }
                catch (Exception ex)
                {
                    ErrorHandler.Publish(LogLevel.Warn, string.Format("XSL failed. Metadata Was: '{0}'", metadataXml), ex);
                }
            }

            return selectedOptions;
        }

        /// <summary>
        /// Tabs the title.
        /// </summary>
        /// <returns>
        /// A string value...
        /// </returns>
        public string TabTitle()
        {
            return this.portalSettings.ActivePage.PageName;
        }

        /// <summary>
        /// Users the email.
        /// </summary>
        /// <returns>
        /// A string value...
        /// </returns>
        public string UserEmail()
        {
            return this.user.Email;
        }

        /// <summary>
        /// Users the ID.
        /// </summary>
        /// <returns>
        /// A string value...
        /// </returns>
        public Guid UserID()
        {
            return (Guid)this.user.ProviderUserKey;
        }

        /// <summary>
        /// Users the name.
        /// </summary>
        /// <returns>
        /// A string value...
        /// </returns>
        public string UserName()
        {
            return this.user.UserName;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Cleans the specified my text.
        /// </summary>
        /// <param name="myText">
        /// My text.
        /// </param>
        /// <returns>
        /// The clean.
        /// </returns>
        private string Clean(string myText)
        {
            // is this faster/slower than using iteration over string?
            var mySeparator = '_';
            var singleSeparator = "_";
            var doubleSeparator = "__";

            // myText = Regex.Replace(myText.ToLower(), @"[^-'/\p{L}\p{N}]",singleSeparator);
            myText = Regex.Replace(myText.ToLower(), @"[^-\p{L}\p{N}]", singleSeparator);

            return myText.Replace(doubleSeparator, singleSeparator).Trim(mySeparator);
        }

        #endregion
    }
}