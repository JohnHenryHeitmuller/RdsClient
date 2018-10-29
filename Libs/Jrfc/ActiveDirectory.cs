using System;
//using System.Windows.Forms;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Security.Principal;
using System.IO;
using System.Net.NetworkInformation;
using Jrfc;

namespace Jrfc
{
    public class ActiveDirectory
    {
        public ActiveDirectory(string _Domain)
        {
            this.Domain = _Domain;
        }

        private string m_Domain;
        public string Domain
        {
            get
            {
                return this.m_Domain;
            }
            set
            {
                this.m_Domain = value;
                this.m_LdapPath = this.GetLdapPathFromDomain(value);
            }
        }
        private string m_LdapPath;
        public string LdapPath
        {
            get
            {
                return this.m_LdapPath;
            }
            set
            {
                this.m_LdapPath = value;
            }
        }

        public string GetLdapPathFromDomain(string _Domain)
        {
            if (string.IsNullOrWhiteSpace(_Domain))
                return "";

            string ldap = @"LDAP://";
            string[] tokens = _Domain.Split('.');
            for(int i=0; i < tokens.Count(); i++)
            {
                if (i != 0)
                    ldap += ",";
                ldap += "DC=" + tokens[i];
            }

            return ldap;
        }
        public static String GetWindowsUserName()
        {
            String strFullName = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
            String[] strName = strFullName.Split('\\');
            return strName[1];
        }

        public string ConvertExchangeAddressToSmtp(String exchAddress)
        {
            String strout = exchAddress; // if lookup fails return the exchange address that was passed in.

            DirectoryEntry de = new DirectoryEntry();
            de.Path = this.LdapPath;
            de.AuthenticationType = AuthenticationTypes.Secure;
            try
            {
                DirectorySearcher search = new DirectorySearcher(de);
                search.Filter = "(legacyExchangeDN=" + exchAddress + ")";
                search.PropertiesToLoad.Add("mail");
                SearchResult result = search.FindOne();
                if (result != null)
                {
                    strout = result.Properties["mail"][0].ToString();
                }
            }
            catch (System.Exception x)
            {
                // Environment.NewLine + "Error looking up e-mail address of (" + exchAddress + ").";
                Jrfc.Exception.HandleException(x);
            }
            return strout;
        }
        
        public string GetEmailAddressOfCurrentUser()
        {
            return ADGetUserProperty(Jrfc.ActiveDirectory.GetWindowsUserName(), "mail");
        }
        public string ADGetUserProperty(String strUser, String strProperty)
        {
            DirectoryEntry de = new DirectoryEntry();
            de.Path = LdapPath;
            de.AuthenticationType = AuthenticationTypes.Secure;
            try
            {
                DirectorySearcher search = new DirectorySearcher(de);
                search.Filter = "(SAMAccountName=" + strUser + ")";
                search.PropertiesToLoad.Add(strProperty);

                SearchResult result = search.FindOne();

                if (result != null)
                {
                    if (result.Properties[strProperty][0] is byte[])
                    {
                        var sidInBytes = (byte[])result.Properties[strProperty][0];
                        var sid = new SecurityIdentifier(sidInBytes, 0);
                        // This gives you what you want
                        return sid.ToString();
                    }
                    else
                    {
                        return result.Properties[strProperty][0].ToString();
                    }
                }
                else
                {
                    return "Unknown User";
                }
            }
            catch (System.Exception x)
            {
                Jrfc.Exception.HandleException(x);
            }
            return string.Empty;
        }

        // HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\FileAssociations\UserSid
        public static string GetCurrentUserSidFromAD_AsString()
        {
            string sid_string = string.Empty;
            try
            {
                string domain_name = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;

                Jrfc.ActiveDirectory ad = new ActiveDirectory(domain_name);
                string username = Jrfc.ActiveDirectory.GetWindowsUserName();
                sid_string = ad.ADGetUserProperty(username, "objectSid");

            }
            catch(System.Exception x)
            {
                Jrfc.Exception.HandleException(x);
            }

            return sid_string;
        }

        public static bool IsClientPcInDomain()
        {
            NativeMethods.NetJoinStatus status = NativeMethods.NetJoinStatus.NetSetupUnknownStatus;
            IntPtr pDomain = IntPtr.Zero;
            int result = NativeMethods.NetGetJoinInformation(null, out pDomain, out status);
            if (pDomain != IntPtr.Zero)
            {
                NativeMethods.NetApiBufferFree(pDomain);
            }
            if (result == 0)
            {
                return status == NativeMethods.NetJoinStatus.NetSetupDomainName;
            }
            else
            {
                throw new System.Exception("Call to Jrfc.Dll-->ActiveDirectory.cs-->IsInDomain() failed. Call to WinApi.NetGetJoinInformation() return (" + result.ToString() + ")");
            }
        }

        public Boolean IsCurrentUserInGroup(String strGroupName)
        {
            Boolean rtn = false;
            String strQualifiedGroupName = Environment.UserDomainName + "\\" + strGroupName;

            //System.Security.Principal.WindowsPrincipal s = new System.Security.Principal.WindowsPrincipal(System.Security.Principal.WindowsIdentity.GetCurrent());

            try
            {
                PrincipalContext ctx = new PrincipalContext(ContextType.Domain, this.Domain);
                GroupPrincipal grp = GroupPrincipal.FindByIdentity(ctx, IdentityType.Name, strGroupName);

                string strUserName = GetWindowsUserName();
                if (grp != null)
                {
                    foreach (Principal p in grp.GetMembers(true))
                    {
                        if (p.SamAccountName.ToUpper() == strUserName.ToUpper())
                        {
                            rtn = true;
                            break;
                        }
                    }
                    grp.Dispose();
                    ctx.Dispose();
                }
                else
                {
                    MessageBox.Show("The group [" + strGroupName + "] was not found in the Active Direcotry.", "IsCurrentUserInGroup(): Error Connecting to Active Directory");
                }
            }
            catch (System.Exception x)
            {
                Jrfc.Exception.HandleException(x);
            }
            return (rtn);
        }

        public void ADGetUsersInGroup(string strGroup, List<string> listUserNames)
        {
            try
            {
                PrincipalContext ctx = new PrincipalContext(ContextType.Domain, this.Domain);
                GroupPrincipal grp = GroupPrincipal.FindByIdentity(ctx, IdentityType.Name, strGroup);

                if (grp != null)
                {
                    foreach (Principal p in grp.GetMembers(true))
                    {
                        listUserNames.Add(p.SamAccountName);
                    }
                    grp.Dispose();
                    ctx.Dispose();
                }
                else
                {
                    MessageBox.Show("The group [" + strGroup + "] was not found in the Active Direcotry.", "ADGetUsersInGroup(): Error Connecting to Active Directory");
                }
            }
            catch (System.Exception x)
            {
                Jrfc.Exception.HandleException(x);
            }
        }

        public string ConvertDomainUserNameToPathSyntaxSafe(string windowsUserName)
        {
            // Converts windows domain user names from DOMAIN\USER_NAME to DOMAIN.USER_NAME.
            // The returned user name may safely be used has a folder or file name.
            string pathSafeUserName = windowsUserName.Replace(Path.DirectorySeparatorChar, '.');

            return pathSafeUserName;
        }
    }
}
