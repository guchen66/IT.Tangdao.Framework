#### 1、命令

正常命令：TangdaoCommand   

异步命令：TangdaoAsyncCommand

静态命令：MinidaoCommand

#### 2、事件聚合器

用于父子组件的通讯

```C#
_eventTransmit.Publish<T>();

_eventTransmit.Subscribe<T>(Execute);

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

ITangdaoContainer还未书写完成

```C#
//容器的初始化
ITangdaoContainer container = new TangdaoContainer();

//解析器的初始化
var provider = container.Builder();

//构建一个全局服务定位器
ServerLocator.InitContainer(container);
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

StringExtension 可以方便一些代码

读取本地txt文件的方法

```
string path = "E://Temp.txt";
string xmlContent=TxtFolderHelper.ReadByFileStream(path);
```

如果是测试读取文件的话，可以简单的读取

```
 string path = "E://Temp.txt";
 string content=path.CreateFolder().UseStreamReadToEnd();
```



读取本地xml文件的方法

```C#
 string path = "E://Temp//Student.xml";
 string xmlContent=TxtFolderHelper.ReadByFileStream(path);
 Student student=XmlFolderHelper.Deserialize<Student>(xmlContent);
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



#### 