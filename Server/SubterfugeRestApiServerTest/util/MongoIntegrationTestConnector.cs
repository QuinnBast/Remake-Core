﻿using System.Security.Cryptography;
using Subterfuge.Remake.Api.Network;
using Subterfuge.Remake.Server.Database;
using Subterfuge.Remake.Server.Database.Models;

namespace Subterfuge.Remake.Server.Test.util
{
    public class MongoIntegrationTestConnector
    {
        public MongoConnector Mongo;
        
        public MongoIntegrationTestConnector()
        {
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var mongoConfig = new MongoConfiguration()
            {
                CreateSuperUser = false,
                FlushDatabase = true,
                Host = environment == "Docker" ? "db" : "localhost",
                Port = 27017,
                SuperUserUsername = "admin",
                SuperUserPassword = "admin",
            };

            Mongo = new MongoConnector(new DefaultMongoConfigurationProviderImpl() { Config = mongoConfig}, null);
        }

        public async Task<DbUserModel> CreateTestingSuperUser()
        {
            var userModel = new DbUserModel()
            {
                Id = "1",
                Username = "admin",
                PasswordHash = HashPassword("admin"),
                PhoneNumber = "9999999999",
                Claims = new[] { UserClaim.User, UserClaim.Administrator, UserClaim.PhoneVerified }
            };
            
            await Mongo.GetCollection<DbUserModel>().Upsert(userModel);
            return userModel;
        }
        
        private String HashPassword(string password)
        {
            // Create salt
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            
            // Create the Rfc2898DeriveBytes and get the hash value: 
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
            byte[] hash = pbkdf2.GetBytes(20);
            
            // Combine salt and password bytes
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            
            // Return value
            return Convert.ToBase64String(hashBytes);
        }
    }
}