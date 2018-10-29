using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Security.Principal;

namespace Jrfc
{
    public class Mru : IEquatable<Mru>, IComparable<Mru>
    {
        public string Hostname { get; set; }
        public string Path { get; set; }
        public string Filename { get; set; }
        public string DisplayName { get; set; }
        public int IconIndex { get; set; }
        public string IconFile { get; set; }
        public Jrfc.Shell.ShortcutType MruShortcutType { get; set; }
        public Mru()
        {
        }
        public Mru(string _Hostname, string _Path, string _Filename, Jrfc.Shell.ShortcutType _ShortcutType = Shell.ShortcutType.WindowsShortcut)
        {
            this.Hostname = _Hostname;
            this.Path = _Path;
            this.Filename = _Filename;
            this.MruShortcutType = _ShortcutType;

            if (!string.IsNullOrWhiteSpace(this.Filename))
            {
                this.DisplayName = this.Filename;
            }
            else
            {
                this.DisplayName = this.Path;
            }
            if(this.MruShortcutType == Jrfc.Shell.ShortcutType.InternetShortcut)
            {
                this.DisplayName = this.DisplayName.Replace("%20", " "); 
            }
        }

        public string FlybyHintText
        {
            get
            {
                return "Hostname: " + this.Hostname + Environment.NewLine +
                       "Path: " + this.Path;
            }
        }

        public bool Equals(Mru other)
        {
            if (other == null) return false;
            return (this.DisplayName.Equals(other.DisplayName));
        }

        public int CompareTo(Mru other)
        {
            if (other == null)
                return 1;
            else
                return this.DisplayName.CompareTo(other.DisplayName);
        }
    }

    public class MruList : List<Mru>
    {
        public MruList()
        {
        }

        public MruList Add(MruList _SourceMruList)
        {
            foreach(Mru mru in _SourceMruList)
            {
                this.Add(mru);
            }
            return this;
        }

        //HKEY_USERS\S-1-5-21-854245398-1935655697-725345543-1131\Software\Microsoft\Office\16.0\Word\User MRU\ADAL_B4A7D7A61D9D29E8FF8070E2AAA3EAFA9B2553B99B4FEE385996F488D3080F11\File MRU
        //public MruList LoadRemoteAppMruList(string _Hostname, string _RegistryKeyString)
        //{
        //    RegistryKey Remote_HKEY_USERS = null;
        //    RegistryKey Remote_MruSubkey = null;

        //    try
        //    {
        //        this.Clear();
        //        Remote_HKEY_USERS = RegistryKey.OpenRemoteBaseKey(RegistryHive.Users, _Hostname);
        //        if (Remote_HKEY_USERS != null)
        //        {
        //            string user_sid = Jrfc.ActiveDirectory.GetCurrentUserSidFromAD_AsString();
        //            string subkey_string = user_sid + @"\Software\Microsoft\Office\16.0\Word\User MRU\ADAL_B4A7D7A61D9D29E8FF8070E2AAA3EAFA9B2553B99B4FEE385996F488D3080F11\File MRU";
        //            Remote_MruSubkey = Remote_HKEY_USERS.OpenSubKey(subkey_string);
        //            string[] mru_values = Remote_MruSubkey.GetValueNames();
        //        }
        //    }
        //    catch(System.Exception x)
        //    {
        //        Jrfc.Exception.HandleException(x);
        //    }
        //    finally
        //    {
        //        if (Remote_HKEY_USERS != null)
        //            Remote_HKEY_USERS.Close();
        //        if (Remote_MruSubkey != null)
        //            Remote_MruSubkey.Close();
        //    }

        //    return this;
        //}

        public static MruList CreateRemoteHostMruListByExt(string _Hostname, string _FileExtension)
        {
            // HKEY_USERS\S-1-5-21-854245398-1935655697-725345543-1131\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\OpenSavePidlMRU

            RegistryKey Remote_HKEY_USERS = null;
            RegistryKey Remote_MruSubkey = null;
            MruList Remote_MruList = null;

            try
            {
                Remote_HKEY_USERS = RegistryKey.OpenRemoteBaseKey(RegistryHive.Users, _Hostname);
                if (Remote_HKEY_USERS != null)
                {
                    string user_sid = Jrfc.ActiveDirectory.GetCurrentUserSidFromAD_AsString();
                    string subkey_string = user_sid + @"\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\OpenSavePidlMRU\" + _FileExtension;
                    Remote_MruSubkey = Remote_HKEY_USERS.OpenSubKey(subkey_string);
                    if (Remote_MruSubkey == null)
                        return null;

                    string[] mru_values = Remote_MruSubkey.GetValueNames();

                    if(mru_values != null)
                    {
                        if(mru_values.Count() > 0)
                        {
                            Remote_MruList = new MruList();

                            foreach (string mru_value in mru_values)
                            {
                                int mru_value_int;
                                if (int.TryParse(mru_value, out mru_value_int)) // only get values with names that are int 0, 1, 2, 3, etc. Ignore "MRUListEx"
                                {
                                    object reg_value = Remote_MruSubkey.GetValue(mru_value);
                                    if(reg_value is byte[])
                                    {
                                        string mru_path = Jrfc.Shell.GetPathFromPIDL((byte[])reg_value);
                                        if(!string.IsNullOrWhiteSpace(mru_path))
                                        {
                                            string mru_file = Path.GetFileName(mru_path);
                                            if (!string.IsNullOrWhiteSpace(mru_file))
                                            {
                                                Remote_MruList.Add(new Mru(_Hostname, mru_path, mru_file));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception x)
            {
                Jrfc.Exception.HandleException(x);
            }
            finally
            {
                if (Remote_HKEY_USERS != null)
                    Remote_HKEY_USERS.Close();
                if (Remote_MruSubkey != null)
                    Remote_MruSubkey.Close();
            }
            if (Remote_MruList != null)
            {
                return Remote_MruList; 
            }
            return null;
        }

        public int LoadRemoteHostMruListByExt(string _Hostname, string[] _FileExtensionList)
        {
            // To do: Modify CreateRemoteHostMruListByExt() to call this function

            return 0;
        }
        public static MruList CreateRemoteHostMruListByExtList(string _Hostname, string[] _FileExtensionList)
        {
            // HKEY_USERS\S-1-5-21-854245398-1935655697-725345543-1131\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\OpenSavePidlMRU

            RegistryKey Remote_HKEY_USERS = null;
            RegistryKey Remote_MruSubkey = null;
            MruList Remote_MruList = null;

            try
            {
                Remote_HKEY_USERS = RegistryKey.OpenRemoteBaseKey(RegistryHive.Users, _Hostname);
                if (Remote_HKEY_USERS != null)
                {
                    string user_sid = Jrfc.ActiveDirectory.GetCurrentUserSidFromAD_AsString();
                    Remote_MruList = new MruList();

                    foreach (string ext in _FileExtensionList)
                    {
                        Remote_MruList.Add(_Hostname, Remote_HKEY_USERS, user_sid, ext);
                    }
                    Remote_MruList.AddOfficeRecent(_Hostname, _FileExtensionList);
                    Remote_MruList.AddOfficeRecent("localhost", _FileExtensionList);
                    //Remote_MruList.AddCommonRecent(_Hostname, Remote_HKEY_USERS, user_sid, _FileExtensionList);
                }
            }
            catch (System.Exception x)
            {
                Jrfc.Exception.HandleException(x);
            }
            finally
            {
                if (Remote_HKEY_USERS != null)
                    Remote_HKEY_USERS.Close();
                if (Remote_MruSubkey != null)
                    Remote_MruSubkey.Close();
            }
            if (Remote_MruList != null)
            {
                Remote_MruList.Sort();
                return Remote_MruList;
            }
            return null;
        }
        public int LoadRemoteHostMruListByExtList(string _Hostname, string[] _FileExtensionList)
        {
            // To do: Modify CreateRemoteHostMruListByExtList() to call this function

            return 0; 
        }

        public void Add(string _Hostname, RegistryKey _Remote_HKEY_USERS, string _UserSid, string _FileExt)
        {
            RegistryKey Remote_MruSubkey = null;
            try
            {
                string subkey_string = _UserSid + @"\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\OpenSavePidlMRU\" + _FileExt;
                Remote_MruSubkey = _Remote_HKEY_USERS.OpenSubKey(subkey_string);
                if (Remote_MruSubkey != null)
                {
                    string[] mru_values = Remote_MruSubkey.GetValueNames();

                    if (mru_values != null)
                    {
                        foreach (string mru_value in mru_values)
                        {
                            int mru_value_int;
                            if (int.TryParse(mru_value, out mru_value_int)) // only get values with names that are int 0, 1, 2, 3, etc. Ignore "MRUListEx"
                            {
                                object reg_value = Remote_MruSubkey.GetValue(mru_value);
                                if (reg_value is byte[])
                                {
                                    string mru_path = Jrfc.Shell.GetPathFromPIDL((byte[])reg_value);
                                    if (!string.IsNullOrWhiteSpace(mru_path))
                                    {
                                        string mru_file = Path.GetFileName(mru_path);
                                        if (!string.IsNullOrWhiteSpace(mru_file))
                                        {
                                            this.Add(new Mru(_Hostname, mru_path, mru_file));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception x)
            {
                Jrfc.Exception.HandleException(x);
            }
            finally
            {
                if (Remote_MruSubkey != null)
                    Remote_MruSubkey.Close();
            }
            return;
        }
        public void AddOfficeRecent(string _Hostname, string[] _FileExtensionList)
        {
            string path = @"\\" + _Hostname + @"\c$\Users\" + Jrfc.ActiveDirectory.GetWindowsUserName() + @"\AppData\Roaming\Microsoft\Office\Recent";

            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] fi_list = di.GetFiles("*.*", SearchOption.TopDirectoryOnly);

            string mru_path;
            string mru_file;
            string mru_ext;
            foreach (FileInfo fi in fi_list)
            {
                if (fi.Name == "Templates")
                    continue;

                Jrfc.Shell.ShortcutType shortcut_type = Jrfc.Shell.GetShortcutType(fi.Name);

                if (shortcut_type == Jrfc.Shell.ShortcutType.WindowsShortcut)
                {
                    mru_path = Jrfc.Shell.GetWindowsShortcutTargetFilePath(fi.FullName);
                    if (string.IsNullOrWhiteSpace(mru_path))
                        continue;
                    if (_Hostname == "localhost")
                    {   // Do not include mru files from local drives. These files
                        // will not be visible to the RDS server.
                        DriveInfo drv = new DriveInfo(mru_path); 
                        if (drv.DriveType != DriveType.Network)
                            continue;
                    }
                    mru_file = Path.GetFileName(fi.Name);
                    if (!string.IsNullOrWhiteSpace(mru_file))
                        continue;

                    mru_ext = Path.GetExtension(mru_path);
                }
                else if (shortcut_type == Jrfc.Shell.ShortcutType.InternetShortcut)
                {
                    string url = Jrfc.Shell.GetInternetShortcut(fi.FullName);
                    if (string.IsNullOrWhiteSpace(url))
                        continue;

                    Uri uri = new Uri(url);
                    mru_file = Path.GetFileName(uri.AbsolutePath);
                    if (string.IsNullOrWhiteSpace(mru_file))
                        continue;

                    mru_ext = Path.GetExtension(mru_file);
                    mru_path = url;
                }
                else
                {
                    continue;
                }
                if (!string.IsNullOrWhiteSpace(mru_ext))
                {
                    mru_ext = mru_ext.Trim('.').ToLower();
                    if (_FileExtensionList.Contains(mru_ext))
                    {
                        this.Add(new Mru(_Hostname, mru_path, mru_file, shortcut_type));
                    }
                }
            }
        }

        public void AddCommonRecent(string _Hostname, RegistryKey _Remote_HKEY_USERS, string _UserSid, string[] _FileExt)
        {
            RegistryKey Remote_MruSubkey = null;
            try
            {
                string subkey_string = _UserSid + @"\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\OpenSavePidlMRU\*";
                Remote_MruSubkey = _Remote_HKEY_USERS.OpenSubKey(subkey_string);
                if (Remote_MruSubkey != null)
                {
                    string[] mru_values = Remote_MruSubkey.GetValueNames();

                    if (mru_values != null)
                    {
                        foreach (string mru_value in mru_values)
                        {
                            int mru_value_int;
                            if (!int.TryParse(mru_value, out mru_value_int)) // only get values with names that are int 0, 1, 2, 3, etc. Ignore "MRUListEx"
                                continue;

                            object reg_value = Remote_MruSubkey.GetValue(mru_value);
                            if (!(reg_value is byte[]))
                                continue;

                            string mru_path = Jrfc.Shell.GetPathFromPIDL((byte[])reg_value);
                            if (string.IsNullOrWhiteSpace(mru_path))
                                continue;

                            string mru_ext = Path.GetExtension(mru_path);
                            if (string.IsNullOrEmpty(mru_ext))
                                continue;

                            mru_ext = mru_ext.Remove(0,1).ToLower(); // remove leading period 
                            foreach (string ext in _FileExt)
                            {
                                if (mru_ext == ext)
                                {
                                    string mru_file = Path.GetFileName(mru_path);
                                    if (!string.IsNullOrWhiteSpace(mru_file))
                                    {
                                        this.Add(new Mru(_Hostname, mru_path, mru_file));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception x)
            {
                Jrfc.Exception.HandleException(x);
            }
            finally
            {
                if (Remote_MruSubkey != null)
                    Remote_MruSubkey.Close();
            }
            return;
        }


    }
}
