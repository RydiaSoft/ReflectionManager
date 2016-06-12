using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;

namespace ReflectionManager
{
    /// <summary>
    /// プロパティに対するアクセスを提供するクラスです
    /// </summary>
    /// <typeparam name="T">対象プロパティの型</typeparam>
    public sealed class PropertyReflector<T>
    {
        private PropertyInfo m_Info;
        private object m_InstanceObject;

        internal PropertyReflector(PropertyInfo info, object instanceObject)
        {
            m_Info = info;
            m_InstanceObject = instanceObject;
        }

        /// <summary>
        /// 現在の<see cref="PropertyReflector{T}"/>が表すプロパティの値を取得または設定します
        /// </summary>
        public T Value
        {
            get
            {
                return (T)m_Info.GetValue(m_InstanceObject, null);
            }
            set
            {
                m_Info.SetValue(m_InstanceObject, value, null);
            }
        }



        /// <summary>
        /// 現在の<see cref="PropertyReflector{T}"/>が表すプロパティの情報を格納する<see cref="PropertyInfo"/>を返します
        /// </summary>
        /// <returns></returns>
        public PropertyInfo GetPropertyInfo()
        {
            return m_Info;
        }

        /// <summary>
        /// 指定されたパラメータを使用してインデクサの情報を格納する<see cref="PropertyInfo"/>を返します
        /// </summary>
        /// <param name="flags">バインディングフラグ</param>
        /// <param name="type">対象の型</param>
        /// <param name="indexTypes">インデクスの型</param>
        /// <param name="info">取得した<see cref="PropertyInfo"/>を格納する変数へのポインタ</param>
        /// <returns></returns>
        internal static bool TryGetIndexerInfo(BindingFlags flags, Type type, Type[] indexTypes, out PropertyInfo info)
        {
            try
            {
                info = GetIndexerInfo(flags, type, indexTypes);
                return true;
            }
            catch (MemberAccessException)
            {
                info = null;
                return false;
            }
        }

        /// <summary>
        /// 指定されたパラメータを使用してインデクサの情報を格納する<see cref="PropertyInfo"/>を返します
        /// </summary>
        /// <param name="flags">バインディングフラグ</param>
        /// <param name="type">対象の型</param>
        /// <param name="indexTypes">インデクスの型</param>
        /// <returns></returns>
        /// <exception cref="System.MemberAccessException"></exception>
        private static PropertyInfo GetIndexerInfo(BindingFlags flags, Type type, Type[] indexTypes)
        {
            var prop = type.GetProperty("Item", flags, null, null, indexTypes, null);
            if (prop == null)
                throw new MemberAccessException();

            return prop;
        }

        /// <summary>
        /// 指定された名前のプロパティの情報を格納する<see cref="PropertyInfo"/>を取得し、
        /// 正常に取得できたかどうかを返します
        /// </summary>
        /// <param name="name">プロパティの名前を表す文字列</param>
        /// <param name="flags">バインディングフラグ</param>
        /// <param name="type">対象の型</param>
        /// <param name="info">取得した<see cref="PropertyInfo"/>を格納する変数へのポインタ</param>
        /// <returns>
        /// 正常に取得できた場合はtrue,
        /// <see cref="MemberAccessException"/>が発生し、正常に取得できなかった場合はfalse,
        /// それ以外の例外が発生した場合はスローされます
        /// </returns>
        internal static bool TryGetPropertyInfo(BindingFlags flags, Type type, string name, out PropertyInfo info)
        {
            try
            {
                info = GetPropertyInfo(name, flags, type);
                return true;
            }
            catch (MemberAccessException)
            {
                info = null;
                return false;
            }
        }

        /// <summary>
        /// 指定された名前のプロパティの情報を格納する<see cref="PropertyInfo"/>を返します
        /// </summary>
        /// <param name="name">プロパティの名前を表す文字列</param>
        /// <param name="flags">バインディングフラグ</param>
        /// <param name="type">対象の型</param>
        /// <returns></returns>
        /// <exception cref="System.MemberAccessException"></exception>
        private static PropertyInfo GetPropertyInfo(string name, BindingFlags flags, Type type)
        {
            var prop = type.GetProperty(name, flags);
            if (prop == null)
                throw new MemberAccessException();
            return prop;
        }
    }
}
