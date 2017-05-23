/// <reference path="~/GeneratedArtifacts/viewModel.js" />

myapp.ViewProject.Details_postRender = function (element, contentItem) {
    // Write code here.
    var name = contentItem.screen.Project.details.getModel()[':@SummaryProperty'].property.name;
    contentItem.dataBind("screen.Project." + name, function (value) {
        contentItem.screen.details.displayName = value;
    });
}

