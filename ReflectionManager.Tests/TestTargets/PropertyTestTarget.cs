using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReflectionManager.Tests.TestTargets
{
    public class PropertyTestTarget<T> : TestTarget
    {

        private static T s_PrivateStaticValue;
        private static T s_InternalStaticValue;
        private static T s_ProtectedStaticValue;
        private static T s_PublicStaticValue;

        private T m_PrivateInstanceValue;
        private T m_InternalInstanceValue;
        private T m_ProtectedInstanceValue;
        private T m_PublicInstanceValue;
        private Dictionary<int, T> m_IndexerItems = new Dictionary<int, T>();
        private Dictionary<int, Dictionary<string, T>> m_IndexerKey = new Dictionary<int, Dictionary<string, T>>();



        private T this[int index]
        {
            get
            {
                return m_IndexerItems[index];
            }
            set
            {
                m_IndexerItems[index] = value;
            }
        }

        internal T this[int index, string name]
        {
            get
            {
                return m_IndexerKey[index][name];
            }
            set
            {
                if (!m_IndexerKey.ContainsKey(index))
                    m_IndexerKey[index] = new Dictionary<string, T>();
                m_IndexerKey[index][name] = value;
            }
        }

        private static T PrivateStaticValue
        {
            get
            {
                return s_PrivateStaticValue;
            }
            set
            {
                s_PrivateStaticValue = value;
            }
        }

        internal static T InternalStaticValue
        {
            get
            {
                return s_InternalStaticValue;
            }
            set
            {
                s_InternalStaticValue = value;
            }
        }

        protected static T ProtectedStaticValue
        {
            get
            {
                return s_ProtectedStaticValue;
            }
            set
            {
                s_ProtectedStaticValue = value;
            }
        }

        public static T PublicStaticValue
        {
            get
            {
                return s_PublicStaticValue;
            }
            set
            {
                s_PublicStaticValue = value;
            }
        }

        private static T PrivateStaticAuto
        {
            get;
            set;
        }

        internal static T InternalStaticAuto
        {
            get;
            set;
        }

        protected static T ProtectedStaticAuto
        {
            get;
            set;
        }

        public static T PublicStaticAuto
        {
            get;
            set;
        }

        private T PrivateInstanceValue
        {
            get
            {
                return m_PrivateInstanceValue;
            }
            set
            {
                m_PrivateInstanceValue = value;
            }
        }

        internal T InternalInstanceValue
        {
            get
            {
                return m_InternalInstanceValue;
            }
            set
            {
                m_InternalInstanceValue = value;
            }
        }

        protected T ProtectedInstanceValue
        {
            get
            {
                return m_ProtectedInstanceValue;
            }
            set
            {
                m_ProtectedInstanceValue = value;
            }
        }

        public T PublicInstanceValue
        {
            get
            {
                return m_PublicInstanceValue;
            }
            set
            {
                m_PublicInstanceValue = value;
            }
        }

        private T PrivateInstanceAuto
        {
            get;
            set;
        }

        internal T InternalInstanceAuto
        {
            get;
            set;
        }

        protected T ProtectedInstanceAuto
        {
            get;
            set;
        }

        public T PublicInstanceAuto
        {
            get;
            set;
        }

        public override string ToString()
        {
            Clear();
            Add(nameof(PrivateStaticValue), PrivateStaticValue);
            Add(nameof(InternalStaticValue), InternalStaticValue);
            Add(nameof(ProtectedStaticValue), ProtectedStaticValue);
            Add(nameof(PublicStaticValue), PublicStaticValue);

            Add(nameof(PrivateStaticAuto), PrivateStaticAuto);
            Add(nameof(InternalStaticAuto), InternalStaticAuto);
            Add(nameof(ProtectedStaticAuto), ProtectedStaticAuto);
            Add(nameof(PublicStaticAuto), PublicStaticAuto);

            Add(nameof(PrivateInstanceValue), PrivateInstanceValue);
            Add(nameof(InternalInstanceValue), InternalInstanceValue);
            Add(nameof(ProtectedInstanceValue), ProtectedInstanceValue);
            Add(nameof(PublicInstanceValue), PublicInstanceValue);

            Add(nameof(PrivateInstanceAuto), PrivateInstanceAuto);
            Add(nameof(InternalInstanceAuto), InternalInstanceAuto);
            Add(nameof(ProtectedInstanceAuto), ProtectedInstanceAuto);
            Add(nameof(PublicInstanceAuto), PublicInstanceAuto);

            return base.ToString();
        }
    }

}
