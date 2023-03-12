using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text;

namespace LinkeD365.FlowToVisio
{
    internal class SharePointAction : Action
    {
        public SharePointAction(JProperty actionProperty, Action parent, int current, int children) : base(actionProperty, parent, current, children, "SharePoint")
        {
            AddName();

            var sb = new StringBuilder();
            if (Property.Value["type"].ToString() == "OpenApiConnection")
            {
                switch (Property.Value["inputs"]["host"]["operationId"].ToString())
                {
                    case "GetAllTables":
                        AddType("Get All Lists and Libraries");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        break;

                    case "CreateAttachment":
                        AddType("Add Attachment");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("Item Id: " + Property.Value["inputs"]["parameters"]["itemId"]);
                        sb.AppendLine("File Name: " + Property.Value["inputs"]["parameters"]["displayName"]);
                        sb.AppendLine("File Content: " + Property.Value["inputs"]["parameters"]["body"]);
                        break;

                    case "ApproveHubSiteJoin":
                        AddType("Approve Hub Site Join");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("Joining Site Id: " + Property.Value["inputs"]["parameters"]["joiningSiteId"]);
                        break;

                    case "CancelHubSiteJoinApproval":
                        AddType("Cancel Hub Site Join Approval");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        if (Property.Value["inputs"]?["parameters"]?["approvalCorrelationId"] != null) sb.AppendLine("Approval Id: " + Property.Value["inputs"]["parameters"]["approvalCorrelationId"]);
                        break;

                    case "CheckInFile":
                        AddType("Check In File");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("Library Id: " + Property.Value["inputs"]["parameters"]["table"]);
                        sb.AppendLine("Item Id: " + Property.Value["inputs"]["parameters"]["id"]);
                        sb.AppendLine("Comment: " + Property.Value["inputs"]["parameters"]["parameter/comment"]);
                        sb.AppendLine("Type: " + (Property.Value["inputs"]["parameters"]["table"].ToString() == "0"
                            ?
                            "Minor (draft)"
                            : (Property.Value["inputs"]["parameters"]["table"].ToString() == "1")
                                ? "Major (Publish)"
                                : "Overwrite"));
                        break;

                    case "CheckOutFile":
                        AddType("Check Out File");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("Library Id: " + Property.Value["inputs"]["parameters"]["table"]);
                        sb.AppendLine("Item Id: " + Property.Value["inputs"]["parameters"]["id"]);
                        break;

                    case "CopyFileAsync":
                        AddType("Copy File");
                        sb.AppendLine("Source Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("Source File: " + Property.Value["inputs"]["parameters"]["parameters/sourceFileId"]);
                        sb.AppendLine("Destination Site: " + Property.Value["inputs"]["parameters"]["parameters/destinationDataset"]);
                        sb.AppendLine("Destination Folder: " + Property.Value["inputs"]["parameters"]["parameters/destinationFolderPath"]);
                        sb.AppendLine("If Duplicate File: " +
                                      (Property.Value["inputs"]["parameters"]["parameters/nameConflictBehavior"]
                                              .ToString() == "0" ? "Fail" :
                                          (Property.Value["inputs"]["parameters"]["parameters/nameConflictBehavior"]
                                              .ToString() == "1") ? "Replace" : "Copy with New"));

                        break;

                    case "CopyFolderAsync":
                        AddType("Copy File");
                        sb.AppendLine("Source Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("Source Folder: " + Property.Value["inputs"]["parameters"]["parameters/sourceFolderId"]);
                        sb.AppendLine("Destination Site: " + Property.Value["inputs"]["parameters"]["parameters/destinationDataset"]);
                        sb.AppendLine("Destination Folder: " + Property.Value["inputs"]["parameters"]["parameters/destinationFolderPath"]);
                        sb.AppendLine("If Duplicate Folder: " +
                                      (Property.Value["inputs"]["parameters"]["parameters/nameConflictBehavior"]
                                              .ToString() == "0" ? "Fail" :
                                          (Property.Value["inputs"]["parameters"]["parameters/nameConflictBehavior"]
                                              .ToString() == "1") ? "Replace" : "Copy with New"));
                        break;

                    case "CreateFile":
                        AddType("Create File");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("Folder: " + Property.Value["inputs"]["parameters"]["folderPath"]);
                        sb.AppendLine("Name: " + Property.Value["inputs"]["parameters"]["name"]);
                        sb.AppendLine("Body: " + Property.Value["inputs"]["parameters"]["body"]);

                        break;

                    case "PostItem":
                        AddType("Create Item");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("Library Id: " + Property.Value["inputs"]["parameters"]["table"]);
                        if (Property.Value["inputs"]?["parameters"]?["view"] != null)
                            sb.AppendLine("View: " + Property.Value["inputs"]["parameters"]["view"]);

                        foreach (var property in Property.Value["inputs"]["parameters"].Children<JProperty>().Where(p => p.Name.StartsWith("item/")))
                        {
                            sb.AppendLine(property.Name.Substring(5, property.Name.Length - 5) + " : " + property.Value);
                        }
                        break;

                    case "CreateNewFolder":
                        AddType("Create New Folder");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("List/Library Id: " + Property.Value["inputs"]["parameters"]["table"]);
                        sb.AppendLine("Path: " + Property.Value["inputs"]["parameters"]["parameters/path"]);

                        if (Property.Value["inputs"]?["parameters"]?["view"] != null)
                            sb.AppendLine("View: " + Property.Value["inputs"]["parameters"]["view"]);
                        break;

                    case "CreateSharingLink":
                        AddType("Create Sharing Link");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("Library: " + Property.Value["inputs"]["parameters"]["table"]);
                        sb.AppendLine("Item Id: " + Property.Value["inputs"]["parameters"]["id"]);
                        sb.AppendLine("Type: " + Property.Value["inputs"]["parameters"]["permission/type"]);
                        sb.AppendLine("Scope: " + Property.Value["inputs"]["parameters"]["permission/scope"]);

                        if (Property.Value["inputs"]?["parameters"]?["permission/expirationDateTime"] != null)
                            sb.AppendLine("Expires: " + Property.Value["inputs"]["parameters"]["permission/expirationDateTime"]);
                        break;

                    case "DiscardFileCheckOut":
                        AddType("Discard Checkout");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("Folder: " + Property.Value["inputs"]["parameters"]["table"]);
                        sb.AppendLine("Item Id: " + Property.Value["inputs"]["parameters"]["id"]);

                        break;

                    case "GetItemAttachments":
                        AddType("Get Attachments");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("Folder: " + Property.Value["inputs"]["parameters"]["table"]);
                        sb.AppendLine("Item Id: " + Property.Value["inputs"]["parameters"]["id"]);
                        break;

                    case "GetItemChanges":
                        AddType("Get Item Changes");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("Folder: " + Property.Value["inputs"]["parameters"]["table"]);
                        sb.AppendLine("Item Id: " + Property.Value["inputs"]["parameters"]["id"]);
                        sb.AppendLine("Changes SInce: " + Property.Value["inputs"]["parameters"]["since"]);

                        if (Property.Value["inputs"]?["parameters"]?["until"] != null)
                            sb.AppendLine("Until: " + Property.Value["inputs"]["parameters"]["until"]);

                        if (Property.Value["inputs"]?["parameters"]?["view"] != null)
                            sb.AppendLine("View: " + Property.Value["inputs"]["parameters"]["view"]);
                        sb.AppendLine("Include Draft: " + Property.Value["inputs"]["parameters"]["includeDrafts"]);

                        break;

                    case "GetFileItem":
                        AddType("Get File (Properties Only)");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("List/Library Id: " + Property.Value["inputs"]["parameters"]["table"]);
                        sb.AppendLine("Item Id: " + Property.Value["inputs"]["parameters"]["id"]);

                        if (Property.Value["inputs"]?["parameters"]?["view"] != null)
                            sb.AppendLine("View: " + Property.Value["inputs"]["parameters"]["view"]);
                        break;

                    case "GetFileItems":
                        AddType("Get Files (Properties Only)");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("List/Library Id: " + Property.Value["inputs"]["parameters"]["table"]);
                        if (Property.Value["inputs"]?["parameters"]?["folderPath"] != null)
                            sb.AppendLine("Limit to Folder Path: " + Property.Value["inputs"]["parameters"]["folderPath"]);
                        if (Property.Value["inputs"]?["parameters"]?["viewScopeOption"] != null) sb.AppendLine("Include Nested Items: " + Property.Value["inputs"]["parameters"]["viewScopeOption"]);

                        if (Property.Value["inputs"]?["parameters"]?["view"] != null)
                            sb.AppendLine("View: " + Property.Value["inputs"]["parameters"]["view"]);

                        foreach (var property in Property.Value["inputs"]["parameters"].Children<JProperty>().Where(p => p.Name.StartsWith("$")))
                        {
                            sb.AppendLine(property.Name.Substring(1, property.Name.Length - 1) + " : " + property.Value);
                        }
                        break;

                    case "GetItems":
                        AddType("Get Items");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("List/Library Id: " + Property.Value["inputs"]["parameters"]["table"]);
                        if (Property.Value["inputs"]?["parameters"]?["folderPath"] != null)
                            sb.AppendLine("Limit to Folder Path: " + Property.Value["inputs"]["parameters"]["folderPath"]);
                        if (Property.Value["inputs"]?["parameters"]?["viewScopeOption"] != null)
                            sb.AppendLine("Include Nested Items: " + Property.Value["inputs"]["parameters"]["viewScopeOption"]);
                        if (Property.Value["inputs"]?["parameters"]?["view"] != null)
                            sb.AppendLine("View: " + Property.Value["inputs"]["parameters"]["view"]);

                        foreach (var property in Property.Value["inputs"]["parameters"].Children<JProperty>().Where(p => p.Name.StartsWith("$")))
                        {
                            sb.AppendLine(property.Name.Substring(1, property.Name.Length - 1) + " : " + property.Value);
                        }
                        break;

                    case "GrantAccess":
                        AddType("Grant Access");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("List/Library Id: " + Property.Value["inputs"]["parameters"]["table"]);
                        sb.AppendLine("Item Id: " + Property.Value["inputs"]["parameters"]["id"]);
                        sb.AppendLine("Recipients: " + Property.Value["inputs"]["parameters"]["parameter/recipients"]);

                        sb.AppendLine("Role: " +
                                      (Property.Value["inputs"]["parameters"]["parameter/roleValue"]
                                          .ToString() == "role:1073741827"
                                          ? "Can Edit"
                                          : "Can View"));

                        if (Property.Value["inputs"]?["parameters"]?["parameter/emailBody"] != null) sb.AppendLine("Message: " + Property.Value["inputs"]["parameters"]["parameter/emailBody"]);
                        if (Property.Value["inputs"]?["parameters"]?["parameter/sendEmail"] != null) sb.AppendLine("Notify: " + Property.Value["inputs"]["parameters"]["parameter/sendEmail"]);

                        break;

                    case "JoinHubSite":
                        AddType("Join Hub Site");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("Hub Site ID: " + Property.Value["inputs"]["parameters"]["hubSiteId"]);
                        break;

                    case "ListFolder":
                        AddType("List Folder");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("ID: " + Property.Value["inputs"]["parameters"]["id"]);
                        break;

                    case "MoveFileAsync":
                        AddType("Move File");
                        sb.AppendLine("Source Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("Source File: " + Property.Value["inputs"]["parameters"]["parameters/sourceFileId"]);
                        sb.AppendLine("Destination Site: " + Property.Value["inputs"]["parameters"]["parameters/destinationDataset"]);
                        sb.AppendLine("Destination Folder: " + Property.Value["inputs"]["parameters"]["parameters/destinationFolderPath"]);
                        sb.AppendLine("If Duplicate File: " +
                                      (Property.Value["inputs"]["parameters"]["parameters/nameConflictBehavior"]
                                              .ToString() == "0" ? "Fail" :
                                          (Property.Value["inputs"]["parameters"]["parameters/nameConflictBehavior"]
                                              .ToString() == "1") ? "Replace" : "Move with New"));
                        break;

                    case "MoveFolderAsync":
                        AddType(" Move Folder");
                        sb.AppendLine("Source Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("Source Folder: " + Property.Value["inputs"]["parameters"]["parameters/sourceFolderId"]);
                        sb.AppendLine("Destination Site: " + Property.Value["inputs"]["parameters"]["parameters/destinationDataset"]);
                        sb.AppendLine("Destination Folder: " + Property.Value["inputs"]["parameters"]["parameters/destinationFolderPath"]);
                        sb.AppendLine("If Duplicate Folder: " +
                                      (Property.Value["inputs"]["parameters"]["parameters/nameConflictBehavior"]
                                              .ToString() == "0" ? "Fail" :
                                          (Property.Value["inputs"]["parameters"]["parameters/nameConflictBehavior"]
                                              .ToString() == "1") ? "Replace" : "Copy with New"));
                        break;

                    case "SearchForUser":
                        AddType("Resolve Person");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("List/Library: " + Property.Value["inputs"]["parameters"]["table"]);

                        sb.AppendLine("Column: " + Property.Value["inputs"]["parameters"]["entityId"]);
                        sb.AppendLine("Search For: " + Property.Value["inputs"]["parameters"]["searchValue"]);

                        if (Property.Value["inputs"]?["parameters"]?["view"] != null)
                            sb.AppendLine("View: " + Property.Value["inputs"]["parameters"]["view"]);
                        break;

                    case "HttpRequest":
                        AddType("Send HTTP request");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("Parameters:");

                        foreach (var property in Property.Value["inputs"]["parameters"].Children<JProperty>().Where(p => p.Name.StartsWith("parameters/")))
                        {
                            sb.AppendLine(property.Name.Substring(11, property.Name.Length - 11) + " : " + property.Value);
                        }

                        break;

                    case "SetApprovalStatus":
                        AddType("Set Approval Status");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("List/Library Id: " + Property.Value["inputs"]["parameters"]["table"]);
                        sb.AppendLine("Item Id: " + Property.Value["inputs"]["parameters"]["id"]);
                        sb.AppendLine("Action: " + Property.Value["inputs"]["parameters"]["approvalAction"]);

                        if (Property.Value["inputs"]?["parameters"]?["comments"] != null) sb.AppendLine("Comments: " + Property.Value["inputs"]["parameters"]["comments"]);
                        if (Property.Value["inputs"]?["parameters"]?["entityTag"] != null) sb.AppendLine("Tag: " + Property.Value["inputs"]["parameters"]["entityTag"]);
                        break;

                    case "NotifyHubSiteJoinApprovalStarted":
                        AddType("Set hub site join to pending");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        if (Property.Value["inputs"]?["parameters"]?["approvalCorrelationId"] != null) sb.AppendLine("Correlation Id: " + Property.Value["inputs"]["parameters"]["approvalCorrelationId"]);
                        break;

                    case "UnshareItem":
                        AddType("Stop Sharing Item");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("List/Library Id: " + Property.Value["inputs"]["parameters"]["table"]);
                        sb.AppendLine("Item Id: " + Property.Value["inputs"]["parameters"]["id"]);
                        break;

                    case "CopyFile":
                        AddType("Copy File (deprecated)");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("Source: " + Property.Value["inputs"]["parameters"]["source"]);
                        sb.AppendLine("Destination: " + Property.Value["inputs"]["parameters"]["destination"]);
                        sb.AppendLine("Overwrite: " + Property.Value["inputs"]["parameters"]["overwrite"]);

                        break;

                    case "DeleteAttachment":
                        AddType("Delete Attachment");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("List/Library Id: " + Property.Value["inputs"]["parameters"]["table"]);
                        sb.AppendLine("Item Id: " + Property.Value["inputs"]["parameters"]["itemId"]);
                        sb.AppendLine("Attachment: " + Property.Value["inputs"]["parameters"]["attachmentId"]);

                        break;

                    case "DeleteFile":
                        AddType("Delete File");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("Id: " + Property.Value["inputs"]["parameters"]["id"]);

                        break;

                    case "ExtractFolderV2":
                        AddType("Extract Folder");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("Source: " + Property.Value["inputs"]["parameters"]["source"]);
                        sb.AppendLine("Destination: " + Property.Value["inputs"]["parameters"]["destination"]);
                        sb.AppendLine("Overwrite: " + Property.Value["inputs"]["parameters"]["overwrite"]);
                        break;

                    case "GetAttachmentContent":
                        AddType("Get Attachment");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("List/Library Id: " + Property.Value["inputs"]["parameters"]["table"]);
                        sb.AppendLine("Item Id: " + Property.Value["inputs"]["parameters"]["itemId"]);
                        sb.AppendLine("Attachment: " + Property.Value["inputs"]["parameters"]["attachmentId"]);
                        break;

                    case "GetFileContent":
                        AddType("Get File");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("Id: " + Property.Value["inputs"]["parameters"]["id"]);
                        sb.AppendLine("Infer Content Type: " + Property.Value["inputs"]["parameters"]["inferContentType"]);
                        break;

                    case "GetFileContentByPath":
                        AddType("Get File By Path");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("Path: " + Property.Value["inputs"]["parameters"]["path"]);
                        sb.AppendLine("Infer Content Type: " + Property.Value["inputs"]["parameters"]["inferContentType"]);
                        break;

                    case "GetFileMetadata":
                        AddType("Get File Meta");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("Id: " + Property.Value["inputs"]["parameters"]["id"]);
                        break;

                    case "GetFileMetadataByPath":
                        AddType("Get File Meta By Path");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("Path: " + Property.Value["inputs"]["parameters"]["path"]);
                        break;

                    case "GetFolderMetadata":
                        AddType("Get Folder Meta");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("Id: " + Property.Value["inputs"]["parameters"]["id"]);
                        break;

                    case "GetFolderMetadataByPath":
                        AddType("Get Folder Meta By Path");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("Path: " + Property.Value["inputs"]["parameters"]["path"]);
                        break;

                    case "GetItem":
                        AddType("Get Item");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("List/Library: " + Property.Value["inputs"]["parameters"]["table"]);
                        sb.AppendLine("Id: " + Property.Value["inputs"]["parameters"]["id"]);
                        if (Property.Value["inputs"]?["parameters"]?["view"] != null)
                            sb.AppendLine("View: " + Property.Value["inputs"]["parameters"]["view"]);
                        break;

                    case "GetTableViews":
                        AddType("Get Views");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("List/Library: " + Property.Value["inputs"]["parameters"]["table"]);
                        break;

                    case "GetTables":
                        AddType("Get Lists");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        break;

                    case "ListRootFolder":
                        AddType("List Root Folder");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        break;

                    case "UpdateFile":
                        AddType("Update File");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("Id: " + Property.Value["inputs"]["parameters"]["id"]);
                        sb.AppendLine("Body: " + Property.Value["inputs"]["parameters"]["body"]);
                        break;

                    case "PatchFileItem":
                        AddType("Update File Properties");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("Library Id: " + Property.Value["inputs"]["parameters"]["table"]);
                        sb.AppendLine("Item Id: " + Property.Value["inputs"]["parameters"]["id"]);

                        if (Property.Value["inputs"]?["parameters"]?["view"] != null)
                            sb.AppendLine("View: " + Property.Value["inputs"]["parameters"]["view"]);

                        foreach (var property in Property.Value["inputs"]["parameters"].Children<JProperty>().Where(p => p.Name.StartsWith("item/")))
                        {
                            sb.AppendLine(property.Name.Substring(5, property.Name.Length - 5) + " : " + property.Value);
                        }

                        break;

                    case "PatchItem":
                        AddType("Update File");
                        sb.AppendLine("Site: " + Property.Value["inputs"]["parameters"]["dataset"]);
                        sb.AppendLine("Library Id: " + Property.Value["inputs"]["parameters"]["table"]);
                        sb.AppendLine("Item Id: " + Property.Value["inputs"]["parameters"]["id"]);

                        if (Property.Value["inputs"]?["parameters"]?["view"] != null)
                            sb.AppendLine("View: " + Property.Value["inputs"]["parameters"]["view"]);

                        foreach (var property in Property.Value["inputs"]["parameters"].Children<JProperty>().Where(p => p.Name.StartsWith("item/")))
                        {
                            sb.AppendLine(property.Name.Substring(5, property.Name.Length - 5) + " : " + property.Value);
                        }

                        break;

                    default:
                        AddType(Property.Value["inputs"]["host"]["operationId"].ToString());
                        Utils.Ai.WriteEvent("No SharePoint Action for " + Property.Value["inputs"]["host"]["operationId"].ToString());
                        foreach (var property in Property.Value["inputs"]["parameters"].Children<JProperty>())
                        {
                            sb.AppendLine(property.Name + " : " + property.Value);
                        }
                        break;
                }
            }
            else
            {
                AddType("SharePoint");
                sb.AppendLine("Properties:");
                foreach (var property in Property.Value["inputs"].Children<JProperty>().Where(p => p.Name != "host"))
                {
                    sb.AppendLine(property.Name + " : " + property.Value);
                }
            }
            AddText(sb);
        }
    }
}