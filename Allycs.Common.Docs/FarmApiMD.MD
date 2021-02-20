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

## DeviceDataUP
>  `POST */data` 传感器数据上传
>> 伪JSON格式（请使用标准JSON）没有的数据不传
``` 
请求体
{
	"id":"24位",
	"temperature":3724,
	"humidity":37,
	"carbonDioxide":34,
	"acidBase":5,
	"nitrogen":1,
	"phosphorus":2,
	"potassium":3,
	"deviceType":1,
	"remark":"12121",
	"information":"121212",
	"updateOn":"2020/12/28"
}


```

```

## Device
>  `POST */device` 传感器(设备)注册上传  管理原权限接口
>> 伪JSON格式（请使用标准JSON）没有的数据不传
``` 
请求体
{
	"deviceCode":"唯一必填",
	"deviceName":"必填",
	"SerialNumber":"必填",
	"Longitude":5,		//非必填
	"Latitude":1,		//非必填
	"configJson":"json格式配置参数",	//非必填
	"remark":"",		//非必填
	"address":"",		//非必填
	"status":10,		// 0 失效，10 使用，90 故障
	"type":0			//0 多传感器，1， 单传感器，10 其它
}


```