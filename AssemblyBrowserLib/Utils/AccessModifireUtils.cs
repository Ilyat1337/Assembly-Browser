using AssemblyBrowserLib.AssemblyTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AssemblyBrowserLib.Utils
{
    class AccessModifireUtils
    {
        private const int EVENT_INFO_METHOD_COUNT = 3;

        public static AccessModifire GetAccessModifireFor(MemberInfo memberInfo)
        {
            if (memberInfo is Type)
            {
                return GetAccessModifireForType(memberInfo as Type);
            }
            return GetAccessModifireForMemberInfo(memberInfo);
        }

        private static AccessModifire GetAccessModifireForType(Type type)
        {
            if (type.IsNested)
            {
                if (type.IsNestedPublic)
                    return AccessModifire.Public;
                if (type.IsNestedPrivate)
                    return AccessModifire.Private;
                if (type.IsNestedFamily || type.IsNestedFamANDAssem || type.IsNestedFamORAssem)
                    return AccessModifire.Protected;
                return AccessModifire.Internal;
            }
            else
            {
                if (type.IsPublic)
                    return AccessModifire.Public;
                return AccessModifire.Internal;
            }
        }

        private static AccessModifire GetAccessModifireForMemberInfo(MemberInfo memberInfo)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Constructor:
                case MemberTypes.Method:
                    return GetAccessModifireForMethod(memberInfo as MethodBase);
                case MemberTypes.Field:
                    return GetAccessModifireForField(memberInfo as FieldInfo);
                case MemberTypes.Property:
                    return GetAccessModifireForProperty(memberInfo as PropertyInfo);
                case MemberTypes.Event:
                    return GetAccessModifireForEvent(memberInfo as EventInfo);
            }
            return 0;
        }

        private static AccessModifire GetAccessModifireForMethod(MethodBase methodBase)
        {
            if (methodBase.IsPublic)
                return AccessModifire.Public;
            if (methodBase.IsPrivate)
                return AccessModifire.Private;
            if (methodBase.IsFamily || methodBase.IsFamilyAndAssembly || methodBase.IsFamilyOrAssembly)
                return AccessModifire.Protected;
            return AccessModifire.Internal;
        }

        private static AccessModifire GetAccessModifireForField(FieldInfo fieldInfo)
        {
            if (fieldInfo.IsPublic)
                return AccessModifire.Public;
            if (fieldInfo.IsPrivate)
                return AccessModifire.Private;
            if (fieldInfo.IsFamily || fieldInfo.IsFamilyAndAssembly || fieldInfo.IsFamilyOrAssembly)
                return AccessModifire.Protected;
            return AccessModifire.Internal;
        }

        private static AccessModifire GetAccessModifireForProperty(PropertyInfo propertyInfo)
        {
            if (propertyInfo.SetMethod == null)
                return GetAccessModifireForMethod(propertyInfo.GetMethod);
            if (propertyInfo.GetMethod == null)
                return GetAccessModifireForMethod(propertyInfo.SetMethod);
            return (AccessModifire)Math.Min((byte)GetAccessModifireForMethod(propertyInfo.GetMethod), 
                (byte)GetAccessModifireForMethod(propertyInfo.SetMethod));
        }

        private static AccessModifire GetAccessModifireForEvent(EventInfo eventInfo)
        {
            List<AccessModifire> accessModifires = new List<AccessModifire>(EVENT_INFO_METHOD_COUNT);
            if (eventInfo.AddMethod != null)
                accessModifires.Add(GetAccessModifireForMethod(eventInfo.AddMethod));
            if (eventInfo.RemoveMethod != null)
                accessModifires.Add(GetAccessModifireForMethod(eventInfo.RemoveMethod));
            if (eventInfo.RaiseMethod != null)
                accessModifires.Add(GetAccessModifireForMethod(eventInfo.RemoveMethod));
            return accessModifires.Min();
        }
    }
}
