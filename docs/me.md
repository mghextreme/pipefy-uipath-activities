# Me

[Official API](https://api-docs.pipefy.com/reference/mutations/getMe/)  
[Implemented Class](../Capgemini.Pipefy/TableRecord/Me.cs)

Gets detailed information about a TableRecord in a Pipefy Table.

## Arguments

### &lt;Out&gt; User : JObject

User obtained (JObject)

## Inherited Arguments

### &lt;In&gt; Bearer : string

The Bearer authorization token generated by Pipefy.

### &lt;In&gt; Timeout : int

The timeout limit (in ms) for the request to be completed.

### &lt;Out&gt; Status : string

A brief status message of the result of the action.

### &lt;Out&gt; Success : boolean

True if the action was successful.