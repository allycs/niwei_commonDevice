# http://47.108.194.100:52800/allycs/server/device/v1/api

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
## DeviceDataGET
>  `GET */data` 传感器数据获取
>> 伪JSON格式（请使用标准JSON）没有的数据不传
``` 
请求体
{
	"type":传感器类型,		//Temperature = 0,Humidity = 1,CarbonDioxide = 2,AcidBase = 3,Nitrogen = 4,Phosphorus = 5,Potassium = 6,Illuminance = 7
	"StartOn":"开始时间",
	"EndOn":"结束时间",
	"DeviceId":"设备ID"
}


```