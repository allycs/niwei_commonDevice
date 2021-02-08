using Dapper;
using System;

namespace Allycs.Common.Devices.Entities
{
    public class MemberInfo
    { /// <summary>
      /// ObjectId
      /// </summary>
        [Key]
        public string Id { get; set; }
        public string Realname { get; set; }
        public SexType Sex { get; set; }
        public string Alias { get; set; }
        public string Avatar { get; set; }
        /// <summary>
        /// 职业
        /// </summary>
        public string Occupation { get; set; }
        public string Telephone { get; set; }
        public string MobilePhone { get; set; }
        public MemberLevel Level { get; set; } = MemberLevel.Bronze;
        public MemberType Type { get; set; } = MemberType.Normal;
        /// <summary>
        /// 所属组值ID
        /// </summary>
        public string MainMemberId { get; set; }
        public double? AvailableMoney { get; set; } = 0;
        public double? UsedMoney { get; set; } = 0;
        public double? FrozenMoney { get; set; } = 0;
        public double? AvailablePoints { get; set; } = 0;
        public double? UsedPoints { get; set; } = 0;
        public double? FrozenPoints { get; set; } = 0;
        public string Email { get; set; }
        public string DivisionsFullName { get; set; }
        public string Address { get; set; }
        public DateTime? Birthday { get; set; }
        public string IdCard { get; set; }
        /// <summary>
        /// 推荐人ID
        /// </summary>
        public string RefereeId { get; set; }
        public bool HasWechat { get; set; }


        /// <summary>
        /// 是否关注公众号
        /// </summary>
        public bool IsFollowOfficialAccount { get; set; } = false;

        public DateTime? SubscribeOn { get; set; }
        public string RegistCode { get; set; }
        public string Remark { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}