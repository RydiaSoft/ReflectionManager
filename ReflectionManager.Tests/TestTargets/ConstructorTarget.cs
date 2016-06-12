using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReflectionManager.Tests.TestTargets
{
    public class ConstructorTarget : TestTarget, IEquatable<ConstructorTarget>
    {
        private bool m_Flag;
        private int m_Value1;
        private int m_Value2;
        private int m_Value3;
        private string m_Message;

        public static readonly ConstructorTarget PublicConstructorVoid = new ConstructorTarget();
        public static readonly ConstructorTarget PublicConstructorBool = new ConstructorTarget(true);
        public static readonly ConstructorTarget PublicConstructorBoolInt1 = new ConstructorTarget(true, 2);
        public static readonly ConstructorTarget InternalConstructorBoolInt2 = new ConstructorTarget(true, 3, 3);
        public static readonly ConstructorTarget ProtectedConstructorBoolInt3 = new ConstructorTarget(true, 4, 4, 4);
        public static readonly ConstructorTarget PrivateConstructorBoolInt3String = new ConstructorTarget(true, 5, 5, 5, "PrivateConstructor(bool, int, int, int, string)");

        #region コンストラクタ

        /// <summary>
        ///   <see cref="ConstructorTarget"/> classの新しいインスタンスを初期化します
        /// </summary>
        public ConstructorTarget()
            : this(true, 0, 0, 0, "PublicConstructor(void)")
        {

        }

        public ConstructorTarget(bool flag)
            : this(flag, 1, 1, 1, "PublicConstructor(bool)")
        {

        }

        public ConstructorTarget(bool flag, int value)
            : this(flag, value, 2, 2, "PublicConstructor(bool, int)")
        {

        }

        internal ConstructorTarget(bool flag, int value1, int value2)
            : this(flag, value1, value2, 3, "InternalConstructor(bool, int, int)")
        {

        }

        protected ConstructorTarget(bool flag, int value1, int value2, int value3)
            : this(flag, value1, value2, value3, "ProtectedConstructor(bool, int, int, int)")
        {

        }

        private ConstructorTarget(bool flag, int value1, int value2, int value3, string message)
        {
            m_Flag = flag;
            m_Value1 = value1;
            m_Value2 = value2;
            m_Value3 = value3;
            m_Message = message;
        }


        #endregion

        public override int GetHashCode()
        {
            return (m_Value1 + m_Value2 + m_Value3).GetHashCode();
        }

        public bool Equals(ConstructorTarget other)
        {
            var result = (
                m_Value1 == other.m_Value1 &&
                m_Value2 == other.m_Value2 &&
                m_Value3 == other.m_Value3 &&
                m_Flag == other.m_Flag &&
                m_Message == other.m_Message);
            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is ConstructorTarget)
                return Equals((ConstructorTarget)obj);
            return false;
        }

        public override string ToString()
        {
            Clear();
            Add(nameof(m_Flag), m_Flag);
            Add(nameof(m_Value1), m_Value1);
            Add(nameof(m_Value2), m_Value2);
            Add(nameof(m_Value3), m_Value3);
            Add(nameof(m_Message), m_Message);

            return base.ToString();
        }

        public static bool operator ==(ConstructorTarget left, ConstructorTarget right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ConstructorTarget left, ConstructorTarget right)
        {
            return !left.Equals(right);
        }

    }

}
