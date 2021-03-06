# GetOrganization

[Official API](https://pipefy.docs.apiary.io/#reference/0/show-organization/organization(id:-organization_id))  
[Implemented Class](../Capgemini.Pipefy/Organization/GetOrganization.cs)

Gets detailed information on an Organization.

## Arguments

### &lt;In&gt; OrganizationID : long

ID of the Organization to be obtained.

You can find this info from the API or from the link when accessing it through the web.

### &lt;Out&gt; Organization : JObject

The Organization obtained (JObject).

Example object:

```json
{
    "id": "12345",
    "name": "Nice Org",
    "pipes": [
        {
            "cards_count": 62,
            "description": null,
            "id": "1233456",
            "name": "My pipe",
            "opened_cards_count": 21,
            "public": false,
            "role": "admin"
        }
    ],
    "role": "normal",
    "tables": {
        "edges": [{
            "node": {
                "id": "M4a7Dqr6",
                "name": "TableName",
                "public": true,
                "url": "http://app.pipefy.com/database_v2/tables/M4a7Dqr6-tablename"
            }
        }]
    },
    "users": [{
        "email": "your.email@company.com",
        "id": "12345",
        "name": "Your Name"
    }, {
        "email": "colleague.email@company.com",
        "id": "98431",
        "name": "Colleague Name"
    }]
}
```

## Inherited Arguments

### &lt;In&gt; Bearer : string

The Bearer authorization token generated by Pipefy.

### &lt;In&gt; Timeout : int

The timeout limit (in ms) for the request to be completed.

### &lt;Out&gt; Status : string

A brief status message of the result of the action.

### &lt;Out&gt; Success : boolean

True if the action was successful.

---

[All actions](../README.md)