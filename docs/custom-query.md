# Custom Queries

If you want to execute a query that is not implemented or has to be different than the ones in the Activities, there is an easy way to do so.

Let's say I have the following query, used to create an Organization, which is not currently available in an Activity.

```graphql
mutation {
    createOrganization(input: {
        industry: "technology"
        name: "Custom Tests"
    }){
        organization {
            created_at
            id
        }
    }
}
```

Using the [PipefyQuery](../Capgemini.Pipefy/PipefyQuery.cs) class it is easy to execute any query.  
You just need to instance the class, with the query and the Bearer code.

```csharp
private const string Bearer = "eyJ0...dAZw";

public string ExecuteQuery(string query)
{
    var pipefyQuery = new PipefyQuery(query, Bearer);
    return pipefyQuery.Execute();
}
```

This method will execute the query and return the content from Pipefy in a string format.  
The content can be parsed as a JObject and interpreted.  
In this case, the result would be something like:

```json
{
    "data": {
        "createOrganization": {
            "organization": {
                "created_at": "2019-03-26T17:24:35-03:00",
                "id": "123456"
            }
        }
    }
}
```