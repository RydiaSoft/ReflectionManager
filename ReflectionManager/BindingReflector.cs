using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;

namespace ReflectionManager
{
    public sealed class BindingReflector : IEquatable<BindingReflector>
    {
        private const string GenerateAccessReflectorError =
            "TypeReflector{T}.Bind()から生成されたBindingReflectorでは無い。" +
            "または現在のインスタンスを生成した際に型オブジェクトを指定してないためこのメソッドは実行できません。" +
            "TypeReflection{T}.Bind(BindingReflector)メソッドを通して生成します";


        private BindingFlags m_Flags;
        private Type m_Type;
        private object m_InstanceObject;

        /// <summary>
        ///   <see cref="BindingReflector"/> classの新しいインスタンスを初期化します
        /// </summary>
        public BindingReflector()
            : this(BindingFlags.Default, null, null)
        {

        }

        /// <summary>
        ///   <see cref="BindingReflector"/> classの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="flags">格納する<see cref="BindingFlags"/></param>
        /// <param name="type">作成する型</param>
        /// <param name="instanceObject">Instanceフラグを設定している場合に使用されるオブジェクト</param>
        public BindingReflector(BindingFlags flags, Type type, object instanceObject)
        {
            m_Flags = flags;
            m_Type = type;
            m_InstanceObject = instanceObject;
        }

        #region メソッド

        private BindingFlags Add(BindingFlags flags)
        {
            if (m_Flags == BindingFlags.Default)
            {
                return flags;
            }
            return m_Flags | flags;
        }

        /// <summary>
        /// 指定された<see cref="BindingFlags"/>のフラグを落とします
        /// </summary>
        /// <param name="flags">flags</param>
        /// <returns></returns>
        public BindingReflector FlagsOff(BindingFlags flags)
        {
            return new BindingReflector(m_Flags & ~flags, m_Type, m_InstanceObject);
        }

        /// <summary>
        /// 現在のバインディング状態で<see cref="AccessReflector"/>を生成して返します
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">
        /// TypeReflector{T}.Bind()から生成されたBindingReflectorでは無い。
        /// または現在のインスタンスを生成した際に型オブジェクトを指定してないためこのメソッドは実行できません。
        /// TypeReflection{T}.Bind(BindingReflector)メソッドを通して生成します
        /// </exception>
        public AccessReflector GenerateAccessReflector()
        {
            if (m_Type == null)
            {
                throw new InvalidOperationException(GenerateAccessReflectorError);
            }
            return new AccessReflector(m_Type, m_InstanceObject, this);
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.Instance"/>が付与され、オブジェクトが関連づけられている
        /// 場合は解除して、<see cref="BindingFlags.Static"/>を付与します
        /// </summary>
        /// <returns></returns>
        public BindingReflector ToStatic()
        {
            var result = this;
            if (HasInstance)
            {
                result = FlagsOff(BindingFlags.Instance);
                result.m_InstanceObject = null;
            }
            return result.Static;
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.Static"/>が付与されている場合は解除し、
        /// 指定されたオブジェクトを関連づけて<see cref="BindingFlags.Instance"/>を付与します
        /// </summary>
        /// <returns></returns>
        public BindingReflector ToInstance(object instanceObject)
        {
            var result = this;
            if (HasStatic)
            {
                result = FlagsOff(BindingFlags.Static);
            }
            return result.SetInstance(instanceObject);
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.Instance"/>を付与し、
        /// 指定されたオブジェクトを関連づけた新しい<see cref="BindingReflector"/>を取得します
        /// インスタンス メンバーを検索に含めるように指定します。
        /// インスタンスを関連づけずに<see cref="BindingFlags.Instance"/>を付与する場合は
        /// <see cref="Instance"/>プロパティを使用します
        /// </summary>
        /// <param name="instanceObject">関連づけるオブジェクト</param>
        /// <returns></returns>
        public BindingReflector SetInstance(object instanceObject)
        {
            return new BindingReflector(Add(BindingFlags.Instance), m_Type, instanceObject);
        }

        public override int GetHashCode()
        {
            return m_Type.GetHashCode();
        }

        public bool Equals(BindingReflector other)
        {
            return (m_Flags == other.m_Flags && m_Type == other.m_Type && m_InstanceObject == other.m_InstanceObject);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is BindingReflector)
                return Equals((BindingReflector)obj);
            return base.Equals(obj);
        }

        /// <summary>
        /// 等価演算子を実装します
        /// </summary>
        /// <param name="left">left</param>
        /// <param name="right">right</param>
        /// <returns></returns>
        public static bool operator ==(BindingReflector left, BindingReflector right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// 不等価演算子を実装します
        /// </summary>
        /// <param name="left">left</param>
        /// <param name="right">right</param>
        /// <returns></returns>
        public static bool operator !=(BindingReflector left, BindingReflector right)
        {
            return !left.Equals(right);
        }

        #endregion

        #region プロパティ

        #region or

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.CreateInstance"/>を付与した新しい<see cref="BindingReflector"/>を取得します
        /// リフレクションが指定した型のインスタンスを作成するように指定します。
        /// 指定した引数と一致するコンストラクターを呼び出します。
        /// 指定したメンバー名は無視されます。
        /// 検索の種類を省略した場合は、(Instance | Public) が適用されます。
        /// タイプ初期化子を呼び出すことはできません。
        /// </summary>
        public BindingReflector CreateInstance
        {
            get
            {
                return new BindingReflector(Add(BindingFlags.CreateInstance), m_Type, m_InstanceObject);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.DeclaredOnly"/>を付与した新しい<see cref="BindingReflector"/>を取得します
        /// 指定した型の階層のレベルで宣言されたメンバーだけが対象になるように指定します。
        /// 継承されたメンバーは対象になりません。
        /// </summary>
        public BindingReflector DeclaredOnly
        {
            get
            {
                return new BindingReflector(Add(BindingFlags.DeclaredOnly), m_Type, m_InstanceObject);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.Default"/>を付与した新しい<see cref="BindingReflector"/>を取得します
        /// バインディング フラグを指定しません。
        /// </summary>
        public BindingReflector Default
        {
            get
            {
                return new BindingReflector(Add(BindingFlags.Default), m_Type, m_InstanceObject);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.ExactBinding"/>を付与した新しい<see cref="BindingReflector"/>を取得します
        /// 指定した引数の型が、対応する仮パラメーターの型と完全に一致する必要があることを指定します。
        /// 呼び出し元が非 null Binder オブジェクトを指定した場合、
        /// これは適切なメソッドをピックする BindToXXX 実装を呼び出し元が提供することを意味するため、
        /// リフレクションは例外をスローします。
        /// </summary>
        public BindingReflector ExactBinding
        {
            get
            {
                return new BindingReflector(Add(BindingFlags.ExactBinding), m_Type, m_InstanceObject);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.FlattenHierarchy"/>を付与した新しい<see cref="BindingReflector"/>を取得します
        /// 階層上位のパブリックおよびプロテクトの静的メンバーを返す場合に指定します。
        /// 継承クラスのプライベートな静的メンバーは返されません。
        /// 静的メンバーには、フィールド、メソッド、イベント、プロパティなどがあります。入れ子にされた型は返されません。
        /// </summary>
        public BindingReflector FlattenHierarchy
        {
            get
            {
                return new BindingReflector(Add(BindingFlags.FlattenHierarchy), m_Type, m_InstanceObject);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.GetField"/>を付与した新しい<see cref="BindingReflector"/>を取得します
        /// 指定したフィールドの値を返すように指定します。
        /// </summary>
        public BindingReflector GetField
        {
            get
            {
                return new BindingReflector(Add(BindingFlags.GetField), m_Type, m_InstanceObject);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.GetProperty"/>を付与した新しい<see cref="BindingReflector"/>を取得します
        /// 指定したプロパティの値を返すように指定します。
        /// </summary>
        public BindingReflector GetProperty
        {
            get
            {
                return new BindingReflector(Add(BindingFlags.GetProperty), m_Type, m_InstanceObject);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.IgnoreCase"/>を付与した新しい<see cref="BindingReflector"/>を取得します
        /// バインディングのときにメンバー名の大文字小文字を区別しないように指定します。
        /// </summary>
        public BindingReflector IgnoreCase
        {
            get
            {
                return new BindingReflector(Add(BindingFlags.IgnoreCase), m_Type, m_InstanceObject);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.IgnoreReturn"/>を付与した新しい<see cref="BindingReflector"/>を取得します
        /// COM 相互運用で使用して、メンバーの戻り値を無視できることを指定します。
        /// </summary>
        public BindingReflector IgnoreReturn
        {
            get
            {
                return new BindingReflector(Add(BindingFlags.IgnoreReturn), m_Type, m_InstanceObject);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.Instance"/>を付与した新しい<see cref="BindingReflector"/>を取得します
        /// インスタンス メンバーを検索に含めるように指定します。
        /// </summary>
        public BindingReflector Instance
        {
            get
            {
                return new BindingReflector(Add(BindingFlags.Instance), m_Type, m_InstanceObject);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.InvokeMethod"/>を付与した新しい<see cref="BindingReflector"/>を取得します
        /// メソッドを呼び出すように指定します。コンストラクターと型初期化子は指定できません。
        /// </summary>
        public BindingReflector InvokeMethod
        {
            get
            {
                return new BindingReflector(Add(BindingFlags.InvokeMethod), m_Type, m_InstanceObject);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.NonPublic"/>を付与付与した新しい<see cref="BindingReflector"/>を取得します
        /// 非パブリック メンバーを検索に含めるように指定します。
        /// </summary>
        public BindingReflector NonPublic
        {
            get
            {
                return new BindingReflector(Add(BindingFlags.NonPublic), m_Type, m_InstanceObject);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.NonPublic"/>を付与した新しい<see cref="BindingReflector"/>を取得します
        /// パラメーター数が指定された引数の数と一致するメンバーのセットを返します。
        /// このバインディング フラグは、既定値を持つパラメーターをとるメソッドと、
        /// 可変個の引数 (varargs) をとるメソッドで使用します。
        /// このフラグを使用するときは、必ず Type.InvokeMember も使用します。
        /// </summary>
        public BindingReflector OptionalParamBinding
        {
            get
            {
                return new BindingReflector(Add(BindingFlags.OptionalParamBinding), m_Type, m_InstanceObject);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.Public"/>を付与した新しい<see cref="BindingReflector"/>を取得します
        /// パブリック メンバーを検索に含めるように指定します。
        /// </summary>
        public BindingReflector Public
        {
            get
            {
                return new BindingReflector(Add(BindingFlags.Public), m_Type, m_InstanceObject);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.PutDispProperty"/>を付与した新しい<see cref="BindingReflector"/>を取得します
        /// COM オブジェクトの PROPPUT メンバーを呼び出す必要があることを指定します。
        /// PROPPUT は、値を使用するプロパティ設定関数を指定します。
        /// プロパティに PROPPUT と PROPPUTREF の両方があり、
        /// どちらを呼び出すかを区別する必要がある場合は、PutDispProperty を使用します。
        /// </summary>
        public BindingReflector PutDispProperty
        {
            get
            {
                return new BindingReflector(Add(BindingFlags.PutDispProperty), m_Type, m_InstanceObject);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.PutRefDispProperty"/>を付与した新しい<see cref="BindingReflector"/>を取得します
        /// COM オブジェクトの PROPPUTREF メンバーを呼び出す必要があることを指定します。
        /// PROPPUTREF は、値ではなく参照を使用するプロパティ設定関数を指定します。
        /// プロパティに PROPPUT と PROPPUTREF の両方があり、どちらを呼び出すかを区別する必要がある場合は、PutRefDispProperty を使用します。
        /// </summary>
        public BindingReflector PutRefDispProperty
        {
            get
            {
                return new BindingReflector(Add(BindingFlags.PutRefDispProperty), m_Type, m_InstanceObject);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.SetField"/>を付与した新しい<see cref="BindingReflector"/>を取得します
        /// 指定したフィールドの値を設定するように指定します。
        /// </summary>
        public BindingReflector SetField
        {
            get
            {
                return new BindingReflector(Add(BindingFlags.SetField), m_Type, m_InstanceObject);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.SetProperty"/>を付与した新しい<see cref="BindingReflector"/>を取得します
        /// 指定したプロパティの値を設定するように指定します。
        /// COM プロパティの場合、このバインディング フラグを指定することは、PutDispProperty と PutRefDispProperty を指定することと同じです。
        /// </summary>
        public BindingReflector SetProperty
        {
            get
            {
                return new BindingReflector(Add(BindingFlags.SetProperty), m_Type, m_InstanceObject);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.Static"/>を付与した新しい<see cref="BindingReflector"/>を取得します
        /// 静的メンバーを検索に含めるように指定します。
        /// </summary>
        public BindingReflector Static
        {
            get
            {
                return new BindingReflector(Add(BindingFlags.Static), m_Type, m_InstanceObject);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.SuppressChangeType"/>を付与した新しい<see cref="BindingReflector"/>を取得します
        /// 実装されていません。
        /// </summary>
        public BindingReflector SuppressChangeType
        {
            get
            {
                return new BindingReflector(Add(BindingFlags.SuppressChangeType), m_Type, m_InstanceObject);
            }
        }

        #endregion

        #region Has

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.CreateInstance"/>が付与されているかどうかを取得します
        /// </summary>
        public bool HasCreateInstance
        {
            get
            {
                return m_Flags.HasFlag(BindingFlags.CreateInstance);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.DeclaredOnly"/>が付与されているかどうかを取得します
        /// </summary>
        public bool HasDeclaredOnly
        {
            get
            {
                return m_Flags.HasFlag(BindingFlags.DeclaredOnly);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.Default"/>が付与されているかどうかを取得します
        /// </summary>
        public bool HasDefault
        {
            get
            {
                return m_Flags.HasFlag(BindingFlags.Default);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.ExactBinding"/>が付与されているかどうかを取得します
        /// </summary>
        public bool HasExactBinding
        {
            get
            {
                return m_Flags.HasFlag(BindingFlags.ExactBinding);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.FlattenHierarchy"/>が付与されているかどうかを取得します
        /// </summary>
        public bool HasFlattenHierarchy
        {
            get
            {
                return m_Flags.HasFlag(BindingFlags.FlattenHierarchy);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.GetField"/>が付与されているかどうかを取得します
        /// </summary>
        public bool HasGetField
        {
            get
            {
                return m_Flags.HasFlag(BindingFlags.GetField);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.GetProperty"/>が付与されているかどうかを取得します
        /// </summary>
        public bool HasGetProperty
        {
            get
            {
                return m_Flags.HasFlag(BindingFlags.GetProperty);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.IgnoreCase"/>が付与されているかどうかを取得します
        /// </summary>
        public bool HasIgnoreCase
        {
            get
            {
                return m_Flags.HasFlag(BindingFlags.IgnoreCase);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.IgnoreReturn"/>が付与されているかどうかを取得します
        /// </summary>
        public bool HasIgnoreReturn
        {
            get
            {
                return m_Flags.HasFlag(BindingFlags.IgnoreReturn);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.Instance"/>が付与されているかどうかを取得します
        /// </summary>
        public bool HasInstance
        {
            get
            {
                return m_Flags.HasFlag(BindingFlags.Instance);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.InvokeMethod"/>が付与されているかどうかを取得します
        /// </summary>
        public bool HasInvokeMethod
        {
            get
            {
                return m_Flags.HasFlag(BindingFlags.InvokeMethod);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.NonPublic"/>が付与されているかどうかを取得します
        /// </summary>
        public bool HasNonPublic
        {
            get
            {
                return m_Flags.HasFlag(BindingFlags.NonPublic);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.OptionalParamBinding"/>が付与されているかどうかを取得します
        /// </summary>
        public bool HasOptionalParamBinding
        {
            get
            {
                return m_Flags.HasFlag(BindingFlags.OptionalParamBinding);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.Public"/>が付与されているかどうかを取得します
        /// </summary>
        public bool HasPublic
        {
            get
            {
                return m_Flags.HasFlag(BindingFlags.Public);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.PutDispProperty"/>が付与されているかどうかを取得します
        /// </summary>
        public bool HasPutDispProperty
        {
            get
            {
                return m_Flags.HasFlag(BindingFlags.PutDispProperty);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.PutRefDispProperty"/>が付与されているかどうかを取得します
        /// </summary>
        public bool HasPutRefDispProperty
        {
            get
            {
                return m_Flags.HasFlag(BindingFlags.PutRefDispProperty);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.SetField"/>が付与されているかどうかを取得します
        /// </summary>
        public bool HasSetField
        {
            get
            {
                return m_Flags.HasFlag(BindingFlags.SetField);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.SetProperty"/>が付与されているかどうかを取得します
        /// </summary>
        public bool HasSetProperty
        {
            get
            {
                return m_Flags.HasFlag(BindingFlags.SetProperty);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.Static"/>が付与されているかどうかを取得します
        /// </summary>
        public bool HasStatic
        {
            get
            {
                return m_Flags.HasFlag(BindingFlags.Static);
            }
        }

        /// <summary>
        /// 現在のインスタンスに<see cref="BindingFlags.SuppressChangeType"/>が付与されているかどうかを取得します
        /// </summary>
        public bool HasSuppressChangeType
        {
            get
            {
                return m_Flags.HasFlag(BindingFlags.SuppressChangeType);
            }
        }

        #endregion

        /// <summary>
        /// 現在のインスタンスに関連づけられているオブジェクトを取得します
        /// </summary>
        public object InstanceObject
        {
            get
            {
                return m_InstanceObject;
            }
        }

        /// <summary>
        /// 現在のインスタンスに関連づけられている型を取得します
        /// </summary>
        public Type Type
        {
            get
            {
                return m_Type;
            }
        }

        /// <summary>
        /// 現在のインスタンスの<see cref="BindingFlags"/>を取得または設定します
        /// </summary>
        public BindingFlags Flags
        {
            get
            {
                return m_Flags;
            }
            set
            {
                m_Flags = value;
            }
        }

        #endregion

    }

}
