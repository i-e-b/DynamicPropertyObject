using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace DynamicPropertyObject
{
    public class DynTypeDescriptor : CustomTypeDescriptor
    {
        private readonly PropertyDescriptorCollection m_pdc = new PropertyDescriptorCollection(null, false);
        private readonly object m_instance;

        public DynTypeDescriptor(ICustomTypeDescriptor ctd, object instance) : base(ctd)
        {
            m_instance = instance;
        }

        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            if (m_pdc.Count == 0)
            {
                GetProperties();
            }

            PropertyDescriptorCollection pdcFilterd = new PropertyDescriptorCollection(null);
            foreach (PropertyDescriptor pd in m_pdc)
            {
                if (pd.Attributes.Contains(attributes))
                {
                    pdcFilterd.Add(pd);
                }
            }

            PreProcess(pdcFilterd);
            return pdcFilterd;
        }

        public override PropertyDescriptorCollection GetProperties()
        {
            if (m_pdc.Count == 0)
            {
                var pdc = base.GetProperties();
                foreach (PropertyDescriptor pd in pdc)
                {
                    if (!(pd is DynPropertyDescriptor))
                    {
                        DynPropertyDescriptor dynPd;
                        if (pd.PropertyType.IsEnum)
                        {
                            dynPd = new EnumPropertyDescriptor(pd);
                        }
                        else if (pd.PropertyType == typeof(bool))
                        {
                            dynPd = new BooleanPropertyDescriptor(pd);
                        }
                        else
                        {
                            dynPd = new DynPropertyDescriptor(pd);
                        }
                        m_pdc.Add(dynPd);
                    }
                    else
                    {
                        m_pdc.Add(pd);
                    }
                }
            }
            return m_pdc;
        }

        private void PreProcess(PropertyDescriptorCollection pdc)
        {
            if (pdc.Count <= 0) return;
            UpdateStringFromResource(pdc);

            var propSorter = new PropertySorter
            {
                CategorySortOrder = CategorySortOrder,
                PropertySortOrder = PropertySortOrder
            };
            var pdcSorted = pdc.Sort(propSorter);

            UpdateAppendCount(pdcSorted);

            pdc.Clear();
            foreach (PropertyDescriptor pd in pdcSorted) { pdc.Add(pd); }
        }

        private void UpdateAppendCount(PropertyDescriptorCollection pdc)
        {
            if (CategorySortOrder == SortOrder.None)
            {
                return;
            }
            int nTabCount = 0;
            if (CategorySortOrder == SortOrder.ByNameAscending || CategorySortOrder == SortOrder.ByNameDescending)
            {
                string sCatName = null;

                // iterate from last to first
                for (int i = pdc.Count - 1; i >= 0; i--)
                {
                    var pd = pdc[i] as DynPropertyDescriptor;
                    if (sCatName == null )
                    {
                        if (pd != null)
                        {
                            sCatName = pd.Category;
                            pd.AppendCount = nTabCount;
                        }
                    }
                    else if (pd != null && string.Compare(pd.Category, sCatName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        pd.AppendCount = nTabCount;
                    }
                    else
                    {
                        nTabCount++;
                        sCatName = pdc[i].Category;
                        if (pd != null) pd.AppendCount = nTabCount;
                    }
                }
            }
            else
            {
                int? nCatID = null;

                // iterate from last to first
                for (int i = pdc.Count - 1; i >= 0; i--)
                {
                    var pd = pdc[i] as DynPropertyDescriptor;
                    if (nCatID == null)
                    {
                        if (pd != null)
                        {
                            nCatID = pd.CategoryId;
                            pd.AppendCount = nTabCount;
                        }
                    }
                    if (pd != null && pd.CategoryId == nCatID)
                    {
                        pd.AppendCount = nTabCount;
                    }
                    else
                    {
                        nTabCount++;
                        if (pd != null)
                        {
                            nCatID = pd.CategoryId;
                            pd.AppendCount = nTabCount;
                        }
                    }
                }
            }
        }

        public SortOrder PropertySortOrder { get; set; } = SortOrder.ByIdAscending;

        public SortOrder CategorySortOrder { get; set; } = SortOrder.ByIdAscending;

        private void UpdateStringFromResource(PropertyDescriptorCollection pdc)
        {
            ResourceAttribute ra = (ResourceAttribute)GetAttributes().Get(typeof(ResourceAttribute), true);
            ResourceManager rm;
            if (ra == null)
            {
                return;
            }

            try
            {
                if (string.IsNullOrEmpty(ra.BaseName) == false && String.IsNullOrEmpty(ra.AssemblyFullName) == false)
                {
                    rm = new ResourceManager(ra.BaseName, Assembly.ReflectionOnlyLoad(ra.AssemblyFullName));
                }
                else if (string.IsNullOrEmpty(ra.BaseName) == false)
                {
                    rm = new ResourceManager(ra.BaseName, m_instance.GetType().Assembly);
                }
                else
                {
                    rm = new ResourceManager(m_instance.GetType());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            string sKeyPrefix = (ra.KeyPrefix);
            foreach (DynPropertyDescriptor pd in pdc)
            {
                LocalizableAttribute la = (LocalizableAttribute)pd.Attributes.Get(typeof(LocalizableAttribute), true);
                if (la != null && !pd.IsLocalizable)
                {
                    continue;
                }
                if (pd.LCID == CultureInfo.CurrentUICulture.LCID)
                {
                    continue;
                }

                //al = pd.AttributeList;
                string sKey;
                string sResult;

                // first category
                if (!string.IsNullOrEmpty(pd.CategoryResourceKey))
                {
                    sKey = sKeyPrefix + pd.CategoryResourceKey;

                    try
                    {
                        sResult = rm.GetString(sKey);
                        if (!string.IsNullOrEmpty(sResult))
                        {
                            pd.Attributes.Add(new CategoryAttribute(sResult), true);
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Key '{0}' does not exist in the resource.", sKey);
                    }
                }

                // now display name
                sKey = sKeyPrefix + pd.Name + "_Name";
                try
                {
                    sResult = rm.GetString(sKey);
                    if (!string.IsNullOrEmpty(sResult))
                    {
                        pd.Attributes.Add(new DisplayNameAttribute(sResult), typeof(DisplayNameAttribute));
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Key '{0}' does not exist in the resource.", sKey);
                }

                // and now description
                sKey = sKeyPrefix + pd.Name + "_Desc";
                try
                {
                    sResult = rm.GetString(sKey);
                    if (!string.IsNullOrEmpty(sResult))
                    {
                        pd.Attributes.Add(new DescriptionAttribute(sResult), true);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Key '{0}' does not exist in the resource.", sKey);
                }
            }
        }

        public void ResetProperties()
        {
            m_pdc.Clear();
            GetProperties();
        }

        private static readonly Hashtable TypeDescriptorTable = new Hashtable();

        public static DynTypeDescriptor GetTypeDescriptor(object instance)
        {
            CleanUpRef();
            return (from DictionaryEntry de in TypeDescriptorTable let wr = de.Key as WeakReference where wr != null && (wr.IsAlive && instance.Equals(wr.Target)) select de.Value as DynTypeDescriptor).FirstOrDefault();
        }

        public static bool InstallTypeDescriptor(object instance)
        {
            CleanUpRef();
            if ((from DictionaryEntry de in TypeDescriptorTable select de.Key as WeakReference).Any(wr => wr != null && (wr.IsAlive && instance.Equals(wr.Target))))
            {
                return false; // because already installed
            }

            // will have to install the provider and create a new entry in the hash table
            var parentProvider = TypeDescriptor.GetProvider(instance);
            var parentCtd = parentProvider.GetTypeDescriptor(instance);
            var ourCtd = new DynTypeDescriptor(parentCtd, instance);
            var ourProvider = new TypeDescriptionProvider(parentProvider, ourCtd);
            TypeDescriptor.AddProvider(ourProvider, instance);
            var weakRef = new WeakReference(instance, true);
            TypeDescriptorTable.Add(weakRef, ourCtd);
            return true;
        }

        private static void CleanUpRef()
        {
            var deadList = TypeDescriptorTable.Keys.Cast<WeakReference>().Where(wr => !wr.IsAlive).ToList();
            foreach (var wr in deadList)
            {
                TypeDescriptorTable.Remove(wr);
            }
        }
    }
}