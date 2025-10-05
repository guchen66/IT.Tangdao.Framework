#### 1、命令

正常命令：TangdaoCommand   

异步命令：TangdaoAsyncCommand

静态命令：MinidaoCommand

1-1、用法

```C#
TangdaoCommand  taodao=new TangdaoCommand();
TangdaoAsyncCommand taodaoAsync=new TangdaoAsyncCommand();
MinidaoCommand.Create();
```



#### 2、事件聚合器

```C#
 public IDaoEventAggregator _daoEventAggregator;

_daoEventAggregator.Publish<T>();

_daoEventAggregator.Subscribe<T>(Execute);

T:DaoEventBase
```

#### 3、增加另外一种全新的方式去发送数据

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

#### 2、容器

ITangdaoContainer

修改WPF启动项

```C#
 public partial class App : TangdaoApplication
 {
     protected override void RegisterServices(ITangdaoContainer container)
     {
         container.AddSingleton<MainView>();
         container.AddSingleton<MainViewModel>();
         container.AddSingleton<HomeViewModel>();
         container.AddSingleton<IReadService, ReadService>();
     }

     protected override Window CreateWindow()
     {
         return CreateShell<MainView>();
     }
 }
```

可以通过服务定位进行接口解析

```
TangdaoApplication.Provider.GetService(viewModel);
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



#### 3、扩展方法

###### 3-1、读写

StringExtension 可以方便一些代码

读取本地txt文件的方法

```c#
string path = "E://Temp.txt";
string xmlContent=TxtFolderHelper.ReadByFileStream(path);
```

如果是测试读取文件的话，可以简单的读取

```c#
 string path = "E://Temp.txt";
 string content=path.CreateFolder().UseStreamReadToEnd();
```

读取本地xml文件的方法

```C#
 string path = "E://Temp//Student.xml";
 string xmlContent=TxtFolderHelper.ReadByFileStream(path);
 Student student=XmlFolderHelper.Deserialize<Student>(xmlContent);
```

也可以使用接口读取

###### 3-2、xml文件是

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

注册接口，然后读取

```c#
  string path = "E://Temp//Student.xml";
  Student stu= await _readService.ReadXmlToEntityAsync<Student>(path,DaoFileType.Xml);
```

简单的读取写入

```C#
  await _writeService.WriteAsync("E://Temp//100.txt","HelloWorld");

  await _readService.ReadAsync("E://Temp//100.txt");
```

以及增加了XML的序列化和反序列化

将对象转成XML，以字符串保存

```c#
string xml=XmlFolderHelper.SerializeXML<Student>(student);
```

XML字符串反序列化为对象

```c#
Student student=XmlFolderHelper.Deserialize<Student>(xml);
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

###### 3-3、config的读取

配置

```
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

读取

```
 Dictionary<string, string> MenuList = _readService.Current.SelectConfig("Menu").ToDictionary();
 var student = _readService.Current.SelectConfig("Student").ToObject<Student>();
```

###### 3-4、基于ViewModel命名约定的隐式键展开容器字典

```
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

#### 4、增加一些常用的帮助类

DirectoryHelper



```
var maybe = TangdaoOptional<string>.Some("Hello")
                                   .Where(s => s.Length > 3)
                                   .Select(s => s.ToUpper())
                                   .ValueOrDefault("NONE");

Console.WriteLine(maybe);   // HELLO
```



#### 5、强制组件通信

```C#
//同级别窗体通信 
//tangdaoParameter为发送的数据，可以在打开窗体的时候直接发送数据过去
this.RunSameLevelWindowAsync<LoginView>(tangdaoParameter);

//父子窗体通信
this.RunChildWindowAsync<LoginView>();
```



#### 6、日志DaoLogger

日志默认是写在桌面上的

#### 7、自动生成器

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



#### 8、增加IRouter路由导航

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



#### 9、时间轮

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



#### 10、增加文本监控

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

#### 11、任务调度器TangdaoTaskScheduler

```C#
  TangdaoTaskScheduler.Execute(dao: daoTask =>
  {
      
  });

         
  TangdaoTaskScheduler.Execute(daoAsync: daoTask =>
  {
     
  });

  TangdaoTaskScheduler.Execute(daoAsync => { }, dao => { });
```

#### 12、Markup的扩展

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

