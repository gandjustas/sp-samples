///<reference path="typings/sharepoint/SharePoint.d.ts" />
///<reference path="typescripttemplates.ts" />

declare var String: {
    new (value?: any): String;
    (value?: any): string;
    prototype: String;
    fromCharCode(...codes: number[]): string;
    format(str: string, ...args: any[]): string;
}

module _ {
    interface TimeTrackerView extends SPClientTemplates.RenderContext_InView {
        totalHours: number;
        openLogItem: number;
        buttonText: string;
        spanId: string;
        buttonId: string;
    }




    function init() {
        SP.SOD.executeFunc('typescripttemplates.ts', 'CSR', () => {
            CSR.override(10000, 2)
                .onPreRender((ctx: TimeTrackerView) => {
                    var rows = ctx.ListData.Row;
                    ctx.totalHours = 0;
                    for (var i = 0; i < rows.length; i++) {
                        if (rows[i]['DurationInHours.']) {
                            ctx.totalHours += parseFloat(rows[i]['DurationInHours.']);
                        } else {
                            ctx.openLogItem = rows[i]['ID'];
                        }
                    }
                    ctx.buttonText = checkInOut(Boolean(ctx.openLogItem));
                    ctx.spanId = ctx.wpq + '_totalHours';
                    ctx.buttonId = ctx.wpq + '_button';
                })
                .header(' ')
                .body(renderTemplate)
                .footer(' ')
                .onPostRender(initializeModel)
                .onPostRender(suppressDefault)
                .register();

        });

        SP.SOD.execute('mQuery.js', 'm$.ready', () => {
            RegisterModuleInit(_spPageContextInfo.webServerRelativeUrl + '/Scripts/view.js', init);
        });


        SP.SOD.notifyScriptLoadedAndExecuteWaitingJobs('view.ts');
    }

    function suppressDefault(ctx: SPClientTemplates.RenderContext_InView) {
        var wpzoneCell = $get('MSOZoneCell_WebPart' + ctx.wpq);
        wpzoneCell.onkeyup = wpzoneCell.onmouseup = function () { };

        var footer = $get('scriptPaging' + ctx.wpq);
        footer.style.display = 'none';
    }

    function checkInOut(checkedIn: boolean): string {
        return checkedIn ? 'Check-Out' : 'Check-In';
    }

    function renderTemplate(ctx: TimeTrackerView): string {
        var result: string[] = [];
        result.push('<div>');
        result.push(String.format('<p>Total hours submitted <span id="{0}" >{1}</span></p>', ctx.spanId, ctx.totalHours.toPrecision(2)));
        result.push(String.format('<button id="{0}" disabled="disabled">{1}</button>', ctx.buttonId, ctx.buttonText));
        result.push('</div>');
        return result.join('');
    }

    function initializeModel(ctx: TimeTrackerView): void {
        SP.SOD.executeFunc('sp.js', 'SP.CleintContext', () => {
            var button = $get(ctx.buttonId);
            var span = $get(ctx.spanId);
            var totalHours = ctx.totalHours;

            var context = SP.ClientContext.get_current();
            var web = context.get_web();
            var list = web.get_lists().getById(ctx.listName);
            var currentItem: SP.ListItem;

            if (ctx.openLogItem) {
                currentItem = list.getItemById(ctx.openLogItem);
                context.load(currentItem);

                executeQuery(() => { button.disabled = false; });
            } else {
                button.disabled = false;
            }

            SP.SOD.executeFunc('mQuery.js', 'm$', () => {
                m$(button).click(e => {
                    button.disabled = true;

                    if (currentItem) {
                        checkOut(updateView);
                    } else {
                        checkIn(updateView);
                    }
                    e.preventDefault();
                });
            });

            function updateView() {
                button.disabled = false;
                button.innerHTML = checkInOut(Boolean(currentItem));
                span.innerHTML = totalHours.toPrecision(2);
            };

            function checkOut(complete: () => void) {
                var startedDate = <Date>currentItem.get_item('StartDate');
                var dateCompleted = new Date();
                var hours = (dateCompleted.getTime() - startedDate.getTime()) / (1000 * 60 * 60);

                currentItem.set_item('DateCompleted', dateCompleted);
                currentItem.set_item('DurationInHours', hours);
                currentItem.update();

                SPAnimationUtility.BasicAnimator.FadeOut(span);

                executeQuery(() => {
                    currentItem = null;
                    totalHours += hours;
                    SPAnimationUtility.BasicAnimator.FadeIn(span);
                    complete();
                });
            }

            function checkIn(complete: () => void) {
                var item = list.addItem(new SP.ListItemCreationInformation());
                item.set_item('StartDate', new Date());
                item.update();
                executeQuery(() => {
                    currentItem = item;
                    complete();
                });
            }

            function executeQuery(callback: () => void) {
                context.executeQueryAsync(
                    () => {
                        callback();
                    },
                    (sender, args) => {
                        alert(args.get_message());
                        SP.Utilities.Utility.logCustomAppError(context,
                            args.get_message() + '\n' + args.get_stackTrace());
                        context.executeQueryAsync();
                    });
            }


        });
    }

    init();
}

