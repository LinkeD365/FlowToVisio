# FlowToVisio

A tool to create Visio representations of your Cloud Flows.

Tool available via [XrmToolBox](https://www.xrmtoolbox.com/plugins/LinkeD365.FlowToVisio/)

[Limitations](Limitations.md)

## Solution Flows

Open Tool

For flows that are part of solutions, connect to your Dataverse environment using standard XrmToolBox [connections](https://www.xrmtoolbox.com/documentation/for-users/manage-connections/). Select the Connect to Dataverse button.

![](https://user-images.githubusercontent.com/43988771/106387685-c2f43b80-63d2-11eb-80e8-bcf3a25a9111.png)

A list of Flows will be displayed. Use the Search bar to find the flow you want. Select Create Visio after selecting. A file dialogue will prompt you where to save your file.

Once complete, a prompt will tell you if you are successful, displaying the number of actions the tool generated.

![](https://user-images.githubusercontent.com/43988771/106387742-0ea6e500-63d3-11eb-9f77-55475121e6ce.png)

Find the Visio and open it up.

![](https://user-images.githubusercontent.com/43988771/106387764-28482c80-63d3-11eb-9af3-92eda5d70867.png)

## Connecting to Non-Solutioned Flows

Flows will only appear in the main list if they are part of a solution. Solutions are only available with a Dataverse configuration. If you want to document flows that appear under My Flows, use the Connect to Flow API button. The dialogue allows you to add more than one connection, depending on your environments. Select the appropriate one, when you have created one, by using the select field. 

Add a new connection by selecting the + button. The Label field is just that, a name for the connection to distinguish it. The other 3 parts need to be populated with information created or retrieved in Azure.

![](https://user-images.githubusercontent.com/43988771/123132474-b25a6580-d446-11eb-9aee-1b0f1f0b3eba.png)

Firstly, you will need to register an Azure application, more details are available [here](https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app). 

In Azure, register an application, copy the App Id and Tenant Id into the corresponding boxes in the window available when you select Connect to Flow API

![](https://user-images.githubusercontent.com/43988771/106388080-ba9d0000-63d4-11eb-86d6-74df19e18279.png)

Environment Id will be displayed when you navigate to the Flow in My Flows

![](https://user-images.githubusercontent.com/43988771/106388113-e0c2a000-63d4-11eb-8012-6892494ed7d8.png)

> ## **The Return URL needs to be specified as a Mobile and Desktop application**

![](https://user-images.githubusercontent.com/43988771/106388141-0bacf400-63d5-11eb-99e9-22efb5c4bdea.png)

### Document Logic Apps (BETA)

To connect to the Azure Logic Apps, there is a new button available in the toolbar. But first, you need to amend or add an app registration to allow access to Logic Apps.

Register or amend an App registration. API Permissions required are Azure Service Management/user impersonation

![](https://user-images.githubusercontent.com/43988771/107113837-75a41e00-6859-11eb-954c-aecbdc253857.png)

In the application, select the Connect to Logic Apps button

![](https://user-images.githubusercontent.com/43988771/107113888-c9af0280-6859-11eb-9bae-327743f74da2.png)

Populate the API connection pop up with the appropriate settings. 

Firstly, the Subscription Id can be found on your Logic App Overview, the purple box here

![](https://user-images.githubusercontent.com/43988771/107113966-4d68ef00-685a-11eb-850e-fbbfe0e9740d.png)

Next, the Application Id and Tenant Id both are available on the App Registration page, the red and yellow items here

![](https://user-images.githubusercontent.com/43988771/107114017-a5075a80-685a-11eb-9723-b4686c7e11c7.png)

> ## **The Return URL needs to be specified as a Mobile and Desktop application**
