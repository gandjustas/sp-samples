/// <reference path="typings/sharepoint/sharepoint.d.ts" />
SP.SOD.executeFunc('sp.js', 'SP.ClientContext', function () {
    var context = SP.ClientContext.get_current();
    var web = context.get_web();
    context.executeQueryAsync(function () {
        SP.SOD.executeFunc('sp.workflowservices.js', 'SP.WorkflowServices', function () {
            var servicesManager = SP.WorkflowServices.WorkflowServicesManager.newObject(context, web);
            var deploymentService = servicesManager.getWorkflowDeploymentService();
            var instanceService = servicesManager.getWorkflowInstanceService();
            var subscriptionService = servicesManager.getWorkflowSubscriptionService();
            var subs = subscriptionService.enumerateSubscriptions();
            context.load(subs);
            context.executeQueryAsync(function () {
                var sub = subs.get_item(0);
                var guid = instanceService.startWorkflow(sub, {});
                context.executeQueryAsync(function () {
                    var instance = instanceService.getInstance((guid.get_value()));
                    var button = $get("sendevent");
                    $addHandler(button, "click", function (e) {
                        instanceService.publishCustomEvent(instance, "MyEvent", "");
                        context.executeQueryAsync();
                        e.preventDefault();
                    });
                });
            });
        });
    });
});
//# sourceMappingURL=app.js.map