
using System.Collections.Generic;



namespace SQLMerge
{


    public static class lalalla
    {

        public static IEnumerable<TSource> Take<TSource>(this IEnumerable<TSource> source, int count)
        {
            if (source == null) 
                throw new System.ArgumentNullException("source");

            return TakeIterator<TSource>(source, count);
        }

        static IEnumerable<TSource> TakeIterator<TSource>(IEnumerable<TSource> source, int count)
        {
            if (count > 0)
            {
                foreach (TSource element in source)
                {
                    yield return element;
                    if (--count == 0) break;
                }
            }
        }

        public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            return SequenceEqual<TSource>(first, second, null);
        }

        public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            if (comparer == null) comparer = EqualityComparer<TSource>.Default;

            if (first == null) 
                throw new System.ArgumentNullException("first");
            if (second == null) 
                throw new System.ArgumentNullException("second");

            using (IEnumerator<TSource> e1 = first.GetEnumerator())
            {
                using (IEnumerator<TSource> e2 = second.GetEnumerator())
                {
                    while (e1.MoveNext())
                    {
                        if (!(e2.MoveNext() && comparer.Equals(e1.Current, e2.Current))) return false;
                    }
                    if (e2.MoveNext()) return false;
                }
            }

            return true;
        }



    }
}
