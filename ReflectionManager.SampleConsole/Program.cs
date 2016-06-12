using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReflectionManager.SampleConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //比較用のテストオブジェクト
            var target = new SampleObject()
            {
                Id = 10,
                Name = "Rydia",
                Address = "Japan",
                Scores = Enumerable.Range(1, 5).ToList(),
                MyEnum = SampleEnum.Sample2
            };

            //型のリフレクターをまず作成します。
            //オブジェクト生成しなければTypeReflectorを作成しなくても可能です。
            //今回はオブジェクト生成するので作成してます。
            var testReflector = new TypeReflector<SampleObject>();
            //オブジェクトの生成: パラメータを指定すれば自動的に検索します。
            //今回はデフォルトコンストラクタなので引数無しです。
            var obj = testReflector.CreateInstance();

            //バインド
            var instanceReflector = testReflector.Bind().Public.NonPublic.SetInstance(obj).GenerateAccessReflector();
            //この書き方でも可能(やってることは全く同じ)
            //var binding = new BindingReflector().Public.NonPublic.SetInstance(obj);
            //var instanceReflector = testReflector.Bind(binding);

            //さらにこういう書き方も可能
            //var instanceFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            //var binding = new BindingReflector(instanceFlags, null, obj);
            //var instanceReflector = testReflector.Bind(binding);

            //型生成しなければTypeReflectorは必要無いので以下の形でも書けます。
            //var instanceFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            //var binding = new BindingReflector(instanceFlags, typeof(SampleObject), obj);
            //var instanceReflector = binding.GenerateAccessReflector();

            //AccessReflectorを作成したらあとはアクセスするだけ。
            //キャストも不要です。
            instanceReflector.Property<int>("Id").Value = 10;
            instanceReflector.Property<string>("Name").Value = "Rydia";
            instanceReflector.Property<string>("Address").Value = "Japan";
            instanceReflector.Property<List<int>>("Scores").Value = Enumerable.Range(1, 5).ToList();
            instanceReflector.Field<SampleEnum>("MyEnum").Value = SampleEnum.Sample2;
            //Valueはプロパティにしてあるため、読み書き両方対応してあります。
            //とはいえ当然ながら読み取り専用プロパティに対してセッター呼び出せば
            //例外がスローされるのできちんとリファレンスで該当メンバの確認をしてください。

            Equals(target, obj, "trueなら成功");

            //プライベートメンバにもアクセスできます
            //(このサンプルではパブリックメンバ・ノンパブリックメンバをバインドしてるため)
            instanceReflector.Field<int>("m_Value").Value = 300;

            Equals(target, obj, "falseなら成功");

            //メソッドの呼び出しも可能です
            var result = instanceReflector.Method<bool>("Equals", target).Invoke();

            Console.WriteLine("メソッドの戻り値: {0} :falseなら成功", result);

            //つまりAccessReflectorクラスはその型オブジェクトとほぼ同じと思ってください。
            //一度メンバを取得すれば、以後キャッシュされたInfoクラスを使用するので
            //たとえば、前に呼び出したNameプロパティに対して
            instanceReflector.Property<string>("Name").Value = "Rydia0000";
            instanceReflector.Property<string>("Name").Value = "Rydia1111";
            instanceReflector.Property<string>("Name").Value = "Rydia";
            //と複数回アクセスしても、再度メンバー検索したりすることはありません。
            //そのうち式木できちんと実装したいと思います。

            //もちろんStaticメンバも可能
            var staticReflector = testReflector.Bind().Public.NonPublic.Static.GenerateAccessReflector();
            //なお、以下の方法でも可能
            //var staticReflector = instanceReflector.ToStatic();
            //逆にStatic用のAccessReflectorからインスタンスの関連づけも可能です。
            //instanceReflector = staticReflector.ToInstance(obj);
            //インスタンス対象オブジェクトの差し替えも可能です。
            //var instanceReflector2 = instanceReflector.ToInstance(target);

            var number = staticReflector.Property<int>("Number"); // 値を取得

            WriteNumber(number);        //比較表示

            number.Value = 1000;        //値の設定

            WriteNumber(number);        //比較表示

            //なお、FieldInfoやPropertyInfo,MethodInfoなどを取得したい場合
            //FieldReflector, PropertyReflector, MethodReflector,IndexerReflectorは内部で各Infoクラスを
            //保持しているので各ReflectorのGet○○Infoメソッドで取得できます。
            //たとえば上記numberプロパティのinfoを取得
            var info = number.GetPropertyInfo();
            Console.WriteLine("プロパティ名: {0}", info.Name);

            //またインデクサはPropertyReflectorを使用してますが、
            //通常のC#プロパティとは異なりインデックスを指定するので
            //AccessReflector.Indexerメソッド
            //AccessReflector.IndexerExactメソッドから取得します。
            //戻り値はPropertyReflectorなのでValueプロパティで読み書きできます。

            //なおまだαバージョンなので、メンバの検索とかの実装は今後アプデしていきます。。

            Console.WriteLine();
            Console.WriteLine("キーを押すと終了します");
            Console.WriteLine();
            Console.ReadKey();

        }

        private static void WriteNumber(PropertyReflector<int> number)
        {
            Console.WriteLine("Number = {0}, WrapperNumber = {1}", number.Value, SampleObject.WrapperNumber);
        }

        private static void Equals(SampleObject left, SampleObject right, string message)
        {
            var equals = left.Equals(right);
            Console.WriteLine("Equals[1]: {0} : {1}", equals, message);
            Console.WriteLine();
            if (!equals)
            {
                Console.WriteLine("//-----------------------------------------------------------------");
                Console.WriteLine("//Equalseメソッドがfalseだったので型の内容を表す文字列を表示します");
                Console.WriteLine("//-----------------------------------------------------------------");
                Console.WriteLine("//Left");
                Console.WriteLine("//-----------------------------------------------------------------");
                Console.WriteLine(left.ToString());
                Console.WriteLine("//-----------------------------------------------------------------");
                Console.WriteLine("//Right");
                Console.WriteLine("//-----------------------------------------------------------------");
                Console.WriteLine(right.ToString());
            }
        }

    }
}
