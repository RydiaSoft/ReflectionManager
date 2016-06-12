using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;

namespace ReflectionManager
{
    /// <summary>
    /// 型に対するアクセスを提供するクラスです
    /// </summary>
    public sealed class TypeReflector<T>
    {

        /// <summary>
        /// インスタンスメンバ検索用フラグ
        /// </summary>
        private const BindingFlags InstanceFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        /// <summary>
        ///   <see cref="TypeReflector{T}"/> classの新しいインスタンスを初期化します
        /// </summary>
        public TypeReflector()
        {

        }

        /// <summary>
        /// 指定されたパラメータに一致するコンストラクタを使って
        /// 対象の型のインスタンスを生成します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <exception cref="System.MemberAccessException"></exception>
        public T CreateInstance(params object[] args)
        {
            return CreateInstanceExact(ReflectionParameter.CreateReflectorParameters(args));
        }

        /// <summary>
        /// 厳密に指定されたパラメータ情報に一致するコンストラクタを使って
        /// 対象の型のインスタンスを生成します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <exception cref="System.MemberAccessException"></exception>
        public T CreateInstanceExact(params ReflectionParameter[] args)
        {
            var constructor = typeof(T).GetConstructor(InstanceFlags, null, args.Select(a => a.Type).ToArray(), null);
            if (constructor == null)
                throw new MemberAccessException();

            var argVals = args.Select(a => a.Value).ToArray();

            var result = constructor.Invoke(argVals);

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Type.IsByRef)
                    args[i].SetValue(argVals[i]);
            }
            return (T)result;
        }

        public MemberInfo[] GetAllMembers()
        {
            return typeof(T).GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        }

        public MemberInfo[] GetPublicStaticMembers()
        {
            return typeof(T).GetMembers(BindingFlags.Public | BindingFlags.Static);
        }

        public MemberInfo[] GetPublicInstanceMembers()
        {
            return typeof(T).GetMembers(BindingFlags.Public | BindingFlags.Instance);
        }

        /// <summary>
        /// バインディングを構築します
        /// </summary>
        /// <returns></returns>
        public BindingReflector Bind()
        {
            return new BindingReflector(BindingFlags.Default, Type, null);
        }

        /// <summary>
        /// 指定された<see cref="BindingReflector"/>を使用して<see cref="AccessReflector"/>を生成して返します
        /// </summary>
        /// <param name="reflector">バインディングオブジェクト</param>
        /// <returns></returns>
        public AccessReflector Bind(BindingReflector reflector)
        {
            return new BindingReflector(reflector.Flags, Type, reflector.InstanceObject).GenerateAccessReflector();
        }

        /// <summary>
        /// 現在のインスタンスに関連づけられている型を取得します
        /// </summary>
        public Type Type
        {
            get
            {
                return typeof(T);
            }
        }
    }
}
