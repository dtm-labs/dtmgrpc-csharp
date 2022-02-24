English | [简体中文](./README-cn.md)


# dtmgrpc-csharp

`dtmgrpc-csharp` is the C# client of Distributed Transaction Manager [DTM](https://github.com/dtm-labs/dtm) that communicates with DTM Server through gRPC protocol. 

It has supported distributed transaction patterns of Saga pattern, TCC pattern and 2-phase message pattern.

![Build_And_UnitTest](https://github.com/catcherwong/dtmgrpc-csharp/actions/workflows/build_and_ut.yml/badge.svg) ![Build_And_IntegrationTests](https://github.com/catcherwong/dtmgrpc-csharp/actions/workflows/build_and_it.yml/badge.svg)

![](https://img.shields.io/nuget/v/Dtmgrpc.svg)  ![](https://img.shields.io/nuget/vpre/Dtmgrpc.svg) ![](https://img.shields.io/nuget/dt/Dtmgrpc) ![](https://img.shields.io/github/license/catcherwong/dtmgrpc-csharp)

## Installation

Add nuget package via the following command

```sh
dotnet add package Dtmgrpc
```

## Configuration

There are two ways to configure

1. Configure with setup action

```cs
services.AddDtmGrpc(x =>
{
    // DTM server grpc address
    x.DtmGrpcUrl = "http://localhost:36790";
    
    // request timeout for DTM server, unit is milliseconds
    x.DtmTimeout = 10000; 
    
    // request timeout for trans branch, unit is milliseconds
    x.BranchTimeout = 10000;
    
    // barrier database type, mysql, postgres, sqlserver
    x.DBType = "mysql";

    // barrier table name
    x.BarrierTableName = "dtm_barrier.barrier";
});
```

2. Configure with `IConfiguration`

```cs
services.AddDtmGrpc(Configuration, "dtm");
```

And the configuration file 

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

## Usage

### SAGA pattern

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
        // Add sub-transaction
        saga.Add(
            // URL of forward action 
            svc + "/busi.Busi/TransOut",
            
            // URL of compensating action
            svc + "/busi.Busi/TransOutCompensate",

            // Arguments of actions
            req);
        saga.Add(
            svc + "/busi.Busi/TransIn",
            svc + "/busi.Busi/TransInCompensate",
            req);

        await saga.Submit();
    }
}
```

### TCC pattern

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

        await _globalTransaction.Excecute(gid, async tcc =>
        {
            // Create tcc sub-transaction
            await tcc.CallBranch<BusiReq, Empty>(
                // Arguments of stages
                req,

                // URL of Try stage
                svc + "/busi.Busi/TransOutTry",

                // URL of Confirm stage
                svc + "/busi.Busi/TransOutConfirm",

                 // URL of Cancel stage
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


### 2-phase message pattern

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
        // Add sub-transaction
        msg.Add(
            // URL of action 
            svc + "/busi.Busi/TransOut",

            // Arguments of action
            req);
        msg.Add(
            svc + "/busi.Busi/TransIn",
            req);

        // Usage 1:
        // Send prepare message 
        await msg.Prepare(svc + "/busi.Busi/QueryPrepared");
        // Send submit message
        await msg.Submit();

        // Usage 2:
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