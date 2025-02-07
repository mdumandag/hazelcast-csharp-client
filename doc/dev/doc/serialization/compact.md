# Compact Serialization

As an enhancement to existing serialization methods, Hazelcast offers compact serialization, with the following main features.

* Separates the schema from the data and stores it per type, not per object which results in less memory and bandwidth usage compared to other formats
* Does not require a class to implement an interface or change the source code of the class in any way
* Supports schema evolution which permits adding or removing fields, or changing the types of fields
* Can work with no configuration or any kind of factory/serializer registration for .NET classes and structs
* Platform and language independent
* Supports partial deserialization of fields, without deserializing the whole objects during queries or indexing

Hazelcast achieves these features by having a well-known schema of objects and replicating them across the cluster which enables members and clients to fetch schemas they don’t have in their local registries. Each serialized object carries just a schema identifier and relies on the schema distribution service or configuration to match identifiers with the actual schema. Once the schemas are fetched, they are cached locally on the members and clients so that the next operations that use the schema do not incur extra costs.

Schemas help Hazelcast to identify the locations of the fields on the serialized binary data. With this information, Hazelcast can deserialize individual fields of the data, without reading the whole binary. This results in a better query and indexing performance.

Schemas can evolve freely by adding or removing fields. Even, the types of the fields can be changed. Multiple versions of the schema may live in the same cluster and both the old and new readers may read the compatible parts of the data. This feature is especially useful in rolling upgrade scenarios.

The Compact serialization does not require any changes in the user classes as it doesn’t need a class to implement a particular interface. Serializers might be implemented and registered separately from the classes.

It also supports zero-configuration use cases by automatically extracting schemas out of the classes and structs records using reflection, which is cached and reused later, with no extra cost.

The underlying format of the compact serialized objects is platform and language independent. Native client supports will be added shortly after promoting this feature to stable status.

## Zero Configuration

Compact serialization can be used without registering a serializer for a type. Hazelcast will then try to extract a schema out of the class, using reflection, by inspecting all public properties. If successful, it registers the reflection-based serializer associated with the extracted schema and uses it while serializing and deserializing instances of that class. If the automatic schema extraction fails, Hazelcast throws an exception.

Currently, most primitive types (`bool`, `int`...) are supported, as well as enums, arrays of those types, and nested classes. More advanced classes (e.g. `List<T>`) are not supported yet. Intefaces are not supported.

## Compact Serializer

Another way to use compact serialization is to implement the `ICompactSerializer<T>` interface for a type, and register it in the configuration. A basic serializer could look like:

```csharp
public class EmployeeSerializer : ICompactSerializer<Employee>
{
    public string TypeName => "employee";

    public Employee Read(ICompactReader reader)
    {
        return new Employee
        {
            Id = reader.ReadInt64("id");
            Name = reader.ReadString("name");
        }
    }

    public void Write(ICompactWriter writer, Employee employee)
    {
        writer.WriteInt64("id", employee.Id);
        writer.WriteString("name", employee.Name);
    }
}
```

Then, that serializer must be registered in the configuration:

```csharp
options.Serialization.Compact.AddSerializer(new EmployeeSerializer());
```

> [!NOTE]
> Only programmatic configuration is supported by the .NET client at the moment.

## Schema Evolution

Compact serialization permits schemas and classes to evolve by adding or removing fields, or by changing the types of fields. More than one version of a class may live in the same cluster and different clients or members might use different versions of the class.

Hazelcast handles the versioning internally. So, you don’t have to change anything in the classes or serializers apart from the added, removed, or changed fields.

Hazelcast achieves this by identifying each version of the class by a unique fingerprint. Any change in a class results in a different fingerprint. Hazelcast uses a 64-bit Rabin Fingerprint to assign identifiers to schemas, which has an extremely low collision rate.

Different versions of the schema with different identifiers are replicated in the cluster and can be fetched by clients or members internally. That allows old readers to read fields of the classes they know when they try to read data serialized by a new writer. Similarly, new readers might read fields of the classes available in the data, when they try to read data serialized by an old writer.

This means that for one *type name*, there can be several schemas.

In addition, the `ICompactReader` interface exposes methods such as `FieldKind GetFieldKind(string name)` which returns the *kind* (i.e. the actual type) of the field. 
