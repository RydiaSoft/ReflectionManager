using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;

namespace ReflectionManager
{
    public sealed class MethodReflector<T>
    {

        private MethodInfo m_Info;
        private ReflectionParameter[] m_args;
        private object m_InstanceObject;

        internal MethodReflector(MethodInfo info, object instanceObject, params ReflectionParameter[] args)
        {
            m_Info = info;
            m_InstanceObject = instanceObject;
            m_args = args;
        }

        /// <summary>
        /// この<see cref="MethodReflector{T}"/>が作成された際に指定されたパラメータを使用してメソッドを実行します
        /// </summary>
        /// <returns></returns>
        public T Invoke()
        {
            return Invoke(m_args);
        }

        /// <summary>
        /// 指定した引数を使用してメソッドを実行します
        /// </summary>
        /// <param name="args">引数</param>
        /// <returns></returns>
        public T Invoke(params object[] args)
        {
            return Invoke(ReflectionParameter.CreateReflectorParameters(args));
        }

        /// <summary>
        /// 指定した引数を使用してメソッドを実行します
        /// </summary>
        /// <param name="args">引数</param>
        /// <returns></returns>
        public T Invoke(params ReflectionParameter[] args)
        {
            var argVals = args.Select(a => a.Value).ToArray();
            var result = m_Info.Invoke(m_InstanceObject, argVals);
            //参照渡しの値を反映
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Type.IsByRef) args[i].SetValue(argVals[i]);
            }
            return (T)result;
        }

        /// <summary>
        /// この<see cref="MethodReflector{T}"/>が参照するメソッドの情報を格納する<see cref="MethodInfo"/>を返します
        /// </summary>
        /// <returns></returns>
        public MethodInfo GetMethodInfo()
        {
            return m_Info;
        }

        /// <summary>
        /// 指定された名前のメソッドの情報を格納する<see cref="MethodInfo"/>を取得し、
        /// 正常に取得できたかどうかを返します
        /// </summary>
        /// <param name="name">メソッドの名前を表す文字列</param>
        /// <param name="isStatic">静的メソッドかどうかを表す値</param>
        /// <param name="info">取得した<see cref="MethodInfo"/>を格納する変数へのポインタ</param>
        /// <returns>
        /// 正常に取得できた場合はtrue,
        /// <see cref="MemberAccessException"/>が発生し、正常に取得できなかった場合はfalse,
        /// それ以外の例外が発生した場合はスローされます
        /// </returns>
        /// <param name="paramTypes">メソッドの引数を表す型の配列</param>
        internal static bool TryGetMethodInfo(string name, Type type, Type[] paramTypes, BindingFlags flags, out MethodInfo info)
        {
            try
            {
                info = GetMethodInfo(name, type, paramTypes, flags);
                return true;
            }
            catch (MemberAccessException)
            {
                info = null;
                return false;
            }
        }

        /// <summary>
        /// 指定された名前のメソッドの情報を格納する<see cref="MethodInfo"/>を返します
        /// </summary>
        /// <param name="name">メソッドの名前を表す文字列</param>
        /// <param name="paramTypes">メソッドの引数を表す型の配列</param>
        /// <param name="isStatic">静的メソッドかどうかを表す値</param>
        /// <returns></returns>
        /// <exception cref="System.MemberAccessException"></exception>
        private static MethodInfo GetMethodInfo(string name, Type type, Type[] paramTypes, BindingFlags flags)
        {
            var mtd = type.GetMethod(name, flags, null, paramTypes, null);
            if (mtd == null)
                throw new MemberAccessException();

            return mtd;
        }
    }

}
