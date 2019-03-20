//-----------------------------------------------------------------------
// <copyright file="ConfigurationHelper.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace GetAPILogsfromServers
{
    using Microsoft.CloudInfrastructure.SecurityHelper;
    using System.Security;
    using System.Threading.Tasks;

    /// <summary>
    /// Helps to work with configuration.
    /// </summary>
    public static class ConfigurationHelper
    {
        /// <summary>
        /// Key vault helper singlenton.
        /// </summary>
        private readonly static KeyVaultHelper keyvault;

        /// <summary>
        ///
        /// </summary>
        static ConfigurationHelper()
        {
            keyvault = new KeyVaultHelper();
        }

        /// <summary>
        /// Gets AX user name.
        /// </summary>
        public static string AxUserName
        {
            get
            {
                return Task.Run(async () =>
                        await keyvault.GetSecretValue("POLCName")).Result.ConvertToString();
            }
        }

        /// <summary>
        /// Gets AX user password.
        /// </summary>
        public static SecureString AxUserPassword
        {
            get
            {
                SecureString str= Task.Run(async () =>
                        await keyvault.GetSecretValue("POLCPassword")).Result;
                string password = new System.Net.NetworkCredential(string.Empty, str).Password;
                return str;
            }
            
            /*   get
            {
                return Task.Run(async () =>
                        await keyvault.GetSecretValue(CloudConfigurationManager.GetSetting("UserAccountPassword"))).Result.ConvertToString();
            } */
           
        }

        /// <summary>
        /// Gets AX user domain.
        /// </summary>
        public static string AxUserDomain
        {
            get
            {
                return Task.Run(async () =>
                        await keyvault.GetSecretValue("POLCDomain")).Result.ConvertToString();
            }
        }
    }
}
