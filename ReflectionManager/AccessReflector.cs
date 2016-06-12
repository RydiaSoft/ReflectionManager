using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;

namespace ReflectionManager
{
    /// <summary>
    /// フィールド、プロパティ、インデクサ、メソッドに対するアクセスを提供するクラスです
    /// </summary>
    public sealed class AccessReflector
    {
        private Type m_Type;
        private object m_InstanceObject;
        private BindingReflector m_Reflector;
        private static Dictionary<Type, Dictionary<int, FieldInfo>> s_FieldMembers = new Dictionary<Type, Dictionary<int, FieldInfo>>();
        private static Dictionary<Type, Dictionary<int, PropertyInfo>> s_PropertyMenbers = new Dictionary<Type, Dictionary<int, PropertyInfo>>();
        private static Dictionary<Type, Dictionary<int, MethodInfo>> s_MethodMembers = new Dictionary<Type, Dictionary<int, MethodInfo>>();

        #region コンストラクタ

        internal AccessReflector(Type type, object instanceObject, BindingReflector reflector)
        {
            m_Type = type;
            m_InstanceObject = instanceObject;
            m_Reflector = reflector;
        }

        #endregion

        #region 実装

        /// <summary>
        /// 指定された名前のフィールドへのアクセスを提供する<see cref="FieldReflector{T}"/>を返します
        /// </summary>
        /// <typeparam name="T">フィールドの型</typeparam>
        /// <param name="name">フィールド名を表す文字列</param>
        /// <returns>対象フィールドへのアクセスを提供する新しい<see cref="FieldReflector{T}"/></returns>
        /// <exception cref="System.MemberAccessException">指定されたフィールドは存在しません</exception>
        public FieldReflector<T> Field<T>(string name)
        {
            FieldInfo info = null;
            var key = name.GetHashCode();
            if (!Fields.TryGetValue(key, out info))
            {
                if (!FieldReflector<T>.TryGetFieldInfo(m_Reflector.Flags, m_Type, name, out info))
                {
                    throw new MemberAccessException("指定されたフィールドは存在しません");
                }
                Fields.Add(key, info);
            }
            return new FieldReflector<T>(info, m_InstanceObject);
        }

        /// <summary>
        /// 指定された名前のプロパティへのアクセスを提供する<see cref="PropertyReflector{T}"/>を返します。
        /// </summary>
        /// <param name="name">プロパティの名前を表す文字列</param>
        /// <typeparam name="T">プロパティの型</typeparam>
        /// <returns></returns>
        /// <exception cref="System.MemberAccessException">指定されたプロパティは存在しません</exception>
        public PropertyReflector<T> Property<T>(string name)
        {
            PropertyInfo info = null;
            var key = name.GetHashCode();
            if (!Propertys.TryGetValue(key, out info))
            {
                if (!PropertyReflector<T>.TryGetPropertyInfo(m_Reflector.Flags, m_Type, name, out info))
                {
                    throw new MemberAccessException("指定されたプロパティは存在しません");
                }
                Propertys.Add(key, info);
            }
            return new PropertyReflector<T>(info, m_InstanceObject);
        }

        /// <summary>
        /// 指定されたインデックスパラメータを使用するインデクサへのアクセスを提供する<see cref="IndexerReflector{T}"/>を返します
        /// </summary>
        /// <param name="indexes">インデクサインデックスのパラメータ</param>
        /// <exception cref="System.MemberAccessException">指定されたインデクサは存在しません</exception>
        public IndexerReflector<T> Indexer<T>(params object[] indexes)
        {
            return IndexerExact<T>(indexes.Select(i => new ReflectionParameter(i)).ToArray());
        }

        /// <summary>
        /// 指定されたインデックスパラメータを使用するインデクサへのアクセスを提供する<see cref="IndexerReflector{T}"/>を返します
        /// </summary>
        /// <param name="indexes">インデクサインデックスのパラメータ</param>
        /// <exception cref="System.MemberAccessException">指定されたインデクサは存在しません</exception>
        public IndexerReflector<T> IndexerExact<T>(params ReflectionParameter[] indexes)
        {
            PropertyInfo info = null;
            var key = ("Indexer:" + string.Join(",", indexes.Select(t => { return t.Type.Name; }))).GetHashCode();
            if (!Propertys.TryGetValue(key, out info))
            {
                if (!PropertyReflector<T>.TryGetIndexerInfo(m_Reflector.Flags, m_Type, indexes.Select(i => i.Type).ToArray(), out info))
                {
                    throw new MemberAccessException("指定されたインデクサは存在しません");
                }
                Propertys.Add(key, info);
            }
            return new IndexerReflector<T>(info, m_InstanceObject, indexes);
        }

        /// <summary>
        /// 指定された名前のメソッドへのアクセスを提供する<see cref="MethodReflector{T}"/>を返します。
        /// </summary>
        /// <param name="name">メソッドの名前を表す文字列</param>
        /// <param name="args">引数</param>
        /// <typeparam name="T">メソッドの戻り値の型</typeparam>
        /// <returns></returns>
        /// <exception cref="System.MemberAccessException">指定されたメソッドは存在しません</exception>
        public MethodReflector<T> Method<T>(string name, params object[] args)
        {
            return Method<T>(name, ReflectionParameter.CreateReflectorParameters(args));
        }

        /// <summary>
        /// 指定された名前のメソッドへのアクセスを提供する<see cref="MethodReflector{T}"/>を返します。
        /// </summary>
        /// <param name="name">メソッドの名前を表す文字列</param>
        /// <param name="args">引数</param>
        /// <typeparam name="T">メソッドの戻り値の型</typeparam>
        /// <returns></returns>
        /// <exception cref="System.MemberAccessException">指定されたメソッドは存在しません</exception>
        public MethodReflector<T> Method<T>(string name, params ReflectionParameter[] args)
        {
            MethodInfo info = null;
            var key = name.GetHashCode();
            if (!Methods.TryGetValue(key, out info))
            {
                if (!MethodReflector<object>.TryGetMethodInfo(name, m_Type, args.Select(a => a.Type).ToArray(), m_Reflector.Flags, out info))
                {
                    throw new MemberAccessException("指定されたメソッドは存在しません");
                }
                Methods.Add(key, info);
            }
            return new MethodReflector<T>(info, m_InstanceObject, args);
        }

        /// <summary>
        /// 指定された名前のメソッドへのアクセスを提供する<see cref="MethodReflector{T}"/>を返します。
        /// </summary>
        /// <param name="name">メソッドの名前を表す文字列</param>
        /// <param name="args">引数</param>
        /// <returns></returns>
        public MethodReflector<object> Method(string name, params object[] args)
        {
            return Method<object>(name, args);
        }

        /// <summary>
        /// 指定された名前のメソッドへのアクセスを提供する<see cref="MethodReflector{T}"/>を返します。
        /// </summary>
        /// <param name="name">メソッドの名前を表す文字列</param>
        /// <param name="args">引数</param>
        /// <returns></returns>
        public MethodReflector<object> Method(string name, params ReflectionParameter[] args)
        {
            return Method<object>(name, args);
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.Instance"/>が付与され、オブジェクトが関連づけられている
        /// 場合は解除して、<see cref="BindingFlags.Static"/>を付与した新しい<see cref="AccessReflector"/>を返します
        /// </summary>
        /// <returns></returns>
        public AccessReflector ToStatic()
        {
            m_Reflector = m_Reflector.ToStatic();
            return new BindingReflector(m_Reflector.Flags, m_Type, m_Reflector.InstanceObject).GenerateAccessReflector();
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.Static"/>が付与されている場合は解除し、
        /// 指定されたオブジェクトを関連づけて<see cref="BindingFlags.Instance"/>を付与した新しい<see cref="AccessReflector"/>を返します
        /// </summary>
        /// <param name="instanceObject">関連づけるオブジェクト</param>
        /// <returns></returns>
        public AccessReflector ToInstance(object instanceObject)
        {
            m_Reflector = m_Reflector.ToInstance(instanceObject);
            return new BindingReflector(m_Reflector.Flags, m_Type, m_Reflector.InstanceObject).GenerateAccessReflector();
        }

        #endregion

        /// <summary>
        /// 現在の型の<see cref="FieldInfo"/>のキャッシュを取得します
        /// </summary>
        private Dictionary<int, FieldInfo> Fields
        {
            get
            {
                Dictionary<int, FieldInfo> members = null;
                if (!s_FieldMembers.TryGetValue(m_Type, out members))
                {
                    members = new Dictionary<int, FieldInfo>();
                    s_FieldMembers.Add(m_Type, members);
                }
                return members;
            }
        }

        /// <summary>
        /// 現在の型の<see cref="PropertyInfo"/>のキャッシュを取得します
        /// </summary>
        private Dictionary<int, PropertyInfo> Propertys
        {
            get
            {
                Dictionary<int, PropertyInfo> members = null;
                if (!s_PropertyMenbers.TryGetValue(m_Type, out members))
                {
                    members = new Dictionary<int, PropertyInfo>();
                    s_PropertyMenbers.Add(m_Type, members);
                }
                return members;
            }
        }

        /// <summary>
        /// 現在の型の<see cref="MethodInfo"/>のキャッシュを取得します
        /// </summary>
        private Dictionary<int, MethodInfo> Methods
        {
            get
            {
                Dictionary<int, MethodInfo> members = null;
                if (!s_MethodMembers.TryGetValue(m_Type, out members))
                {
                    members = new Dictionary<int, MethodInfo>();
                    s_MethodMembers.Add(m_Type, members);
                }
                return members;
            }
        }

        /// <summary>
        /// このインスタンスに関連づけられているオブジェクトを取得します
        /// </summary>
        public object InstanceObject
        {
            get
            {
                return m_InstanceObject;
            }
        }

        /// <summary>
        /// このインスタンスに関連づけられている型を取得します
        /// </summary>
        public Type Type
        {
            get
            {
                return m_Type;
            }
        }

        /// <summary>
        /// このインスタンスに関連づけられている<see cref="BindingReflector"/>を取得します
        /// </summary>
        public BindingReflector BindingReflector
        {
            get
            {
                return m_Reflector;
            }
        }
    }
}
