[English](./README.md) | 简体中文

# dtmgrpc-csharp

`dtmgrpc-csharp` 是分布式事务管理器 [DTM](https://github.com/dtm-labs/dtm) 的 C# 客户端，使用 gRPC 协议和 DTM 服务端进行交互。 

目前已经支持 SAGA 、 TCC 和二阶段消息三种事务模式。

![Build_And_UnitTest](https://github.com/catcherwong/dtmgrpc-csharp/actions/workflows/build_and_ut.yml/badge.svg) ![Build_And_IntegrationTests](https://github.com/catcherwong/dtmgrpc-csharp/actions/workflows/build_and_it.yml/badge.svg)

![](https://img.shields.io/nuget/v/Dtmgrpc.svg)  ![](https://img.shields.io/nuget/vpre/Dtmgrpc.svg) ![](https://img.shields.io/nuget/dt/Dtmgrpc) ![](https://img.shields.io/github/license/catcherwong/dtmgrpc-csharp)

## 安装

通过下面的命名安装 nuget 包

```sh
dotnet add package Dtmgrpc
```

## 配置

这里有两种方式进行配置

1. 使用 setup action

```cs
services.AddDtmGrpc(x =>
{
    // DTM server 的 gRPC 地址
    x.DtmGrpcUrl = "http://localhost:36790";
    
    // 请求 DTM server 的超时时间, 单位是毫秒
    x.DtmTimeout = 10000; 
    
    // 请求分支事务的超时时间, 单位是毫秒
    x.BranchTimeout = 10000;
    
    // 子事务屏障的数据库类型, mysql, postgres, sqlserver
    x.DBType = "mysql";

    // 子事务屏障的数据表名
    x.BarrierTableName = "dtm_barrier.barrier";
});
```

2. 使用 `IConfiguration`

```cs
services.AddDtmGrpc(Configuration, "dtm");
```

添加配置文件，以 JSON 为例： 

```JSON
{
  "dtm": {
    "DtmGrpcUrl": "http://localhost:36790",
    "DtmTimeout": 10000,
    "BranchTimeout": 10000,
    "DBType": "mysql",
    "BarrierTableName": "dtm_barrier.barrier",
  }
}
```

## 用法

### SAGA

```cs
public class MyBusi
{ 
    private readonly Dtmgrpc.IDtmTransFactory _transFactory;

    public MyBusi(Dtmgrpc.IDtmTransFactory transFactory)
    {
        this._transFactory = transFactory;
    }

    public async Task DoBusAsync()
    {
        var gid = Guid.NewGuid().ToString();
        var req = new BusiReq {  Amount = 30 };
        var svc = "localhost:5005";

        var saga = _transFactory.NewSagaGrpc(gid);
        // 添加子事务操作
        saga.Add(
            // 正向操作 URL
            svc + "/busi.Busi/TransOut",
            
            // 逆向操作 URL
            svc + "/busi.Busi/TransOutCompensate",

            // 参数
            req);
        saga.Add(
            svc + "/busi.Busi/TransIn",
            svc + "/busi.Busi/TransInCompensate",
            req);

        await saga.Submit();
    }
}
```

### TCC

```cs
public class MyBusi
{ 
    private readonly Dtmgrpc.TccGlobalTransaction _globalTransaction;

    public MyBusi(Dtmgrpc.TccGlobalTransaction globalTransaction)
    {
        this._globalTransaction = globalTransaction;
    }

    public async Task DoBusAsync()
    {
        var gid = Guid.NewGuid().ToString();
        var req = new BusiReq {  Amount = 30 };
        var svc = "localhost:5005";

        await _globalTransaction.Excecute("http://localhost:36790", gid, async tcc =>
        {
            // 调用 TCC 子事务
            await tcc.CallBranch<BusiReq, Empty>(
                // 参数
                req,

                // Try 阶段的 URL
                svc + "/busi.Busi/TransOutTry",

                // Confirm 阶段的 URL 
                svc + "/busi.Busi/TransOutConfirm",

                 // Cancel 阶段的 URL
                svc + "/busi.Busi/TransOutCancel");

            await tcc.CallBranch<BusiReq, Empty>(
                req,
                svc + "/busi.Busi/TransInTry",
                svc + "/busi.Busi/TransInConfirm",
                svc + "/busi.Busi/TransInCancel");
        });
    }
}
```


### 二阶段消息

```cs
public class MyBusi
{ 
    private readonly Dtmgrpc.IDtmTransFactory _transFactory;

    public MyBusi(Dtmgrpc.IDtmTransFactory transFactory)
    {
        this._transFactory = transFactory;
    }

    public async Task DoBusAsync()
    {
        var gid = Guid.NewGuid().ToString();
        var req = new BusiReq {  Amount = 30 };
        var svc = "localhost:5005";

        var msg = _transFactory.NewMsgGrpc(gid);
        // 添加子事务操作
        msg.Add(
            // 操作的 URL
            svc + "/busi.Busi/TransOut",

            // 参数
            req);
        msg.Add(
            svc + "/busi.Busi/TransIn",
            req);

        // 用法 1:
        // 发送 prepare 消息
        await msg.Prepare(svc + "/busi.Busi/QueryPrepared");
        // 发送 submit 消息
        await msg.Submit();

        // 用法 2:
        using (MySqlConnection conn = new MySqlConnection("You connection string ...."))
        {
            await msg.DoAndSubmitDB(svc + "/busi.Busi/QueryPrepared", conn, async tx => 
            {
                await conn.ExecuteAsync("insert ....", new { }, tx);
                await conn.ExecuteAsync("update ....", new { }, tx);
                await conn.ExecuteAsync("delete ....", new { }, tx);
            });
        }
    }
}
```