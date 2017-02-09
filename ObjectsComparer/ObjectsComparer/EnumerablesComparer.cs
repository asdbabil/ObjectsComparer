﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ObjectsComparer
{
    public class EnumerablesComparer : IEnumerablesComparer
    {
        private readonly IObjectsDataComparer _parentComparer;

        public EnumerablesComparer(IObjectsDataComparer parentComparer = null)
        {
            _parentComparer = parentComparer;
        }

        public IEnumerable<Difference> Compare(object obj1, object obj2)
        {
            obj1 = obj1 ?? Enumerable.Empty<object>();
            obj2 = obj2 ?? Enumerable.Empty<object>();

            if (obj1.GetType().IsAssignableFrom(typeof(IEnumerable)))
            {
                throw new ArgumentException(nameof(obj1));
            }

            if (obj2.GetType().IsAssignableFrom(typeof(IEnumerable)))
            {
                throw new ArgumentException(nameof(obj2));
            }

            var array1 = ((IEnumerable)obj1).Cast<object>().ToArray();
            var array2 = ((IEnumerable)obj2).Cast<object>().ToArray();

            if (array1.Length != array2.Length)
            {
                yield return new Difference("[]", array1.Length.ToString(), array2.Length.ToString());
                yield break;
            }

            for (var i = 0; i < array2.Length; i++)
            {
                if (array1[i].GetType() != array2[i].GetType())
                {
                    yield return new Difference($"[{i}]", array1[i] + string.Empty, array2[i] + string.Empty);
                    continue;
                }

                var comparer = ObjectsesDataComparer<object>.CreateComparer(array1[i].GetType());
                _parentComparer.ConfigureChildComparer(comparer);

                foreach (var failure in comparer.Compare(array1[i], array2[i]))
                {
                    yield return failure.InsertPath($"[{i}]");
                }
            }
        }
    }
}
