using IT.Tangdao.Framework;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using IT.Tangdao.Framework.Ioc;
using IT.Tangdao.Framework.Attributes;
using IT.Tangdao.Framework.Bootstrap;

// 常规信息
[assembly: AssemblyTitle("Tangdao.Framework")]
[assembly: AssemblyProduct("Tangdao.Framework")]
[assembly: ComVisible(false)]

// 模块自注册（关键一行）
[assembly: TangdaoModule(typeof(FrameworkDefaultComponentModule))]
[assembly: TangdaoModule(typeof(AutoRegisterComponentModule))]