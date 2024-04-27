## Adding Actions to Actions.json

Actions.json is hosted within this repository and holds configuration for the display of actions.

To be more dynamic in loading them and as new actions get created, add actions to this file.

![](/images/Screenshot%202024-04-27%20150033.png)

Each action has a title, what will be used in the header of the Action in Visio

Type: Leave as OpenApiConnection, this is the standard

visioShape: Links back to the list of shapes in the Visio with apporpriate images

![](/images/Screenshot%202024-04-27%20150357.png)

connectionName: This is taken from the connectionName property in the peek code

operationId: This is taken from the operationId property via peek code

Display: This is a list of the main properties for your action, those that you know about, usually those that are not hidden under advanced. 

Each display has a name and where to get the value from. Each node in the JSON is seperated by a |. This can be linked back to the peek code.

For those values which are not essential to be displayed, you can use the value option

![](/images/Screenshot%202024-04-27%20151451.png)

This will repeat for all parameters defined with a prefix (and node stepping with |) of "item/" in this case.