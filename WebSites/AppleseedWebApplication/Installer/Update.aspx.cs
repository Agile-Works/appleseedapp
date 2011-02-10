﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Update.aspx.cs" company="--">
//   Copyright © -- 2010. All Rights Reserved.
// </copyright>
// <summary>
//   The update.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AppleseedWebApplication.Installer
{
    using System;
    using System.Web.UI;

    using Appleseed.Framework.Core.Update;
    using Appleseed.Framework.Settings;

    /// <summary>
    /// The update.
    /// </summary>
    public partial class Update : Page
    {
        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="T:System.EventArgs"/> object that contains the event data.
        /// </param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            using (var s = new Services())
            {
                s.RunDBUpdate(Config.ConnectionString);
            }

            this.Response.Redirect("~/Default.aspx");
        }

        #endregion
    }
}