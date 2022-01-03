using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JB.Infrastructure.Helpers
{
    public static class PropertyHelper
    {

        private static class PropertyLister<T1, T2>
        {
            public static readonly IEnumerable<Tuple<PropertyInfo, PropertyInfo>> PropertyMap;

            static PropertyLister()
            {
                var b = BindingFlags.Public | BindingFlags.Instance;
                PropertyMap =
                    (from f in typeof(T1).GetProperties(b)
                     join t in typeof(T2).GetProperties(b) on f.Name equals t.Name
                     select Tuple.Create(f, t))
                        .ToArray();
            }
        }
        public static T InjectNonNull<T>(T dest, T src)
        {
            foreach (var propertyPair in PropertyLister<T, T>.PropertyMap)
            {
                var fromValue = propertyPair.Item2.GetValue(src, null);
                if (fromValue != null && propertyPair.Item1.CanWrite)
                {

                    propertyPair.Item1.SetValue(dest, fromValue, null);
                }
            }

            return dest;
        }
       


        public static FieldInfo[] GetConstants(this Type type)
        {
            ArrayList constants = new ArrayList();

            FieldInfo[] fieldInfos = type.GetFields(
                // Gets all public and static fields

                BindingFlags.Public | BindingFlags.Static |
                // This tells it to get the fields from all base types as well

                BindingFlags.FlattenHierarchy);

            // Go through the list and only pick out the constants
            foreach (FieldInfo fi in fieldInfos)
                // IsLiteral determines if its value is written at 
                //   compile time and not changeable
                // IsInitOnly determines if the field can be set 
                //   in the body of the constructor
                // for C# a field which is readonly keyword would have both true 
                //   but a const field would have only IsLiteral equal to true
                if (fi.IsLiteral && !fi.IsInitOnly)
                    constants.Add(fi);

            // Return an array of FieldInfos
            return (FieldInfo[])constants.ToArray(typeof(FieldInfo));
        }

        public static bool TryValidateObject(object instance, out ICollection<string> results)
        {
            bool isValid = true;
            var validationContext = new ValidationContext(instance, null);
            results = null;

            try
            {
                Validator.ValidateObject(instance, validationContext, true);
            }
            catch (Exception e)
            {
                if (e is ValidationException)
                {
                    results = new List<string>
                    {
                        e.Message,
                    };
                }

                isValid = false;
            }

            return isValid;
        }
    }
}
