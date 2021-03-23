using System;

namespace Permaisuri.Controllers.ControllerExt
{

    public class RecordAttribute : Attribute
    {
        private readonly string _recordType; // 记录类型：更新/创建   
        private readonly string _author; // 作者   

        // 构造函数，构造函数的参数在特性中也称为“位置参数”。   
        public RecordAttribute(string recordType, string author, string date)
        {
            //Positional Parameter
            _recordType = recordType;
            _author = author;
        }

        // 对于Positional Parameter，通常只提供get访问器    
        public string RecordType
        {
            get { return _recordType; }
        }

        public string Author
        {
            get { return _author; }
        }

        // 构建一个属性，在特性中也叫“Named Parameter”   
        public string Memo { get; set; }
    }

    /*微软的软件工程师们就想到了这样的办法：不管是构造函数的参数 还是 属性，统统写到构造函数的圆括号中，
     * 对于构造函数的参数，必须按照构造函数参数的顺序和类型；对于属性，采用“属性=值”这样的格式，它们之间
     * 用逗号分隔。于是上面的代码就减缩成了这样：
     * [AttributeUsage(AttributeTargets.Class, AllowMutiple=true, Inherited=false)]*/

}