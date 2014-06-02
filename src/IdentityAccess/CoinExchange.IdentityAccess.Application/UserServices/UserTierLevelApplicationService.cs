﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Domain.Model;
using CoinExchange.IdentityAccess.Application.UserServices.Commands;
using CoinExchange.IdentityAccess.Application.UserServices.Representations;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;

namespace CoinExchange.IdentityAccess.Application.UserServices
{
    /// <summary>
    /// Implementation of user tier level application service
    /// </summary>
    public class UserTierLevelApplicationService:IUserTierLevelApplicationService
    {
        private IUserRepository _userRepository;
        private ISecurityKeysRepository _securityKeysRepository;
        private IIdentityAccessPersistenceRepository _persistenceRepository;
        private IDocumentPersistence _documentPersistence;

        /// <summary>
        /// parameterized constructor
        /// </summary>
        /// <param name="userRepository"></param>
        /// <param name="securityKeysRepository"></param>
        /// <param name="persistenceRepository"></param>
        public UserTierLevelApplicationService(IUserRepository userRepository, ISecurityKeysRepository securityKeysRepository, IIdentityAccessPersistenceRepository persistenceRepository,IDocumentPersistence documentPersistence)
        {
            _userRepository = userRepository;
            _securityKeysRepository = securityKeysRepository;
            _persistenceRepository = persistenceRepository;
            _documentPersistence = documentPersistence;
        }

        /// <summary>
        /// Apply for tier 1 verification
        /// </summary>
        /// <param name="command"></param>
        public void ApplyForTier1Verification(VerifyTier1Command command)
        {
            SecurityKeysPair securityKeysPair = _securityKeysRepository.GetByApiKey(command.SystemGeneratedApiKey);
            User user = _userRepository.GetUserById(securityKeysPair.UserId);
            //add user phone number
            user.PhoneNumber = command.PhoneNumber;
            //update user tier 1 status
            user.UpdateTierStatus(TierLevelConstant.Tier1, Status.Preverified);
            _persistenceRepository.SaveUpdate(user);
        }

        /// <summary>
        /// Apply for tier 2 verification
        /// </summary>
        /// <param name="command"></param>
        public void ApplyForTier2Verification(VerifyTier2Command command)
        {
            if (command.ValidateCommand())
            {
                SecurityKeysPair securityKeysPair = _securityKeysRepository.GetByApiKey(command.SystemGeneratedApiKey);
                User user = _userRepository.GetUserById(securityKeysPair.UserId);
                //update info
                user.UpdateTier2Information(command.City,command.State,command.AddressLine1,command.AddressLine2,command.ZipCode);
                //update tier status
                user.UpdateTierStatus(TierLevelConstant.Tier2, Status.Preverified);
                //update user
                _persistenceRepository.SaveUpdate(user);
            }
        }

        /// <summary>
        /// Apply for tier 3 verification
        /// </summary>
        /// <param name="command"></param>
        public void ApplyForTier3Verification(VerifyTier3Command command)
        {
            //if (command.ValidateCommand())
            {
                SecurityKeysPair securityKeysPair = _securityKeysRepository.GetByApiKey(command.SystemGeneratedApiKey);
                User user = _userRepository.GetUserById(securityKeysPair.UserId);
                //update info
                user.UpdateTier3Information(command.SocialSecurityNumber,command.Nin);
                //update tier status
                user.UpdateTierStatus(TierLevelConstant.Tier3, Status.Preverified);
                UserDocument document = _documentPersistence.PersistDocument(command.FileName,
                    Constants.USER_DOCUMENT_PATH, command.DocumentStream, command.DocumentType, user.Id);
                //update user
                _persistenceRepository.SaveUpdate(user);
                _persistenceRepository.SaveUpdate(document);
            }
        }

        /// <summary>
        /// Apply for tier 4 verification
        /// </summary>
        public void ApplyForTier4Verification()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get user tier status
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public UserTierStatusRepresentation[] GetTierLevelStatuses(string apiKey)
        {
            List<UserTierStatusRepresentation> representations=new List<UserTierStatusRepresentation>();
            SecurityKeysPair keysPair = _securityKeysRepository.GetByApiKey(apiKey);
            if (keysPair != null)
            {
                User user = _userRepository.GetUserById(keysPair.UserId);
                UserTierLevelStatus[] getLevelStatuses = user.GetAllTiersStatus();
                for (int i = 0; i < getLevelStatuses.Length; i++)
                {
                    representations.Add(new UserTierStatusRepresentation(getLevelStatuses[i].Status.ToString(),getLevelStatuses[i].Tier));
                }
                return representations.ToArray();
            }
            throw new InvalidOperationException("Invlaid apiKey");
        }
    }
}
