using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReflectionManager
{
    /// <summary>
    /// メンバ検索のパラメータ情報を表すクラスです。
    /// </summary>
    public class ReflectionParameter
    {

        /// <summary>
        /// パラメータの型を保持するフィールドです
        /// </summary>
        private Type m_Type;

        private object m_Value;

        public ReflectionParameter(object value, bool isRef = false)
        {
            var type = GetType(value);
            if (isRef && !type.IsByRef)
            {
                m_Type = type.MakeByRefType();
            }
            else
            {
                m_Type = type;
            }
            m_Value = value;
        }

        internal ReflectionParameter(object value)
        {
            m_Type = GetType(value);
            m_Value = value;
        }

        private Type GetType(object value)
        {
            return value == null ? typeof(object) : value.GetType();
        }

        /// <summary>
        /// 複数の指定されたパラメータを使用して、<see cref="ReflectionParameter"/>の配列を作成して返します
        /// </summary>
        /// <param name="args">引数</param>
        /// <returns>生成された各パラメータの<see cref="ReflectionParameter"/>配列</returns>
        public static ReflectionParameter[] CreateReflectorParameters(params object[] args)
        {
            return args.Select(a => new ReflectionParameter(a)).ToArray();
        }

        internal void SetValue(object value)
        {
            m_Value = value;
        }

        /// <summary>
        /// パラメータの型を取得します。
        /// </summary>
        public Type Type
        {
            get
            {
                return m_Type;
            }
        }

        /// <summary>
        /// パラメータの値を取得します。
        /// </summary>
        public object Value
        {
            get
            {
                return m_Value;
            }
        }


    }
}
