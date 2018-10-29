using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Shell32;

namespace Jrfc
{
    public class Shell
    {
        public enum PIN_DESTINATION
        {
            Taskbar,
            Start
        }
        public static void Pin(string _SourceObj, PIN_DESTINATION _PinTo, string _IconFileName = null)
        {   //
            //  Current version assumes _SourceObj is an RDS RemoteApp shortcut
            //  C:\Users\John\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\misrds.jrfcorp.net (RADC)\Funds Database (misrds.jrfcorp.net).lnk
            try
            {
                if (_IconFileName == null)
                    _IconFileName = String.Empty;
                string appdata_folder = Jrfc.Shell.GetUserAppdataFolder();
                string source_file_wo_ext = Path.GetFileNameWithoutExtension(_SourceObj);
                string source_file_ext = Path.GetExtension(_SourceObj);
                string stubexe_filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TaskBarStub.exe");
                string stubnew_filename = source_file_wo_ext + ".exe";
                string stubnew_filepath = Path.Combine(appdata_folder, stubnew_filename);
                string stubini_filename = Path.Combine(appdata_folder, source_file_wo_ext + ".ini");
                string stubico_filename = Path.Combine(appdata_folder, source_file_wo_ext + ".ico");
                string PinTo10_exepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PinTo10", "PinTo10.exe");

                //// Create the source
                //BitmapImage img = new BitmapImage();
                //img.BeginInit();
                //img.UriSource = new Uri(_IconFileName);
                //img.EndInit();
                //img.
                //BitmapSource bs = CreateBitmapSource(Colors.Green);
                
                
                /* Write (renamed) TaskbarStub.ini
                */
                List<string> ini_lines = new List<string>();
                ini_lines.Add("LaunchFilename=" + _SourceObj);
                ini_lines.Add("IconFilename=" + _IconFileName);
                System.IO.File.WriteAllLines(stubini_filename, ini_lines.ToArray());

                /* Create new renamed TaskbarStub.exe, then inkove to replace icon
                */
                File.Copy(stubexe_filepath, stubnew_filepath, true); // true = overwrite

                if (!string.IsNullOrWhiteSpace(_IconFileName))
                {
                    if (File.Exists(_IconFileName))
                    {
                        IconChanger ic = new IconChanger();
                        ic.ChangeIcon(stubnew_filepath, _IconFileName);
                    }
                }

                /*  Pin to Taskbar
                */
                ProcessStartInfo psi2 = new ProcessStartInfo(PinTo10_exepath);
                if(_PinTo == PIN_DESTINATION.Taskbar)
                {
                    psi2.Arguments = @"/PTFOL01:'" + appdata_folder + "' /PTFILE01:'" + stubnew_filename + "'";
                }
                else if(_PinTo == PIN_DESTINATION.Start)
                {
                    psi2.Arguments = @"/PSFOL01:'" + appdata_folder + "' /PSFILE01:'" + stubnew_filename + "'";
                }
                psi2.WindowStyle = ProcessWindowStyle.Hidden;
                psi2.CreateNoWindow = true;
                Process.Start(psi2);
            }
            catch (System.Exception x)
            {
                throw x;
            }

        }

        public static void Unpin(string _SourceObj, PIN_DESTINATION _PinTo)
        {
            // Known bug: Incomplete
        }

        public static bool IsPinned(string _SourceObj, PIN_DESTINATION _PinTo)
        {
            // Known bug: Incomplete

            return false;
        }

        public static string GetUserAppdataFolder()
        {
            string app_name = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            string appdata_folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            appdata_folder = Path.Combine(appdata_folder, app_name);

            if (!Directory.Exists(appdata_folder))
            {
                Directory.CreateDirectory(appdata_folder);
            }
            return appdata_folder;
        }

        public static string GetUserLocalAppdataFolder()
        {
            string app_name = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            string appdata_folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            appdata_folder = Path.Combine(appdata_folder, app_name);

            if (!Directory.Exists(appdata_folder))
            {
                Directory.CreateDirectory(appdata_folder);
            }
            return appdata_folder;
        }

        private static BitmapSource CreateBitmapSource(System.Windows.Media.Color color)
        {
            int width = 5;
            int height = 5;
            int stride = width / 8;
            byte[] pixels = new byte[height * stride];

            List<System.Windows.Media.Color> colors = new List<System.Windows.Media.Color>();
            colors.Add(color);
            BitmapPalette myPalette = new BitmapPalette(colors);

            BitmapSource image = BitmapSource.Create(
                width,
                height,
                96,
                96,
                PixelFormats.Indexed1,
                myPalette,
                pixels,
                stride);

            return image;
        }

        public static string GetPathFromPIDL(byte[] byteCode)
        {
            //MAX_PATH = 260
            StringBuilder builder = new StringBuilder(260);

            IntPtr ptr = IntPtr.Zero;
            GCHandle h0 = GCHandle.Alloc(byteCode, GCHandleType.Pinned);
            try
            {
                ptr = h0.AddrOfPinnedObject();
            }
            finally
            {
                h0.Free();
            }

            NativeMethods.SHGetPathFromIDListW(ptr, builder);

            return builder.ToString();
        }

        public static string GetWindowsShortcutTargetFileArguments(string shortcutFilename)
        {
            string pathOnly = System.IO.Path.GetDirectoryName(shortcutFilename);
            string filenameOnly = System.IO.Path.GetFileName(shortcutFilename);

            Shell32.Shell shell = new Shell32.Shell();
            Folder folder = shell.NameSpace(pathOnly);
            FolderItem folderItem = folder.ParseName(filenameOnly);
            if (folderItem != null)
            {
                Shell32.ShellLinkObject link = (Shell32.ShellLinkObject)folderItem.GetLink;
                string args = link.Arguments.Trim('"');
                return args;
            }

            return string.Empty;
        }
        public static string GetWindowsShortcutTargetFilePath(string shortcutFilename)
        {
            string pathOnly = System.IO.Path.GetDirectoryName(shortcutFilename);
            string filenameOnly = System.IO.Path.GetFileName(shortcutFilename);

            Shell32.Shell shell = new Shell32.Shell();
            Folder folder = shell.NameSpace(pathOnly);
            FolderItem folderItem = folder.ParseName(filenameOnly);
            if (folderItem != null)
            {
                Shell32.ShellLinkObject link = (Shell32.ShellLinkObject)folderItem.GetLink;
                string Path = link.Path.Trim('"');
                return Path;
            }

            return string.Empty;
        }

        public static string GetWindowsShortcutTargetFilePath(string shortcutFilename, out string arguments)
        {
            arguments = string.Empty;
            string pathOnly = System.IO.Path.GetDirectoryName(shortcutFilename);
            string filenameOnly = System.IO.Path.GetFileName(shortcutFilename);

            Shell32.Shell shell = new Shell32.Shell();
            Folder folder = shell.NameSpace(pathOnly);
            FolderItem folderItem = folder.ParseName(filenameOnly);
            if (folderItem != null)
            {
                Shell32.ShellLinkObject link = (Shell32.ShellLinkObject)folderItem.GetLink;
                arguments = link.Arguments.Trim('"');
                string Path = link.Path.Trim('"');
                return Path;
            }

            return string.Empty;
        }

        public enum ShortcutType
        {
            WindowsShortcut,
            InternetShortcut,
            NotAShortcut
        }

        public static ShortcutType GetShortcutType(string _ShortcutFile)
        {
            string ext = Path.GetExtension(_ShortcutFile);
            if (string.IsNullOrEmpty(ext))
                return ShortcutType.NotAShortcut;
            ext = ext.Trim('.').ToLower();
            if (ext == "lnk")
                return ShortcutType.WindowsShortcut;
            if (ext == "url")
                return ShortcutType.InternetShortcut;

            return ShortcutType.NotAShortcut;
        }
        
        public static string[] GetRdpFileParameter_remoteapplicationfileextensions(string _RdpFile)
        {
            string[] rtn_params = GetRdpFileParameterValue(_RdpFile, "remoteapplicationfileextensions");

            if (rtn_params == null)
                return null;

            for(int i=0; i < rtn_params.Count(); i ++) // strip leading '.' period from extension names
            {
                if(rtn_params[i].Length > 0)
                {
                    if (rtn_params[i][0] == '.')
                    {
                        rtn_params[i] = rtn_params[i].Remove(0, 1);
                    }
                }
            }

            return rtn_params;
        }

        public static string[] GetRdpFileParameterValue(string _RdpFile, string _Parameter)
        {
            string parm_lc = _Parameter.Trim().ToLower();
            string[] rtn_params = null;
            try
            {
                string[] lines = File.ReadAllLines(_RdpFile);
                foreach(string line in lines)
                {
                    string[] tokens = line.Split(':');
                    if (tokens == null)
                        break;
                    if (string.IsNullOrWhiteSpace(tokens[0]))
                        break;
                    if (tokens.Count() < 2)
                        break;
                    if(tokens[0].Trim().ToLower() == parm_lc)
                    {
                        int val_ndx = 1;
                        if(tokens[1].Trim().ToLower() == "s")
                        {
                            if (tokens.Count() < 3)
                                break;
                            val_ndx = 2;
                        }
                        if(tokens[val_ndx].Contains(','))
                        {   // Assume multiple csv parameters
                            rtn_params = tokens[val_ndx].Split(',');
                            return rtn_params;
                        }
                        else if(tokens[val_ndx].Contains(';'))
                        {   // Assume multiple parameters delimited by ';'
                            rtn_params = tokens[val_ndx].Split(';');
                            return rtn_params;
                        }
                        else
                        {
                            int array_size = tokens.Count() - val_ndx;
                            if (array_size <= 0)
                                break;
                            rtn_params = new string[array_size];
                            for(int i=0; i < array_size; i++ )
                            {
                                rtn_params[i] = tokens[i + val_ndx];
                            }
                            return rtn_params;
                        }
                    }
                }
            }
            catch(System.Exception x)
            {
                Jrfc.Exception.HandleException(x);
            }
            return null;
        }

        public static string GetInternetShortcut(string filePath)
        {
            string url = "";

            using (TextReader reader = new StreamReader(filePath))
            {
                string line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("URL="))
                    {
                        string[] splitLine = line.Split('=');
                        if (splitLine.Length > 0)
                        {
                            url = splitLine[1];
                            break;
                        }
                    }
                }
            }

            return url;
        }

    }
}
