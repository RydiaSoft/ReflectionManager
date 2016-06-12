using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using ReflectionManager.Tests.TestTargets;
namespace ReflectionManager.Tests
{
    [TestFixture]
    public class TypeReflectorTests
    {

        private TypeReflector<ConstructorTarget> CreateReflector()
        {
            return new TypeReflector<ConstructorTarget>();
        }

        [Test]
        public void PublicConstructorVoid()
        {
            AreEqual(ConstructorTarget.PublicConstructorVoid);
        }

        [Test]
        public void PublicConstructorBool()
        {
            AreEqual(ConstructorTarget.PublicConstructorBool, true);
        }

        [Test]
        public void PublicConstructorBoolInt1()
        {
            AreEqual(ConstructorTarget.PublicConstructorBoolInt1, true, 2);
        }

        [Test]
        public void InternalConstructorBoolInt2()
        {
            AreEqual(ConstructorTarget.InternalConstructorBoolInt2, true, 3, 3);
        }

        [Test]
        public void ProtectedConstructorBoolInt3()
        {
            AreEqual(ConstructorTarget.ProtectedConstructorBoolInt3, true, 4, 4, 4);
        }

        [Test]
        public void PrivateConstructorBoolInt3String()
        {
            AreEqual(ConstructorTarget.PrivateConstructorBoolInt3String, true, 5, 5, 5, "PrivateConstructor(bool, int, int, int, string)");
        }

        [Test]
        public void Bind1()
        {
            var reflector = CreateReflector();
            var binding = reflector.Bind();
            Assert.IsTrue(binding.HasDefault);
            Assert.AreEqual(typeof(ConstructorTarget), binding.Type);
            Assert.AreEqual(null, binding.InstanceObject);
        }

        [Test]
        public void Bind2()
        {
            var reflector = CreateReflector();
            var binding = new BindingReflector().Public.NonPublic.Static;
            var acceser = reflector.Bind(binding);
            Assert.IsTrue(acceser.BindingReflector.HasPublic);
            Assert.IsTrue(acceser.BindingReflector.HasNonPublic);
            Assert.IsTrue(acceser.BindingReflector.HasStatic);
            Assert.AreEqual(binding.InstanceObject, acceser.InstanceObject);
            Assert.AreEqual(typeof(ConstructorTarget), acceser.Type);
        }

        private void AreEqual(ConstructorTarget expected, params object[] args)
        {
            var reflector = CreateReflector();
            var obj = reflector.CreateInstance(args);
            var result = obj.Equals(expected);
            Console.WriteLine("Equals[1]: {0}", result);
            if (!result)
            {
                Console.WriteLine("//-----------------------------------------------------------------");
                Console.WriteLine("//Equalseメソッドがfalseだったので型の内容を表す文字列を表示します");
                Console.WriteLine("//-----------------------------------------------------------------");
                Console.WriteLine("//Left");
                Console.WriteLine("//-----------------------------------------------------------------");
                Console.WriteLine(expected.ToString());
                Console.WriteLine("//-----------------------------------------------------------------");
                Console.WriteLine("//Right");
                Console.WriteLine("//-----------------------------------------------------------------");
                Console.WriteLine(obj.ToString());
            }
            Assert.IsTrue(result);
        }
    }

}
