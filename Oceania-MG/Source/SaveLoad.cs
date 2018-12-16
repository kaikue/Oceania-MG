using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;

namespace Oceania_MG
{
	/// <summary>
	/// Class for saving and loading serializable objects to a file.
	/// Based on https://stackoverflow.com/a/12845153
	/// </summary>
	class SaveLoad
	{
		public static byte[] Serialize<T>(T obj)
		{
			var serializer = new DataContractSerializer(typeof(T));
			var stream = new MemoryStream();
			using (var writer = XmlDictionaryWriter.CreateBinaryWriter(stream))
			{
				serializer.WriteObject(writer, obj);
			}
			return stream.ToArray();
		}

		public static T Deserialize<T>(byte[] data)
		{
			var serializer = new DataContractSerializer(typeof(T));
			using (var stream = new MemoryStream(data))
			using (var reader = XmlDictionaryReader.CreateBinaryReader(stream, XmlDictionaryReaderQuotas.Max))
			{
				return (T)serializer.ReadObject(reader);
			}
		}

		//TODO: Make the public-facing methods write/read a file, and use the byte[] ones privately
	}

	class SaveLoadTest
	{
		[DataContract]
		class People
		{
			[DataMember]
			public Person person1;
			[DataMember]
			public Person person2;
		}

		[DataContract(IsReference = true)]
		class Person
		{
			[DataMember]
			public string Name;
			[DataMember]
			public int Age;
			[DataMember]
			public Person Friend;
		}

		public static void MainTest()
		{
			Person person = new Person { Name = "John", Age = 32 };
			Person person2 = new Person { Name = "Billy", Age = 30 };
			person.Friend = person2;
			person2.Friend = person;

			People people = new People { person1 = person, person2 = person2 };

			byte[] data = SaveLoad.Serialize(people);
			//byte[] data = Serialize(person);
			//byte[] data2 = Serialize(person2);
			// Write/read data to/from file
			//Person newPerson = Deserialize<Person>(data);
			//Person newPerson2 = Deserialize<Person>(data2);
			People newPeople = SaveLoad.Deserialize<People>(data);
			Person newPerson = newPeople.person1;
			Person newPerson2 = newPeople.person2;

			Debug.Assert(newPerson.Age == 32);
			Debug.Assert(newPerson.Name == "John");
			Debug.Assert(newPerson.Friend.Name == "Billy");

			Debug.Assert(newPerson2.Name == "Billy");
			Debug.Assert(newPerson2.Friend.Name == "John");

			Debug.Assert(newPerson.Friend == newPerson2);
			Debug.Assert(newPerson2.Friend == newPerson);

			Debug.Print("All tests passed.");
		}
	}
}
