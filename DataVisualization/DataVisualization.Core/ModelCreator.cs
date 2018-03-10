using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace DataVisualization.Core
{
    public sealed class ModelCreator
    {
        private const MethodAttributes GetterAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;
        private const MethodAttributes SetterAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

        public static object Create(Tuple<string, Type>[] properties)
        {
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));

            // Init
            var currentDomain = Thread.GetDomain();
            var dynamicAssemblyName = new AssemblyName { Name = "DynamicAssembly" };
            var asmBuilder = currentDomain.DefineDynamicAssembly(dynamicAssemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = asmBuilder.DefineDynamicModule(dynamicAssemblyName.Name);

            var typeBuilder = moduleBuilder.DefineType("DynamicData", TypeAttributes.Public); ;

            foreach (var property in properties)
            {
                GeneratePropertyWithBackingField(typeBuilder, property.Item1, property.Item2);
            }

            return Activator.CreateInstance(typeBuilder.CreateType());
        }

        private static void GeneratePropertyWithBackingField(TypeBuilder typeBuilder, string propName, Type propType)
        {
            var firstLetter = propName[0];
            var publicPropName = char.ToUpper(firstLetter) + propName.Substring(1);
            var backingFieldName = char.ToLower(firstLetter) + propName.Substring(1);

            var propertyBuilder = typeBuilder.DefineProperty(publicPropName, PropertyAttributes.HasDefault, propType, null);
            var fieldBuilder = typeBuilder.DefineField(backingFieldName, propType, FieldAttributes.Private);

            var getter = GenerateGetter(typeBuilder, fieldBuilder, publicPropName, propType);
            var setter = GenerateSetter(typeBuilder, fieldBuilder, publicPropName, propType);

            propertyBuilder.SetGetMethod(getter);
            propertyBuilder.SetSetMethod(setter);
        }

        private static MethodBuilder GenerateGetter(TypeBuilder typeBuilder, FieldBuilder fieldBuilder, string name, Type type)
        {
            var propertyMethodBuilder = typeBuilder.DefineMethod($"get_{name}", GetterAttributes, type, Type.EmptyTypes);

            var generator = propertyMethodBuilder.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, fieldBuilder);
            generator.Emit(OpCodes.Ret);

            return propertyMethodBuilder;
        }

        private static MethodBuilder GenerateSetter(TypeBuilder typeBuilder, FieldBuilder fieldBuilder, string name, Type type)
        {
            var propertyMethodBuilder = typeBuilder.DefineMethod($"set_{name}", SetterAttributes, null, new[] { type });

            var generator = propertyMethodBuilder.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Stfld, fieldBuilder);
            generator.Emit(OpCodes.Ret);

            return propertyMethodBuilder;
        }
    }
}
