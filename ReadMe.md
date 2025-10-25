

#### 1、命令

正常命令：TangdaoCommand   

异步命令：TangdaoAsyncCommand

静态命令：MinidaoCommand

###### 1-1、用法

```C#
TangdaoCommand  taodao=new TangdaoCommand(()=>{});
TangdaoAsyncCommand taodaoAsync=new TangdaoAsyncCommand(async () => { });
MinidaoCommand.Create(()=>{});
MinidaoCommand.CreateFromTask(async () => { });
```

###### 1-2、集成快捷命令

```
//当ViewModel继承DaoViewModelBase的时候，可以快速创建命令，省去new TangdaoCommand();
HomeViewModel : DaoViewModelBase

public ICommand SaveCommand => this.Cmd(Execute);
```



#### 2、事件聚合器 

IDaoEventAggregator

###### 2-1、用法

```C#
 public IDaoEventAggregator _daoEventAggregator;

_daoEventAggregator.Publish<T>();

_daoEventAggregator.Subscribe<T>(Execute);

T:DaoEventBase
```

#### 3、IOC容器

###### 3-1、容器ITangdaoContainer

修改启动项

```C#
public partial class App : TangdaoApplication
 {
     protected override void RegisterServices(ITangdaoContainer container)
     {
         container.AddTangdaoSingleton<MainView>();
         container.AddTangdaoSingleton<MainViewModel>();
         container.AddTangdaoSingleton<HomeViewModel>();
         container.AddTangdaoSingleton<IReadService, ReadService>();
     }

     protected override Window CreateWindow()
     {
         return CreateShell<MainView>();
     }
 }
```

除了单例注册外，还可以瞬态注册，工厂注册，Key值注册

```C#

 container.AddTangdaoScoped<T>();
 container.AddTangdaoTransient<T>();
 container.AddTangdaoSingleton<T>();
 container.AddKeyedTransient<T>();
 container.AddTangdaoSingletonFactory<IWeatherService>(provider =>
new WeatherService(provider.GetService<ITangdaoLogger>(), provider.GetService<IConfig>()));
```

###### 3-2、解析器ITangdaoProvider

```
Provider.GetService<T>();
```

###### 3-3、服务定位器

```C#
TangdaoApplication.Provider.GetService(viewModel);
```

#### 4、插件式注册

###### 4-1、对于.Netframwork版本继承TangdaoModuleBase

```C#
 public class DemoModule : TangdaoModuleBase
 {
     public override void RegisterServices(ITangdaoContainer container)
     {
         container.AddKeyedSingleton<IReadContentService, ReadConfigService>("config");
         container.AddKeyedSingleton<IReadContentService, ReadJsonService>("json");
         container.AddKeyedSingleton<IReadContentService, ReadXmlService>("xml");
     }

     public override void OnInitialized(ITangdaoProvider provider)
     {
         base.OnInitialized(provider);
     }
 }
```

###### 4-2、对于.NetCore版本继承ITangdaoModule

```C#
 public class DemoModule : ITangdaoModule
 {
     public override void RegisterServices(ITangdaoContainer container)
     {
         container.AddKeyedSingleton<IReadContentService, ReadConfigService>("config");
         container.AddKeyedSingleton<IReadContentService, ReadJsonService>("json");
         container.AddKeyedSingleton<IReadContentService, ReadXmlService>("xml");
     }

     public override void OnInitialized(ITangdaoProvider provider)
     {
         base.OnInitialized(provider);
     }
 }
```



#### 5、常用文件的读写

###### 5-1、对XML文件的读写

xml文件：

```xml
<?xml version="1.0" encoding="utf-8"?>
<LoginDto xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <UserName>Admin</UserName>
  <Password>123</Password>
  <IsRemember>true</IsRemember>
  <IsAdmin>true</IsAdmin>
  <Role>管理员</Role>
</LoginDto>
```

cs代码：

```C#
 string foldPath = Path.Combine(IgniteInfoLocation.Cache, "LoginInfo.xml");
 var isRememberValue = _readService.Default.Read(foldPath).AsXml().SelectNode("IsRemember").Value;
```

支持XML的序列化和反序列化

```C#
 Student student=XmlFolderHelper.Deserialize<Student>(xmlContent);
 string xml=XmlFolderHelper.SerializeXML<Student>(student);
```

也可以使用缓存模式序列化，只要读过本地的文件，就可以使用缓存

```C#
_readService.Cache.DeserializeCache<LoginDto>(foldPath.Value, DaoFileType.Xml);
```



###### 5-2、对Config文件的读写

1、读取默认的App.config配置

```C#
<configuration>
	<configSections>
		<section name="Menu" type="System.Configuration.DictionarySectionHandler" />
		<section name="Student" type="System.Configuration.DictionarySectionHandler" />
	</configSections>
	<Menu>
		<add key="0" value="我的样本" />
		<add key="1" value="动态记录" />
		<add key="2" value="存储" />
		<add key="3" value="实验" />
	</Menu>
	<Student>
		<add key="Id" value="1" />
		<add key="Name" value="张三" />
		<add key="Age" value="18" />
		<add key="Source" value="18" />
	</Student>
</configuration>
```

cs代码：使用SelectAppConfig

```C#
 readService.Default.AsConfig().SelectAppConfig(HandlerName);
```

也可以直接将读取到的数据转成List或ObservableCollection

```C#
readService.Default.AsConfig().SelectAppConfig(HandlerName).ToList(v => new TangdaoMenuItem { MenuName = v }).ToObservableCollection();
```



2、读取自定义配置config文件

对于.Framework版本

```C#
<section name="Tangdao" type="IT.Tangdao.Framework.Common.TangdaoMenuSection,IT.Tangdao.Framework" />
```

对于.NetCore版本

```C#
<section name="Tangdao" type="IT.Tangdao.Core.Common.TangdaoMenuSection,IT.Tangdao.Core" />
```



```C#

<Tangdao>
	<menus>
		<add title="首页" value="DefaultViewModel" />
		<add title="用户信息" value="UserInfoViewModel" />
		<add title="设置" value="SetViewModel" />
		<add title="监控" value="MonitorViewModel" />
		<add title="维护" value="MaintionViewModel" />
		<add title="配方" value="RecipeViewModel" />
		<add title="参数" value="ParameBaseViewModel" />
	</menus>
</Tangdao>
```

cs代码：使用SelectCustomConfig

```C#
var responseResult = readService.Default.AsConfig().SelectCustomConfig(readTitle, section);
```

```C#
 if (responseResult.Payload is Dictionary<string, string> data)
 {
     List<HomeMenuItem> menuItems = data.Select(kvp => new HomeMenuItem
     {
         Title = kvp.Key,
         ViewModelName = kvp.Value
     }).ToList();
     return new ReadOnlyCollection<HomeMenuItem>(menuItems);
 }
```



###### 5-3、对Json文件的读写

```C#
 var json = readService.Current.SelectValue(key);
```



###### 5-4、对ini文件的读写

###### 5-5、便捷式读写文件

注册接口，然后读取

```c#
  string path = "E://Temp//Student.xml";
  Student stu= await _readService.ReadXmlToEntityAsync<Student>(path,DaoFileType.Xml);
```

###### 5-6、支持异步读写

```C#
  await _writeService.WriteAsync("E://Temp//100.txt","HelloWorld");

  await _readService.ReadAsync("E://Temp//100.txt");
```

对各种文件子节点的读取

```C#
<?xml version="1.0" encoding="utf-8"?>
<Student target="学生">
	<Id target="009">1</Id>
	<Age>18</Age>
	<Name>李四</Name>
</Student>
```

实体类为

```c#
 [XmlRoot("Student")]
 public class Student
 {
     [XmlAttribute("target")]
     public string Target { get; set; }

     [XmlElement("Id")]
     public int Id { get; set; }

     [XmlElement("Age")]
     public int Age { get; set; }

     [XmlElement("Name")]
     public string Name { get; set; }
    
 }
```

使用SelectNode读取xml节点

例如xml文档如下

```C#
<?xml version="1.0" encoding="utf-8"?>
<UserInfo>
  <Login Id="0">
    <UserName>Admin</UserName>
    <Password>2</Password>
    <Role>管理员</Role>
    <IsAdmin>True</IsAdmin>
    <IP>192.168.0.1</IP>
  </Login>
  <Register Id="1">
    <UserName>Ad</UserName>
    <Password>12</Password>
    <Role>普通用户</Role>
    <IsAdmin>False</IsAdmin>
    <IP>127.0.0.1</IP>
  </Register>
</UserInfo>
```

使用方式

```C#
// 正确调用（多节点必须指定索引）
var ip1 = _readService.Current[1].SelectNode("IP").Value;

// 错误调用（多节点未指定索引）
var ip2 = _readService.Current.SelectNode("IP").Value; 
// 返回错误："存在多个节点，请指定索引"

// 正确调用（单节点可不指定索引）
var ip3 = _readService.Current.SelectNode("IP").Value; 
```

优化繁琐的读取,不需要知道类的所有属性

```C#
  var readResult = _readService.Current.SelectNodes("ProcessItem", x => new ProcessItem
  {
      Name = x.Element("Name")?.Value,
      IsFeeding = (bool)x.Element("IsFeeding"),
      IsBoardMade = (bool)x.Element("IsBoardMade"),
      IsBoardCheck = (bool)x.Element("IsBoardCheck"),
      IsSeal = (bool)x.Element("IsSeal"),
      IsSafe = (bool)x.Element("IsSafe"),
      IsCharge = (bool)x.Element("IsCharge"),
      IsBlanking = (bool)x.Element("IsBlanking"),
  });

```

直接通过反射+泛型

```C#
 var readResult = _readService.Current.SelectNodes<ProcessItem>();
```

#### 6、扩展

###### 6-1、基于ViewModel命名约定的隐式键展开容器字典

```C#
 public class Test
 {
     public static MultiKeyDictionary<string> keyValuePairs = new MultiKeyDictionary<string>();

     public static MultiKeyDictionary<string> Print()
     {
         keyValuePairs["Sample"] = "Test";
         //keyValuePairs["SampleView"] = "Test";
         // keyValuePairs["SampleViewModel"] = "Test";

         return keyValuePairs;
     }
 }
```

内部跟据View和ViewModel后缀自动绑定字典

增加自定义排序字典TangdaoSortedDictionary，以及对它的扩展



###### 6-2、增加另外一种全新的方式去发送数据

```C#
MainViewModel: 发送
private void Execute()
{
      ITangdaoParameter tangdaoParameter = new TangdaoParameter();
      tangdaoParameter.Add("001",Name);
      this.RunSameLevelWindowAsync<LoginView>(tangdaoParameter);
}
LoginViewModel:接收
 public void Response(ITangdaoParameter tangdaoParameter)
 {
     Name = tangdaoParameter.Get<string>("001");
 }
```



对PLC的读取进行了扩展未完成

```c#
  container.RegisterPlcServer(plc => 
  {
      plc.PlcType= PlcType.Siemens;
      plc.PlcIpAddress = "127.0.0.1";
      plc.Port = "502";

  });

  container.RegisterType<IPlcReadService,PlcReadService>();
  var plcservice=provider.Resolve<IPlcReadService>();
  plcservice.ReadAsync("DM200");
```





```C#
var maybe = TangdaoOptional<string>.Some("Hello")
                                   .Where(s => s.Length > 3)
                                   .Select(s => s.ToUpper())
                                   .ValueOrDefault("NONE");

Console.WriteLine(maybe);   // HELLO
```



#### 7、组件通信

```C#
//同级别窗体通信 
//tangdaoParameter为发送的数据，可以在打开窗体的时候直接发送数据过去
this.RunSameLevelWindowAsync<LoginView>(tangdaoParameter);

//父子窗体通信
this.RunChildWindowAsync<LoginView>();
```

###### 7-1、线程之间的数据传输 AmbientContext

```C#
//值类型使用
AmbientContext.Set(123);
AmbientContext.Get<int>();

//引用类型使用
Student student=new Student(1,"张三");
AmbientContext.SetCurrent(student);
AmbientContext.Get<Student>();

//按指定Key值传输数据
AmbientContext.SetCurrent("学生",student);
AmbientContext.Get<Student>("学生");

```

###### 7-2、进程之间的数据传输 TangdaoContext

功能更加强大，可以进行数据传输，委托传输，字典传输，命令传输

```C#
TangdaoContext.SetTangdaoParameter<T>();

TangdaoContext.GetTangdaoParameter<T>();
```

###### 7-3、Socket通信

#### 8、日志DaoLogger

日志默认是写在桌面上的

```C#
 private static readonly ITangdaoLogger Logger = TangdaoLogger.Get(typeof(Bootstrapper));
 Logger.WriteLocal($"注册成功");
```

也可以自定义配置日志的路径：

下面代码放在启动项即可

```C#
 LogPathConfig.SetRoot($@"{IgniteInfoLocation.Logger}");
```

#### 9、自动生成器

可以自动生成虚假数据，用于平时调试

在WPF可以这样使用

```C#
public class MainWindowViewModel : BindableBase
 {
     private ObservableCollection<Student> _students;

     public ObservableCollection<Student> Students
     {
         get => _students;
         set => SetProperty(ref _students, value);
     }

     public MainWindowViewModel()
     {
         Loaded();
     }

     private void Loaded()
     {
         var generator = new DaoFakeDataGeneratorProvider<Student>();
         List<Student> randomStudents = generator.GenerateRandomData(10);
         Students = new ObservableCollection<Student>(randomStudents);
     }
 }
 public class Student
 {
     public int Id { get; set; }

     [DaoFakeDataInfo("姓名")] // 使用 DaoName 枚举生成姓名
     public string Name { get; set; }

     public int Age { get; set; }

     [DaoFakeDataInfo("爱好")] // 使用 DaoHobby 枚举生成爱好
     public string Hobby { get; set; }

     [DaoFakeDataInfo("城市")] // 使用 ChineseCities 数组生成城市
     public string Address { get; set; }
 }
```



#### 10、增加路由导航

###### 1、简单的导航，具有翻页功能ISingleRouter

使用方式，与ISingleNavigateView配合使用

使用IOC容器注册所有的视图

XAML Code：

```
 <!--  动态内容区  -->
 <ContentControl
     HorizontalContentAlignment="Stretch"
     VerticalContentAlignment="Stretch"
     s:View.Model="{Binding CurrentView}" />

 <!--  智能控制栏  -->
 <Border
     Grid.Row="1"
     Padding="10"
     Background="{DynamicResource {x:Static SystemColors.ControlBrushKey}}">
     <StackPanel HorizontalAlignment="Center" Orientation="Horizontal">
         <!--  导航按钮  -->
         <Button
             Width="100"
             Command="{s:Action Previous}"
             Content="◄ 上一页"
             IsEnabled="{Binding CanPrevious}" />

         <!--  自动轮播开关 Mode=OneWay允许 UI 反映状态，但禁止 UI 修改状态  -->
         <ToggleButton
             Width="200"
             Margin="20,0"
             Background="{Binding IsAutoRotating, Converter={StaticResource BoolToColorConverter}}"
             Command="{s:Action ToggleAutoCarousel}"
             Content="{Binding AutoRotateStatusText}"
             IsChecked="{Binding IsAutoRotating, Mode=OneWay}" />

         <!--  导航按钮  -->
         <Button
             Width="100"
             Command="{s:Action Next}"
             Content="下一页 ►"
             IsEnabled="{Binding CanNext}" />
     </StackPanel>
 </Border>
```

CS Code：

```
 public class GlobalPhotoViewModel : Screen
 {
     private readonly ISingleRouter _router;

     public ISingleNavigateView CurrentView => _router.CurrentView;
     public bool CanPrevious => _router.CanPrevious;
     public bool CanNext => _router.CanNext;
     public bool IsAutoRotating => _router.IsAutoRotating;
     public string AutoRotateStatusText => _router.IsAutoRotating ? "自动轮播开启中" : "自动轮播已禁用";

     public GlobalPhotoViewModel(ISingleRouter router)
     {
         _router = router;
         _router.PropertyChanged += OnRouterPropertyChanged;
         _router.NavigationChanged += OnRouterNavigationChanged;
     }

     public void Previous()
     {
         _router.Previous();
     }

     public void Next()
     {
         _router.Next();
     }

     public void ToggleAutoCarousel() => _router.ToggleAutoCarousel();

     private void OnRouterPropertyChanged(object sender, PropertyChangedEventArgs e)
     {
         // 将路由器的属性变化转发到视图模型
         NotifyOfPropertyChange(e.PropertyName);

         if (e.PropertyName == nameof(ISingleRouter.IsAutoRotating))
         {
             NotifyOfPropertyChange(nameof(AutoRotateStatusText));
         }
     }

     private void OnRouterNavigationChanged(object sender, EventArgs e)
     {
         NotifyOfPropertyChange(nameof(CurrentView));
         NotifyOfPropertyChange(nameof(CanPrevious));
         NotifyOfPropertyChange(nameof(CanNext));
     }

     protected override void OnDeactivate()
     {
         _router.IsAutoRotating = false;
         base.OnDeactivate();
     }
 }
```



###### 2、工业级别导航，具有拦截器ITangdaoRouter

使用时与ITangdaoPage配合

XAML Code：

```C#
  <!--  路由视图容器  -->
  <ContentControl Grid.Row="0" Content="{Binding Router.CurrentView}" />

  <!--  导航控制  -->
  <StackPanel
      Grid.Row="1"
      HorizontalAlignment="Right"
      Orientation="Horizontal">

      <Button
          Margin="2"
          Command="{Binding GoBackCommand}"
          Content="◄"
          IsEnabled="{Binding Router.CanGoBack}"
          ToolTip="上一页" />
      <Button
          Margin="2"
          Command="{Binding GoForwardCommand}"
          Content="►"
          IsEnabled="{Binding Router.CanGoForward}"
          ToolTip="下一页" />
      <Button
          Margin="5"
          Command="{s:Action GoToVacuumGaugeView}"
          Content="真空表" />
      <Button
          Margin="5"
          Command="{s:Action GoToDigitalSmartGaugeView}"
          Content="数字智能测量仪" />
  </StackPanel>
```

CS Code:

```
 public class PressureViewModel : BaseDeviceViewModel, IRouteComponent
 {
     public ITangdaoRouter Router { get; set; }
     public IContainer _container;

     public PressureViewModel(ITangdaoRouter router, IContainer container) : base("Pressure")
     {
         Router = router;
         _container = container;
         Router.RouteComponent = this;
         Router.RegisterPage<DigitalSmartGaugeViewModel>();
         Router.RegisterPage<DifferentialGaugeViewModel>();
         Router.RegisterPage<VacuumGaugeViewModel>();
         GoBackCommand = MinidaoCommand.Create(ExecuteGoBack);
         GoForwardCommand = MinidaoCommand.Create(ExecuteGoForward);
     }

     private void ExecuteGoForward()
     {
         Router.GoForward();
     }

     private void ExecuteGoBack()
     {
         Router.GoBack();
     }

     public ICommand GoBackCommand { get; set; }
     public ICommand GoForwardCommand { get; set; }

     public void GoToDigitalSmartGaugeView()
     {
         Router.NavigateTo<DigitalSmartGaugeViewModel>();
     }

     public void GoToVacuumGaugeView()
     {
         Router.NavigateTo<VacuumGaugeViewModel>();
     }

     protected override void OnViewLoaded()
     {
         base.OnViewLoaded();
     }

     public ITangdaoPage ResolvePage(string route)
     {
         var result = _container.Get<ITangdaoPage>(route);
         return result;
     }
 }
```



#### 11、时间轮

```C#
class Program
{
    static async Task Main(string[] args)
    {
        // 创建时间轮实例
        var timeWheel = new TimeWheel<Student>();
        timeWheel.Start(); // 启动时间轮
        
        // 创建几个学生
        var student1 = new Student { Id = 1, Name = "张三", Grade = 3 };
        var student2 = new Student { Id = 2, Name = "李四", Grade = 2 };
        var student3 = new Student { Id = 3, Name = "王五", Grade = 1 };
        
        Console.WriteLine($"当前时间: {DateTime.Now:HH:mm:ss}");
        
        // 添加任务：5秒后打印学生信息
        await timeWheel.AddTaskAsync(5, student1, async s => 
        {
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} - 处理学生1: {s}");
            await Task.Delay(100); // 模拟异步工作
        });
        
        // 添加任务：10秒后升级学生年级
        await timeWheel.AddTaskAsync(10, student2, async s => 
        {
            s.Grade++;
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} - {s.Name}升级到{s.Grade}年级");
            await Task.Delay(100);
        });
        
        // 添加任务：15秒后发送通知
        await timeWheel.AddTaskAsync(15, student3, async s => 
        {
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} - 发送通知给{s.Name}的家长");
            await Task.Delay(100);
        });
        
        // 防止程序退出
        Console.ReadLine();
    }
}
```



#### 12、文本监控

在程序启动时注册事件

```C#
  protected override void OnLaunch()
  {
      base.OnLaunch();
      // 启动监控服务
      var monitorService = Container.Get<IMonitorService>();
      monitorService.FileChanged += OnFileChanged;
      monitorService.StartMonitoring();
  }

  private void OnFileChanged(object sender, DaoFileChangedEventArgs e)
  {
      Logger.WriteLocal($"XML 文件变化: {e.FilePath}, 变化类型: {e.ChangeType}，变化详情：{e.ChangeDetails}，old:{e.OldContent},new:{e.NewContent}");
  }
```

注册代码

```C#
 // 注册配置
 Bind<FileMonitorConfig>().ToFactory(container =>
 {
     return new FileMonitorConfig
     {
         MonitorRootPath = @"E:\IgniteDatas\",
         IncludeSubdirectories = true,
         MonitorFileTypes = new List<DaoFileType>
         {
             DaoFileType.Xml,
            // DaoFileType.Config,
           //  DaoFileType.Json
         },
         DebounceMilliseconds = 800,
         FileReadRetryCount = 3
     };
 }).InSingletonScope();

 // 注册监控服务
 Bind<IMonitorService>().To<FileMonitorService>().InSingletonScope();
```

#### 13、任务调度器

```C#
  TangdaoTaskScheduler.Execute(dao: daoTask =>
  {
      
  });

         
  TangdaoTaskScheduler.Execute(daoAsync: daoTask =>
  {
     
  });

  TangdaoTaskScheduler.Execute(daoAsync => { }, dao => { });
```

#### 14、Markup的扩展

###### 1、对Combobox进行定制列表

```
public class ComboboxOptions
{
    public static void SetTheme()
    {
        OptionListExtension.OptionsPool["Accuracy"] = new[] { "X1", "Y1", "X2", "Y2" };
    }
}
```

在启动时自定义设置自己需要的字典

然后当使用Combobox的下拉列表时候，可缓存直接使用，当多个视图都需要使用同一个下拉列表的时候非常方便

```
  xmlns:markup="clr-namespace:IT.Tangdao.Core.Markup;assembly=IT.Tangdao.Core"
   <ComboBox x:Name="com" ItemsSource="{markup:OptionList Key=Accuracy}"  />
```

并且你什么都不写的时候我具有缺省默认数据

```
 OptionsPool["Default"] = new[] { "全部", "Load", "Upload" };
```

如果你不想在程序启动使用代码复用，也可以直接写

```
<ComboBox ItemsSource="{markup:OptionList Values='X1,Y1,X2,Y2'}" />
```



###### 2、 Bool-String转换器，可以指定显示结果

常规的bool转string需要在ViewModel设置想要动态改变还需要写触发器,我可以自己指定

```
 <TextBlock Text="{marup:BoolToStringMode Binding={Binding BoolValue}, FalseValue='失败', TrueValue='成功'}" />
```

###### 3、列表绑定枚举时使用

```c#
  <daoMarkup:EnumBindSource x:Key="plcType" EnumType="{x:Type shared:PlcType}" />
```



###### 4、获取当前控件所在的Window实例

```C#
  <Button
      Width="100"
      Height="30"
      Margin="20,20,20,-100"
      HorizontalAlignment="Center"
      Background="{StaticResource OceanBlueBrush}"
      Command="{s:Action ExecuteCancel}"
      CommandParameter="{markup:GetWindow}"
      Content="取消"
      IsCancel="True" />
```

###### 6、列表中的按钮绑定

在一般情况下绑定比较复杂

对比如下：

```C#
<DataGridTemplateColumn Width="200" Header="操作">
    <DataGridTemplateColumn.CellTemplate>
        <DataTemplate>
            <StackPanel
               HorizontalAlignment="Center"
               VerticalAlignment="Center"
               Orientation="Horizontal">
                <Button
                   Width="70"
                   Margin="0,0,15,0"
                   Background="LightGreen"
                   Command="{markup:AncestorBinding Path=DataContext.UpdateUserCommand,AncestorType=UserControl}"
                   CommandParameter="{Binding Id}"
                   Content="修改"
                   FontSize="14"
                   Foreground="Black" />
                <Button
                   Width="70"
                   Background="Red"
                   Command="{Binding DataContext.DeleteUserCommand, RelativeSource={RelativeSource Mode=FindAncestor, AncestorType=DataGrid}}"
                   CommandParameter="{Binding Id}"
                   Content="删除"
                   FontSize="14"
                   Foreground="Black" />
            </StackPanel>
        </DataTemplate>
    </DataGridTemplateColumn.CellTemplate>
</DataGridTemplateColumn>
```

#### 15、路径处理

| 场景                                       | 用途                                 | 如何使用                                                     |
| ------------------------------------------ | ------------------------------------ | ------------------------------------------------------------ |
| **加载本地资源文件**（如JSON、图片、配置） | 避免硬编码路径，支持相对解决方案路径 | `TangdaoPath.Instance.Solution().Combine("Assets", "Icons", "logo.png").Build()` |
| **日志文件输出路径**                       | 确保日志写入到临时目录或指定目录     | `TangdaoPath.Instance.Temp().Combine("logs", "app.log").Build()` |
| **用户导出/保存文件**                      | 提供默认路径，避免平台差异           | `TangdaoPath.Instance.GetEnvironmentDirectory("EXPORT_DIR").Combine("report.xlsx")` |
| **插件加载路径**                           | 动态加载插件DLL，避免路径错误        | `TangdaoPath.Instance.Solution().Combine("Plugins", "PluginA.dll").Build()` |
| **配置文件路径**                           | 支持CI/CD中配置路径注入              | `TangdaoPath.Instance.GetThisFilePath().Parent().Combine("settings.json")` |
| **单元测试中模拟路径**                     | 避免使用真实文件系统                 | 使用 `AbsolutePath` 和 `RelativePath` 构造虚拟路径，不依赖磁盘 |

###### 1、WPF中加载图片资源

```C#
// ViewModel 或代码背后
var imagePath = TangdaoPath.Instance
    .Solution()
    .Combine("Assets", "Images", "user_avatar.png")
    .Build();

if (imagePath.FileExists)
{
    AvatarImageSource = new BitmapImage(new Uri(imagePath.Value));
}
```

###### 2、使用 `PathTemplate` 动态生成路径

```C#
var template = PathTemplate.Create("{Solution}/Exports/{UserId}/{Date}/report.xlsx");

var path = template.Resolve(new
{
    Solution = TangdaoPath.Instance.GetSolutionDirectory().Value,
    UserId = 12345,
    Date = DateTime.Now.ToString("yyyy-MM-dd")
});

// 输出：C:\Projects\MySolution\Exports\12345\2025-10-10\report.xlsx
```

###### 3、缓存解决方案目录，避免重复查找

```C#
// 全局初始化一次
var solutionDir = TangdaoPath.Instance.GetSolutionDirectory();

// 后续使用
var configPath = solutionDir.Combine("config", "appSettings.json");
var dbPath = solutionDir.Combine("data", "local.db");
```

###### 4、清理临时文件

```C#
var tempDir = TangdaoPath.Instance.GetTempDirectory().Combine("MyApp");
if (tempDir.DirectoryExists)
{
    Directory.Delete(tempDir.Value, true);
}
```

###### 5、文件导出

```C#
 public void Export()
 {
     try
     {
         var template = PathTemplate.Create("{Solution}/Exports/{Date}/report.txt");
         var exportPath = template.Resolve(new
         {
             Solution = TangdaoPath.Instance.GetSolutionDirectory().Value,
             Date = DateTime.Now.ToString("yyyy-MM-dd")
         });

         Directory.CreateDirectory(exportPath.Parent().Value);
         File.WriteAllText(exportPath.Value, $"Hello Stylet! Exported at {DateTime.Now}");
     }
     catch (Exception ex)
     {
     }
 }
```

###### 6、创建备份

```C#
// 使用 Combine 构建路径
AbsolutePath sourcePath = TangdaoPath.Instance
    .GetThisFilePath()
    .Parent()                           // 移除文件名
    .Combine("..")                      // 上级目录
    .Combine("Models")                  // Models 文件夹
    .Combine("User.cs");                // 具体文件

// 使用 Backup 创建备份
 var backup = sourcePath.Backup(".bak");
```

#### 16、自定义排序

带后期增加接口优化

```C#
List<Student> students = new List<Student>
{
    new Student { Id=4, Name = "张三1111", Education = "一本" },
    new Student { Id=5, Name = "李四2222", Education = "大专" },
    new Student { Id=2, Name = "王五3333", Education = "研究生" },
    new Student { Id=1, Name = "张三4444", Education = "一本" },
    new Student { Id=0, Name = "李四5555", Education = "大专" },
    new Student { Id=6, Name = "王五6666", Education = "研究生" },
};

var priority = new Dictionary<string, int>
{
    ["大专"] = 1,
    ["一本"] = 2,
    ["研究生"] = 3
};

var comparer = TangdaoSortProvider.Priority<Student>(s => s.Education, priority);
```

