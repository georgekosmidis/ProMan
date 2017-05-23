/// <reference path="~/GeneratedArtifacts/viewModel.js" />

myapp.ViewProjectDeveloper.Details_postRender = function (element, contentItem) {
    // Write code here.
    var name = contentItem.screen.ProjectDeveloper.details.getModel()[':@SummaryProperty'].property.name;
    contentItem.dataBind("screen.ProjectDeveloper." + name, function (value) {
        contentItem.screen.details.displayName = value;
    });
}

