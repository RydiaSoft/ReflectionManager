using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;

using NUnit.Framework;

namespace ReflectionManager.Tests
{
    [TestFixture]
    public class BindingReflectorTests
    {
        #region テストメソッド


        /// <summary>
        /// インスタンス生成時の状態を確認します
        /// </summary>
        [Test]
        public void CreateInstance1()
        {
            var obj = new BindingReflector();
            Assert.IsTrue(obj.HasDefault);
            Assert.IsTrue(obj.InstanceObject == null);
            Assert.IsTrue(obj.Type == null);
            Assert.IsTrue(obj.Flags == BindingFlags.Default);
        }

        [Test]
        public void CreateInstance2()
        {
            var testobj = new object();
            var testBinding = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var testType = typeof(object);
            var obj = new BindingReflector(testBinding, testType, testobj);
            Assert.IsTrue(obj.HasInstance);
            Assert.IsTrue(obj.HasPublic);
            Assert.IsTrue(obj.HasNonPublic);
            Assert.AreEqual(testType, obj.Type);
            Assert.AreEqual(testobj, obj.InstanceObject);
            Assert.AreEqual(testBinding, obj.Flags);
        }

        [Test]
        public void FlagsOff()
        {
            var obj = new BindingReflector();
            var publicNonPublic = obj.Public.NonPublic;
            Assert.IsTrue(publicNonPublic.HasPublic);
            Assert.IsTrue(publicNonPublic.HasNonPublic);
            publicNonPublic = publicNonPublic.FlagsOff(BindingFlags.Public);
            Assert.IsFalse(publicNonPublic.HasPublic);
            Assert.IsTrue(publicNonPublic.HasNonPublic);
        }

        [Test]
        public void GenerateAccessReflectorError()
        {
            var publicNonPublic = new BindingReflector().Public.NonPublic;
            Assert.Throws<InvalidOperationException>(() => { publicNonPublic.GenerateAccessReflector(); });
        }

        [Test]
        public void GenerateAccessReflector()
        {
            var reflector = new TypeReflector<object>();
            var bindingReflector = reflector.Bind().Public.NonPublic;
            var acceser = bindingReflector.GenerateAccessReflector();
            Assert.AreEqual(bindingReflector.InstanceObject, acceser.InstanceObject);
            Assert.AreEqual(typeof(object), acceser.Type);
            Assert.AreEqual(bindingReflector, acceser.BindingReflector);
        }

        [Test]
        public void ToStatic1()
        {
            var obj = new object();
            var reflector = CreatePublicNonPublicInstance(obj).ToStatic();

            Assert.IsFalse(reflector.HasInstance);
            Assert.IsTrue(reflector.HasPublic);
            Assert.IsTrue(reflector.HasNonPublic);
            Assert.IsTrue(reflector.HasStatic);
            AssertTypeAndObject<object>(reflector, null);
        }

        [Test]
        public void ToStatic2()
        {
            var reflector = CreatePublicNonPublicStatic<object>().ToStatic();
            Assert.IsTrue(reflector.HasPublic);
            Assert.IsTrue(reflector.HasNonPublic);
            Assert.IsTrue(reflector.HasStatic);
            AssertTypeAndObject<object>(reflector, null);
        }

        [Test]
        public void ToInstance1()
        {
            var obj = new object();
            var reflector = CreatePublicNonPublicStatic<object>();

            reflector = reflector.ToInstance(obj);

            Assert.IsFalse(reflector.HasStatic);
            Assert.IsTrue(reflector.HasPublic);
            Assert.IsTrue(reflector.HasNonPublic);
            Assert.IsTrue(reflector.HasInstance);
            AssertTypeAndObject(reflector, obj);

        }

        [Test]
        public void ToInstance2()
        {
            var obj1 = new object();
            var reflector = CreatePublicNonPublicInstance(obj1);

            var obj2 = new object();
            reflector = reflector.ToInstance(obj2);

            Assert.IsTrue(reflector.HasInstance);
            Assert.IsTrue(reflector.HasPublic);
            Assert.IsTrue(reflector.HasNonPublic);
            Assert.AreNotEqual(obj1, reflector.InstanceObject, "obj1");
            AssertTypeAndObject(reflector, obj2);
        }

        [Test]
        public void Instance1()
        {
            var obj = new object();
            var reflector = CreatePublicNonPublicStatic<object>().SetInstance(obj);

            Assert.IsTrue(reflector.HasInstance, "HasInstance");
            Assert.IsTrue(reflector.HasStatic);
            Assert.IsTrue(reflector.HasPublic);
            Assert.IsTrue(reflector.HasNonPublic);
            AssertTypeAndObject(reflector, obj);
        }

        [Test]
        public void Instance2()
        {
            var obj = new object();
            var reflector = CreatePublicNonPublicInstance(obj);

            Assert.IsTrue(reflector.HasInstance, "HasInstance");
            Assert.IsTrue(reflector.HasPublic);
            Assert.IsTrue(reflector.HasNonPublic);
            AssertTypeAndObject(reflector, obj);
        }

        [Test]
        public void Equals()
        {
            var obj = new object();
            var reflector1 = CreatePublicNonPublicStatic<object>().ToInstance(obj);
            var reflector2 = CreatePublicNonPublicInstance(obj);

            Assert.IsTrue(reflector1.Equals(reflector2));
        }

        [Test]
        public void CreateInstanceFlags()
        {
            var reflector = new BindingReflector().CreateInstance;
            Assert.IsTrue(reflector.HasCreateInstance);
            HasFlags(reflector, BindingFlags.CreateInstance);
        }

        [Test]
        public void DeclaredOnlyFlags()
        {
            var flags = BindingFlags.DeclaredOnly;
            var reflector = new BindingReflector().DeclaredOnly;

            HasFlags(reflector, flags);
        }

        [Test]
        public void DefaultFlags()
        {
            var flags = BindingFlags.Default;
            var reflector = new BindingReflector();

            HasFlags(reflector, flags);
        }

        [Test]
        public void ExactBindingFlags()
        {
            var flags = BindingFlags.ExactBinding;
            var reflector = new BindingReflector().ExactBinding;

            HasFlags(reflector, flags);
        }

        [Test]
        public void FlattenHierarchyFlags()
        {
            var flags = BindingFlags.FlattenHierarchy;
            var reflector = new BindingReflector().FlattenHierarchy;

            HasFlags(reflector, flags);
        }

        [Test]
        public void GetFieldFlags()
        {
            var flags = BindingFlags.GetField;
            var reflector = new BindingReflector().GetField;

            HasFlags(reflector, flags);
        }

        [Test]
        public void GetPropertyFlags()
        {
            var flags = BindingFlags.GetProperty;
            var reflector = new BindingReflector().GetProperty;

            HasFlags(reflector, flags);
        }

        [Test]
        public void IgnoreCaseFlags()
        {
            var flags = BindingFlags.IgnoreCase;
            var reflector = new BindingReflector().IgnoreCase;

            HasFlags(reflector, flags);
        }

        [Test]
        public void IgnoreReturnFlags()
        {
            var flags = BindingFlags.IgnoreReturn;
            var reflector = new BindingReflector().IgnoreReturn;

            HasFlags(reflector, flags);
        }

        [Test]
        public void InvokeMethodFlags()
        {
            var flags = BindingFlags.InvokeMethod;
            var reflector = new BindingReflector().InvokeMethod;

            HasFlags(reflector, flags);
        }

        [Test]
        public void NonPublicFlags()
        {
            var flags = BindingFlags.NonPublic;
            var reflector = new BindingReflector().NonPublic;

            HasFlags(reflector, flags);
        }

        [Test]
        public void OptionalParamBindingFlags()
        {
            var flags = BindingFlags.OptionalParamBinding;
            var reflector = new BindingReflector().OptionalParamBinding;

            HasFlags(reflector, flags);
        }

        [Test]
        public void PublicFlags()
        {
            var flags = BindingFlags.Public;
            var reflector = new BindingReflector().Public;

            HasFlags(reflector, flags);
        }

        [Test]
        public void PutDispPropertyFlags()
        {
            var flags = BindingFlags.PutDispProperty;
            var reflector = new BindingReflector().PutDispProperty;

            HasFlags(reflector, flags);
        }

        [Test]
        public void PutRefDispPropertyFlags()
        {
            var flags = BindingFlags.PutRefDispProperty;
            var reflector = new BindingReflector().PutRefDispProperty;

            HasFlags(reflector, flags);
        }

        [Test]
        public void SetFieldFlags()
        {
            var flags = BindingFlags.SetField;
            var reflector = new BindingReflector().SetField;

            HasFlags(reflector, flags);
        }

        [Test]
        public void SetPropertyFlags()
        {
            var flags = BindingFlags.SetProperty;
            var reflector = new BindingReflector().SetProperty;

            HasFlags(reflector, flags);
        }

        [Test]
        public void StaticFlags()
        {
            var flags = BindingFlags.Static;
            var reflector = new BindingReflector().Static;

            HasFlags(reflector, flags);
        }

        [Test]
        public void SuppressChangeTypeFlags()
        {
            var flags = BindingFlags.SuppressChangeType;
            var reflector = new BindingReflector().SuppressChangeType;

            HasFlags(reflector, flags);
        }



        #endregion

        public void HasFlags(BindingReflector reflector, BindingFlags flags)
        {
            Assert.AreEqual(flags, reflector.Flags);

            reflector = reflector.Public.NonPublic.Static;

            Assert.AreNotEqual(flags, reflector.Flags);
        }

        private void AssertTypeAndObject<T>(BindingReflector reflector, T obj)
        {
            Assert.AreEqual(obj, reflector.InstanceObject);
            Assert.AreEqual(typeof(T), reflector.Type);
        }

        private BindingReflector CreatePublicNonPublicInstance<T>(T obj)
        {
            var reflector = CreatePublicNonPublic<T>().SetInstance(obj);
            Assert.IsTrue(reflector.HasInstance);
            Assert.IsTrue(reflector.HasPublic);
            Assert.IsTrue(reflector.HasNonPublic);
            Assert.AreEqual(obj, reflector.InstanceObject);
            Assert.AreEqual(typeof(T), reflector.Type);
            return reflector;
        }

        private BindingReflector CreatePublicNonPublicStatic<T>()
        {
            var reflector = CreatePublicNonPublic<T>().Static;
            Assert.IsTrue(reflector.HasStatic);
            return reflector;
        }

        private BindingReflector CreatePublicNonPublic<T>()
        {
            var reflector = new TypeReflector<T>().Bind().Public.NonPublic;
            Assert.IsTrue(reflector.HasPublic);
            Assert.IsTrue(reflector.HasNonPublic);
            Assert.AreEqual(null, reflector.InstanceObject);
            Assert.AreEqual(typeof(T), reflector.Type);
            return reflector;
        }
    }
}
