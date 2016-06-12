using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;

namespace ReflectionManager
{
    public class FieldReflector<T>
    {

        private FieldInfo m_FieldInfo;
        private object m_InstanceObject;

        internal FieldReflector(FieldInfo info, object instanceObject)
        {
            m_FieldInfo = info;
            m_InstanceObject = instanceObject;
        }

        /// <summary>
        /// 現在の<see cref="FieldReflector{T}"/>が表すフィールドの値を取得または設定します
        /// </summary>
        public T Value
        {
            get
            {
                return (T)m_FieldInfo.GetValue(m_InstanceObject);
            }
            set
            {
                m_FieldInfo.SetValue(m_InstanceObject, value);
            }
        }

        /// <summary>
        /// 現在の<see cref="FieldReflector{T}"/>が表すフィールドの情報を格納する<see cref="FieldInfo"/>を返します
        /// </summary>
        /// <returns></returns>
        public FieldInfo GetFieldInfo()
        {
            return m_FieldInfo;
        }

        /// <summary>
        /// 指定された名前のフィールドの情報を格納する<see cref="FieldInfo"/>を取得し、
        /// 正常に取得できたかどうかを返します
        /// </summary>
        /// <param name="name">フィールドの名前を表す文字列</param>
        /// <param name="isStatic">静的フィールドかどうかを表す値</param>
        /// <param name="info">取得した<see cref="FieldInfo"/>を格納する変数へのポインタ</param>
        /// <returns>
        /// 正常に取得できた場合はtrue,
        /// <see cref="MemberAccessException"/>が発生し、正常に取得できなかった場合はfalse,
        /// それ以外の例外が発生した場合はスローされます
        /// </returns>
        internal static bool TryGetFieldInfo(BindingFlags flags, Type type, string name, out FieldInfo info)
        {
            try
            {
                info = GetFieldInfo(flags, type, name);
                return true;
            }
            catch (MemberAccessException)
            {
                info = null;
                return false;
            }
        }

        /// <summary>
        /// 指定された名前のフィールドの情報を格納する<see cref="FieldInfo"/>を返します
        /// </summary>
        /// <param name="name">フィールドの名前を表す文字列</param>
        /// <param name="isStatic">静的メンバかどうかを表す値</param>
        /// <returns></returns>
        /// <exception cref="System.MemberAccessException"></exception>
        private static FieldInfo GetFieldInfo(BindingFlags flags, Type type, string name)
        {
            var fld = type.GetField(name, flags);
            if (fld == null)
                throw new MemberAccessException();
            return fld;
        }
    }

}
