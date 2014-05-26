﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;

namespace CoinExchange.IdentityAccess.Application.Tests
{
    /// <summary>
    /// Mocks User Repository
    /// </summary>
    public class MockUserRepository : IUserRepository
    {
        private List<User> _userList = new List<User>(); 

        /// <summary>
        /// Get the User by Username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public User GetUserByUserName(string username)
        {
            foreach (var user in _userList)
            {
                if (user.Username.Equals(username))
                {
                    return user;
                }
            }
            return null;
        }

        /// <summary>
        /// Adds the User to the User collection only in this Mock implementation
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool AddUser(User user)
        {
            _userList.Add(user);
            return true;
        }
    }
}
