# http://47.108.194.100:52800/allycs/server/farm/v1/api

## FarmInfoGET
>  `GET */farms` 获取农场列表
>> 伪JSON格式（请使用标准JSON）没有的数据不传
``` 
请求体
{
	"Name":"农场名称",
	"Remark":"农场备注",
	"Address":"农场地址",
	"Telephone":"农场电话",
	"Type":"农场类型",	//非必填，暂定0
	"page":1,
	"limit":25
}
``` 

## NewFarmInfo
>  `POST */farm` 新建农场信息
>> 伪JSON格式（请使用标准JSON）没有的数据不传
``` 
请求体
{
	"name":"农场名称",
	"longitude":121.1,
	"latitude":37.1,
	"description":"农场描述",
	"remark":"备注",
	"address":"农场地址",
	"telephone":"农场电话",
	"type":0,
	"PersonLiable":"负责人ID",
	"mainImg":HTTPFile类型,		//该属性可不在json中
	"status":0
}


```


## UpdateFarmInfo
>  `POST */update/farm` 修改农场信息
>> 伪JSON格式（请使用标准JSON）没有的数据不传
``` 
请求体
请求体
{
	"id":"农场ID",
	"name":"农场名称",
	"longitude":121.1,
	"latitude":37.1,
	"description":"农场描述",
	"remark":"备注",
	"address":"农场地址",
	"telephone":"农场电话",
	"type":0,
	"PersonLiable":"负责人ID",
	"mainImg":HTTPFile类型,		//该属性可不在json中
	"status":0
}

```