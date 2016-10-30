var _;
(function (_) {
    function init() {
        SP.SOD.executeFunc('typescripttemplates.ts', 'CSR', function () {
            CSR.override(10000, 2).onPreRender(function (ctx) {
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
            }).header(' ').body(renderTemplate).footer(' ').onPostRender(initializeModel).onPostRender(suppressDefault).register();
        });

        SP.SOD.execute('mQuery.js', 'm$.ready', function () {
            RegisterModuleInit(_spPageContextInfo.webServerRelativeUrl + '/Scripts/view.js', init);
        });

        SP.SOD.notifyScriptLoadedAndExecuteWaitingJobs('view.ts');
    }

    function suppressDefault(ctx) {
        var wpzoneCell = $get('MSOZoneCell_WebPart' + ctx.wpq);
        wpzoneCell.onkeyup = wpzoneCell.onmouseup = function () {
        };

        var footer = $get('scriptPaging' + ctx.wpq);
        footer.style.display = 'none';
    }

    function checkInOut(checkedIn) {
        return checkedIn ? 'Check-Out' : 'Check-In';
    }

    function renderTemplate(ctx) {
        var result = [];
        result.push('<div>');
        result.push(String.format('<p>Total hours submitted <span id="{0}" >{1}</span></p>', ctx.spanId, ctx.totalHours.toPrecision(2)));
        result.push(String.format('<button id="{0}" disabled="disabled">{1}</button>', ctx.buttonId, ctx.buttonText));
        result.push('</div>');
        return result.join('');
    }

    function initializeModel(ctx) {
        SP.SOD.executeFunc('sp.js', 'SP.CleintContext', function () {
            var button = $get(ctx.buttonId);
            var span = $get(ctx.spanId);
            var totalHours = ctx.totalHours;

            var context = SP.ClientContext.get_current();
            var web = context.get_web();
            var list = web.get_lists().getById(ctx.listName);
            var currentItem;

            if (ctx.openLogItem) {
                currentItem = list.getItemById(ctx.openLogItem);
                context.load(currentItem);

                executeQuery(function () {
                    button.disabled = false;
                });
            } else {
                button.disabled = false;
            }

            SP.SOD.executeFunc('mQuery.js', 'm$', function () {
                m$(button).click(function (e) {
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
            }
            ;

            function checkOut(complete) {
                var startedDate = currentItem.get_item('StartDate');
                var dateCompleted = new Date();
                var hours = (dateCompleted.getTime() - startedDate.getTime()) / (1000 * 60 * 60);

                currentItem.set_item('DateCompleted', dateCompleted);
                currentItem.set_item('DurationInHours', hours);
                currentItem.update();

                SPAnimationUtility.BasicAnimator.FadeOut(span);

                executeQuery(function () {
                    currentItem = null;
                    totalHours += hours;
                    SPAnimationUtility.BasicAnimator.FadeIn(span);
                    complete();
                });
            }

            function checkIn(complete) {
                var item = list.addItem(new SP.ListItemCreationInformation());
                item.set_item('StartDate', new Date());
                item.update();
                executeQuery(function () {
                    currentItem = item;
                    complete();
                });
            }

            function executeQuery(callback) {
                context.executeQueryAsync(function () {
                    callback();
                }, function (sender, args) {
                    alert(args.get_message());
                    SP.Utilities.Utility.logCustomAppError(context, args.get_message() + '\n' + args.get_stackTrace());
                    context.executeQueryAsync();
                });
            }
        });
    }

    init();
})(_ || (_ = {}));
//# sourceMappingURL=view.js.map
