/// <reference path="~/GeneratedArtifacts/viewModel.js" />

myapp.ViewDeveloper.Details_postRender = function (element, contentItem) {
    // Write code here.
    var name = contentItem.screen.Developer.details.getModel()[':@SummaryProperty'].property.name;
    contentItem.dataBind("screen.Developer." + name, function (value) {
        contentItem.screen.details.displayName = value;
    });
}

