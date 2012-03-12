﻿using System;
using System.Collections.Generic;
using System.Web.Security;
using BP.Domain.Entities;

namespace BP.Domain.Abstract
{
    public interface IAccountRepository
    {
        bool IsValidLogin(string username, string password);
        IEnumerable<UserModel> GetUsers();
        MembershipCreateStatus CreateUser(UserModel user);
        UserProfile GetUserProfileByUserName(string username);

        bool DeleteUser(Guid id, out string error);
    }
}
