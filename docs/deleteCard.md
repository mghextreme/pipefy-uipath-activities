# DeleteCard

[Official API](https://api-docs.pipefy.com/reference/mutations/deleteCard/)  
[Implemented Class](../Capgemini.Pipefy/Card/DeleteCard.cs)

Deletes a card in Pipefy.

## Arguments

### &lt;In&gt; CardID : string

ID of the Card to be deleted.

You can find this info from the API or from the link when accessing it through the web.

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