# http://47.108.194.100:52800/allycs/server/member/v1/api

## Login
>  `POST */login` 用户登录
>> 伪JSON格式（请使用标准JSON）
``` 
请求体
{
    "account":"账号", //未作合法性限定30位
    "password":"密码", //未作合法性限定30位
    "clientType":0, //参考基础中的ClientType类型值
    "RequestUrl":"url" //暂未使用
}


```

## Regist
>  `POST */regist` 用户注册
>> 伪JSON格式（请使用标准JSON）
``` 
请求体
{
    "account":"账号"
    "realname":"真实姓名",           //以此名称为准
    "sex":0,                        //性别参考SexType类型
    "alias":"昵称",                 //暂未使用
    "avatar":"头像",                //暂未使用
    "telephone":"座机/紧急联系电话", //座机或者紧急联系电话
    "phoneNumber":"16653555299",    //手机号
    "email":"email",                //邮箱
    "address":"地址",                //细化地址
    "birthday":"2019/01/01',         //时间格式（支持标准的各种时间格式）
    "idCard":"3706~",                //身份证号
    "registCode":"注册码",             //注册码（用于推荐指向推荐人或者注册限定）
    "checkCode":"",                 //验证码
    "password":""                    //密码
}
```

## CheckLogin
>  `GET */check-auth` 登录检测
>> 伪JSON格式（请使用标准JSON）
----

## RenewPasswordByPhoneNumber
>  `POST */renew-password` 用户手机修改密码
>> 伪JSON格式（请使用标准JSON）
``` 
请求体
{
    "oldPassword":"",    //原密码
    "checkCode":"",            //验证码
    "password":""                    //密码
}
返回体
{
    "message":"用户***密码已更新"
}
```
## Logout
>  `Post */logout` 登出
>> 伪JSON格式（请使用标准JSON）
``` 
请求体
{
    "Token":"身份令牌"
}
返回体
"安全登出"
```

## RegistCode
>  `Get */code/regist` 获取注册验证码
>> 伪JSON格式（请使用标准JSON）
``` 
请求体

返回体
{
    "code":"验证码"
}
```
## RegistCode
>  `Post */code/regist/check/{code}` 验证注册验证码
>> 伪JSON格式（请使用标准JSON）
``` 
请求体

返回体
{
    "message":"验证合法"
}
```