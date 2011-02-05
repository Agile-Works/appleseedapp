﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace AppleseedWebApplication.API
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IPortalServices" in both code and config file together.
    [ServiceContract]
    public interface IPortalServices
    {


        #region Users

        [OperationContract]
        void GetUsers();
        [OperationContract]
        void GetRoles();
        [OperationContract]
        void CreateUser();
        [OperationContract]
        void CreateRole();
        [OperationContract]
        void AddUserToRole();
        [OperationContract]
        void RemoveUserFromRole();
        [OperationContract]
        void AddUserToPortal();
        [OperationContract]
        void RemoveUserFromPortal();
        #endregion

        #region Pages

        [OperationContract]
        void GetPages();
        [OperationContract]
        void CreatePageInPortal();
        #endregion

        #region Modules

        [OperationContract]
        void GetModulesOnPage();
        [OperationContract]
        void GetAllInstalledModules();
        [OperationContract]
        void UnInstallModule();
        [OperationContract]
        void AddModuleToPage();
        [OperationContract]
        void UpdateModuleInfo();
        [OperationContract]
        void EditModuleData();
        //- For Html/Content modules only
        //InstallModule- future
        #endregion

        #region Portal

        [OperationContract]
        void GetPortals();
        [OperationContract]
        void CreatePortal();
        [OperationContract]
        void ChangePortalSettings();
        [OperationContract]
        void RemovePortal();
        #endregion
    }
}
