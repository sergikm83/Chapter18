using System;
using System.Reflection.Emit;
using System.Reflection;
using System.Threading;

namespace MyAsmBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
        // Объект AppDomain отправляется вызывающим кодом.
        public static void CreateMyAsm(AppDomain curAppDomain)
        {
            // Установить общие характеристики сборки.
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = "MyAssembly";
            assemblyName.Version = new Version("1.0.0.0");
            // Создать новую сборку внутри текущего домена приложения.
            AssemblyBuilder assembly = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
            // Поскольку строится однофайловая сборка, имя модуля
            // будет таким же, как у сборки.
            ModuleBuilder module = assembly.DefineDynamicModule("MyAssembly");
            // Определить открытый класс по имени HelloWorld.
            TypeBuilder helloWorldClass = module.DefineType("MyAssembly.HelloWorld", TypeAttributes.Public);
            // Определить закрытыю переменную-член типа String по имени theMessage.
            FieldBuilder msgField = helloWorldClass.DefineField("theMessage", Type.GetType("System.String"), FieldAttributes.Private);
            // Создать специальный конструктор.
            Type[] constructorArgs = new Type[1];
            constructorArgs[0] = typeof(string);
            ConstructorBuilder constructor = helloWorldClass.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, constructorArgs);
            ILGenerator constructorIL = constructor.GetILGenerator();
            constructorIL.Emit(OpCodes.Ldarg_0);
            Type objectClass = typeof(object);
            ConstructorInfo superConstructor = objectClass.GetConstructor(new Type[0]);
            constructorIL.Emit(OpCodes.Call, superConstructor);
            constructorIL.Emit(OpCodes.Ldarg_0);
            constructorIL.Emit(OpCodes.Ldarg_1);
            constructorIL.Emit(OpCodes.Stfld,msgField);
            constructorIL.Emit(OpCodes.Ret);
            // Создать стандартный коструктор.
            helloWorldClass.DefineDefaultConstructor(MethodAttributes.Public);
            // Создать метод GetMsg().
            MethodBuilder getMsgMethod = helloWorldClass.DefineMethod("GetMsg", MethodAttributes.Public, typeof(string), null);
            ILGenerator methodIl = getMsgMethod.GetILGenerator();
            methodIl.Emit(OpCodes.Ldarg_0);
            methodIl.Emit(OpCodes.Ldfld, msgField);
            methodIl.Emit(OpCodes.Ret);
        }
    }
}
