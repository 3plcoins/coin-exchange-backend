﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate
{
    /// <summary>
    /// List for storing the Permissions
    /// </summary>
    public class PermissionsList : IEnumerable<DigitalSignaturePermission>
    {
        private List<DigitalSignaturePermission> _permissionList = new List<DigitalSignaturePermission>();

        /// <summary>
        /// Add an element
        /// </summary>
        /// <returns></returns>
        internal bool AddPermission(DigitalSignaturePermission digitalSignaturePermission)
        {
            _permissionList.Add(digitalSignaturePermission);
            return true;
        }

        /// <summary>
        /// Remove an element
        /// </summary>
        /// <param name="digitalSignaturePermission"></param>
        /// <returns></returns>
        internal bool RemoveTierStatus(DigitalSignaturePermission digitalSignaturePermission)
        {
            _permissionList.Remove(digitalSignaturePermission);
            return true;
        }
        public IEnumerator<DigitalSignaturePermission> GetEnumerator()
        {
            foreach (var permission in _permissionList)
            {
                // Lets check for end of list (its bad code since we used arrays)
                if (permission == null)
                {
                    break;
                }

                // Return the current element and then on next function call 
                // resume from next element rather than starting all over again;
                yield return permission;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
