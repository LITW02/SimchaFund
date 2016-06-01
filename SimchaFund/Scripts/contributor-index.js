$(function() {
    $("#new-contributor").on('click', function() {
        $("#contrib-modal").modal();
    });

    $("#contributorCreatedAt").datetimepicker();
    $("#datepicker").datetimepicker();

    $(".deposit").on('click', function () {
        var contribId = $(this).data('contributor-id');
        $("#contrib-id-deposit").val(contribId);
        $("#deposit-modal").modal();
    });

    $(".edit").on('click', function () {
        var contribId = $(this).data('contributor-id');
        var firstName = $(this).data('first-name');
        var lastName = $(this).data('last-name');
        var cell = $(this).data('cell-number');
        var date = $(this).data('date');
        var alwaysInclude = $(this).data('always-include');

        $("#contrib-id-edit").val(contribId);
        $("#firstName").val(firstName);
        $("#lastName").val(lastName);
        $("#cellNumber").val(cell);
        $("#firstName").val(firstName);
        $("#contributorCreatedAt").val(date);
        $("#initialDepositDiv").hide();

        $("#contrib-modal form").attr('action', '/contributors/edit');
        $("#alwaysInclude").prop('checked', alwaysInclude === "True");
        $("#contrib-modal .submit-button").text('Update');
        $("#contrib-modal").modal();

    });
});