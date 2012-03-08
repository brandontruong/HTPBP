﻿using System;
using System.Web.Security;
using BP.Domain.Entities;

namespace BP.Domain.Abstract
{
    public interface IAccountRepository
    {
        bool IsValidLogin(string username, string password);

        MembershipCreateStatus CreateUser(UserModel user);
        UserProfile GetUserProfileByUserName(string username);
    }
}
