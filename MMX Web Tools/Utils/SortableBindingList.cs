using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace MMX_Web_Tools.Utils
{
    public class SortableBindingList<T> : BindingList<T>
    {
        private bool _isSorted;
        private ListSortDirection _sortDirection;
        private PropertyDescriptor _sortProperty;

        protected override bool SupportsSortingCore => true;
        protected override bool IsSortedCore => _isSorted;
        protected override PropertyDescriptor SortPropertyCore => _sortProperty;
        protected override ListSortDirection SortDirectionCore => _sortDirection;

        public SortableBindingList() : base() { }
        public SortableBindingList(IList<T> list) : base(list) { }

        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            var list = (List<T>)Items;
            var comparer = new PropertyComparer<T>(prop, direction);
            list.Sort(comparer);
            _isSorted = true;
            _sortDirection = direction;
            _sortProperty = prop;
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected override void RemoveSortCore()
        {
            _isSorted = false;
            _sortProperty = null;
        }

        private class PropertyComparer<TItem> : IComparer<TItem>
        {
            private readonly PropertyDescriptor _property;
            private readonly ListSortDirection _direction;

            public PropertyComparer(PropertyDescriptor property, ListSortDirection direction)
            {
                _property = property;
                _direction = direction;
            }

            public int Compare(TItem x, TItem y)
            {
                var xValue = _property.GetValue(x);
                var yValue = _property.GetValue(y);

                int result;
                if (xValue == null && yValue == null) result = 0;
                else if (xValue == null) result = -1;
                else if (yValue == null) result = 1;
                else if (xValue is IComparable cmp) result = cmp.CompareTo(yValue);
                else if (xValue.Equals(yValue)) result = 0;
                else result = string.Compare(xValue.ToString(), yValue.ToString(), StringComparison.CurrentCultureIgnoreCase);

                if (_direction == ListSortDirection.Descending) result = -result;
                return result;
            }
        }
    }
}
