using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrfc.Settings
{
    public class AppSettingMetadata
    {
        public AppSettingMetadata(string _SettingName, string _SettingCategory, string _SettingDescription)
        {
            this.SettingName = _SettingName;
            this.SettingCategory = _SettingCategory;
            this.SettingDescription = _SettingDescription;
        }
        public string SettingName { get; set; }
        public string SettingCategory { get; set; }
        public string SettingDescription { get; set; }

    }

    public class AppSettingMetadataDictionary : Dictionary<string, AppSettingMetadata>
    {
        public void Add(string _SettingName, string _SettingCategory, string _SettingDescription)
        {
            AppSettingMetadata asmd = new AppSettingMetadata(_SettingName, _SettingCategory, _SettingDescription);
            this.Add(_SettingName, asmd);
        }

        public void ApplyAppSettingMetadataDictionary(System.Configuration.ApplicationSettingsBase _SettingsCollection)
        {
            PropertyOverridingTypeDescriptor ctd = new PropertyOverridingTypeDescriptor(TypeDescriptor.GetProvider(_SettingsCollection).GetTypeDescriptor(_SettingsCollection));


            // iterate through properies in the supplied object/type
            foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(_SettingsCollection))
            {
                // for every property that complies to our criteria

                AppSettingMetadata asmd;
                bool found = this.TryGetValue(pd.Name, out asmd);

                if (found)
                {
                    // we first construct the custom PropertyDescriptor with the TypeDescriptor's
                    // built-in capabilities
                    Attribute[] attribute_list = new Attribute[2];
                    attribute_list[0] = new CategoryAttribute(asmd.SettingCategory);
                    attribute_list[1] = new DescriptionAttribute(asmd.SettingDescription);


                    PropertyDescriptor pd2 =
                           TypeDescriptor.CreateProperty(
                               _SettingsCollection.GetType(), // or just _settings, if it's already a type
                               pd, // base property descriptor to which we want to add attributes
                                   // The PropertyDescriptor which we'll get will just wrap that
                                   // base one returning attributes we need.
                               attribute_list
                               //new CategoryAttribute(asmd.SettingCategory)
                           // this method really can take as many attributes as you like,
                           // not just one
                           );
                    
                    //PropertyDescriptor pd3 =
                    //       TypeDescriptor.CreateProperty(
                    //           _SettingsCollection.GetType(), // or just _settings, if it's already a type
                    //           pd, // base property descriptor to which we want to add attributes
                    //               // The PropertyDescriptor which we'll get will just wrap that
                    //               // base one returning attributes we need.
                    //           new DescriptionAttribute(asmd.SettingDescription)
                    //       // this method really can take as many attributes as you like,
                    //       // not just one
                    //       );

                    // and then we tell our new PropertyOverridingTypeDescriptor to override that property
                    ctd.OverrideProperty(pd2);
                    //ctd.OverrideProperty(pd3);
                }
            }

            // then we add new descriptor provider that will return our descriptor instead of default
            TypeDescriptor.AddProvider(new TypeDescriptorOverridingProvider(ctd), _SettingsCollection);
        }

    }

    public class PropertyOverridingTypeDescriptor : CustomTypeDescriptor
    {
        private readonly Dictionary<string, PropertyDescriptor> overridePds = new Dictionary<string, PropertyDescriptor>();

        public PropertyOverridingTypeDescriptor(ICustomTypeDescriptor parent)
            : base(parent)
        { }

        public void OverrideProperty(PropertyDescriptor pd)
        {
            overridePds[pd.Name] = pd;
        }

        public override object GetPropertyOwner(PropertyDescriptor pd)
        {
            object o = base.GetPropertyOwner(pd);

            if (o == null)
            {
                return this;
            }

            return o;
        }

        public PropertyDescriptorCollection GetPropertiesImpl(PropertyDescriptorCollection pdc)
        {
            List<PropertyDescriptor> pdl = new List<PropertyDescriptor>(pdc.Count + 1);

            foreach (PropertyDescriptor pd in pdc)
            {
                if (overridePds.ContainsKey(pd.Name))
                {
                    pdl.Add(overridePds[pd.Name]);
                }
                else
                {
                    pdl.Add(pd);
                }
            }

            PropertyDescriptorCollection ret = new PropertyDescriptorCollection(pdl.ToArray());

            return ret;
        }

        public override PropertyDescriptorCollection GetProperties()
        {
            return GetPropertiesImpl(base.GetProperties());
        }
        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetPropertiesImpl(base.GetProperties(attributes));
        }
    }

    public class TypeDescriptorOverridingProvider : TypeDescriptionProvider
    {
        private readonly ICustomTypeDescriptor ctd;

        public TypeDescriptorOverridingProvider(ICustomTypeDescriptor ctd)
        {
            this.ctd = ctd;
        }

        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return ctd;
        }
    }
}
