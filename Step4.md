# Step 4 - Client Refresh

## Server Configration

* Add the new folder Hubs to Question.Web
* Add new class QuestionsHub to folder Hub
* Set the base class of the QuestionsHub class to Hub

### Configure and activate SignalR in the Startup class

<details><summary>ConfigureServices</summary>

~~~c#
// Configuration for SignalR
services.AddSignalR();
~~~
</details>

<details><summary>Configure (after endpoints.MapControllers)</summary>

~~~c#
// Activate SignalR Hub
endpoints.MapHub<QuestionsHub>("/hub");
~~~
</details>

## Add calls to the hub

### Commands\QuestionController

<details><summary>Add a IHubContext<QuestionsHub> as parameter to the constructor for dependency injection</summary>

~~~c#
private readonly IHubContext<QuestionsHub> _hub;
public QuestionsController(QuestionsContext context, IHubContext<QuestionsHub> hub)
{
    _context = context;
    _hub = hub;
}
~~~
</details>

<details><summary>Add the method RefreshClients</summary>

~~~c#
private void RefreshClients()
{
    _hub?.Clients.All.SendAsync("refresh").Wait();
}
~~~
</details>

Call the method RefreshClients in the Ask and Vote method

### Fix the unit tests

<details><summary>Modify the NewCommand method</summary>

~~~c#
private QuestionsApp.Web.Api.Controllers.Commands.QuestionsController NewCommand(QuestionsContext context)
{
    return new Web.Api.Controllers.Commands.QuestionsController(context, null);
}
~~~
</details>

## Web site Implementation

<details><summary>Add SignalR script import</summary>

~~~html
<script src="https://cdn.jsdelivr.net/npm/@microsoft/signalr@3.1.0/dist/browser/signalr.min.js" crossorigin="anonymous"></script>    
<title>Ask your questions</title>
~~~
</details>

Remove the call of getQuestions from the methods ask and vote

<details><summary>Add connection to hub and register to the refresh message to call getQuestions</summary>

~~~js
const connection = new signalR.HubConnectionBuilder()
    .withUrl("hub")
    .build();
connection.start().catch(err => console.error(err.toString()));
connection.on("Refresh", () => { console.log("Refresh"); questionListController.getQuestions(); });
~~~
</details>