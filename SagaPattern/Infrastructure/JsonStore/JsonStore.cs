using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace SagaPattern.Infrastructure.JsonStore
{
    public class JsonStore<T> : IStore<T> where T : class
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new PrivateSetterContractResolver()
        };

        private readonly string folder;
        readonly object flag = new object();

        public JsonStore(string folder = ".")
        {
            this.folder = Path.GetFullPath(folder);
            if (!Directory.Exists(folder))
            {
                throw new ArgumentException($"Folder [{folder}] does not exist", nameof(folder));
            }
            PropertyExistsOrThrow("Id");
            PropertyExistsOrThrow("Version");
        }

        public T Get(Guid id)
        {
            lock (flag)
            {
                var file = new FileInfo(FileFor(id));
                return file.Exists 
                    ? Deserialize(File.ReadAllText(file.FullName))
                    : null;
            }
        }

        public void Save(T item)
        {
            lock (flag)
            {
                var id = IdOf(item);
                var versionOfLast = VersionOf(Get(id));
                var versionOfCurrent = VersionOf(item);

                if (versionOfLast != versionOfCurrent)
                    throw new StoreConcurrencyException(typeof(T), versionOfLast, versionOfCurrent);

                var clone = Clone(item);

                IncrementVersion(clone, versionOfCurrent);
                File.WriteAllText(FileFor(id), Serialize(clone));
            }
        }

        static T Clone(T item)
        {
            return Deserialize(Serialize(item));
        }

        private static string Serialize(T item)
        {
            return JsonConvert.SerializeObject(item);
        }

        private static T Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, JsonSerializerSettings);
        }

        static void IncrementVersion(T item, int version)
        {
            typeof(T)
                .GetProperty("Version")
                .SetValue(item, ++version);
        }

        static Guid IdOf(T item)
        {
            return (Guid)typeof(T)
                .GetProperty("Id")
                .GetValue(item);
        }


        static int VersionOf(T item)
        {
            if (item == null)
            {
                return 0;
            }
            return (int)typeof(T)
                .GetProperty("Version")
                .GetValue(item);
        }

        string FileFor(Guid id)
        {
            return Path.Combine(folder, $"{typeof(T).Name.ToLowerInvariant()}-{id:N}.data");
        }

        static void PropertyExistsOrThrow(string propertyName)
        {
            if (!HasProperty(propertyName))
                throw new InvalidOperationException($"Missing required property {propertyName} for type {typeof(T).Name}.");
        }

        static bool HasProperty(string propertyName)
        {
            return typeof(T).GetProperty(propertyName) != null;
        }

        private class PrivateSetterContractResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(
                MemberInfo member,
                MemberSerialization memberSerialization)
            {
                var prop = base.CreateProperty(member, memberSerialization);

                if (!prop.Writable)
                {
                    var property = member as PropertyInfo;
                    if (property != null)
                    {
                        var hasPrivateSetter = property.GetSetMethod(true) != null;
                        prop.Writable = hasPrivateSetter;
                    }
                }
                return prop;
            }
        }
    }

    public class StoreConcurrencyException : Exception
    {
        public StoreConcurrencyException(Type type, int last, int current)
            : base($"FOR: {type.Name} LAST: {last} CURRENT: {current}")
        {
        }
    }
}