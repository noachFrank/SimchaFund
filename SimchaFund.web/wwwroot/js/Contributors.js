$(() => {
    console.log("in Java Script")

    $("#new-contributor").on("click", function () {
        console.log("Add Clicked")
        new bootstrap.Modal($(".new-contrib")[0]).show();
    })

    $(".deposit-button").on("click", function () {
        console.log("deposit hit")

        const id = $(this).data("contribid");
        new bootstrap.Modal($(".deposit")[0]).show();
        $("#contributorId").val(id)
        console.log(id);
    })

    $(".edit-contrib").on("click", function () {
        console.log("in Edit")

        const id = $(this).data("id")
        const firstName = $(this).data("first-name");
        const lastName = $(this).data("last-name");
        const cell = $(this).data("cell");
        const alwaysInclude = $(this).data("always-include");

        $("#contributor_first_name").val(firstName)
        $("#contributor_last_name").val(lastName)
        $("#contributor_cell_number").val(cell)
        $("#contributor_always_include").prop("checked", alwaysInclude === true)
        $("#contributor_id").val(id)
        $("#initialDepositDiv").hide()
        $("#dateDiv").hide()
        new bootstrap.Modal($(".new-contrib")[0]).show();




    })
})