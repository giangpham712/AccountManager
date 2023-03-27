using AccountManager.Application;
using AccountManager.Application.Accounts.Commands.CreateAccount;
using AccountManager.Application.Accounts.Commands.UpdateAccount;
using AccountManager.Application.Accounts.Commands.UpdateBackupSettings;
using AccountManager.Application.Accounts.Commands.UpdateIdleSchedule;
using AccountManager.Application.Accounts.Commands.UpdateInstanceSettings;
using AccountManager.Application.Accounts.Commands.UpdateLicenseSettings;
using AccountManager.Domain;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Audit;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace AccountManager.Persistence.MongoDb
{
    public class AccountManagerMongoDbContext : IAccountManagerDbContext
    {
        private readonly IMongoDatabase _database;

        public AccountManagerMongoDbContext(MongoContextSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            _database = client.GetDatabase(settings.Database);

            
            if (!BsonClassMap.IsClassMapRegistered(typeof(MongoEntityBase)))
                BsonClassMap.RegisterClassMap<MongoEntityBase>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdProperty(c => c.Id)
                        .SetIgnoreIfDefault(true)
                        .SetIdGenerator(StringObjectIdGenerator.Instance)
                        .SetSerializer(new StringSerializer(BsonType.ObjectId));
                });

            if (!BsonClassMap.IsClassMapRegistered(typeof(AuditLog)))
                BsonClassMap.RegisterClassMap<AuditLog>(cm =>
                {
                    cm.AutoMap();
                });


            RegisterCommandClassMap<CreateAccountCommand>();
            RegisterCommandClassMap<UpdateInstanceSettingsCommand>();
            RegisterCommandClassMap<UpdateLicenseSettingsCommand>();
            RegisterCommandClassMap<UpdateAccountCommand>();
            RegisterCommandClassMap<UpdateIdleScheduleCommand>();
            RegisterCommandClassMap<UpdateBackupSettingsCommand>();
        }

        public IMongoCollection<AuditLog> AuditLogs => _database.GetCollection<AuditLog>("AuditLogs");

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }

        private void RegisterCommandClassMap<T>()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(T)))
                BsonClassMap.RegisterClassMap<T>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
        }
    }

    public class MongoContextSettings
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
    }
}