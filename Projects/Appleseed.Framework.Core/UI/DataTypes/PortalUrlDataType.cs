// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PortalUrlDataType.cs" company="--">
//   Copyright � -- 2010. All Rights Reserved.
// </copyright>
// <summary>
//   Portal URL Data Type
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.Framework.DataTypes
{
    using System.Web;

    using Appleseed.Framework.Settings;
    using Appleseed.Framework.Site.Configuration;

    /// <summary>
    /// Portal URL Data Type
    /// </summary>
    public class PortalUrlDataType : StringDataType
    {
        #region Constants and Fields

        /// <summary>
        /// The inner full path.
        /// </summary>
        private string innerFullPath;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "PortalUrlDataType" /> class.
        /// </summary>
        public PortalUrlDataType()
        {
            this.PortalPathPrefix = string.Empty;
            this.Type = PropertiesDataType.String;

            // InitializeComponents();
            if (HttpContext.Current.Items["PortalSettings"] == null)
            {
                return;
            }

            // Obtain PortalSettings from Current Context
            var portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
            this.PortalPathPrefix = portalSettings.PortalFullPath;
            if (!this.PortalPathPrefix.EndsWith("/"))
            {
                this.PortalPathPrefix += "/";
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PortalUrlDataType"/> class. 
        ///   Use this on portal setting or when you want turn off automatic discovery
        /// </summary>
        /// <param name="portalFullPath">
        /// The portal full path.
        /// </param>
        public PortalUrlDataType(string portalFullPath)
        {
            this.Type = PropertiesDataType.String;

            // InitializeComponents();
            this.PortalPathPrefix = portalFullPath;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the description.
        /// </summary>
        public override string Description
        {
            get
            {
                return "URL relative to Portal";
            }
        }

        /// <summary>
        ///   Gets the full path.
        /// </summary>
        public override string FullPath
        {
            get
            {
                if (this.innerFullPath == null)
                {
                    this.innerFullPath = Path.WebPathCombine(this.PortalPathPrefix, this.Value);

                    // Removes trailing /
                    this.innerFullPath = this.innerFullPath.TrimEnd('/');
                }

                return this.innerFullPath;
            }
        }

        /// <summary>
        ///   Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public override string Value
        {
            get
            {
                return this.Value;
            }

            set
            {
                // Remove portal path if present
                this.Value = value.StartsWith(this.PortalPathPrefix)
                                 ? value.Substring(this.PortalPathPrefix.Length)
                                 : value;

                // Reset innerFullPath
                this.innerFullPath = null;
            }
        }

        /// <summary>
        ///   Gets the portal path prefix.
        /// </summary>
        /// <value>The portal path prefix.</value>
        protected string PortalPathPrefix { get; private set; }

        #endregion
    }
}