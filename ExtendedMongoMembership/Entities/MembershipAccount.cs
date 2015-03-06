using ExtendedMongoMembership.Entities;
using System;
using System.Collections.Generic;

namespace ExtendedMongoMembership
{
    public class MembershipAccount : MembershipAccountBase
    {
        public MembershipAccount()
        {
            Roles = new List<MembershipRole>();
            OAuthData = new List<OAuthAccountDataEmbedded>();
        }
        [MongoDB.Bson.Serialization.Attributes.BsonDateTimeOptions(Kind=DateTimeKind.Local)]
        public DateTime? CreationDate { get; set; }
        public string ConfirmationToken { get; set; }
        public bool IsConfirmed { get; set; }
        [MongoDB.Bson.Serialization.Attributes.BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? LastPasswordFailureDate { get; set; }
        public int PasswordFailuresSinceLastSuccess { get; set; }
        public string Password { get; set; }
        [MongoDB.Bson.Serialization.Attributes.BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? PasswordChangedDate { get; set; }
        public string PasswordSalt { get; set; }
        public string PasswordVerificationToken { get; set; }
        public DateTime? PasswordVerificationTokenExpirationDate { get; set; }
        [MongoDB.Bson.Serialization.Attributes.BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? LastLoginDate { get; set; }

        public List<MembershipRole> Roles { get; set; }

        public List<OAuthAccountDataEmbedded> OAuthData { get; set; }
    }
}
