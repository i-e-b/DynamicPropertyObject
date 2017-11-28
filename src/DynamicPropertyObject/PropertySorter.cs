using System;
using System.Collections;

namespace DynamicPropertyObject
{
    internal class PropertySorter : IComparer
    {
        public int Compare(object x, object y)
        {
            DynPropertyDescriptor xCpd = x as DynPropertyDescriptor;
            DynPropertyDescriptor yCpd = y as DynPropertyDescriptor;

            if (xCpd == null || yCpd == null)
            {
                return 0;
            }
            xCpd.AppendCount = 0;
            yCpd.AppendCount = 0;
            int nCompResult = 0;
            switch (m_CategorySortOrder)
            {
                case SortOrder.None:
                    nCompResult = 0;
                    break;

                case SortOrder.ByIdAscending:
                    nCompResult = xCpd.CategoryId.CompareTo(yCpd.CategoryId);
                    break;

                case SortOrder.ByIdDescending:
                    nCompResult = xCpd.CategoryId.CompareTo(yCpd.CategoryId) * -1;
                    break;

                case SortOrder.ByNameAscending:
                    nCompResult = string.Compare(xCpd.Category, yCpd.Category, StringComparison.Ordinal);
                    break;

                case SortOrder.ByNameDescending:
                    nCompResult = string.Compare(xCpd.Category, yCpd.Category, StringComparison.Ordinal) * -1;
                    break;
            }
            if (nCompResult == 0)
            {
                nCompResult = CompareProperty(xCpd, yCpd);
            }
            return nCompResult;
        }

        private int CompareProperty(DynPropertyDescriptor xCpd, DynPropertyDescriptor yCpd)
        {
            int nCompResult = 0;

            switch (m_PropertySortOrder)
            {
                case SortOrder.None:
                    nCompResult = xCpd._ID.CompareTo(yCpd._ID);
                    break;

                case SortOrder.ByIdAscending:
                    nCompResult = xCpd.PropertyId.CompareTo(yCpd.PropertyId);
                    break;

                case SortOrder.ByIdDescending:
                    nCompResult = xCpd.PropertyId.CompareTo(yCpd.PropertyId) * -1;
                    break;

                case SortOrder.ByNameAscending:
                    nCompResult = string.Compare(xCpd.DisplayName, yCpd.DisplayName, StringComparison.Ordinal);
                    break;

                case SortOrder.ByNameDescending:
                    nCompResult = string.Compare(xCpd.DisplayName, yCpd.DisplayName, StringComparison.Ordinal) * -1;
                    break;
            }
            return nCompResult;
        }

        private SortOrder m_PropertySortOrder = SortOrder.ByNameAscending;

        public SortOrder PropertySortOrder
        {
            get
            {
                return m_PropertySortOrder;
            }
            set
            {
                m_PropertySortOrder = value;
            }
        }

        private SortOrder m_CategorySortOrder = SortOrder.ByNameAscending;

        public SortOrder CategorySortOrder
        {
            get
            {
                return m_CategorySortOrder;
            }
            set
            {
                m_CategorySortOrder = value;
            }
        }
    }
}