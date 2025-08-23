#### 1、命令

正常命令：TangdaoCommand   

异步命令：TangdaoAsyncCommand

静态命令：MinidaoCommand

1-1、用法

```js
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

```C#
//容器的初始化
ITangdaoContainer container = new TangdaoContainer();

//解析器的初始化
var provider = container.Builder();

//构建一个全局服务定位器
ServerLocator.InitContainer(container);

//注册接口和实体类
container.RegisterType<IReadService,ReadService>();
container.RegisterType<IWriteService,WriteService>();
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

3-1、读写

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

```C#C#
 string path = "E://Temp//Student.xml";
 string xmlContent=TxtFolderHelper.ReadByFileStream(path);
 Student student=XmlFolderHelper.Deserialize<Student>(xmlContent);
```

也可以使用接口读取

xml文件是

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

```
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

```
// 正确调用（多节点必须指定索引）
var ip1 = _readService.Current[1].SelectNode("IP").Value;

// 错误调用（多节点未指定索引）
var ip2 = _readService.Current.SelectNode("IP").Value; 
// 返回错误："存在多个节点，请指定索引"

// 正确调用（单节点可不指定索引）
var ip3 = _readService.Current.SelectNode("IP").Value; 
```

优化繁琐的读取,不需要知道类的所有属性

```
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

```
 var readResult = _readService.Current.SelectNodes<ProcessItem>();
```



#### 4、增加一些常用的Helper类

DirectoryHelper

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

```
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

```
Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="*" />
        <RowDefinition Height="Auto" />
    </Grid.RowDefinitions>

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
            Command="{Binding GoToStudentListCommand}"
            Content="学生列表" />
        <Button
            Margin="5"
            Command="{Binding GoToDashboardCommand}"
            Content="仪表盘" />
    </StackPanel>
</Grid>
```



#### 9、时间轮

```
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

```
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

```
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

