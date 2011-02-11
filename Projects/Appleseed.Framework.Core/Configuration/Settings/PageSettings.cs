// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PageSettings.cs" company="--">
//   Copyright � -- 2011. All Rights Reserved.
// </copyright>
// <summary>
//   PageSettings Class encapsulates the detailed settings
//   for a specific Page in the Portal
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.Framework.Site.Configuration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Web;
    using System.Web.UI.WebControls;

    using Appleseed.Framework.DataTypes;
    using Appleseed.Framework.Design;
    using Appleseed.Framework.Localization;
    using Appleseed.Framework.Settings;
    using Appleseed.Framework.Settings.Cache;

    using Path = Appleseed.Framework.Settings.Path;

    /// <summary>
    /// PageSettings Class encapsulates the detailed settings 
    ///   for a specific Page in the Portal
    /// </summary>
    public class PageSettings
    {
        #region Constants and Fields

        /// <summary>
        ///   The authorized roles.
        /// </summary>
        public string AuthorizedRoles;

        /// <summary>
        ///   The mobile page name.
        /// </summary>
        public string MobilePageName;

        /// <summary>
        ///   The modules.
        /// </summary>
        public ArrayList Modules = new ArrayList();

        /// <summary>
        ///   The page id.
        /// </summary>
        public int PageID;

        /// <summary>
        ///   The page layout.
        /// </summary>
        public string PageLayout;

        /// <summary>
        ///   The page order.
        /// </summary>
        public int PageOrder;

        /// <summary>
        ///   The parent page id.
        /// </summary>
        public int ParentPageID;

        /// <summary>
        ///   The show mobile.
        /// </summary>
        public bool ShowMobile;

        /// <summary>
        ///   The custom settings.
        /// </summary>
        private Dictionary<string, ISettingItem> customSettings;

        // Jes1111
        // public int TemplateId;

        /// <summary>
        ///   The portal path.
        /// </summary>
        /// <remarks>
        ///   thierry (tiptopweb)
        ///   to have dropdown list for the themes and layout, we need the data path for the portal (for private theme and layout)
        ///   we need the portalPath here for this use and it has to be set from the current portalSettings before getting the
        ///   CustomSettings for a tab
        /// </remarks>
        private string portalPath;

        /// <summary>
        ///   The portal settings.
        /// </summary>
        private PortalSettings thePortalSettings;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets Page Settings For Search Engines
        /// </summary>
        /// <value>The custom settings.</value>
        public Dictionary<string, ISettingItem> CustomSettings
        {
            get
            {
                return this.customSettings ?? (this.customSettings = this.GetPageCustomSettings(this.PageID));
            }
        }

        /// <summary>
        ///   Gets or sets the name of the page.
        /// </summary>
        /// <value>The name of the page.</value>
        /// <remarks>
        /// </remarks>
        public string PageName { get; set; }

        /// <summary>
        ///   Gets or sets the portal path.
        /// </summary>
        /// <value>The portal path.</value>
        /// <remarks>
        /// </remarks>
        public string PortalPath
        {
            get
            {
                return this.portalPath;
            }

            set
            {
                this.portalPath = value;

                if (!this.portalPath.EndsWith("/"))
                {
                    this.portalPath += "/";
                }
            }
        }

        /// <summary>
        ///   Stores current portal settings
        /// </summary>
        /// <value>The portal settings.</value>
        public PortalSettings portalSettings
        {
            get
            {
                if (this.thePortalSettings == null)
                {
                    // Obtain PortalSettings from Current Context
                    if (HttpContext.Current != null)
                    {
                        this.thePortalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
                    }
                }

                return this.thePortalSettings;
            }

            set
            {
                this.thePortalSettings = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Read Current Page sub-tabs
        /// </summary>
        /// <param name="pageId">
        /// The page ID.
        /// </param>
        /// <returns>
        /// A SQL data reader.
        /// </returns>
        [Obsolete("Replace me and move to DAL")]
        public static SqlDataReader GetPageSettings(int pageId)
        {
            // Create Instance of Connection and Command Object
            using (var connection = Config.SqlConnectionString)
            using (var command = new SqlCommand("rb_GetTabSettings", connection))
            {
                // Mark the Command as a SPROC
                command.CommandType = CommandType.StoredProcedure;

                // PageID passed type FIXED by Bill Anderson (reedtek)
                // see: http://sourceforge.net/tracker/index.php?func=detail&aid=813789&group_id=66837&atid=515929
                // Add Parameters to SPROC
                var parameterPageId = new SqlParameter("@TabID", SqlDbType.Int) { Value = pageId };
                command.Parameters.Add(parameterPageId);

                // The new parameter "PortalLanguage" has been added to sp rb_GetPageSettings  
                // Onur Esnaf
                var parameterPortalLanguage = new SqlParameter("@PortalLanguage", SqlDbType.NVarChar, 12)
                    {
                       Value = Thread.CurrentThread.CurrentUICulture.Name 
                    };
                command.Parameters.Add(parameterPortalLanguage);

                // Open the database connection and execute the command
                connection.Open();
                var dr = command.ExecuteReader(CommandBehavior.CloseConnection);
                return dr;
            }
        }

        /// <summary>
        /// Read Current Page sub-tabs
        /// </summary>
        /// <param name="pageId">
        /// The page ID.
        /// </param>
        /// <returns>
        /// A Pages Box
        /// </returns>
        public static Collection<PageStripDetails> GetPageSettingsPagesBox(int pageId)
        {
            // Create Instance of Connection and Command Object
            using (var connection = Config.SqlConnectionString)
            using (var command = new SqlCommand("rb_GetTabSettings", connection))
            {
                // Mark the Command as a SPROC
                command.CommandType = CommandType.StoredProcedure;

                // PageID passed type FIXED by Bill Anderson (reedtek)
                // see: http://sourceforge.net/tracker/index.php?func=detail&aid=813789&group_id=66837&atid=515929
                // Add Parameters to SPROC
                var parameterPageId = new SqlParameter("@PageID", SqlDbType.Int) { Value = pageId };
                command.Parameters.Add(parameterPageId);

                // The new parameter "PortalLanguage" has been added to sp rb_GetPageSettings  
                // Onur Esnaf
                var parameterPortalLanguage = new SqlParameter("@PortalLanguage", SqlDbType.NVarChar, 12)
                    {
                       Value = Thread.CurrentThread.CurrentUICulture.Name 
                    };
                command.Parameters.Add(parameterPortalLanguage);

                // Open the database connection and execute the command
                connection.Open();

                using (var result = command.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    var tabs = new Collection<PageStripDetails>();

                    try
                    {
                        while (result.Read())
                        {
                            var tabDetails = new PageStripDetails { PageID = (int)result["PageID"] };
                            var cts = new PageSettings().GetPageCustomSettings(tabDetails.PageID);
                            tabDetails.PageImage = cts["CustomMenuImage"].ToString();
                            tabDetails.ParentPageID = Int32.Parse("0" + result["ParentPageID"]);
                            tabDetails.PageName = (string)result["PageName"];
                            tabDetails.PageOrder = (int)result["PageOrder"];
                            tabDetails.AuthorizedRoles = (string)result["AuthorizedRoles"];
                            tabs.Add(tabDetails);
                        }
                    }
                    finally
                    {
                        // by Manu, fixed bug 807858
                        result.Close();
                    }

                    return tabs;
                }
            }
        }

        /// <summary>
        /// Update Page Custom Settings
        /// </summary>
        /// <param name="pageId">
        /// The page ID.
        /// </param>
        /// <param name="key">
        /// The setting key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void UpdatePageSettings(int pageId, string key, string value)
        {
            // Create Instance of Connection and Command Object
            using (var connection = Config.SqlConnectionString)
            using (var command = new SqlCommand("rb_UpdateTabCustomSettings", connection))
            {
                // Mark the Command as a SPROC
                command.CommandType = CommandType.StoredProcedure;

                // Add Parameters to SPROC            
                var parameterPageId = new SqlParameter("@TabID", SqlDbType.Int, 4) { Value = pageId };
                command.Parameters.Add(parameterPageId);
                var parameterKey = new SqlParameter("@SettingName", SqlDbType.NVarChar, 50) { Value = key };
                command.Parameters.Add(parameterKey);
                var parameterValue = new SqlParameter("@SettingValue", SqlDbType.NVarChar, 1500) { Value = value };
                command.Parameters.Add(parameterValue);
                connection.Open();

                try
                {
                    command.ExecuteNonQuery();
                }
                finally
                {
                    connection.Close();
                }
            }

            // Invalidate cache
            if (CurrentCache.Exists(Key.TabSettings(pageId)))
            {
                CurrentCache.Remove(Key.TabSettings(pageId));
            }

            // Clear URL builder elements
            HttpUrlBuilder.Clear(pageId);
        }

        /// <summary>
        /// The PageSettings.GetPageCustomSettings Method returns a hash table of
        ///   custom Page specific settings from the database. This method is
        ///   used by Portals to access misc Page settings.
        /// </summary>
        /// <param name="pageId">
        /// The page ID.
        /// </param>
        /// <returns>
        /// The hash table.
        /// </returns>
        public Dictionary<string, ISettingItem> GetPageCustomSettings(int pageId)
        {
            Dictionary<string, ISettingItem> baseSettings;

            if (CurrentCache.Exists(Key.TabSettings(pageId)))
            {
                baseSettings = (Dictionary<string, ISettingItem>)CurrentCache.Get(Key.TabSettings(pageId));
            }
            else
            {
                baseSettings = this.GetPageBaseSettings();

                // Get Settings for this Page from the database
                var settings = new Hashtable();

                // Create Instance of Connection and Command Object
                using (var connection = Config.SqlConnectionString)
                using (var command = new SqlCommand("rb_GetTabCustomSettings", connection))
                {
                    // Mark the Command as a SPROC
                    command.CommandType = CommandType.StoredProcedure;

                    // Add Parameters to SPROC
                    var parameterPageId = new SqlParameter("@TabID", SqlDbType.Int, 4) { Value = pageId };
                    command.Parameters.Add(parameterPageId);

                    // Execute the command
                    connection.Open();
                    var dr = command.ExecuteReader(CommandBehavior.CloseConnection);

                    try
                    {
                        while (dr.Read())
                        {
                            settings[dr["SettingName"].ToString()] = dr["SettingValue"].ToString();
                        }
                    }
                    finally
                    {
                        dr.Close(); // by Manu, fixed bug 807858
                        connection.Close();
                    }
                }

                // Thierry (Tiptopweb)
                // TODO : put back the cache in GetPageBaseSettings() and reset values not found in the database
                // REVIEW: This code is duplicated in portal settings.
                foreach (var key in
                    baseSettings.Keys.Where(key => settings[key] != null).Where(
                        key => settings[key].ToString().Length != 0))
                {
                    baseSettings[key].Value = settings[key];
                }

                CurrentCache.Insert(Key.TabSettings(pageId), baseSettings);
            }

            return baseSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the image menu.
        /// </summary>
        /// <returns>
        /// A System.Collections.Hashtable value...
        /// </returns>
        private Hashtable GetImageMenu()
        {
            Hashtable imageMenuFiles;

            if (!CurrentCache.Exists(Key.ImageMenuList(this.portalSettings.CurrentLayout)))
            {
                imageMenuFiles = new Hashtable { { "-Default-", string.Empty } };
                var layoutManager = new LayoutManager(this.PortalPath);

                var menuDirectory = Path.WebPathCombine(
                    layoutManager.PortalLayoutPath, this.portalSettings.CurrentLayout);
                if (Directory.Exists(menuDirectory))
                {
                    menuDirectory = Path.WebPathCombine(menuDirectory, "menuimages");
                }
                else
                {
                    menuDirectory = Path.WebPathCombine(
                        LayoutManager.Path, this.portalSettings.CurrentLayout, "menuimages");
                }

                if (Directory.Exists(menuDirectory))
                {
                    var menuImages = (new DirectoryInfo(menuDirectory)).GetFiles("*.gif");

                    foreach (var fi in menuImages.Where(fi => fi.Name != "spacer.gif" && fi.Name != "icon_arrow.gif"))
                    {
                        imageMenuFiles.Add(fi.Name, fi.Name);
                    }
                }

                CurrentCache.Insert(Key.ImageMenuList(this.portalSettings.CurrentLayout), imageMenuFiles, null);
            }
            else
            {
                imageMenuFiles = (Hashtable)CurrentCache.Get(Key.ImageMenuList(this.portalSettings.CurrentLayout));
            }

            return imageMenuFiles;
        }

        /// <summary>
        /// Changed by Thierry@tiptopweb.com.au
        ///   Page are different for custom page layout an theme, this cannot be static
        ///   Added by john.mandia@whitelightsolutions.com
        ///   Cache by Manu
        ///   non static function, Thierry : this is necessary for page custom layout and themes
        /// </summary>
        /// <returns>
        /// A System.Collections.Hashtable value...
        /// </returns>
        private Dictionary<string, ISettingItem> GetPageBaseSettings()
        {
            // Define base settings
            var baseSettings = new Dictionary<string, ISettingItem>();

            // 2_aug_2004 Cory Isakson
            var groupOrderBase = (int)SettingItemGroup.NAVIGATION_SETTINGS;
            var group = SettingItemGroup.NAVIGATION_SETTINGS;

            var tabPlaceholder = new SettingItem<bool, CheckBox>(new BooleanDataType())
                {
                    Group = group, 
                    Order = groupOrderBase, 
                    Value = false, 
                    EnglishName = "Act as a Placeholder?", 
                    Description = "Allows this tab to act as a navigation placeholder only."
                };
            baseSettings.Add("TabPlaceholder", tabPlaceholder);

            var tabLink = new SettingItem<string, TextBox>(new StringDataType())
                {
                    Group = group, 
                    Value = string.Empty, 
                    Order = groupOrderBase + 1, 
                    EnglishName = "Static Link URL", 
                    Description = "Allows this tab to act as a navigation link to any URL."
                };
            baseSettings.Add("TabLink", tabLink);

            var tabUrlKeyword = new SettingItem<string, TextBox>(new StringDataType())
                {
                    Group = group, 
                    Order = groupOrderBase + 2, 
                    EnglishName = "URL Keyword", 
                    Description = "Allows you to specify a keyword that would appear in your URL."
                };
            baseSettings.Add("TabUrlKeyword", tabUrlKeyword);

            var urlPageName = new SettingItem<string, TextBox>(new StringDataType())
                {
                    Group = group, 
                    Order = groupOrderBase + 3, 
                    EnglishName = "URL Page Name", 
                    Description =
                        "This setting allows you to specify a name for this tab that will show up in the URL instead of default.aspx"
                };
            baseSettings.Add("UrlPageName", urlPageName);

            // groupOrderBase = (int)SettingItemGroup.META_SETTINGS;
            group = SettingItemGroup.META_SETTINGS;
            var tabTitle = new SettingItem<string, TextBox>(new StringDataType())
                {
                    Group = group, 
                    EnglishName = "Tab / Page Title", 
                    Description =
                        "Allows you to enter a title (Shows at the top of your browser) for this specific Tab / Page. Enter something here to override the default portal wide setting."
                };
            baseSettings.Add("TabTitle", tabTitle);

            var tabMetaKeyWords = new SettingItem<string, TextBox>(new StringDataType())
                {
                    Group = group, 
                    EnglishName = "Tab / Page Keywords", 
                    Description =
                        "This setting is to help with search engine optimization. Enter 1-15 Default Keywords that represent what this Tab / Page is about.Enter something here to override the default portal wide setting."
                };
            baseSettings.Add("TabMetaKeyWords", tabMetaKeyWords);
            var tabMetaDescription = new SettingItem<string, TextBox>(new StringDataType())
                {
                    Group = group, 
                    EnglishName = "Tab / Page Description", 
                    Description =
                        "This setting is to help with search engine optimization. Enter a description (Not too long though. 1 paragraph is enough) that describes this particular Tab / Page. Enter something here to override the default portal wide setting."
                };
            baseSettings.Add("TabMetaDescription", tabMetaDescription);
            var tabMetaEncoding = new SettingItem<string, TextBox>(new StringDataType())
                {
                    Group = group, 
                    EnglishName = "Tab / Page Encoding", 
                    Description =
                        "Every time your browser returns a page it looks to see what format it is retrieving. This allows you to specify the content type for this particular Tab / Page. Enter something here to override the default portal wide setting."
                };
            baseSettings.Add("TabMetaEncoding", tabMetaEncoding);
            var tabMetaOther = new SettingItem<string, TextBox>(new StringDataType())
                {
                    Group = group, 
                    EnglishName = "Additional Meta Tag Entries", 
                    Description =
                        "This setting allows you to enter new tags into this Tab / Page's HEAD Tag. Enter something here to override the default portal wide setting."
                };
            baseSettings.Add("TabMetaOther", tabMetaOther);
            var tabKeyPhrase = new SettingItem<string, TextBox>(new StringDataType())
                {
                    Group = group, 
                    EnglishName = "Tab / Page Keyphrase", 
                    Description =
                        "This setting can be used by a module or by a control. It allows you to define a message/phrase for this particular Tab / Page This can be used for search engine optimisation. Enter something here to override the default portal wide setting."
                };
            baseSettings.Add("TabKeyPhrase", tabKeyPhrase);

            

            // changed Thierry (Tiptopweb) : have a dropdown menu to select layout and themes
            groupOrderBase = (int)SettingItemGroup.THEME_LAYOUT_SETTINGS;
            group = SettingItemGroup.THEME_LAYOUT_SETTINGS;

            // get the list of available layouts
            // changed: Jes1111 - 2004-08-06
            var layoutsList = new ArrayList(new LayoutManager(this.portalSettings.PortalPath).GetLayouts());
            var noCustomLayout = new LayoutItem { Name = string.Empty };
            layoutsList.Insert(0, noCustomLayout);

            // get the list of available themes
            // changed: Jes1111 - 2004-08-06
            var themesList = new ArrayList(new ThemeManager(this.portalSettings.PortalPath).GetThemes());
            var noCustomTheme = new ThemeItem { Name = string.Empty };
            themesList.Insert(0, noCustomTheme);

            // changed: Jes1111 - 2004-08-06
            var customLayout = new SettingItem<string, ListControl>(new CustomListDataType(layoutsList, "Name", "Name"))
                {
                    Group = group, 
                    Order = groupOrderBase + 11, 
                    EnglishName = "Custom Layout", 
                    Description = "Set a custom layout for this tab only"
                };
            baseSettings.Add("CustomLayout", customLayout);

            // SettingItem CustomTheme = new SettingItem<string, TextBox>(new StringDataType());
            // changed: Jes1111 - 2004-08-06
            var customTheme = new SettingItem<string, ListControl>(new CustomListDataType(themesList, "Name", "Name"))
                {
                    Group = group, 
                    Order = groupOrderBase + 12, 
                    EnglishName = "Custom Theme", 
                    Description = "Set a custom theme for the modules in this tab only"
                };
            baseSettings.Add("CustomTheme", customTheme);

            // SettingItem CustomThemeAlt = new SettingItem<string, TextBox>(new StringDataType());
            // changed: Jes1111 - 2004-08-06
            var customThemeAlt = new SettingItem<string, ListControl>(
                new CustomListDataType(themesList, "Name", "Name"))
                {
                    Group = group, 
                    Order = groupOrderBase + 13, 
                    EnglishName = "Custom Alt Theme", 
                    Description = "Set a custom alternate theme for the modules in this tab only"
                };
            baseSettings.Add("CustomThemeAlt", customThemeAlt);

            var customMenuImage =
                new SettingItem<string, ListControl>(new CustomListDataType(this.GetImageMenu(), "Key", "Value"))
                    {
                        Group = group, 
                        Order = groupOrderBase + 14, 
                        EnglishName = "Custom Image Menu", 
                        Description = "Set a custom menu image for this tab"
                    };
            baseSettings.Add("CustomMenuImage", customMenuImage);

            

            #region Language/Culture Management

            groupOrderBase = (int)SettingItemGroup.CULTURE_SETTINGS;
            group = SettingItemGroup.CULTURE_SETTINGS;
            var cultureList = LanguageSwitcher.GetLanguageList(true);

            // Localized tab title
            var counter = groupOrderBase + 11;

            // Ignore invariant
            foreach (
                var c in cultureList.Where(c => c != CultureInfo.InvariantCulture && !baseSettings.ContainsKey(c.Name)))
            {
                var localizedTabKeyPhrase = new SettingItem<string, TextBox>(new StringDataType())
                    {
                        Order = counter, 
                        Group = group, 
                        EnglishName = string.Format("Tab Key Phrase ({0})", c.Name), 
                        Description = string.Format("Key Phrase this Tab/Page for {0} culture.", c.EnglishName)
                    };
                baseSettings.Add(string.Format("TabKeyPhrase_{0}", c.Name), localizedTabKeyPhrase);
                var localizedTitle = new SettingItem<string, TextBox>(new StringDataType())
                    {
                        Order = counter, 
                        Group = group, 
                        EnglishName = string.Format("Title ({0})", c.Name), 
                        Description = string.Format("Set title for {0} culture.", c.EnglishName)
                    };
                baseSettings.Add(c.Name, localizedTitle);
                counter++;
            }

            #endregion

            return baseSettings;
        }

        #endregion
    }
}