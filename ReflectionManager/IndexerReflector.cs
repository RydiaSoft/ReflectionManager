using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;

namespace ReflectionManager
{
    public class IndexerReflector<T>
    {
        private PropertyInfo m_Info;
        private object m_InstanceObject;
        private ReflectionParameter[] m_Args;

        internal IndexerReflector(PropertyInfo info, object instanceObject, ReflectionParameter[] args)
        {
            m_Info = info;
            m_InstanceObject = instanceObject;
            m_Args = args;
        }

        /// <summary>
        /// 現在のインスタンスが参照しているインデクサのインデックスを使用して値を取得または設定します
        /// </summary>
        public T Value
        {
            get
            {
                return (T)m_Info.GetValue(m_InstanceObject, m_Args);
            }
            set
            {
                m_Info.SetValue(m_InstanceObject, value, m_Args);
            }
        }

        /// <summary>
        /// 現在のインスタンスが参照しているインデクサの指定されたインデックスを参照する<see cref="IndexerReflector{T}"/>を返します
        /// </summary>
        /// <param name="args">新しいインデックス</param>
        /// <returns></returns>
        public IndexerReflector<T> Index(params object[] args)
        {
            return Index(ReflectionParameter.CreateReflectorParameters(args));
        }

        /// <summary>
        /// 現在のインスタンスが参照しているインデクサの指定されたインデックスを参照する<see cref="IndexerReflector{T}"/>を返します
        /// </summary>
        /// <param name="args">新しいインデックス</param>
        /// <returns></returns>
        public IndexerReflector<T> Index(params ReflectionParameter[] args)
        {
            return new IndexerReflector<T>(m_Info, m_InstanceObject, args);
        }

        /// <summary>
        /// 現在の<see cref="PropertyReflector{T}"/>が表すプロパティの情報を格納する<see cref="PropertyInfo"/>を返します
        /// </summary>
        /// <returns></returns>
        public PropertyInfo GetPropertyInfo()
        {
            return m_Info;
        }
    }

}
