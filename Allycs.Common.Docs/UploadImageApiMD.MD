#~/allycs/server/upload-image/v1/api

## UploadImage
>  `POST */upload` 图片上传 需登录用户
>> 伪JSON格式（请使用标准JSON）没有的数据不传
``` 
请求体
{
	"name":"泛名称",
	"description":"描述",
	"remark":"备注",
	"type":0,	//暂定0为农场详细图
	"RefereeId":"24", //农场ID
	"Images":[HttpFile]//图片列表支持单张多张
}


```
## UploadImageList
>  `POST */list` 图片属性列
>> 伪JSON格式（请使用标准JSON）没有的数据不传
``` 
请求体
{
	"name":"泛名称",
	"extension":"扩展名",
	"description":"描述",
	"remark":"备注",
	"type":0,	//暂定0为农场详细图
	"RefereeId":"24", //农场ID
	"createdBy":"上传者Id",
	"page":1,
	"limit":25
}


```
## GetImage
>  `POST */image/{id}/{extension}` 图片地址
>> 伪JSON格式（请使用标准JSON）没有的数据不传
``` 

```