using System;
using System.Collections.Generic;
using System.Text;

namespace Allycs.Common.Devices
{
    public enum MemberType
    {
        /// <summary>
        /// 系统管理员
        /// </summary>
        SystemAdministrator = 0,

        /// <summary>
        /// 区域管理员
        /// </summary>
        Administrator = 10,

        Person = 20,
        Normal = 21,

        Organization = 30,

        School = 40,
        Teacher = 42,
        Student = 43,

        /// <summary>
        /// 专家
        /// </summary>
        Expert = 50


    }
}
