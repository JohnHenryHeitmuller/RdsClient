using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using System.Xml.Serialization;

namespace Jrfc
{
    public static class Utility
    {
        public static int GetIndex(ItemCollection _c, string _header)
        {
            string header_lower = _header.ToLower();

            for(int i=0; i < _c.Count; i++)
            {
                if(((MenuItem)_c[i]).Header.ToString().ToLower() == header_lower)
                {
                    return i;
                }

            }
            return -1;
        }

        public static void SetMenuItemsVisibility(ContextMenu _Menu, string[] _ItemHeaders, System.Windows.Visibility _Visibility)
        {
            //int[] menu_indexes = new int[_ItemHeaders.Count()];
            for(int i=0; i < _ItemHeaders.Count(); i++)
            {
                int ndx = Jrfc.Utility.GetIndex(_Menu.Items, _ItemHeaders[i]);
                ((MenuItem)_Menu.Items[ndx]).Visibility = _Visibility;
            }
        }
        public enum TYPE_OF_ADD
        {
            SkipIfExists,
            ReplaceIfExists,
            AllowDuplicates
        }

        public enum ADD_LOCATION
        {
            Top,
            Bottom
        }
        public static void AddItemToContextMenu(ContextMenu _ContextMenu, MenuItem _MenuItem, 
                                                TYPE_OF_ADD _TypeOfAdd, 
                                                ADD_LOCATION _AddLocation = ADD_LOCATION.Bottom)
        {
            bool okay_to_add = false;
            if (_TypeOfAdd == TYPE_OF_ADD.AllowDuplicates)
            {
                okay_to_add = true;
            }
            else
            {
                bool found = false;
                foreach (Object obj in _ContextMenu.Items)
                {
                    if (obj is MenuItem)
                    {
                        if (((MenuItem)obj).Header.ToString() == _MenuItem.Header.ToString())
                        {
                            found = true;
                            if (_TypeOfAdd == TYPE_OF_ADD.ReplaceIfExists)
                            {
                                _MenuItem.Items.Remove((MenuItem)obj); // remove item that is to be replaced
                            }
                            break;
                        }
                    }
                }
                if(found == false ||
                    _TypeOfAdd == TYPE_OF_ADD.ReplaceIfExists ||
                    _TypeOfAdd != TYPE_OF_ADD.SkipIfExists)
                {
                    okay_to_add = true;
                }
            }
            if (okay_to_add == true)
            {
                if (_AddLocation == ADD_LOCATION.Bottom)
                {
                    _ContextMenu.Items.Add(_MenuItem);
                }
                else
                {
                    _ContextMenu.Items.Insert(0, _MenuItem);
                }
            }
        }

        public static TabItem FindTabByHeader(TabControl _TabControl, string _Header)
        {
            foreach(TabItem t in _TabControl.Items)
            {
                if(t.Header.ToString() == _Header)
                {
                    return t;
                }
            }
            return null;
        }

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            try
            {
                T parent = VisualTreeHelper.GetParent(child) as T;

                if (parent != null)
                    return parent;
                else
                    return FindParent<T>(VisualTreeHelper.GetParent(child));
            }
            catch(System.Exception x)
            {
                Jrfc.Exception.HandleException(x, System.Diagnostics.EventLogEntryType.Warning, Exception.DisplayMessage.No);
            }
            return null;
        }

        /// <summary>
        /// Analyzes both visual and logical tree in order to find all elements
        /// of a given type that are descendants of the <paramref name="source"/>
        /// item.
        /// </summary>
        /// <typeparam name="T">The type of the queried items.</typeparam>
        /// <param name="source">The root element that marks the source of the
        /// search. If the source is already of the requested type, it will not
        /// be included in the result.</param>
        /// <returns>All descendants of <paramref name="source"/> that match the
        /// requested type.</returns>
        public static IEnumerable<T> FindChildren<T>(this DependencyObject source)
                                                     where T : DependencyObject
        {
            if (source != null)
            {
                var childs = GetChildObjects(source);
                foreach (DependencyObject child in childs)
                {
                    //analyze if children match the requested type
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    //recurse tree
                    foreach (T descendant in FindChildren<T>(child))
                    {
                        yield return descendant;
                    }
                }
            }
        }


        /// <summary>
        /// This method is an alternative to WPF's
        /// <see cref="VisualTreeHelper.GetChild"/> method, which also
        /// supports content elements. Do note, that for content elements,
        /// this method falls back to the logical tree of the element.
        /// </summary>
        /// <param name="parent">The item to be processed.</param>
        /// <returns>The submitted item's child elements, if available.</returns>
        public static IEnumerable<DependencyObject> GetChildObjects(
                                                    this DependencyObject parent)
        {
            if (parent == null) yield break;


            if (parent is ContentElement || parent is FrameworkElement)
            {
                //use the logical tree for content / framework elements
                foreach (object obj in LogicalTreeHelper.GetChildren(parent))
                {
                    var depObj = obj as DependencyObject;
                    if (depObj != null) yield return (DependencyObject)obj;
                }
            }
            else
            {
                //use the visual tree per default
                int count = VisualTreeHelper.GetChildrenCount(parent);
                for (int i = 0; i < count; i++)
                {
                    yield return VisualTreeHelper.GetChild(parent, i);
                }
            }
        }        //public static T FindChild<T>(this DependencyObject depObj) where T : DependencyObject
        //{
        //    if (depObj == null)
        //        return null;

        //    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        //    {
        //        var child = VisualTreeHelper.GetChild(depObj, i);

        //        var result = (child as T) ?? FindChild<T>(child);
        //        if (result != null)
        //            return result;
        //    }
        //    return null;
        //}

        public static string SerializeObject<T>(this T toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());

            using (System.IO.StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }
        public static class ObjectSerializer<T>
        {
            // Serialize to xml  
            public static string ToXml(T value)
            {
                XmlRootAttribute xroot = new XmlRootAttribute(typeof(T).FullName);
                xroot.IsNullable = true;
                XmlSerializer serializer = new XmlSerializer(typeof(T), xroot);
                StringBuilder stringBuilder = new StringBuilder();
                XmlWriterSettings settings = new XmlWriterSettings()
                {
                    Indent = true,
                    OmitXmlDeclaration = true,
                };

                using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, settings))
                {
                    serializer.Serialize(xmlWriter, value);
                }
                return stringBuilder.ToString();
            }

            // Deserialize from xml  
            public static T FromXml(string xml)
            {
                XmlRootAttribute xroot = new XmlRootAttribute(typeof(T).FullName);
                xroot.IsNullable = true;
                XmlSerializer serializer = new XmlSerializer(typeof(T),xroot);
                T value;
                using (StringReader stringReader = new StringReader(xml))
                {
                    object deserialized = serializer.Deserialize(stringReader);
                    value = (T)deserialized;
                }

                return value;
            }
        }
        public static MemoryStream SerializeToMemoryStream(object o)
        {
            MemoryStream stream = new MemoryStream();
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, o);
            return stream;
        }

        public static object DeserializeFromMemoryStream(MemoryStream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            stream.Seek(0, SeekOrigin.Begin);
            object o = formatter.Deserialize(stream);
            return o;
        }
    }
}
