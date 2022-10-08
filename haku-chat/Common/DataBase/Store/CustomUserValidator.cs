using Microsoft.AspNetCore.Identity;

using haku_chat.Models;
using haku_chat.DbContexts;
using System.Threading.Tasks;
using MySqlX.XDevAPI.Common;
using System.Collections.Generic;

namespace haku_chat.Common.DataBase.Store
{
    public class CustomUserValidator : UserValidator<UserMasterModel>
    {
        public async override Task<IdentityResult> ValidateAsync(
        UserManager<UserMasterModel> manager, UserMasterModel user)
        {
            IdentityResult result = await base.ValidateAsync(manager, user);

            foreach (IdentityError error in result.Errors)
            {
                if (error.Code == "InvalidUserName")
                {
                    ((List<IdentityError>)result.Errors).Remove(error);
                }
            }
            return ((List<IdentityError>)result.Errors).Count == 0 ? IdentityResult.Success
                : IdentityResult.Failed(((List<IdentityError>)result.Errors).ToArray());
        }
    }
}
