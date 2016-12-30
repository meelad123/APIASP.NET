using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using projectNew.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;


namespace projectNew
{
    public class AuthUser:IDisposable
    {
        #region Variables
        private DBContext _db;
        private UserManager<IdentityUser> _userManager;
        #endregion

        #region constructor
        public AuthUser()
        {
            _db = new DBContext();
            _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_db));
        }
        #endregion

        #region functions
        /*********************
         RegisterUser function 
        *********************/
        public async Task<IdentityResult> RegisterUser(Account accountModel)
        {
            IdentityUser user = new IdentityUser();
            User _user = new User 
            { 
                Email = accountModel.Email,
                UserName = accountModel.UserName
            };
            user.UserName = accountModel.UserName;
            user.Email = accountModel.Email;

            var result = await _userManager.CreateAsync(user, accountModel.Password);
            _db.MyUsers.Add(_user);
            _db.SaveChanges();
            return result;
        }

        public async Task<IdentityUser> FindUser(string userName, string password)
        {
            IdentityUser user = await _userManager.FindAsync(userName, password);
            return user;
        }

        public void Dispose()
        {
            _db.Dispose();
            _userManager.Dispose();
        }
        #endregion
    }
}
