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

#### 4、增加一些常用的Helper类

DirectoryHelper

#### 