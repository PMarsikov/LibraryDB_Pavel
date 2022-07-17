using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LibraryDB_Pavel.Extensions
{
    public static class Extensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> original)
        {
            return new ObservableCollection<T>(original);
        }
    }
}
