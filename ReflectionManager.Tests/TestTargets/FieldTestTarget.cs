using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReflectionManager.Tests.TestTargets
{
    public class FieldTestTarget<T> : TestTarget
    {
        /// <summary>
        /// プライベート静的フィールド
        /// </summary>
        private static T s_PrivateStaticField = default(T);
        /// <summary>
        /// インターナル静的フィールド
        /// </summary>
        internal static T s_InternalStaticField = default(T);
        /// <summary>
        /// プロテクテッド静的フィールド
        /// </summary>
        protected static T s_ProtectedStaticField = default(T);
        /// <summary>
        /// パブリック静的フィールド
        /// </summary>
        public static T PublicStaticField = default(T);

        /// <summary>
        /// プライベートインスタンスフィールド
        /// </summary>
        private T m_PrivateInstanceField = default(T);
        /// <summary>
        /// インターナルインスタンスフィールド
        /// </summary>
        internal T m_InternalInstanceField = default(T);
        /// <summary>
        /// プロテクテッドインスタンスフィールド
        /// </summary>
        protected T m_ProtectedInstanceField = default(T);
        /// <summary>
        /// パブリックインスタンスフィールド
        /// </summary>
        public T PublicInstanceField = default(T);

        public override string ToString()
        {
            Clear();
            Add(nameof(s_PrivateStaticField), s_PrivateStaticField);
            Add(nameof(s_InternalStaticField), s_InternalStaticField);
            Add(nameof(s_ProtectedStaticField), s_ProtectedStaticField);
            Add(nameof(PublicStaticField), PublicStaticField);

            Add(nameof(m_PrivateInstanceField), m_PrivateInstanceField);
            Add(nameof(m_InternalInstanceField), m_InternalInstanceField);
            Add(nameof(m_ProtectedInstanceField), m_ProtectedInstanceField);
            Add(nameof(PublicInstanceField), PublicInstanceField);
            return base.ToString();
        }

    }

}
